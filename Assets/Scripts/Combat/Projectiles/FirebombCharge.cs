using UnityEngine;
using System.Collections;

public class FirebombCharge : Projectile, IGroundEffect
{
	private const bool AMMO_USES_GRAVITY = true;

	private const float MAX_LIVE_TIME = 300f;
	private const float DAMAGE = 20f;

	#region IGroundEffect Properties
	public GameObject GroundEffectPrefab { get; set; }
	#endregion

	#region IProjectile Methods
	public override void Init()
	{
		MaxLiveTime = MAX_LIVE_TIME;
		Damage = DAMAGE;

		AmmoUsesGravity = AMMO_USES_GRAVITY;

		GroundEffectPrefab = ResourcesHelper.FirebombPack;

		base.Init();
	}
	#endregion

	#region IGroundEffect Methods
	public void ShowGroundEffect(Collider other)
	{
		Transform otherTransform = other.transform;

		GameObject pack = (GameObject)Instantiate(GroundEffectPrefab,
													otherTransform.position,
													Owner.transform.rotation);

		if (pack != null)
		{
			FirebombPack packComponent = pack.GetComponent<FirebombPack>();

			if (packComponent != null)
			{
				packComponent.Damage = DAMAGE;
				packComponent.Owner = Owner;
			}
		}

		GameObject.Destroy(gameObject);
	}
	#endregion
}