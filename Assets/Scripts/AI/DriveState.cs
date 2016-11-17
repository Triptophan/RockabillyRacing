using UnityEngine;

public sealed class Drive : State
{
	#region Private members
	private static Drive _instance;

	private float steeringFix = 0f;

	private const float STEERING_DAMPING = 25f;
	private const float TIGHT_TURN_DAMPING = 25f;
	#endregion

	#region Properties
	public static State Instance
	{
		get
		{
			if (_instance == null)
				_instance = new Drive();

			return _instance;
		}
	}
	#endregion

	#region State Overrides
	public override void Enter(GameObject agent)
	{
		SteeringBehavior steeringBehavior = agent.GetComponent<SteeringBehavior>();

		steeringBehavior.FollowPathOn();
		steeringBehavior.WallAvoidanceOn();
		steeringBehavior.SeparationOn();

		VehicleMovement movement = agent.GetComponent<VehicleMovement>();

		if (movement)
		{
			steeringBehavior.maxSpeed = movement.drivetrain.maxSpeed;
		}
	}

	public override void Execute(GameObject agent)
	{
		VehicleMovement movement = agent.GetComponent<VehicleMovement>();
		Rigidbody agentRigidbody = agent.rigidbody;

		if (agentRigidbody.velocity.sqrMagnitude <= 5f && IsStuckOnWall(agent))
		{
			movement.throttle = -1f;
			movement.steering = -movement.steering;
			return;
		}

		SteeringBehavior steeringBehavior = agent.GetComponent<SteeringBehavior>();
		Transform agentTransform = agent.transform;

		Vector3 steeringForce = steeringBehavior.Calculate();
		Debug.DrawLine(agentTransform.position, agentTransform.position + steeringForce, Color.green);

		float angleToSteeringForce = Vector3.Angle(agentTransform.forward, steeringForce);

		float steerSign = GetSteering(agent, steeringForce);
		float throttle = 1f;
		float steering;

		steering = Mathf.Clamp(angleToSteeringForce / movement.drivetrain.turningRadius * steerSign, -1f, 1f);

		CheckAndFixAlignment(agent, steeringForce, ref steering, ref throttle);

		movement.throttle = throttle;
		movement.steering = steering * (steeringFix == 0f ? 1f : steeringFix);
	}

	public override void Exit(GameObject agent)
	{
		SteeringBehavior steeringBehavior = agent.GetComponent<SteeringBehavior>();

		steeringBehavior.FollowPathOff();
		steeringBehavior.WallAvoidanceOff();
		steeringBehavior.SeparationOff();
	}
	#endregion

	private void CheckAndFixAlignment(GameObject agent, Vector3 steeringForce, ref float steering, ref float throttle)
	{
		SteeringBehavior steeringBehavior = agent.GetComponent<SteeringBehavior>();

		float dot = Vector3.Dot(agent.transform.forward, Vector3.Normalize(steeringForce));
		float velocityDot = Vector3.Dot(Vector3.Normalize(agent.transform.rigidbody.velocity), agent.transform.forward);
		float turnedAroundCountdown = steeringBehavior.TurnedAroundCountdown;

		if (dot < 0.3f && turnedAroundCountdown >= 5f)
		{
			throttle = -0.4f;

			if (velocityDot <= 0f)
			{
				if (steeringFix == 0f)
				{
					steeringFix = steering > 0f ? -1f : 1f;
				}
			}
		}
		else if (dot < 0.4f)
		{
			if (turnedAroundCountdown > 0f)
			{
				throttle = 0.6f;

				turnedAroundCountdown -= Time.deltaTime;
			}
		}
		else
		{
			if (steeringFix != 0f) steeringFix = 0f;

			if (turnedAroundCountdown < 5f) turnedAroundCountdown = 5f;
		}

		steeringBehavior.TurnedAroundCountdown = turnedAroundCountdown;
	}

	private float GetSteering(GameObject agent, Vector3 target)
	{
		Vector3 cross = Vector3.Cross(agent.transform.forward, target);

		float steerSign = Mathf.Sign(cross.y);
		if (cross.y == 0) steerSign = 0f;

		return steerSign;
	}

	private bool IsStuckOnWall(GameObject agent)
	{
		int layerMask = 1 << LayerMask.NameToLayer("Player");
		layerMask = ~layerMask;

		RaycastHit hit = new RaycastHit();

		Vector3 position = agent.transform.position + new Vector3(0f, 0.2f, 0f);
		Vector3 relativeForward = agent.transform.TransformDirection(Vector3.forward);

		return Physics.Raycast(position, relativeForward, out hit, 7f, layerMask);
	}
}