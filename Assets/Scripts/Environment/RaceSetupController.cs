using System.Collections.Generic;
using UnityEngine;

public class RaceSetupController : MonoBehaviour
{
	public GameObject AIPrefab;
	public GameObject PlayerPrefab;
	public GameObject WaypointPrefab;

	public Material[] carMaterials;

	public Transform[] startPositions;

	public bool UseCarLights;

	private List<bool> _positionUsed;

	private PlayerData _data;

	private float _playerArmor;
	private float _playerAcceleration;
	private float _playerCornering;
	private float _playerTopSpeed;

	void Awake()
	{
		_data = new PlayerData();
		_positionUsed = new List<bool>() { false, false, false, false };
	}

	public bool SpawnPlayers()
	{
		return SetHuman() && InsertAI();
	}

	private bool SetHuman()
	{
		int position = _data.LastWinPosition - 1 == -1 ? _positionUsed.Count - 1 : _data.LastWinPosition - 1;

		_positionUsed[position] = true;

		GameObject player = (GameObject)Instantiate(PlayerPrefab, startPositions[position].position, startPositions[position].rotation);

		if (player != null)
		{
			InitWaypoints(player);

			AmmoHealthController ammoGUIController = player.GetComponent<AmmoHealthController>();
			if (ammoGUIController != null)
			{
				GameObject AmmoHealthGUI = GameObjectHelper.AmmoHealthGUIObject;
				if (AmmoHealthGUI != null)
				{
					ammoGUIController.GuiText = AmmoHealthGUI.guiText;
				}
			}

			GameObject mainCamera = GameObjectHelper.MainCamera;
			if (mainCamera != null)
			{
				PlayerCamera playerCameraComponent = mainCamera.GetComponent<PlayerCamera>();
				if (playerCameraComponent != null)
				{
					playerCameraComponent.cameraTarget = player.transform;
				}
			}

			InitLights(player);

			InitCarColor(player, -1);

			AbilityScore abilityScores = player.GetComponent<AbilityScore>();
			if (abilityScores != null)
			{
				abilityScores.AssignAbilityScores();

				_playerAcceleration = abilityScores.AccelerationAbility;
				_playerArmor = abilityScores.ArmorAbility;
				_playerCornering = abilityScores.CorneringAbility;
				_playerTopSpeed = abilityScores.TopSpeedAbility;
			}

			return true;
		}

		return false;
	}

	private bool InsertAI()
	{
		bool success = true;
		for (int i = 0; i < _positionUsed.Count; i++)
		{
			if (!_positionUsed[i])
			{
				success &= SetAI(i);
			}
		}

		return true;
	}

	private bool SetAI(int startIndex)
	{
		int nameIndex = 0;

		for (int i = 0; i < _positionUsed.Count; i++)
		{
			if (!_positionUsed[i])
			{
				GameObject aiObject = (GameObject)Instantiate(AIPrefab, startPositions[i].position, startPositions[i].rotation);

				if (aiObject != null)
				{
					_positionUsed[i] = true;

					InitAIName(aiObject, nameIndex);

					InitLights(aiObject);

					InitCarColor(aiObject, nameIndex);

					InitWaypoints(aiObject);

					InitAIAbilities(aiObject);

					nameIndex++;
				}
			}
		}

		return true;
	}

	private void InitAIName(GameObject aiObject, int nameIndex)
	{
		aiObject.name = _data.GetAIName(nameIndex);
		Transform nameTransform = aiObject.transform.FindChild("PlayerName");
		if (nameTransform != null)
		{
			GameObject nameObject = nameTransform.gameObject;
			if (nameObject != null)
			{
				TextMesh textMesh = nameObject.GetComponent<TextMesh>();
				if (textMesh != null)
				{
					textMesh.text = aiObject.name;
				}
			}
		}
	}

	private void InitLights(GameObject car)
	{
		VehicleLightController lightController = car.GetComponent<VehicleLightController>();
		if (lightController != null)
		{
			lightController.ToggleHeadLights(UseCarLights);
			lightController.SetRearLights(UseCarLights ? VehicleLightState.On : VehicleLightState.Off);
		}
	}

	private void InitCarColor(GameObject car, int index)
	{
		Transform chassis = car.transform.FindChild("Chassis");
		if (chassis != null)
		{
			MeshRenderer mesh = chassis.GetComponent<MeshRenderer>();
			if (mesh != null)
			{
				int materialIndex = index < 0 ? _data.CarColor : _data.GetAICarColor(index);
				mesh.material = carMaterials[materialIndex];
			}
		}
	}

	private void InitWaypoints(GameObject car)
	{
		WaypointController waypointController = car.GetComponent<WaypointController>();
		if (waypointController != null)
		{
			GameObject waypointContainer = (GameObject)Instantiate(WaypointPrefab, WaypointPrefab.transform.position, WaypointPrefab.transform.rotation);
			if (waypointContainer != null)
			{
				string name = car.tag == TagHelper.PLAYER ? _data.PlayerName : car.name;
				waypointContainer.name = string.Format("{0}sWaypoints", name);
				waypointController.waypointContainer = waypointContainer.transform;
				waypointController.Init();
			}
		}
	}

	private void InitStateMachine(GameObject car)
	{
		StateMachine stateMachineComponent = car.GetComponent<StateMachine>();
		if (stateMachineComponent != null)
		{
			stateMachineComponent.ChangeState(Idle.Instance);
			stateMachineComponent.ChangeGlobalState(Idle.Instance);
		}
	}

	private void InitAIAbilities(GameObject car)
	{
		AbilityScore abilityScores = car.GetComponent<AbilityScore>();
		if (abilityScores != null)
		{
			abilityScores.ArmorAbility = Mathf.Clamp(Random.Range(1, _playerArmor + 2), AbilityScore.MIN_ARMOR_ABILITY, AbilityScore.MAX_ARMOR_ABILITY);
			abilityScores.AccelerationAbility = Mathf.Clamp(Random.Range(1, _playerAcceleration + 2), AbilityScore.MIN_ACCELERATION_ABILITY, AbilityScore.MAX_ACCELERATION_ABILITY);
			abilityScores.CorneringAbility = Mathf.Clamp(Random.Range(1, _playerCornering + 2), AbilityScore.MIN_CORNERING_ABILITY, AbilityScore.MAX_CORNERING_ABILITY);
			abilityScores.TopSpeedAbility = Mathf.Clamp(Random.Range(1, _playerTopSpeed + 2), AbilityScore.MIN_TOP_SPEED_ABILITY, AbilityScore.MAX_TOP_SPEED_ABILITY);
		}
	}
}