using UnityEngine;

public class Rocket : Projectile, IGroundEffect
{
	private const bool AMMO_USES_GRAVITY = false;

	private const float MAX_LIVE_TIME = 7f;
	private const float PROJECTILE_SPEED = 50f;
	private const float DAMAGE = 45f;

	#region IGroundEffect Properties
	public GameObject GroundEffectPrefab { get; set; }
	#endregion

	#region IProjectile Methods
	public override void Init()
	{
		MaxLiveTime = MAX_LIVE_TIME;
		ProjectileSpeed = PROJECTILE_SPEED;
		Damage = DAMAGE;

		AmmoUsesGravity = AMMO_USES_GRAVITY;

		GroundEffectPrefab = ResourcesHelper.ExplosionEffect;

		base.Init();
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
}