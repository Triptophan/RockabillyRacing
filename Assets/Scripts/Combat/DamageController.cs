using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageController : MonoBehaviour
{
	private GameObject _myGameObject;
	
	private GameController _gameController;

	private Transform _myTransform;

	private VehicleMovement _myMovement;

	private int _moneyPickedUpSoFar;

	private const int MIN_MONEY_VALUE = 100;
	private const int MAX_MONEY_VALUE = 1000;

	private const int MIN_SMOKE_RATE = 10;
	private const int MAX_SMOKE_RATE = 1;
	private const int MIN_HEALTH_FOR_SMOKE = 10;
	private const int MAX_HEALTH_FOR_SMOKE = 70;

	public float Health;
	public float MaxHealth;
	public float Armor;

	public AudioClip AmmoUpSound;
	public AudioClip HealthUpSound;
	public AudioClip MoneyUpSound;

	public AudioClip[] CrashSounds;

	public bool FlaggedForRespawn { get; set; }

	public ParticleSystem SmokeEffect;

	public AudioSource PowerUpAudioSource;
	public AudioSource CollisionAudioSource;

	#region UnityEngine Methods
	void Awake()
	{
		_myGameObject = gameObject;
		_myTransform = _myGameObject.transform;

		if (Health == 0f) Health = 100f;
		if (MaxHealth == 0f) MaxHealth = 100f;
		if (Armor == 0f) Armor = 1f;

		FlaggedForRespawn = false;
	}

	void Start()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag(TagHelper.GAME_CONTROLLER);
		if (gameControllerObject != null)
		{
			_gameController = gameControllerObject.GetComponent<GameController>();
		}

		_myMovement = _myGameObject.GetComponent<VehicleMovement>();
	}

	void OnTriggerEnter(Collider other)
	{
		GameObject collidedObject = null;

		if (other.transform.parent != null)
		{
			collidedObject = other.transform.parent.gameObject;
		}
		else if (other.transform != null)
		{
			collidedObject = other.transform.gameObject;
		}

		if (collidedObject == null)
		{
			return;
		}

		if (other.gameObject.tag == "PowerUpObject")
		{
			HandlePowerUp(collidedObject);
		}

		if (other.gameObject.tag == "Projectile")
		{
			HandleProjectile(collidedObject);
		}
	}

	void OnCollisionEnter(Collision other)
	{
		GameObject otherObject = other.gameObject;

		if (GameObjectHelper.IsACar(otherObject))
		{
			PlayCollisionNoises(otherObject);
		}
	}
	#endregion

	public void AddHealth(float healingValue)
	{
		float newHealth = Health + healingValue;
		Health = Mathf.Clamp(newHealth, 0f, MaxHealth);
	}

	public void ApplyDamage(GameObject attacker, float damage)
	{
		Health -= (1 - Armor / 100f) * damage;

		if (Health <= 0f)
		{
			DoDeathCondition(attacker, 5f);
		}

		ApplySmoke();
	}

	private void ApplySmoke()
	{
		if (SmokeEffect == null) return;

		if (!SmokeEffect.isPlaying && Health > MIN_HEALTH_FOR_SMOKE && Health <= MAX_HEALTH_FOR_SMOKE)
		{
			SmokeEffect.Play();
		}

		if (Health <= 0f || Health > MIN_HEALTH_FOR_SMOKE)
		{
			SmokeEffect.emissionRate = 1f;
			SmokeEffect.Stop();
		}

		float oldRange, newRange;

		oldRange = (MAX_HEALTH_FOR_SMOKE - MIN_HEALTH_FOR_SMOKE);
		newRange = (MAX_SMOKE_RATE - MIN_SMOKE_RATE);
		SmokeEffect.emissionRate = (((Health - MIN_HEALTH_FOR_SMOKE) * newRange) / oldRange) + MIN_SMOKE_RATE;
	}

	public void StartSlip()
	{
		if (_myMovement == null)
		{
			Debug.LogError("VehicleMovement component missing for " + _myGameObject.name);
		}

		_myMovement.StartSlip();
	}

	public void StopSlip(float duration)
	{
		if (_myMovement == null)
		{
			Debug.LogError("VehicleMovement component missing for " + _myGameObject.name);
		}

		_myMovement.StopSlip(duration);
	}

	private void HandlePowerUp(GameObject powerUpHit)
	{
		
		if (powerUpHit.name.Contains("Ammo"))
		{
			ReplenishAmmo(powerUpHit.name.Contains("Front"));
		}
		else if (powerUpHit.name.Contains("HealthPowerUp"))
		{
			ReplenishHealth();
		}
		else if (powerUpHit.name.Contains("MoneyPowerUp"))
		{
			AddMoney();
		}

		PowerUpController powerUpControllerComponent = _gameController.GetComponent<PowerUpController>();
		if (powerUpControllerComponent != null)
		{
			powerUpControllerComponent.KillPowerUp();
			Destroy(powerUpHit);
		}
	}

	private void HandleProjectile(GameObject projectileObject)
	{
		IProjectile projectile = projectileObject.GetComponent<Projectile>();

		if (projectile == null) return;
		if (projectile.Owner == _myGameObject) return;

		projectile.DoCollision(_myGameObject);
	}

	#region Death & Respawn
	public void DoDeathCondition(GameObject attacker, float delayTime)
	{
		if (attacker != null && GameObjectHelper.IsACar(attacker))
		{
			_gameController.DoActionText(string.Format("{0} was killed by {1}", _myGameObject.name, attacker.name));
		}
		else
		{
			_gameController.DoActionText(string.Format("{0} asploded.", _myGameObject.name));
		}

		GameObject explosion = (GameObject)Instantiate(ResourcesHelper.ExplosionEffect, _myGameObject.transform.position, Quaternion.identity);
		Destroy(explosion, 5f);

		DoRespawn(delayTime);

		Health = 100f;
	}

	public void DoRespawn(float delayTime)
	{
		if (!FlaggedForRespawn)
		{
			StartCoroutine("Respawn", delayTime);
		}

		FlaggedForRespawn = true;
	}

	public IEnumerator Respawn(float delayTime)
	{
		ToggleVisibility();
		StartCoroutine(Immunity());

		GameObject cameraObject = GameObject.Find("Main Camera");
		PlayerCamera camera = null;

		if (cameraObject != null)
		{
			camera = cameraObject.GetComponent<PlayerCamera>();
			if (camera.cameraTarget == _myTransform)
			{
				camera.IsAttached = false;
			}
		}

		StateMachine stateMachine = _myGameObject.GetComponent<StateMachine>();

		if (stateMachine != null)
		{
			stateMachine.ChangeGlobalState(Idle.Instance);
		}

		Rigidbody myRigidBody = _myGameObject.rigidbody;

		if (myRigidBody)
		{
			myRigidBody.velocity = Vector3.zero;
			myRigidBody.useGravity = false;
			myRigidBody.Sleep();
		}

		_myTransform.position = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);

		yield return new WaitForSeconds(delayTime);

		VehicleMonitor carMonitor = _myGameObject.GetComponent<VehicleMonitor>();
		if (carMonitor != null)
		{
			carMonitor.VelocityBelowThresholdTime = 0f;
		}

		WaypointController waypointController = _myGameObject.GetComponent<WaypointController>();

		if (waypointController != null)
		{
			Transform previousWaypoint = waypointController.PreviousWaypoint();

			_myTransform.position = previousWaypoint.position + Vector3.up;
			_myTransform.rotation = previousWaypoint.rotation;
		}

		if (myRigidBody)
		{
			myRigidBody.useGravity = true;

			myRigidBody.WakeUp();
			if (myRigidBody.isKinematic == false)
			{
				myRigidBody.velocity = Vector3.zero;
			}
		}

		if (camera != null && camera.cameraTarget == _myTransform)
		{
			camera.IsAttached = true;
		}

		ToggleVisibility();

		if (stateMachine != null)
		{
			stateMachine.ChangeGlobalState(Drive.Instance);
		}

		FlaggedForRespawn = false;
	}

	private void ToggleVisibility()
	{
		List<Renderer> renderers = _myGameObject.GetComponentsInChildren<Renderer>().ToList();

		if (renderers.Count > 0)
		{
			foreach (Renderer r in renderers)
			{
				r.enabled = !r.enabled;
			}
		}
	}

	private void ToggleColliders()
	{
		Transform chassis = _myTransform.FindChild("Chassis");
		if (chassis == null) return;
		List<Collider> colliders = chassis.GetComponentsInChildren<Collider>().ToList();
		foreach (Collider c in colliders.Where(col => col as WheelCollider == null))
		{
			c.enabled = !c.enabled;
		}
	}

	private IEnumerator Immunity()
	{
		ToggleColliders();

		yield return new WaitForSeconds(3f);

		ToggleColliders();
	}
	#endregion

	#region PowerUp Effects
	private void ReplenishAmmo(bool isFront)
	{
		if (PowerUpAudioSource != null && _myGameObject.tag == TagHelper.PLAYER)
		{
			PowerUpAudioSource.PlayOneShot(AmmoUpSound);
		}

		PlayerCombatController playerCombatController = _myGameObject.GetComponent<PlayerCombatController>();
		if (playerCombatController != null)
		{
			playerCombatController.AddAmmo(isFront);
			return;
		}

		CombatController combatController = _myGameObject.GetComponent<CombatController>();
		if (combatController != null)
		{
			combatController.AddAmmo(isFront);
		}
	}

	private void ReplenishHealth()
	{
		if (PowerUpAudioSource != null && _myGameObject.tag == TagHelper.PLAYER)
		{
			PowerUpAudioSource.PlayOneShot(HealthUpSound);
		}

		AddHealth(Random.Range(10f, 30f));
	}

	private void AddMoney()
	{
		if (PowerUpAudioSource != null && _myGameObject.tag == TagHelper.PLAYER)
		{
			PowerUpAudioSource.PlayOneShot(MoneyUpSound);
		}

		_moneyPickedUpSoFar += Random.Range(MIN_MONEY_VALUE, MAX_MONEY_VALUE);
	}

	public void CommitMoney()
	{
		PlayerData data = new PlayerData();
		data.PlayerCash += _moneyPickedUpSoFar;
		data.Save();
	}
	#endregion

	public void PlayCollisionNoises(GameObject other)
	{
		if (_myMovement == null) return;

		if (GameObjectHelper.IsACar(other) || _myMovement.IsOverCrashVelocityThreshold)
		{
			if (CollisionAudioSource != null && !CollisionAudioSource.isPlaying)
			{
				int randomIndex = Random.Range(0, CrashSounds.Length - 1);

				CollisionAudioSource.PlayOneShot(CrashSounds[randomIndex]);
			}
		}
	}
}