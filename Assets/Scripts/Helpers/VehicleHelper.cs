using System.Collections.Generic;
using System.Linq;

public static class VehicleHelper
{
	public const int MAX_CORNERING_COUPE = 10;
	public const int MAX_ACCELERATION_COUPE = 10;
	public const int MAX_TOP_SPEED_COUPE = 10;
	public const int MAX_ARMOR_COUPE = 10;

	public const int MAX_CORNERING_HOTROD = 10;
	public const int MAX_ACCELERATION_HOTROD = 10;
	public const int MAX_TOP_SPEED_HOTROD = 10;
	public const int MAX_ARMOR_HOTROD = 10;

	public const int MAX_CORNERING_LOWRIDER = 10;
	public const int MAX_ACCELERATION_LOWRIDER = 10;
	public const int MAX_TOP_SPEED_LOWRIDER = 10;
	public const int MAX_ARMOR_LOWRIDER = 10;

	private static List<VehicleUpgrade> _vehicleUpgrades;

	public static bool IsAtMax(UpgradeType upgradeType, VehicleType vehicleType)
	{
		bool isMax = false;
		int abilityScore = 0;

		if (_vehicleUpgrades == null || _vehicleUpgrades.Count == 0)
		{
			GetVehicleUpgrades();
		}

		PlayerData data = new PlayerData();

		switch (upgradeType)
		{
			case UpgradeType.Acceleration:
				abilityScore = data.AccelerationAbilityPoints;
				break;
			case UpgradeType.Armor:
				abilityScore = data.ArmorAbilityPoints;
				break;
			case UpgradeType.Cornering:
				abilityScore = data.CorneringAbilityPoints;
				break;
			case UpgradeType.TopSpeed:
				abilityScore = data.TopSpeedAbilityPoints;
				break;
		}

		VehicleUpgrade vehicleUpgrade = _vehicleUpgrades.First(vu => vu.Type == vehicleType);
		Upgrade<UpgradeType> upgrade = vehicleUpgrade.MaxUpgradeLevels.First(mul => mul.Type == upgradeType);

		isMax = upgrade.MaxLevel <= abilityScore;

		return isMax;
	}

	public static void PurchaseUpgrade(UpgradeType upgrade, VehicleType vehicle, int cost)
	{
		if (!IsAtMax(upgrade, vehicle))
		{
			MoneyHelper.MakePurchase(cost);
			AbilityScoreHelper.UpgradeAbility(upgrade);
		}
	}

	private static List<VehicleUpgrade> GetVehicleUpgrades()
	{
		if (_vehicleUpgrades == null || _vehicleUpgrades.Count == 0)
		{
			_vehicleUpgrades = new List<VehicleUpgrade>
			{
				new VehicleUpgrade(VehicleType.Coupe)
				{
					MaxUpgradeLevels = new List<Upgrade<UpgradeType>>
					{
						new Upgrade<UpgradeType>(UpgradeType.Acceleration, MAX_ACCELERATION_COUPE),
						new Upgrade<UpgradeType>(UpgradeType.Armor, MAX_ARMOR_COUPE),
						new Upgrade<UpgradeType>(UpgradeType.Cornering, MAX_CORNERING_COUPE),
						new Upgrade<UpgradeType>(UpgradeType.TopSpeed, MAX_TOP_SPEED_COUPE)
					}
				},
				new VehicleUpgrade(VehicleType.HotRod)
				{
					MaxUpgradeLevels = new List<Upgrade<UpgradeType>>
					{
						new Upgrade<UpgradeType>(UpgradeType.Acceleration, MAX_ACCELERATION_HOTROD),
						new Upgrade<UpgradeType>(UpgradeType.Armor, MAX_ARMOR_HOTROD),
						new Upgrade<UpgradeType>(UpgradeType.Cornering, MAX_CORNERING_HOTROD),
						new Upgrade<UpgradeType>(UpgradeType.TopSpeed, MAX_TOP_SPEED_HOTROD)
					}
				},
				new VehicleUpgrade(VehicleType.LowRider)
				{
					MaxUpgradeLevels = new List<Upgrade<UpgradeType>>
					{
						new Upgrade<UpgradeType>(UpgradeType.Acceleration, MAX_ACCELERATION_LOWRIDER),
						new Upgrade<UpgradeType>(UpgradeType.Armor, MAX_ARMOR_LOWRIDER),
						new Upgrade<UpgradeType>(UpgradeType.Cornering, MAX_CORNERING_LOWRIDER),
						new Upgrade<UpgradeType>(UpgradeType.TopSpeed, MAX_TOP_SPEED_LOWRIDER)
					}
				}
			};
		}

		return _vehicleUpgrades;
	}
}