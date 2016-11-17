using UnityEngine;
using System.Collections;

public class SingleMinesCharge : Projectile, IGroundEffect
{
	private const bool AMMO_USES_GRAVITY = true;

	private const float MAX_LIVE_TIME = 300f;
	private const float PROJECTILE_SPEED = 1f;
	private const float DAMAGE = 25f;

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

		GroundEffectPrefab = ResourcesHelper.SingleMine;

		base.Init();
	}
	#endregion

	#region IGroundEffect Methods
	public void ShowGroundEffect(Collider other)
	{
		Transform otherTransform = other.transform;

		GameObject mine = (GameObject)Instantiate(GroundEffectPrefab, otherTransform.position, AmmoSpawnPoint.rotation);

		if (mine != null)
		{
			SingleMine mineComponent = mine.GetComponent<SingleMine>();

			if (mineComponent != null)
			{
				mineComponent.Damage = DAMAGE;
				mineComponent.Owner = Owner;
			}
		}

		GameObject.Destroy(gameObject);
	}
	#endregion
}