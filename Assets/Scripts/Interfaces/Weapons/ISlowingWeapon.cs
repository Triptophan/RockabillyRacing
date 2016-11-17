using UnityEngine;
using System.Collections;

public interface ISlowingWeapon
{
	float SlowPercentage { get; set; }
	
	void SlowTarget();
}