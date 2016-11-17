using UnityEngine;

[RequireComponent(typeof(VehicleMovement))]
[RequireComponent(typeof(DamageController))]
public class PlayerControl : MonoBehaviour
{
	private VehicleMovement movement;
	private DamageController damageController;

	public void Start()
	{
		movement = GetComponent<VehicleMovement>();
		damageController = GetComponent<DamageController>();

		PlayerData data = new PlayerData();
		gameObject.name = data.PlayerName;
	}

	void Update()
	{
		GetInput();
	}

	private void GetInput()
	{
		if (movement.canDrive)
		{
			float acceleration = Input.GetButton("Accelerate") ? 1f : Input.GetAxis("Accelerate");
			float braking = Input.GetButton("Brake") ? -0.7f : Input.GetAxis("Brake");
			
			movement.throttle = 0f;
			
			if (acceleration > 0f)
			{
				movement.throttle = acceleration;
			}
			if (braking < 0f)
			{
				movement.throttle = braking;
			}

			movement.steering = Input.GetAxis("Steering");
		}

		if (Input.GetButton("Respawn"))
		{
			damageController.DoRespawn(1f);
		}
	}
}