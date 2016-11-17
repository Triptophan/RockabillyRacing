using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinnersCircleController : MonoBehaviour
{
	private WinnersCircle _winnersCircleComponent;

	private Camera _mainCamera;

	private PlayerData _data;

	private bool _goingToNextLevel = false;

	private const float MAX_LIGHT_INTENSITY = 1f;

	private int[] Prizes = { 10000, 6000, 3000, 1000 };

	public GameObject[] winnerCirclePlaceholders;

	public Light showcaseLight;

	public GUISkin UISkin;

	public bool debugMode;

	#region Unity Methods
	void Awake()
	{
		_data = new PlayerData();
	}

	void Start()
	{
		GameObject cameraObject = GameObject.Find("Main Camera");
		if (cameraObject == null)
		{
			Debug.LogError("Could not find main camera object.");
		}
		else
		{
			_mainCamera = cameraObject.camera;
		}

		GameObject winnersCircleObject = GameObject.Find("WinnersCircle");
		if (winnersCircleObject == null) return;

		_winnersCircleComponent = winnersCircleObject.GetComponent<WinnersCircle>();

		if (winnerCirclePlaceholders.Length > 0)
		{
			GameObject[] winners = _winnersCircleComponent.winners;
			for (int i = 0; i < winners.Length; i++)
			{
				winners[i].SetActive(true);

				if (!debugMode)
				{
					DisableCarComponents(winners[i]);
				}
				else
				{
					Debug.LogWarning("Debug mode is on! Car components will not be disabled.");
				}

				Transform winnerTransform = winners[i].transform;
				winnerTransform.parent = winnerCirclePlaceholders[i].transform;
				winnerTransform.position = Vector3.zero;
				winnerTransform.localPosition = Vector3.zero;
				winnerTransform.localScale = Vector3.one;
				winnerTransform.localRotation = Quaternion.Euler(Vector3.zero);

				if (i == winners.Length - 1)
				{
					SetToUnlitLayer(winners[i]);
				}

				ApplyPrizeWinnings(winners[i], i);
			}

			_data.Save();
		}
	}

	void Update()
	{
		if (showcaseLight != null && showcaseLight.intensity <= MAX_LIGHT_INTENSITY)
		{
			showcaseLight.intensity += Time.deltaTime * 0.1f;
		}

		if (Input.anyKey)
		{
			GameObject winnersCircleObject = GameObject.Find("WinnersCircle");
			if (winnersCircleObject == null) return;

			_winnersCircleComponent = winnersCircleObject.GetComponent<WinnersCircle>();

			foreach (GameObject winner in _winnersCircleComponent.winners)
			{
				Destroy(winner);
			}
			Destroy(winnersCircleObject);
			_goingToNextLevel = true;
			Application.LoadLevel(LevelHelper.MainMenu);
		}
	}

	void OnGUI()
	{
		if (_mainCamera == null || _goingToNextLevel) return;

		if (UISkin != null)
		{
			GUI.matrix = GUIHelper.SetScale();

			GUI.skin = UISkin;
		}

		if (_winnersCircleComponent != null)
		{
			RenderText();
		}
	}
	#endregion

	#region Menu/Scene Methods
	private void DisableCarComponents(GameObject driver)
	{
		VehicleMovement movementComponent = driver.GetComponent<VehicleMovement>();
		if (movementComponent != null)
		{
			movementComponent.ToggleWheelColliders(false);
			movementComponent.enabled = false;
		}

		StateMachine stateMachineComponent = driver.GetComponent<StateMachine>();
		if (stateMachineComponent != null)
		{
			stateMachineComponent.enabled = false;
		}

		VehicleMonitor vehicleMonitorComponent = driver.GetComponent<VehicleMonitor>();
		if (vehicleMonitorComponent != null)
		{
			vehicleMonitorComponent.enabled = false;
		}

		Rigidbody rigidBody = driver.rigidbody;
		if (rigidBody != null)
		{
			rigidBody.useGravity = false;
			rigidBody.isKinematic = true;
		}

		Transform nameObject = driver.transform.FindChild("PlayerName");
		if (nameObject != null)
		{
			nameObject.gameObject.SetActive(false);
		}

		Transform carLights = driver.transform.FindChild("Lights");
		if (carLights != null)
		{
			carLights.gameObject.SetActive(false);
		}

		AudioSource audio = driver.GetComponent<AudioSource>();
		if (audio != null)
		{
			audio.Stop();
		}

		DamageController damageController = driver.GetComponent<DamageController>();
		if (damageController != null && damageController.SmokeEffect != null)
		{
			damageController.SmokeEffect.Stop();
		}
	}

	private void SetToUnlitLayer(GameObject driver)
	{
		int layer = LayerMask.NameToLayer("TransparentFX");

		Transform[] transforms = driver.GetComponentsInChildren<Transform>();
		foreach (Transform t in transforms)
		{
			t.gameObject.layer = layer;
		}
	}

	private void RenderText()
	{
		float yOffsetBorder = 100f;
		float dividerHeight = 10f;

		float boxWidth = GUIHelper.HalfScreenWidth;
		float boxHeight = GUIHelper.NATIVE_HEIGHT / 4f - dividerHeight;

		float leftHandLeft = 200f;
		float rightHandLeft = boxWidth;

		float textPadding = yOffsetBorder * 1f;

		Rect nameRect = new Rect(0f, yOffsetBorder, boxWidth, boxHeight);
		Rect prizeRect = new Rect(0f, textPadding + yOffsetBorder, boxWidth, boxHeight);

		//First
		GUI.BeginGroup(new Rect(rightHandLeft, 0f, boxWidth, boxHeight));
		string firstPrizeString = string.Format("Prize: {0:C0}", Prizes[0]);
		GUI.Label(nameRect, _winnersCircleComponent.winners[0].name);
		GUI.Label(prizeRect, firstPrizeString);
		GUI.EndGroup();

		//Second
		GUI.BeginGroup(new Rect(leftHandLeft, boxHeight, boxWidth, boxHeight));
		string secondPrizeString = string.Format("Prize: {0:C0}", Prizes[1]);
		GUI.Label(nameRect, _winnersCircleComponent.winners[1].name);
		GUI.Label(prizeRect, secondPrizeString);
		GUI.EndGroup();

		//Third
		GUI.BeginGroup(new Rect(rightHandLeft, boxHeight * 2f, boxWidth, boxHeight));
		string thirdPrizeString = string.Format("Prize: {0:C0}", Prizes[2]);
		GUI.Label(nameRect, _winnersCircleComponent.winners[2].name);
		GUI.Label(prizeRect, thirdPrizeString);
		GUI.EndGroup();

		//Fourth
		GUI.BeginGroup(new Rect(leftHandLeft, boxHeight * 3f, boxWidth, boxHeight));
		string fourthPrizeString = string.Format("Prize: {0:C0}", Prizes[3]);
		GUI.Label(nameRect, _winnersCircleComponent.winners[3].name);
		GUI.Label(prizeRect, fourthPrizeString);
		GUI.EndGroup();
	}
	#endregion

	private void ApplyPrizeWinnings(GameObject driver, int placeIndex)
	{
		if (driver.tag == TagHelper.PLAYER)
		{
			_data.PlayerCash += Prizes[placeIndex];
		}
	}
}