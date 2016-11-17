using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VehicleMovement))]
public class VehicleMonitor : MonoBehaviour
{
	private GameObject _myGameObject;

	private Transform _myTransform;

	private Rigidbody _myRigidBody;

	private VehicleMovement _movement;

	private GameController _gameController;

	private float lastUprightTime;

	private const float DISPLACEMENT_LIMIT_TEST = 25f;
	private const float VELOCITY_LIMIT_TEST = 80f;

	public float VelocityBelowThresholdTime;

	void Awake()
	{
		_myGameObject = gameObject;
		_myTransform = transform;
		_myRigidBody = rigidbody;

		_movement = _myGameObject.GetComponent<VehicleMovement>();
	}

	void Start()
	{
		if (_gameController == null)
		{
			GameObject gameControllerObject = GameObject.FindGameObjectWithTag(TagHelper.GAME_CONTROLLER);
			if (gameControllerObject != null)
			{
				_gameController = gameControllerObject.GetComponent<GameController>();
			}
		}
	}

	void Update()
	{
		if (_myGameObject.tag == TagHelper.PLAYER)
		{
			if (IsUpright())
			{
				lastUprightTime = Time.time;
			}

			float timeSince = Time.time - lastUprightTime;

			if (timeSince > 5f && _movement.canDrive)
			{
				AddToRespawn();
			}
		}
		else
		{
			if (_gameController != null)
			{
				if (!_gameController.debugMode && _gameController.raceActive)
				{
					bool displacementTest = (_myTransform.position - _movement.LastPosition).sqrMagnitude < DISPLACEMENT_LIMIT_TEST;

					if (_myRigidBody.velocity.sqrMagnitude <= VELOCITY_LIMIT_TEST || displacementTest)
					{
						VelocityBelowThresholdTime += Time.deltaTime;
					}
					else
					{
						VelocityBelowThresholdTime = 0f;
					}

					if (VelocityBelowThresholdTime >= 5.0 && _movement.canDrive)
					{
						AddToRespawn();
					}
				}
			}
		}
	}

	private bool IsUpright()
	{
		if (_myTransform.eulerAngles.z > 275 || _myTransform.eulerAngles.z < 65)
		{
			return true;
		}

		return false;
	}

	private void AddToRespawn()
	{
		if (_gameController != null)
		{
			_gameController.QueueRespawn(_myGameObject);
		}
	}
}