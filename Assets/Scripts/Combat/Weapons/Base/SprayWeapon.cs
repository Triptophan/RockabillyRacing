using UnityEngine;
using System.Collections;

public class SprayWeapon : SingleFireWeapon, ISprayWeapon
{
	#region ISprayWeapon Properties
	public float SprayDuration { get; set; }
	#endregion
	
	public override IEnumerator Fire(GameObject target)
	{
		SprayFire();
		
		yield break;
	}
	
	private void SprayFire()
	{
		GameObject projectile = (GameObject)GameObject.Instantiate(AmmoPrefab, 
														WeaponMount.transform.TransformPoint(AmmoSpawnPoint.position), 
														Owner.transform.rotation);
		
		if(projectile == null) return;
		
		IProjectile projectileComponent = projectile.GetComponent<Projectile>();
		
		if(projectileComponent == null) return;
		
		projectileComponent.Init();
		projectileComponent.Owner = Owner;
		projectileComponent.AmmoSpawnPoint = AmmoSpawnPoint;
						
		ParticleSystem p = projectile.GetComponent<ParticleSystem>();
		
		if(p) p.Play();
		
		SprayDuration = p.duration;
		
		WeaponHelper.SetEmmisionDuration(projectileComponent, p);
		
		BoxCollider sprayCollider = projectile.GetComponent<BoxCollider>();
		
		if(sprayCollider)
		{
			sprayCollider.enabled = true;
		}
		
		GameObject.Destroy(projectile.gameObject, SprayDuration);
	}	
}