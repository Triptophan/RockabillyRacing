using UnityEngine;

public interface IProjectile
{
	float MaxLiveTime { get; set; }
	float ProjectileSpeed { get; set; }
	float Damage { get; set; }
	
	bool AmmoUsesGravity { get; set; }
	
	GameObject Owner { get; set; }
	
	Transform AmmoSpawnPoint { get; set; }
	
	void Init();
	void DoCollision(GameObject target);
	void DoDamage(GameObject target);
}