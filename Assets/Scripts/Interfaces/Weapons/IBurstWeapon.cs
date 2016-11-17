using UnityEngine;

public interface IBurstWeapon
{
	float RateOfFire { get; set; }	
	float BurstRate { get; set; }
	
	void BurstFire(GameObject target);
}