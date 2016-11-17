using UnityEngine;
using System.Collections;

public interface ICombatController
{
	void SetWeapon(FrontWeaponType type);
	void SetWeapon(RearWeaponType type);
	void ReloadWeapons();
	bool CanFire { set; }
}

