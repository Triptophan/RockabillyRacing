using UnityEngine;
using System.Collections;

public interface IWeapon
{
	bool CanFire { get; set; }

	string WeaponName { get; set; }
	string ProjectilePrefabName { get; set; }
	string WeaponPrefabName { get; set; }

	int Capacity { get; set; }
	int ProjectilesPerLevel { get; set; }
	int AmmoAbilityLevel { get; set; }
	int AmmoRemaining { get; set; }
	int WeaponType { get; set; }

	float CooldownTime { get; set; }
	float Damage { get; set; }

	GameObject WeaponPrefab { get; set; }
	GameObject AmmoPrefab { get; set; }
	GameObject Owner { get; set; }

	Transform WeaponMount { get; set; }
	Transform AmmoSpawnPoint { get; set; }

	AudioSource WeaponSound { get; set; }

	IEnumerator Fire(GameObject target);

	void Init();

	void Reload();

	void AddAmmo();

	void PlayFireSound();
}