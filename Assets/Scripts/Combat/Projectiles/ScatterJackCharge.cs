using UnityEngine;
using System.Collections;

public class ScatterJackCharge : Projectile, IGroundEffect
{
	private const bool AMMO_USES_GRAVITY = true;

	private const float MAX_LIVE_TIME = 300f;
	private const float PROJECTILE_SPEED = 1f;
	private const float DAMAGE = 5f;

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

		GroundEffectPrefab = ResourcesHelper.ScatterJack;

		base.Init();
	}
	#endregion

	#region IGroundEffect Methods
	public void ShowGroundEffect(Collider other)
	{
		Vector3 newPosition = other.transform.position;
		GameObject pack = (GameObject)Instantiate(GroundEffectPrefab, newPosition, GroundEffectPrefab.transform.rotation);

		if (pack != null)
		{
			SingleMine[] mineComponents = pack.GetComponentsInChildren<SingleMine>();

			if (mineComponents != null && mineComponents.Length > 0)
			{
				foreach (SingleMine mine in mineComponents)
				{
					mine.Damage = Damage;
					mine.Owner = Owner;
				}
			}
		}

		GameObject.Destroy(gameObject);
	}
	#endregion
}