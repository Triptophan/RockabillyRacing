using UnityEngine;

public class HomingProjectile : Projectile, IHomingProjectile, IGroundEffect
{
	private const bool AMMO_USES_GRAVITY = false;

	private const float MAX_LIVE_TIME = 7f;
	private const float PROJECTILE_SPEED = 40f;
	private const float DAMAGE = 35f;

	private SteeringBehavior _steeringBehavior;

	#region IGroundEffect Properties
	public GameObject GroundEffectPrefab { get; set; }
	#endregion

	#region IHomingProjectile Properties
	public GameObject Target { get; set; }
	#endregion

	public GameObject waypoints;

	#region IProjectile Methods
	public override void Init()
	{
		MaxLiveTime = MAX_LIVE_TIME;
		ProjectileSpeed = PROJECTILE_SPEED;
		Damage = DAMAGE;

		AmmoUsesGravity = AMMO_USES_GRAVITY;

		GroundEffectPrefab = ResourcesHelper.ExplosionEffect;

		base.Init();

		_steeringBehavior = _myGameObject.AddComponent<SteeringBehavior>();

		_steeringBehavior.PursuitOn();
		_steeringBehavior.WallAvoidanceOff();

		_steeringBehavior.maxSpeed = PROJECTILE_SPEED;
	}

	public override void DoCollision(GameObject target)
	{
		if (target == _myGameObject) return;

		Collider collider = target.GetComponentInChildren<BoxCollider>();

		if (collider == null)
		{
			Debug.LogError(target.name + " does not have a collider attached or one cannot be found.");
			return;
		}

		ShowGroundEffect(collider);

		base.DoCollision(target);
	}
	#endregion

	#region IGroundEffect Methods
	public void ShowGroundEffect(Collider other)
	{
		GameObject effectObject = (GameObject)GameObject.Instantiate(GroundEffectPrefab,
																		other.transform.position,
																		GroundEffectPrefab.transform.rotation);

		if (effectObject)
		{
			ParticleSystem effect = effectObject.GetComponent<ParticleSystem>();

			if (effect != null)
			{
				GameObject.Destroy(effectObject, effect.duration);
			}
		}
	}
	#endregion

	void FixedUpdate()
	{
		if (Target)
		{
			if (_steeringBehavior.currentTarget == null)
			{
				_steeringBehavior.currentTarget = Target;
			}

			Vector3 steering = _steeringBehavior.Calculate();
			Vector3 steeringNormalized = Vector3.Normalize(steering);
			Vector3 newVelocity = steeringNormalized * _steeringBehavior.maxSpeed;

			Transform mesh = _myTransform.FindChild("HomingRocketMesh");
			if (mesh != null)
			{
				mesh.rotation = Quaternion.LookRotation(steering);
			}

			_myTransform.rigidbody.velocity = newVelocity;
		}
	}
}