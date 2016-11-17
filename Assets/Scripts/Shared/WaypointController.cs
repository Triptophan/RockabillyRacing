using System;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour
{
	#region Private Members
	private Transform[] _waypoints;
	private Transform _myTransform;

	private List<int> _touchedWaypoints;

	private Vector3[] _basePositions;

	private int _waypointIndex;

	private const float NOISE_MIN = -4f;
	private const float NOISE_MAX = 4f;
	#endregion

	#region Public Members
	public Transform waypointContainer;
	public Transform currentWaypoint;

	public List<int> touchedWaypoints;

	public int NumberOfWaypoints { get { return _waypoints.Length; } }
	#endregion

	#region Unity Methods
	void Awake()
	{
		_myTransform = transform;

		Init();
	}

	public void OnTriggerEnter(Collider other)
	{
		if (currentWaypoint.name == other.name)
		{
			if (!touchedWaypoints.Contains(_waypointIndex))
			{
				touchedWaypoints.Add(_waypointIndex);
			}

			SetNextWaypoint();
		}
	}
	#endregion

	#region Public Methods
	public void Init()
	{
		if (waypointContainer != null)
		{
			Transform[] temp = waypointContainer.GetComponentsInChildren<Transform>();

			_waypoints = new Transform[temp.Length - 1];
			for (int i = 1; i < temp.Length; i++)
			{
				_waypoints[i - 1] = temp[i];
			}

			_basePositions = new Vector3[_waypoints.Length];

			Array.Sort(_waypoints, new IndexedNameSorter());

			for (int i = 0; i < _waypoints.Length; i++)
			{
				_basePositions[i] = _waypoints[i].position;
			}

			currentWaypoint = _waypoints[0];

			touchedWaypoints = new List<int>();

			_waypointIndex = 0;

			AddWaypointNoise();
		}
	}

	public void SetNextWaypoint()
	{
		if (_waypointIndex++ >= _waypoints.Length - 1)
		{
			_waypointIndex = 0;

			ResetAllWaypointPositions();
		}

		currentWaypoint = _waypoints[_waypointIndex];
		AddWaypointNoise();
	}

	public Transform PreviousWaypoint()
	{
		ResetAllWaypointPositions();

		int index = _waypointIndex - 1;

		if (_waypointIndex == 0)
		{
			index = _waypoints.Length - 1;
		}

		return _waypoints[index];
	}

	public float GetPercentageWaypointsTouched(bool normalized)
	{
		float percentNormalized = (float)touchedWaypoints.Count / (float)_waypoints.Length;

		return normalized ? percentNormalized : percentNormalized * 100f;
	}

	public void ResetWaypointsTouched()
	{
		touchedWaypoints.Clear();
	}

	public float GetDistanceToNextWaypoint()
	{
		int lastIndex = _waypointIndex - 1 < 0 ? _waypoints.Length - 1: _waypointIndex - 1;
		float betweenDistance = Vector3.Distance(_waypoints[lastIndex].position, currentWaypoint.position);
		return 1 - (Vector3.Distance(_myTransform.position, currentWaypoint.position) / betweenDistance);
	}
	#endregion

	#region Private Methods
	private void ResetAllWaypointPositions()
	{
		for (int i = 0; i < _waypoints.Length; i++)
		{
			_waypoints[i].position = _basePositions[i];
		}
	}

	private void AddWaypointNoise()
	{
		currentWaypoint.position = currentWaypoint.position + (currentWaypoint.right * UnityEngine.Random.Range(NOISE_MIN, NOISE_MAX));
	}
	#endregion
}