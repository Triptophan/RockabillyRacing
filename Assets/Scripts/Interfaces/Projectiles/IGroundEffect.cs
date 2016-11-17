using UnityEngine;
using System.Collections;

public interface IGroundEffect
{
	GameObject GroundEffectPrefab { get; set; }

	void ShowGroundEffect(Collider other);
}