using System.Collections.Generic;

public class VehicleUpgrade
{
	private List<Upgrade<UpgradeType>> _maxUpgradeLevels;

	public VehicleType Type { get; private set; }
	public List<Upgrade<UpgradeType>> MaxUpgradeLevels 
	{
		get
		{
			if (_maxUpgradeLevels == null) _maxUpgradeLevels = new List<Upgrade<UpgradeType>>();

			return _maxUpgradeLevels;
		}
		set
		{
			_maxUpgradeLevels = value;
		}
	}

	public VehicleUpgrade(VehicleType type)
	{
		Type = type;
		MaxUpgradeLevels = new List<Upgrade<UpgradeType>>();

		UpgradeType[] values = (UpgradeType[])System.Enum.GetValues(typeof(UpgradeType));
		for (int i = 0; i < values.Length; i++)
		{
			MaxUpgradeLevels.Add(new Upgrade<UpgradeType>((UpgradeType)values[i], 0));
		}
	}
}