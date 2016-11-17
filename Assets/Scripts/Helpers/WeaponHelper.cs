using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class WeaponHelper
{
	#region Constants
	private const string AMMO_SPAWN_NAME = "AmmoSpawn";
	private const string WEAPON_MOUNT_NAME = "WeaponMount";

	private const int ROCKET_LAUNCHER_WEAPON_COST = 75000;
	private const int OOZE_GUN_WEAPON_COST = 68000;
	private const int HOMING_ROCKET_LAUNCHER_WEAPON_COST = 125000;
	private const int FLAMETHROWER_WEAPON_COST = 100000;
	private const int GRAPPLING_HOOK_WEAPON_COST = 50000;

	private const int FORCEFIELD_WEAPON_COST = 100000;
	private const int FIREBOMB_WEAPON_COST = 80000;
	private const int NITRO_BOOSTER_WEAPON_COST = 65000;
	private const int LANDMINE_WEAPON_COST = 55000;
	private const int SCATTERJACK_WEAPON_COST = 75000;

	#region Ammo Costs
	private const int MACHINE_GUN_AMMO_BASE_COST = 2500;
	private const int MACHINE_GUN_AMMO_MULTIPLIER_COST = 2500;
	private const int ROCKET_LAUNCHER_AMMO_BASE_COST = 7500;
	private const int ROCKET_LAUNCHER_AMMO_MULTIPLIER_COST = 2500;
	private const int OOZE_GUN_AMMO_BASE_COST = 3000;
	private const int OOZE_GUN_AMMO_MULTIPLIER_COST = 3000;
	private const int HOMING_ROCKET_LAUNCHER_AMMO_BASE_COST = 5000;
	private const int HOMING_ROCKET_LAUNCHER_AMMO_MULTIPLIER_COST = 2500;
	private const int FLAMETHROWER_AMMO_BASE_COST = 2000;
	private const int FLAMETHROWER_AMMO_MULTIPLIER_COST = 2500;
	private const int GRAPPLING_HOOK_AMMO_BASE_COST = 1000;
	private const int GRAPPLING_HOOK_AMMO_MULTIPLIER_COST = 1000;

	private const int OIL_SLICK_AMMO_BASE_COST = 2500;
	private const int OIL_SLICK_AMMO_MULTIPLIER_COST = 2500;
	private const int FIREBOMB_AMMO_BASE_COST = 7500;
	private const int FIREBOMB_AMMO_MULTIPLIER_COST = 2500;
	private const int NITRO_BOOSTER_AMMO_BASE_COST = 3000;
	private const int NITRO_BOOSTER_AMMO_MULTIPLIER_COST = 3000;
	private const int LANDMINE_AMMO_BASE_COST = 5000;
	private const int LANDMINE_AMMO_MULTIPLIER_COST = 2500;
	private const int SCATTERJACK_AMMO_BASE_COST = 2000;
	private const int SCATTERJACK_AMMO_MULTIPLIER_COST = 2500;
	private const int FORCE_FIELD_AMMO_BASE_COST = 1000;
	private const int FORCE_FIELD_AMMO_MULTIPLIER_COST = 1000;
	#endregion

	private static List<WeaponUpgrade<FrontWeaponType>> _frontWeaponUpgrades;
	private static List<WeaponUpgrade<RearWeaponType>> _rearWeaponUpgrades;
	#endregion

	#region Public Methods
	public static Transform GetAmmoSpawn(GameObject parent)
	{
		if (!parent) return null;

		return GetAmmoSpawn(parent.transform);
	}

	public static Transform GetAmmoSpawn(Transform parent)
	{
		if (!parent) return null;

		foreach (Transform child in parent)
		{
			if (child.name == AMMO_SPAWN_NAME)
			{
				return child;
			}
		}

		return null;
	}

	public static Transform GetWeaponMount(Transform parent, bool isFront)
	{
		string mountName = (isFront ? "Front" : "Rear") + WEAPON_MOUNT_NAME;

		return parent.FindChild(mountName);
	}

	public static IWeapon SetWeapon(FrontWeaponType type)
	{
		switch (type)
		{
			case FrontWeaponType.MachineGun:
				return new MachineGun();
			case FrontWeaponType.RocketLauncher:
				return new RocketLauncher();
			case FrontWeaponType.OozeGun:
				return new OozeGun();
			case FrontWeaponType.HomingRocketLauncher:
				return new HomingRocketLauncher();
			case FrontWeaponType.Flamethrower:
				return new Flamethrower();
			case FrontWeaponType.GrapplingHook:
				return new GrapplingHook();
			default:
				return null;
		}
	}

	public FrontWeaponType PickFrontWeapon()
	{
		FrontWeaponType[] values = (FrontWeaponType[])System.Enum.GetValues(typeof(FrontWeaponType));

		int randomValue = Random.Range(1, values.Length);

		return values[randomValue];
	}

	public RearWeaponType PickRearWeapon()
	{
		RearWeaponType[] values = (RearWeaponType[])System.Enum.GetValues(typeof(RearWeaponType));

		int randomValue = Random.Range(1, values.Length);

		return values[randomValue];
	}

	public static IWeapon SetWeapon(RearWeaponType type)
	{
		switch (type)
		{
			case RearWeaponType.OilSlick:
				return new OilSlick();
			case RearWeaponType.Landmines:
				return new SingleMines();
			case RearWeaponType.NitroBooster:
				return new Booster();
			case RearWeaponType.ScatterJack:
				return new ScatterJacks();
			case RearWeaponType.Firebomb:
				return new Firebomb();
			case RearWeaponType.ForceField:
				return new ForceFieldGenerator();
			default:
				return null;
		}
	}

	public static void SetEmmisionDuration(IProjectile projectileComponent, ParticleSystem particleSystem)
	{
		ISprayEmission sprayEmission = projectileComponent as ISprayEmission;
		if (sprayEmission != null)
		{
			sprayEmission.Duration = particleSystem.duration;
		}
	}

	public static int GetWeaponCost(FrontWeaponType weaponType)
	{
		switch (weaponType)
		{
			case FrontWeaponType.RocketLauncher:
				return ROCKET_LAUNCHER_WEAPON_COST;
			case FrontWeaponType.OozeGun:
				return OOZE_GUN_WEAPON_COST;
			case FrontWeaponType.HomingRocketLauncher:
				return HOMING_ROCKET_LAUNCHER_WEAPON_COST;
			case FrontWeaponType.Flamethrower:
				return FLAMETHROWER_WEAPON_COST;
			case FrontWeaponType.GrapplingHook:
				return GRAPPLING_HOOK_WEAPON_COST;
			default:
				return 0;
		}
	}

	public static int GetAmmoCost(FrontWeaponType weaponType)
	{
		PlayerData data = new PlayerData();
		switch (weaponType)
		{
			case FrontWeaponType.MachineGun:
				return CalculateCost(data.MachineGunAbilityPoints, MACHINE_GUN_AMMO_BASE_COST, MACHINE_GUN_AMMO_MULTIPLIER_COST);
			case FrontWeaponType.RocketLauncher:
				return CalculateCost(data.RocketLauncherAbilityPoints, ROCKET_LAUNCHER_AMMO_BASE_COST, ROCKET_LAUNCHER_AMMO_MULTIPLIER_COST);
			case FrontWeaponType.OozeGun:
				return CalculateCost(data.OozeGunAbilityPoints, OOZE_GUN_AMMO_BASE_COST, OOZE_GUN_AMMO_MULTIPLIER_COST);
			case FrontWeaponType.Flamethrower:
				return CalculateCost(data.FlameThrowerAbilityPoints, FLAMETHROWER_AMMO_BASE_COST, FLAMETHROWER_AMMO_MULTIPLIER_COST);
			case FrontWeaponType.GrapplingHook:
				return CalculateCost(data.GrapplingHookAbilityPoints, GRAPPLING_HOOK_AMMO_BASE_COST, GRAPPLING_HOOK_AMMO_MULTIPLIER_COST);
			case FrontWeaponType.HomingRocketLauncher:
				return CalculateCost(data.HomingRocketLauncherAbilityPoints, HOMING_ROCKET_LAUNCHER_AMMO_BASE_COST, HOMING_ROCKET_LAUNCHER_AMMO_MULTIPLIER_COST);
			default:
				return 0;
		}
	}

	public static int GetWeaponCost(RearWeaponType weaponType)
	{
		switch (weaponType)
		{
			case RearWeaponType.Landmines:
				return 55000;
			case RearWeaponType.NitroBooster:
				return 65000;
			case RearWeaponType.ScatterJack:
				return 75000;
			case RearWeaponType.Firebomb:
				return 80000;
			case RearWeaponType.ForceField:
				return 100000;
			default:
				return 0;
		}
	}

	public static int GetAmmoCost(RearWeaponType weaponType)
	{
		PlayerData data = new PlayerData();
		switch (weaponType)
		{
			case RearWeaponType.OilSlick:
				return CalculateCost(data.OilSlickAbilityPoints, OIL_SLICK_AMMO_BASE_COST, OIL_SLICK_AMMO_MULTIPLIER_COST);
			case RearWeaponType.Landmines:
				return CalculateCost(data.LandMineAbilityPoints, LANDMINE_AMMO_BASE_COST, ROCKET_LAUNCHER_AMMO_MULTIPLIER_COST);
			case RearWeaponType.NitroBooster:
				return CalculateCost(data.NitroBoosterAbilityPoints, NITRO_BOOSTER_AMMO_BASE_COST, NITRO_BOOSTER_AMMO_MULTIPLIER_COST);
			case RearWeaponType.ScatterJack:
				return CalculateCost(data.ScatterJackAbilityPoints, SCATTERJACK_AMMO_BASE_COST, SCATTERJACK_AMMO_MULTIPLIER_COST);
			case RearWeaponType.Firebomb:
				return CalculateCost(data.FireBombAbilityPoints, FIREBOMB_AMMO_BASE_COST, FIREBOMB_AMMO_MULTIPLIER_COST);
			case RearWeaponType.ForceField:
				return CalculateCost(data.ForceFieldAbilityPoints, FORCE_FIELD_AMMO_BASE_COST, FORCE_FIELD_AMMO_MULTIPLIER_COST);
			default:
				return 0;
		}
	}

	public static int GetAbilityLevel(FrontWeaponType weaponType)
	{
		PlayerData data = new PlayerData();
		switch (weaponType)
		{
			case FrontWeaponType.RocketLauncher:
				return data.RocketLauncherAbilityPoints;
			case FrontWeaponType.OozeGun:
				return data.OozeGunAbilityPoints;
			case FrontWeaponType.HomingRocketLauncher:
				return data.HomingRocketLauncherAbilityPoints;
			case FrontWeaponType.Flamethrower:
				return data.FlameThrowerAbilityPoints;
			case FrontWeaponType.GrapplingHook:
				return data.GrapplingHookAbilityPoints;
		}

		return data.MachineGunAbilityPoints;
	}

	public static int GetAbilityLevel(RearWeaponType weaponType)
	{
		PlayerData data = new PlayerData();
		switch (weaponType)
		{
			case RearWeaponType.Landmines:
				return data.LandMineAbilityPoints;
			case RearWeaponType.NitroBooster:
				return data.NitroBoosterAbilityPoints;
			case RearWeaponType.ScatterJack:
				return data.ScatterJackAbilityPoints;
			case RearWeaponType.Firebomb:
				return data.FireBombAbilityPoints;
			case RearWeaponType.ForceField:
				return data.ForceFieldAbilityPoints;
		}

		return data.OilSlickAbilityPoints;
	}

	public static bool IsAtMax(FrontWeaponType upgradeType)
	{
		bool isMax = false;
		int abilityScore = 0;

		if (_frontWeaponUpgrades == null || _frontWeaponUpgrades.Count == 0)
		{
			GetAmmoUpgrades();
		}

		PlayerData data = new PlayerData();
		switch (upgradeType)
		{
			case FrontWeaponType.Flamethrower:
				abilityScore = data.FlameThrowerAbilityPoints;
				break;
			case FrontWeaponType.GrapplingHook:
				abilityScore = data.GrapplingHookAbilityPoints;
				break;
			case FrontWeaponType.HomingRocketLauncher:
				abilityScore = data.HomingRocketLauncherAbilityPoints;
				break;
			case FrontWeaponType.MachineGun:
				abilityScore = data.MachineGunAbilityPoints;
				break;
			case FrontWeaponType.OozeGun:
				abilityScore = data.OozeGunAbilityPoints;
				break;
			case FrontWeaponType.RocketLauncher:
				abilityScore = data.RocketLauncherAbilityPoints;
				break;
		}

		WeaponUpgrade<FrontWeaponType> weaponUpgrade = _frontWeaponUpgrades.First(vu => vu.Type == upgradeType);
		Upgrade<FrontWeaponType> upgrade = weaponUpgrade.MaxUpgradeLevels.First(mul => mul.Type == upgradeType);

		isMax = upgrade.MaxLevel <= abilityScore;

		return isMax;
	}

	public static bool IsAtMax(RearWeaponType upgradeType)
	{
		bool isMax = false;
		int abilityScore = 0;

		if (_rearWeaponUpgrades == null || _rearWeaponUpgrades.Count == 0)
		{
			GetAmmoUpgrades();
		}

		PlayerData data = new PlayerData();
		switch (upgradeType)
		{
			case RearWeaponType.OilSlick:
				abilityScore = data.OilSlickAbilityPoints;
				break;
			case RearWeaponType.Firebomb:
				abilityScore = data.FireBombAbilityPoints;
				break;
			case RearWeaponType.NitroBooster:
				abilityScore = data.NitroBoosterAbilityPoints;
				break;
			case RearWeaponType.Landmines:
				abilityScore = data.LandMineAbilityPoints;
				break;
			case RearWeaponType.ScatterJack:
				abilityScore = data.ScatterJackAbilityPoints;
				break;
			case RearWeaponType.ForceField:
				abilityScore = data.ForceFieldAbilityPoints;
				break;
		}

		WeaponUpgrade<RearWeaponType> weaponUpgrade = _rearWeaponUpgrades.First(vu => vu.Type == upgradeType);
		Upgrade<RearWeaponType> upgrade = weaponUpgrade.MaxUpgradeLevels.First(mul => mul.Type == upgradeType);

		isMax = upgrade.MaxLevel <= abilityScore;

		return isMax;
	}

	public static void PurchaseUpgrade(bool ownsWeapon, FrontWeaponType weaponType, int cost)
	{
		MoneyHelper.MakePurchase(cost);
		PlayerData data = new PlayerData();
		if (ownsWeapon)
		{
			switch (weaponType)
			{
				case FrontWeaponType.Flamethrower:
					data.FlameThrowerAbilityPoints++;
					break;
				case FrontWeaponType.GrapplingHook:
					data.GrapplingHookAbilityPoints++;
					break;
				case FrontWeaponType.HomingRocketLauncher:
					data.HomingRocketLauncherAbilityPoints++;
					break;
				case FrontWeaponType.MachineGun:
					data.MachineGunAbilityPoints++;
					break;
				case FrontWeaponType.OozeGun:
					data.OozeGunAbilityPoints++;
					break;
				case FrontWeaponType.RocketLauncher:
					data.RocketLauncherAbilityPoints++;
					break;
			}
		}
		else
		{
			switch (weaponType)
			{
				case FrontWeaponType.Flamethrower:
					data.OwnsFlamethrower = true;
					break;
				case FrontWeaponType.GrapplingHook:
					data.OwnsGrapplingHook = true;
					break;
				case FrontWeaponType.HomingRocketLauncher:
					data.OwnsHomingRocketLauncher = true;
					break;
				case FrontWeaponType.MachineGun:
					data.OwnsMachineGun = true;
					break;
				case FrontWeaponType.OozeGun:
					data.OwnsOozeCannon = true;
					break;
				case FrontWeaponType.RocketLauncher:
					data.OwnsRocketLauncher = true;
					break;
			}
		}

		data.Save();
	}

	public static void PurchaseUpgrade(bool ownsWeapon, RearWeaponType weaponType, int cost)
	{
		MoneyHelper.MakePurchase(cost);
		PlayerData data = new PlayerData();
		if (ownsWeapon)
		{
			switch (weaponType)
			{
				case RearWeaponType.OilSlick:
					data.OilSlickAbilityPoints++;
					break;
				case RearWeaponType.Firebomb:
					data.FireBombAbilityPoints++;
					break;
				case RearWeaponType.NitroBooster:
					data.NitroBoosterAbilityPoints++;
					break;
				case RearWeaponType.Landmines:
					data.LandMineAbilityPoints++;
					break;
				case RearWeaponType.ScatterJack:
					data.ScatterJackAbilityPoints++;
					break;
				case RearWeaponType.ForceField:
					data.ForceFieldAbilityPoints++;
					break;
			}
		}
		else
		{
			switch (weaponType)
			{
				case RearWeaponType.OilSlick:
					data.OwnsOilSlicks = true;
					break;
				case RearWeaponType.Firebomb:
					data.OwnsFirebomb = true;
					break;
				case RearWeaponType.NitroBooster:
					data.OwnsNitroBooster = true;
					break;
				case RearWeaponType.Landmines:
					data.OwnsLandmines = true;
					break;
				case RearWeaponType.ScatterJack:
					data.OwnsScatterJack = true;
					break;
				case RearWeaponType.ForceField:
					data.OwnsForcefield = true;
					break;
			}
		}

		data.Save();
	}

	public static void ResetWeaponOwnership()
	{
		PlayerData data = new PlayerData();
		data.PlayerFrontWeapon = (int)FrontWeaponType.MachineGun;
		data.OwnsMachineGun = true;
		data.OwnsRocketLauncher = false;
		data.OwnsOozeCannon = false;
		data.OwnsHomingRocketLauncher = false;
		data.OwnsFlamethrower = false;
		data.OwnsGrapplingHook = false;

		data.PlayerRearWeapon = (int)RearWeaponType.OilSlick;
		data.OwnsOilSlicks = true;
		data.OwnsLandmines = false;
		data.OwnsNitroBooster = false;
		data.OwnsScatterJack = false;
		data.OwnsFirebomb = false;
		data.OwnsForcefield = false;

		data.Save();
	}

	public static void ResetWeaponUpgrades()
	{
		PlayerData data = new PlayerData();
		data.MachineGunAbilityPoints = 0;
		data.RocketLauncherAbilityPoints = 0;
		data.OozeGunAbilityPoints = 0;
		data.HomingRocketLauncherAbilityPoints = 0;
		data.FlameThrowerAbilityPoints = 0;
		data.GrapplingHookAbilityPoints = 0;

		data.OilSlickAbilityPoints = 0;
		data.FireBombAbilityPoints = 0;
		data.NitroBoosterAbilityPoints = 0;
		data.LandMineAbilityPoints = 0;
		data.ScatterJackAbilityPoints = 0;
		data.ForceFieldAbilityPoints = 0;

		data.Save();
	}
	#endregion

	#region Private Methods
	private static void GetAmmoUpgrades()
	{
		if (_frontWeaponUpgrades == null || _frontWeaponUpgrades.Count == 0)
		{
			_frontWeaponUpgrades = new List<WeaponUpgrade<FrontWeaponType>>
			{
				new WeaponUpgrade<FrontWeaponType>(FrontWeaponType.MachineGun),
				new WeaponUpgrade<FrontWeaponType>(FrontWeaponType.RocketLauncher),
				new WeaponUpgrade<FrontWeaponType>(FrontWeaponType.OozeGun),
				new WeaponUpgrade<FrontWeaponType>(FrontWeaponType.Flamethrower),
				new WeaponUpgrade<FrontWeaponType>(FrontWeaponType.GrapplingHook),
				new WeaponUpgrade<FrontWeaponType>(FrontWeaponType.HomingRocketLauncher)
			};

			foreach (WeaponUpgrade<FrontWeaponType> weaponUpgrade in _frontWeaponUpgrades)
			{
				foreach (Upgrade<FrontWeaponType> upgrade in weaponUpgrade.MaxUpgradeLevels)
				{
					upgrade.MaxLevel = 3;
				}
			}
		}

		if (_rearWeaponUpgrades == null || _rearWeaponUpgrades.Count == 0)
		{
			_rearWeaponUpgrades = new List<WeaponUpgrade<RearWeaponType>>
			{
				new WeaponUpgrade<RearWeaponType>(RearWeaponType.OilSlick),
				new WeaponUpgrade<RearWeaponType>(RearWeaponType.Firebomb),
				new WeaponUpgrade<RearWeaponType>(RearWeaponType.NitroBooster),
				new WeaponUpgrade<RearWeaponType>(RearWeaponType.Landmines),
				new WeaponUpgrade<RearWeaponType>(RearWeaponType.ScatterJack),
				new WeaponUpgrade<RearWeaponType>(RearWeaponType.ForceField)
			};

			foreach (WeaponUpgrade<RearWeaponType> weaponUpgrade in _rearWeaponUpgrades)
			{
				foreach (Upgrade<RearWeaponType> upgrade in weaponUpgrade.MaxUpgradeLevels)
				{
					upgrade.MaxLevel = 3;
				}
			}
		}
	}

	private static int CalculateCost(int ablityScore, int baseCost, int multiplier)
	{
		return baseCost + (ablityScore * multiplier);
	}
	#endregion
}