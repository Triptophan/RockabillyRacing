using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LapTracker : MonoBehaviour
{
	private Dictionary<GameObject, int> _lapDictionary = new Dictionary<GameObject, int>();

	private GameController _gameController;

	private Transform _myTransform;

	private Dictionary<GameObject, float> _lastCrossedTime;
	private Dictionary<GameObject, float> _placeDictionary;

	public Dictionary<GameObject, int> LapDictionary { get { return _lapDictionary; } }

	public Dictionary<GameObject, float> PlaceDictionary { get { return _placeDictionary; } }

	void Awake()
	{
		GameObject gameControllerObject = GameObject.Find("GameController");
		
		if (gameControllerObject != null)
		{
			_gameController = gameControllerObject.GetComponent<GameController>();
		}
	}

	void Start()
	{
		InvokeRepeating("UpdatePlacing", 0, 1f);
	}

	void OnTriggerEnter(Collider other)
	{
		GameObject otherObject = other.transform.root.gameObject;
		if (GameObjectHelper.IsACar(otherObject))
		{
			CheckForLapCompletion(otherObject);
		}
	}

	public void SetupLapTracker()
	{
		List<GameObject> allDrivers;
		if (_gameController.allDrivers.Length < 4)
		{
			allDrivers = _gameController.GetAllDrivers().ToList();
		}
		else
		{
			allDrivers = _gameController.allDrivers.ToList();
		}

		_lapDictionary = new Dictionary<GameObject, int>(allDrivers.Count);
		_lastCrossedTime = new Dictionary<GameObject, float>(allDrivers.Count);
		_placeDictionary = new Dictionary<GameObject, float>(allDrivers.Count);
		foreach (GameObject driver in allDrivers)
		{
			if (!_lapDictionary.ContainsKey(driver))
			{
				_lapDictionary.Add(driver, 0);
			}

			if (!_lastCrossedTime.ContainsKey(driver))
			{
				_lastCrossedTime.Add(driver, Time.time);
			}

			if (!_placeDictionary.ContainsKey(driver))
			{
				_placeDictionary.Add(driver, 0f);
			}
		}
	}

	public string GetCurrentPlayerPlacementString()
	{
		StringBuilder text = new StringBuilder();

		if (PlaceDictionary == null || PlaceDictionary.Count == 0) return "";

		int index = 0;
		foreach (GameObject driver in PlaceDictionary.Keys)
		{
			index++;
			if (driver.tag == TagHelper.PLAYER)
			{
				break;
			}
		}

		text.AppendFormat("Currently: {0} of {1}", index, PlaceDictionary.Count);

		return text.ToString();
	}

	public string GetCurrentPlayerLapString()
	{
		if (LapDictionary == null || LapDictionary.Count == 0) return "";

		foreach (GameObject driver in LapDictionary.Keys)
		{
			if (driver.tag == TagHelper.PLAYER)
			{
				return string.Format("Lap: {0} of {1}", LapDictionary[driver], _gameController.MaxLaps);
			}
		}

		return "";
	}

	public void UpdatePlacing()
	{
		Dictionary<GameObject, float> newPlacing = new Dictionary<GameObject,float>();
		foreach (GameObject driver in _placeDictionary.Keys)
		{
			float lapsCompleted = _lapDictionary[driver] * 10f;
			WaypointController waypointController = driver.GetComponent<WaypointController>();
			float waypointCompletion = waypointController.GetPercentageWaypointsTouched(true) * 10f;
			newPlacing.Add(driver, lapsCompleted + waypointCompletion + waypointController.GetDistanceToNextWaypoint());
		}

		List<KeyValuePair<GameObject, float>> newPlacingList = newPlacing.ToList();
		newPlacingList.Sort((first, second) => { return first.Value.CompareTo(second.Value); });
		newPlacingList.Reverse();

		_placeDictionary = newPlacingList.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
	}

	private void CheckForLapCompletion(GameObject driverObject)
	{
		float lastCrossedTime = _lastCrossedTime[driverObject];
		float deltaTime = Time.time - lastCrossedTime;

		if (deltaTime > 2f)
		{
			float headingDot = Vector3.Dot(transform.forward, driverObject.transform.forward);
			float velocityDot = Vector3.Dot(Vector3.Normalize(driverObject.rigidbody.velocity), transform.forward);

			if (headingDot >= 0f && velocityDot >= 0f)
			{
				WaypointController waypointController = driverObject.GetComponent<WaypointController>();

				if (waypointController != null)
				{
					bool isNormalized = _gameController.percentageOfTrackRequired <= 1f;
					if (waypointController.GetPercentageWaypointsTouched(isNormalized) >= _gameController.percentageOfTrackRequired)
					{
						IncrementLap(driverObject);

						RefreshAmmo(driverObject);

						_lastCrossedTime[driverObject] = Time.time;
					}
				}
			}
		}
	}

	private void IncrementLap(GameObject driverObject)
	{
		if (_lapDictionary != null && _lapDictionary.Count > 0)
		{
			_lapDictionary[driverObject]++;

			if (_lapDictionary[driverObject] == _gameController.MaxLaps)
			{
				_gameController.EndRace(driverObject);
			}
			else
			{
				WaypointController waypointController = driverObject.GetComponent<WaypointController>();

				if (waypointController != null)
				{
					waypointController.ResetWaypointsTouched();
				}
			}
		}
	}

	private void RefreshAmmo(GameObject driver)
	{
		if (driver.tag == TagHelper.PLAYER)
		{
			PlayerCombatController combatController = driver.GetComponent<PlayerCombatController>();

			if (combatController != null)
			{
				combatController.ReloadWeapons();
			}
		}
		else if (driver.tag == TagHelper.AI_AGENT)
		{
			CombatController combatController = driver.GetComponent<CombatController>();

			if (combatController != null)
			{
				combatController.ReloadWeapons();
			}
		}
	}
}