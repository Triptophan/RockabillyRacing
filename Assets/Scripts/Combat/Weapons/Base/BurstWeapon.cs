using UnityEngine;
using System.Collections;

public class BurstWeapon : SingleFireWeapon, IBurstWeapon
{
	#region IBurstWeapon Properties
	public float RateOfFire { get; set; }
	public float BurstRate { get; set; }
	#endregion

	#region IWeapon Methods
	public override IEnumerator Fire(GameObject target)
	{
		if (CanFire)
		{
			float rate = BurstRate > AmmoRemaining ? AmmoRemaining : BurstRate;
			for (int i = 0; i < rate; i++)
			{
				if (target != null)
				{
					if (BarrelFireEffect != null)
					{
						BarrelFireEffect.Play();
					}
					BurstFire(target);
				}

				yield return new WaitForSeconds(RateOfFire);
			}
		}

		yield break;
	}
	#endregion

	#region IBurstWeapon Methods
	public void BurstFire(GameObject target)
	{
		GameObject projectile = (GameObject)GameObject.Instantiate(AmmoPrefab,
														WeaponMount.transform.TransformPoint(AmmoSpawnPoint.position),
														Owner.transform.rotation);

		IProjectile projectileComponent = projectile.GetComponent<Projectile>();

		if (projectileComponent != null)
		{
			projectileComponent.Init();
			projectileComponent.Owner = Owner;

			projectile.rigidbody.velocity = Owner.transform.rigidbody.velocity +
											WeaponMount.TransformDirection(Vector3.forward * projectileComponent.ProjectileSpeed);

			PlayFireSound();

			if (AmmoRemaining > 0)
			{
				AmmoRemaining--;
			}

			GameObject.Destroy(projectile, projectileComponent.MaxLiveTime);
		}
	}
	#endregion
}