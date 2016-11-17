using UnityEngine;
using System.Collections;

public class SteeringBehavior : MonoBehaviour
{
	private GameObject _myObject;
	private Transform _myTransform;

	private float _weightFollowPath = 0.8f;
	private float _weightSeparation = 0.7f;
	private float _weightPursuit = 0.6f;
	private float _weightEvade = 0.4f;
	private float _weightWallAvoidance = 0.7f;

	private WaypointController _waypoints;

	private int _flags;

	private Vector3 steeringForce = Vector3.zero;

	public GameObject[] AIAgents;

	public float TurnedAroundCountdown = 5f;
	public float maxSpeed;

	public GameObject currentTarget;

	public void Awake()
	{
		_myObject = gameObject;
		_myTransform = _myObject.transform;
	}

	public void Start()
	{
		_waypoints = GetComponent<WaypointController>();
	}

	public Vector3 Calculate()
	{
		steeringForce = Vector3.zero;

		if (On(behavior_type.follow_path))
		{
			steeringForce += FollowPath() * _weightFollowPath;
		}

		if (On(behavior_type.separation))
		{
			steeringForce += Separation() * _weightSeparation;
		}

		if (On(behavior_type.pursuit))
		{
			steeringForce += Pursuit() * _weightPursuit;
		}

		if (On(behavior_type.evade))
		{
			steeringForce += Evade() * _weightEvade;
		}

		if (On(behavior_type.wall_avoidance))
		{
			steeringForce += WallAvoidance() * _weightWallAvoidance;
		}

		return steeringForce;
	}

	private Vector3 FollowPath()
	{
		if (!(_waypoints && _waypoints.currentWaypoint)) return new Vector3(0, 0, 0);

		Transform currentWaypoint = _waypoints.currentWaypoint.transform;
		if (Vector3.Dot(_myTransform.forward, currentWaypoint.forward) > 0)
		{
			if (GameObjectHelper.IsInFront(_myTransform.position, currentWaypoint.position, currentWaypoint.forward))
			{
				_waypoints.SetNextWaypoint();
			}
		}

		return Seek(_waypoints.currentWaypoint.position);
	}

	private Vector3 Pursuit()
	{
		Transform targetTransform = currentTarget.transform;

		Vector3 toTarget = targetTransform.position - _myTransform.position;

		float relativeHeading = Vector3.Dot(_myTransform.forward, targetTransform.forward);

		if (Vector3.Dot(toTarget, _myTransform.forward) > 0f && relativeHeading < -0.95f)
		{
			return Seek(targetTransform.position);
		}

		Vector3 targetVelocity = Vector3.Normalize(currentTarget.GetComponent<VehicleMovement>().rigidbody.velocity);

		float evaderSpeed = targetVelocity.magnitude;

		float LookAheadTime = toTarget.magnitude / (maxSpeed + evaderSpeed);

		return Seek(targetTransform.position + targetVelocity * LookAheadTime, Color.red);
	}

	private Vector3 Evade()
	{
		Transform targetTransform = currentTarget.transform;

		Vector3 toPursuer = targetTransform.position - _myTransform.position;

		Vector3 targetVelocity = Vector3.Normalize(currentTarget.GetComponent<VehicleMovement>().rigidbody.velocity);

		float pursuerSpeed = targetVelocity.magnitude;

		float LookAheadTime = toPursuer.magnitude / (maxSpeed + pursuerSpeed);

		return Flee(targetTransform.position + targetVelocity * LookAheadTime, Color.blue);
	}

	private Vector3 Flee(Vector3 targetPosition)
	{
		return (Vector3.Normalize(_myTransform.position - targetPosition) * _myObject.rigidbody.velocity.magnitude);
	}

	private Vector3 Flee(Vector3 targetPosition, Color color)
	{
		Vector3 desiredVelocity = Vector3.Normalize(_myTransform.position - targetPosition) * maxSpeed;

		Debug.DrawLine(_myTransform.position, _myTransform.position + desiredVelocity, color);

		return desiredVelocity;
	}

	private Vector3 Seek(Vector3 targetPosition)
	{
		return (Vector3.Normalize(targetPosition - _myTransform.position) * maxSpeed);
	}

	private Vector3 Seek(Vector3 targetPosition, Color color)
	{
		Vector3 desiredVelocity = Vector3.Normalize(targetPosition - _myTransform.position) * maxSpeed;

		Debug.DrawLine(_myTransform.position, _myTransform.position + desiredVelocity, color);

		return desiredVelocity;
	}

	//returns a steering force which will attempt to keep the agent away from any obstacles it may encounter
	private Vector3 Separation()
	{
		Vector3 steeringForce = Vector3.zero;

		foreach (GameObject ai in AIAgents)
		{
			if (!_myObject.name.Equals(ai.name) && ai.activeInHierarchy)
			{
				Vector3 toAgent = _myTransform.position - ai.transform.position;

				if (toAgent.sqrMagnitude < _myTransform.rigidbody.velocity.magnitude / 2f)
					steeringForce += toAgent;
			}
		}

		Debug.DrawLine(_myTransform.position, _myTransform.position + steeringForce, Color.magenta);
		return steeringForce;
	}

	//returns a steering force which will keep the agent away from any walls it might encounter
	private Vector3 WallAvoidance()
	{
		Vector3 steeringForce = Vector3.zero;
		int layerMask = 1 << LayerMask.NameToLayer("Player");
		layerMask = ~layerMask;

		RaycastHit hit = new RaycastHit();

		float velocity = _myObject.rigidbody.velocity.magnitude;
		Vector3 position = _myTransform.position + new Vector3(0f, 0.2f, 0f);
		Vector3 relativeForward = _myTransform.TransformDirection(Vector3.forward);
		float overShoot = 0f;

		if (velocity <= 0f)
		{
			velocity = 1f;
		}

		if (Physics.Raycast(position, relativeForward, out hit, velocity, layerMask))
		{
			overShoot = velocity - hit.distance;

			steeringForce = hit.normal * overShoot;

			Debug.DrawRay(position, relativeForward * hit.distance, Color.yellow);
			Debug.DrawRay(hit.point, steeringForce, Color.gray);
		}

		return steeringForce;
	}

	private bool On(behavior_type bt) { return (_flags & (int)bt) == (int)bt; }

	public void FollowPathOn() { _flags |= (int)behavior_type.follow_path; }
	public void SeparationOn() { _flags |= (int)behavior_type.separation; }
	public void PursuitOn() { _flags |= (int)behavior_type.pursuit; }
	public void EvadeOn() { _flags |= (int)behavior_type.evade; }
	public void WallAvoidanceOn() { _flags |= (int)behavior_type.wall_avoidance; }

	public void FollowPathOff() { if (On(behavior_type.follow_path)) _flags ^= (int)behavior_type.follow_path; }
	public void SeparationOff() { if (On(behavior_type.separation)) _flags ^= (int)behavior_type.separation; }
	public void PursuitOff() { if (On(behavior_type.pursuit)) _flags ^= (int)behavior_type.pursuit; }
	public void EvadeOff() { if (On(behavior_type.evade)) _flags ^= (int)behavior_type.evade; }
	public void WallAvoidanceOff() { if (On(behavior_type.wall_avoidance)) _flags ^= (int)behavior_type.wall_avoidance; }

	enum behavior_type : int
	{
		none = 0x00000,
		separation = 0x00100,
		wall_avoidance = 0x00200,
		follow_path = 0x00400,
		pursuit = 0x00800,
		evade = 0x01000
	};
}