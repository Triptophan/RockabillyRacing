using UnityEngine;

public class OilSlickCharge : Projectile, IGroundEffect
{
	private const bool AMMO_USES_GRAVITY = true;

	private const float MAX_LIVE_TIME = 5f;
	private const float PROJECTILE_SPEED = 1f;
	private const float DAMAGE = 0f;

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

		GroundEffectPrefab = ResourcesHelper.OilSlickPuddle;

		base.Init();
	}
	#endregion

	#region IGroundEffect Methods
	public void ShowGroundEffect(Collider other)
	{
		Transform otherTransform = other.transform;

		Instantiate(GroundEffectPrefab,
					otherTransform.TransformPoint(AmmoSpawnPoint.position),
					GroundEffectPrefab.transform.rotation);

		GameObject.Destroy(gameObject);
	}
	#endregion
}