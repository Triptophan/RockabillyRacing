using UnityEngine;

public class FlamethrowerFlames : Projectile, ISprayEmission
{
	public float Duration { get; set; }

	private const float MAX_LIVE_TIME = 5f;
	private const float PROJECTILE_SPEED = 0f;
	private const float DAMAGE = 5f;

	private const bool AMMO_USES_GRAVITY = false;

	private static GameObject _burningPrefab;

	void Update()
	{
		Vector3 newPosition = Owner.transform.TransformPoint(AmmoSpawnPoint.position) + (Owner.transform.forward * 2f);
		_myTransform.position = newPosition;
		_myTransform.rotation = Owner.transform.rotation;
	}

	public override void Init()
	{
		MaxLiveTime = MAX_LIVE_TIME;
		ProjectileSpeed = PROJECTILE_SPEED;
		Damage = DAMAGE;

		AmmoUsesGravity = AMMO_USES_GRAVITY;

		_burningPrefab = ResourcesHelper.BurningEffect;

		if (_burningPrefab != null)
		{
			WeaponHelper.SetEmmisionDuration(this, _burningPrefab.GetComponent<ParticleSystem>());
		}

		base.Init();
	}

	public override void DoCollision(GameObject target)
	{
		FlameToken existingToken = target.GetComponent<FlameToken>();

		if (existingToken == null)
		{
			FlameToken fireDamage = target.AddComponent<FlameToken>();

			if (!fireDamage) return;

			fireDamage.Duration = Duration;
			fireDamage.DamagePerTick = Damage;
			fireDamage.Owner = Owner;

			GameObject burningEffect = (GameObject)Instantiate(_burningPrefab, target.transform.position, _burningPrefab.transform.rotation);
			burningEffect.transform.parent = target.transform;

			Destroy(burningEffect, Duration);
		}
	}
}