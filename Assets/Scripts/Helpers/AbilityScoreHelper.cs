
public static class AbilityScoreHelper
{
	private static PlayerData _data = new PlayerData();

	#region Private Constants
	private const int BASE_CORNERING_COST = 5000;
	private const int CORNERING_COST_MULTIPLIER = 7500;
	private const int BASE_ACCELERATION_COST = 20000;
	private const int ACCELERATION_COST_MULTIPLIER = 7500;
	private const int BASE_TOPSPEED_COST = 20000;
	private const int TOPSPEED_COST_MULTIPLIER = 5000;
	private const int BASE_ARMOR_COST = 10000;
	private const int ARMOR_COST_MULTIPLIER = 10000;
	#endregion

	#region Upgrade Cost Methods
	public static int GetCorneringTrainingCost(int currentLevel)
	{
		return BASE_CORNERING_COST + (CORNERING_COST_MULTIPLIER * GetAdjustedLevel(currentLevel));
	}

	public static int GetAccelerationTrainingCost(int currentLevel)
	{
		return BASE_ACCELERATION_COST + (ACCELERATION_COST_MULTIPLIER * GetAdjustedLevel(currentLevel));
	}

	public static int GetTopSpeedTrainingCost(int currentLevel)
	{
		return BASE_TOPSPEED_COST + (TOPSPEED_COST_MULTIPLIER * GetAdjustedLevel(currentLevel));
	}

	public static int GetArmorTrainingCost(int currentLevel)
	{
		return BASE_ARMOR_COST + (ARMOR_COST_MULTIPLIER * GetAdjustedLevel(currentLevel));
	}
	#endregion

	public static void UpgradeAbility(UpgradeType type)
	{
		switch (type)
		{
			case UpgradeType.Acceleration:
				_data.AccelerationAbilityPoints++;
				break;
			case UpgradeType.Cornering:
				_data.CorneringAbilityPoints++;
				break;
			case UpgradeType.TopSpeed:
				_data.TopSpeedAbilityPoints++;
				break;
			case UpgradeType.Armor:
				_data.ArmorAbilityPoints++;
				break;
		}

		_data.Save();
	}

	public static void ResetAbilityScores()
	{
		_data.AccelerationAbilityPoints = 1;
		_data.ArmorAbilityPoints = 1;
		_data.CorneringAbilityPoints = 1;
		_data.TopSpeedAbilityPoints = 1;

		_data.Save();
	}

	private static int GetAdjustedLevel(int currentLevel)
	{
		return currentLevel == 0 ? 0 : currentLevel - 1;
	}
}