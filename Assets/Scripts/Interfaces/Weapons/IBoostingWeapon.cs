using UnityEngine;
using System.Collections;

public interface IBoostingWeapon
{
	float BoostPercentage { get; set; }
	
	void Boost();
}