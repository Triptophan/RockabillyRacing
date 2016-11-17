using UnityEngine;
using System.Collections;

public class GrapplingHook : SingleFireWeapon
{
	private const float COOLDOWN_TIME = 2f;

	private const int PROJECTILES_PER_LEVEL = 1;

	private const string NAME = "Grappling Hook";

	public override void Init()
	{
		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)FrontWeaponType.GrapplingHook;
		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.GrapplingHookWeapon;
		AmmoPrefab = ResourcesHelper.Hook;

		base.Init();
	}
}