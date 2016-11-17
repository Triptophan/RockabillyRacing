using UnityEngine;
using System.Collections;

public class FirebombEffect : Projectile
{
	private const float SPEED = 20f;
	private const float MAX_LIVE_TIME = 3f;
	private const float BURN_DURATION = 2f;
	private const float DAMAGE = 5f;

	private GameObject _burningPrefab;

	void Update()
	{
		if (_myTransform != null)
		{
			_myTransform.position += -transform.forward * Time.deltaTime * SPEED;
		}
	}

	public override void Init()
	{
		MaxLiveTime = MAX_LIVE_TIME;
		ProjectileSpeed = SPEED;
		Damage = DAMAGE;

		AmmoUsesGravity = false;

		_burningPrefab = ResourcesHelper.BurningEffect;

		if (_burningPrefab != null)
		{
			WeaponHelper.SetEmmisionDuration(this, _burningPrefab.GetComponent<ParticleSystem>());
		}

		Destroy(_myGameObject, 5f);

		base.Init();
	}

	public override void DoCollision(GameObject target)
	{
		if (GameObjectHelper.IsACar(target))
		{
			FlameToken existingToken = target.GetComponent<FlameToken>();

			if (existingToken == null)
			{
				FlameToken fireDamage = target.AddComponent<FlameToken>();

				if (!fireDamage) return;

				fireDamage.Duration = BURN_DURATION;
				fireDamage.DamagePerTick = DAMAGE;
				fireDamage.Owner = Owner;

				GameObject burningEffect = (GameObject)Instantiate(_burningPrefab,
																	target.transform.position,
																	_burningPrefab.transform.rotation);
				burningEffect.transform.parent = target.transform;

				Destroy(burningEffect, BURN_DURATION);
			}
		}
	}
}