using UnityEngine;
using System.Collections;

public class Flamethrower : SprayWeapon
{
	private const float COOLDOWN_TIME = 5f;

	private const int PROJECTILES_PER_LEVEL = 1;

	private const string NAME = "Flamethrower";

	public override void Init()
	{
		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)FrontWeaponType.Flamethrower;

		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.FlamethrowerWeapon;
		AmmoPrefab = ResourcesHelper.FlamesEffect;

		base.Init();
	}
}