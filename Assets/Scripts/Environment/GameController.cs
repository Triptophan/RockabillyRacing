using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RaceSetupController))]
[RequireComponent(typeof(MusicController))]
public class GameController : MonoBehaviour
{
	#region Private Members
	private Queue<string> actionTextQueue;
	private Queue<GameObject> respawnQueue;

	private RaceSetupController _raceSetupController;
	private MusicController _musicController;
	private LapTracker _lapTracker;

	private AudioListener _cameraListener;
	private AudioListener _playerListener;

	private PlayerData _data;

	private List<string> actionTexts;

	private bool _gameOver;

	private int _timeLeft;
	private int _driversCountedForWinnersCircle;
	#endregion

	#region Public Members
	public GameObject[] aiAgents;
	public GameObject[] allPlayers;
	public GameObject[] allDrivers;
	public GameObject[] miniMapMarkers;

	public GameObject winnersCircle;

	public Material[] carMaterials;
	public Material[] markerMaterials;

	public GUIText actionText;

	public float MaxLaps;
	public float percentageOfTrackRequired;

	public bool debugMode;
	public bool raceActive;
	#endregion

	#region Unity LifeCycle
	void Awake()
	{
		_gameOver = false;
		raceActive = false;
		actionTextQueue = new Queue<string>();
		respawnQueue = new Queue<GameObject>();
		_timeLeft = 3;
		_raceSetupController = GetComponent<RaceSetupController>();
		_musicController = GetComponent<MusicController>();
		_data = new PlayerData();

		GameObject lapTrackerObject = GameObject.Find("LapTracker");
		if (lapTrackerObject != null)
		{
			_lapTracker = lapTrackerObject.GetComponent<LapTracker>();
		}
	}

	void Start()
	{
		if (_raceSetupController == null) return;
		if (_raceSetupController.SpawnPlayers())
		{
			GetAllDrivers();

			SetMiniMapMarkers();

			SetAIWeapons();

			SetPlayerWeapons();

			if (_lapTracker != null) _lapTracker.SetupLapTracker();

			_cameraListener = GameObject.Find("Main Camera").GetComponent<AudioListener>();
			_cameraListener.enabled = false;

			if (!Application.isLoadingLevel)
			{
				if (debugMode)
				{
					foreach (GameObject driverObject in allDrivers)
					{
						ToggleDriver(driverObject, true, false, true);
					}
				}
				else
				{
					StartCountdown();

					_musicController.PlayMusic();
				}
			}

			ProcessRespawns();
		}
	}

	void Update()
	{
		UpdateMiniMapMarkers();

		PositionActionText();
	}

	void OnGUI()
	{
		if (_gameOver)
		{
			Texture2D fade = new Texture2D(0, 0);
			fade.SetPixel(0, 0, new Color(0, 0, 0, 0));

			GUI.color = new Color(0, 0, 0, 0.2f);
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), fade, ScaleMode.StretchToFill, false);
		}
	}

	void OnDestroy()
	{
		_data.LastLevelLoaded = Application.loadedLevel;
		_data.Save();
	}
	#endregion

	#region Driver Setup
	public GameObject[] GetAIDrivers()
	{
		if (aiAgents == null || aiAgents.Length == 0)
		{
			aiAgents = GameObject.FindGameObjectsWithTag(TagHelper.AI_AGENT);
		}

		for (int i = 0; i < aiAgents.Length; i++)
		{
			aiAgents[i].GetComponent<SteeringBehavior>().AIAgents = aiAgents;
		}

		return aiAgents;
	}

	public GameObject[] GetPlayers()
	{
		if (allPlayers == null || allPlayers.Length == 0)
		{
			allPlayers = GameObject.FindGameObjectsWithTag(TagHelper.PLAYER);
		}

		if(allPlayers.Length > 0)
		{
			_playerListener = allPlayers[0].GetComponent<AudioListener>();
		}

		return allPlayers;
	}

	public GameObject[] GetAllDrivers()
	{
		if (aiAgents.Length == 0) GetAIDrivers();
		if (allPlayers.Length == 0) GetPlayers();

		allDrivers = new GameObject[aiAgents.Length + allPlayers.Length];

		int counter = 0;
		for (int i = 0; i < aiAgents.Length; i++)
		{
			allDrivers[counter++] = aiAgents[i];
		}

		for (int i = 0; i < allPlayers.Length; i++)
		{
			allDrivers[counter++] = allPlayers[i];
		}

		return allDrivers;
	}
	#endregion

	#region MiniMapMarkers
	private void SetMiniMapMarkers()
	{
		miniMapMarkers = GameObject.FindGameObjectsWithTag("MiniMapMarker");

		if (miniMapMarkers.Length == 0) return;

		for (int i = 0; i < allDrivers.Length; i++)
		{
			Transform chassis = allDrivers[i].transform.FindChild("Chassis");

			if (chassis == null) return;

			MeshRenderer mesh = chassis.GetComponent<MeshRenderer>();

			if (mesh != null)
			{
				GameObject marker = miniMapMarkers[i];

				if (marker != null)
				{
					marker.name = "MiniMapMarker" + allDrivers[i].name;

					MeshRenderer markerMesh = marker.GetComponent<MeshRenderer>();
					if (markerMesh != null)
					{
						markerMesh.material = GetMarkerMaterial(mesh.material.name);
					}
				}
			}
		}
	}

	private void UpdateMiniMapMarkers()
	{
		if (allDrivers.Length == 0) return;

		for (int i = 0; i < miniMapMarkers.Length && i < allDrivers.Length; i++)
		{
			Vector3 newPosition = allDrivers[i].transform.position;
			newPosition.y = miniMapMarkers[i].transform.position.y;

			miniMapMarkers[i].transform.position = newPosition;
		}
	}

	private Material GetMarkerMaterial(string carMaterialName)
	{
		for (int i = 0; i < carMaterials.Length; i++)
		{
			if (carMaterialName.Contains(carMaterials[i].name))
			{
				return markerMaterials[i];
			}
		}

		return null;
	}
	#endregion

	private void SetAIWeapons()
	{
		WeaponHelper helper;

		foreach (GameObject aiObject in aiAgents)
		{
			CombatController combatController = aiObject.GetComponent<CombatController>();

			if (combatController != null)
			{
				helper = new WeaponHelper();
				combatController.SetWeapon(helper.PickRearWeapon());
				combatController.SetWeapon(helper.PickFrontWeapon());
			}
		}
	}

	private void SetPlayerWeapons()
	{
		foreach (GameObject playerObject in allPlayers)
		{
			PlayerCombatController combatControllerComponent = playerObject.GetComponent<PlayerCombatController>();
			if (combatControllerComponent != null)
			{
				int frontWeaponValue = 0;
				int rearWeaponValue = 0;
				if (combatControllerComponent.frontWeaponType == FrontWeaponType.None)
				{
					frontWeaponValue = _data.PlayerFrontWeapon;
				}
				else
				{
					frontWeaponValue = (int)combatControllerComponent.frontWeaponType;
				}

				if (combatControllerComponent.rearWeaponType == RearWeaponType.None)
				{
					rearWeaponValue = _data.PlayerRearWeapon;
				}
				else
				{
					rearWeaponValue = (int)combatControllerComponent.rearWeaponType;
				}

				combatControllerComponent.AmmoAbilityLevel = WeaponHelper.GetAbilityLevel((FrontWeaponType)frontWeaponValue);
				combatControllerComponent.SetWeapon((FrontWeaponType)frontWeaponValue);

				combatControllerComponent.AmmoAbilityLevel = WeaponHelper.GetAbilityLevel((FrontWeaponType)frontWeaponValue);
				combatControllerComponent.SetWeapon((RearWeaponType)rearWeaponValue);
			}
		}
	}

	#region Action Text
	public void DoActionText(string text)
	{
		StartCoroutine("ShowActionText", text);
	}

	private void PositionActionText()
	{
		float yOffset = Screen.height * 3f / 4f;
		float xOffset = Screen.width / 2f;
		actionText.pixelOffset = new Vector2(xOffset, yOffset);
	}

	private System.Collections.IEnumerator ShowActionText(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			actionTextQueue.Enqueue(text);
		}
		else
		{
			if (actionTextQueue.Count == 0)
			{
				actionText.text = "";
				yield break;
			}
		}
		string nextText = (string)actionTextQueue.Dequeue();
		actionText.text = nextText;

		yield return new WaitForSeconds(3f);

		if (actionTextQueue.Count > 0 || !string.IsNullOrEmpty(actionText.text))
		{
			StartCoroutine("ShowActionText", "");
		}
	}
	#endregion

	#region Race Startup
	private void StartCountdown()
	{
		StartCoroutine(CountDown());
	}

	private System.Collections.IEnumerator CountDown()
	{
		DoActionText(_timeLeft.ToString());

		yield return new WaitForSeconds(1f);

		_timeLeft--;

		if (_timeLeft > 0)
		{
			StartCoroutine(CountDown());
		}
		else
		{
			DoActionText("GO!");

			foreach (GameObject driverObject in allDrivers)
			{
				ToggleDriver(driverObject, true, true, true);
			}

			raceActive = true;

			yield break;
		}
	}
	#endregion

	#region Race End
	public void EndRace(GameObject driver)
	{
		DontDestroyOnLoad(driver);

		driver.SetActive(false);

		if (driver.tag == TagHelper.PLAYER)
		{
			_playerListener.enabled = false;
			_cameraListener.enabled = true;
		}

		StartCoroutine("StopCar", driver);

		AddToWinnersCircle(driver);

		if (_driversCountedForWinnersCircle == allDrivers.Length)
		{
			_gameOver = true;

			StartCoroutine(GoToWinnersCircle());
		}
	}
	#endregion

	private void ToggleDriver(GameObject driverObject, bool enable, bool enableStateMachine, bool enableWeapons)
	{
		VehicleMovement movement = driverObject.GetComponent<VehicleMovement>();

		if (movement != null)
		{
			movement.ToggleWheelColliders(enable);
			movement.canDrive = enable;
		}

		CombatControllerBase combatController = driverObject.GetComponent<CombatControllerBase>();

		if (combatController != null)
		{
			combatController.CanFire = enableWeapons;
		}

		StateMachine stateMachine = driverObject.GetComponent<StateMachine>();
		if (stateMachine != null)
		{
			stateMachine.enabled = enableStateMachine;
			if (enableStateMachine)
			{
				stateMachine.ChangeState(CombatSentryState.Instance);
				stateMachine.ChangeGlobalState(Drive.Instance);
			}
			else
			{
				stateMachine.ChangeState(Idle.Instance);
				stateMachine.ChangeGlobalState(Idle.Instance);
			}
		}

		Collider[] colliders = driverObject.GetComponentsInChildren<Collider>();
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = enabled;
		}
	}

	private System.Collections.IEnumerator StopCar(GameObject car)
	{
		yield return new WaitForSeconds(0.5f);

		car.rigidbody.isKinematic = true;
	}

	#region Respawn Logic
	public void QueueRespawn(GameObject carToRespawn)
	{
		DamageController damageController = carToRespawn.GetComponent<DamageController>();

		if (damageController != null)
		{
			if (!damageController.FlaggedForRespawn)
			{
				if (!respawnQueue.Contains(carToRespawn))
				{
					respawnQueue.Enqueue(carToRespawn);
				}
			}
		}
	}

	private void ProcessRespawns()
	{
		StartCoroutine(Respawn());
	}

	private System.Collections.IEnumerator Respawn()
	{
		if (respawnQueue.Count > 0)
		{
			GameObject carObject = respawnQueue.Dequeue();

			if (carObject != null)
			{
				DamageController damageController = carObject.GetComponent<DamageController>();

				if (damageController != null)
				{
					if (!damageController.FlaggedForRespawn)
					{
						damageController.DoRespawn(1f);
					}
				}
			}
		}

		yield return new WaitForSeconds(1f);

		StartCoroutine(Respawn());
	}
	#endregion

	#region WinnersCircle Methods
	private System.Collections.IEnumerator GoToWinnersCircle()
	{
		if (winnersCircle != null)
		{
			WinnersCircle wcComponent = winnersCircle.GetComponent<WinnersCircle>();
			if (wcComponent != null)
			{
				wcComponent.DisableWinnersCircleWinners();
				Time.timeScale = 1f;
			}
		}

		yield return new WaitForSeconds(1f);

		Application.LoadLevel(LevelHelper.WinnersCircle);
	}

	private void AddToWinnersCircle(GameObject driver)
	{
		if (winnersCircle != null)
		{
			WinnersCircle wcComponent = winnersCircle.GetComponent<WinnersCircle>();
			if (wcComponent != null)
			{
				DamageController damageController = driver.GetComponent<DamageController>();
				if (damageController != null && driver.tag == TagHelper.PLAYER)
				{
					damageController.CommitMoney();
					Time.timeScale = 5f;
				}
				wcComponent.AddWinner(driver);
				_driversCountedForWinnersCircle++;
			}
		}
	}
	#endregion
}