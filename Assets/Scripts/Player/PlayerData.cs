using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
	#region Private Members
	private List<string> _aiPlayerNames;
	private List<int> _aiColors;

	private string filePath;
	#endregion

	public PlayerData()
	{
		filePath = Application.persistentDataPath + "/save.dat";
		Load();
	}

	public void Clear()
	{
		File.Delete(filePath);
	}

	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter();
		using (FileStream file = File.Create(filePath))
		{
			bf.Serialize(file, this);
			file.Close();
		}
	}

	public void Load()
	{
		if (File.Exists(filePath))
		{
			BinaryFormatter bf = new BinaryFormatter();
			PlayerData data;
			using (FileStream file = File.Open(filePath, FileMode.Open))
			{
				data = (PlayerData)bf.Deserialize(file);
				file.Close();
			}

			CopyData(data);
		}
		else
		{
			if (_aiColors == null) _aiColors = new List<int>();
			if (_aiPlayerNames == null) _aiPlayerNames = new List<string>();
			IsFirstVehicleShopVisit = true;
		}
	}

	private void CopyData(PlayerData data)
	{
		PlayerName = data.PlayerName;
		_aiPlayerNames = data._aiPlayerNames;
		LastLevelLoaded = data.LastLevelLoaded;
		PlayerVehicleType = data.PlayerVehicleType;
		PlayerFrontWeapon = data.PlayerFrontWeapon;
		PlayerRearWeapon = data.PlayerRearWeapon;
		LastWinPosition = data.LastWinPosition;
		CarColor = data.CarColor;
		PlayerCash = data.PlayerCash;
		CorneringAbilityPoints = data.CorneringAbilityPoints;
		AccelerationAbilityPoints = data.AccelerationAbilityPoints;
		TopSpeedAbilityPoints = data.TopSpeedAbilityPoints;
		ArmorAbilityPoints = data.ArmorAbilityPoints;
		MachineGunAbilityPoints = data.MachineGunAbilityPoints;
		FlameThrowerAbilityPoints = data.FlameThrowerAbilityPoints;
		GrapplingHookAbilityPoints = data.GrapplingHookAbilityPoints;
		HomingRocketLauncherAbilityPoints = data.HomingRocketLauncherAbilityPoints;
		OozeGunAbilityPoints = data.OozeGunAbilityPoints;
		RocketLauncherAbilityPoints = data.RocketLauncherAbilityPoints;
		OilSlickAbilityPoints = data.OilSlickAbilityPoints;
		FireBombAbilityPoints = data.FireBombAbilityPoints;
		NitroBoosterAbilityPoints = data.NitroBoosterAbilityPoints;
		LandMineAbilityPoints = data.LandMineAbilityPoints;
		ScatterJackAbilityPoints = data.ScatterJackAbilityPoints;
		ForceFieldAbilityPoints = data.ForceFieldAbilityPoints;
		_aiColors = data._aiColors;
		IsFirstVehicleShopVisit = data.IsFirstVehicleShopVisit;
		OwnsMachineGun = data.OwnsMachineGun;
		OwnsRocketLauncher = data.OwnsRocketLauncher;
		OwnsOozeCannon = data.OwnsOozeCannon;
		OwnsHomingRocketLauncher = data.OwnsHomingRocketLauncher;
		OwnsFlamethrower = data.OwnsFlamethrower;
		OwnsGrapplingHook = data.OwnsGrapplingHook;
		OwnsOilSlicks = data.OwnsOilSlicks;
		OwnsLandmines = data.OwnsLandmines;
		OwnsNitroBooster = data.OwnsNitroBooster;
		OwnsScatterJack = data.OwnsScatterJack;
		OwnsFirebomb = data.OwnsFirebomb;
		OwnsForcefield = data.OwnsForcefield;
	}

	#region Public String Values
	#region Properties
	public string PlayerName { get; set; }
	#endregion

	#region Methods
	public string GetAIName(int index)
	{
		if (_aiPlayerNames == null || _aiPlayerNames.Count < index) return "";

		return _aiPlayerNames[index];
	}

	public void SetAIName(int index, string value)
	{
		if (_aiPlayerNames == null)
		{
			_aiPlayerNames = new List<string>(4);
		}

		if (_aiPlayerNames.Count <= index)
		{
			_aiPlayerNames.Add(value);
		}
		else
		{
			_aiPlayerNames[index] = value;
		}
	}
	#endregion
	#endregion

	#region Public Int Values
	#region Properties
	public int LastLevelLoaded { get; set; }
	public int PlayerVehicleType { get; set; }
	public int PlayerFrontWeapon { get; set; }
	public int PlayerRearWeapon { get; set; }
	public int LastWinPosition { get; set; }
	public int CarColor { get; set; }
	public int PlayerCash { get; set; }
	public int CorneringAbilityPoints { get; set; }
	public int AccelerationAbilityPoints { get; set; }
	public int TopSpeedAbilityPoints { get; set; }
	public int ArmorAbilityPoints { get; set; }
	public int MachineGunAbilityPoints { get; set; }
	public int FlameThrowerAbilityPoints { get; set; }
	public int GrapplingHookAbilityPoints { get; set; }
	public int HomingRocketLauncherAbilityPoints { get; set; }
	public int OozeGunAbilityPoints { get; set; }
	public int RocketLauncherAbilityPoints { get; set; }
	public int OilSlickAbilityPoints { get; set; }
	public int FireBombAbilityPoints { get; set; }
	public int NitroBoosterAbilityPoints { get; set; }
	public int LandMineAbilityPoints { get; set; }
	public int ScatterJackAbilityPoints { get; set; }
	public int ForceFieldAbilityPoints { get; set; }
	#endregion

	#region Methods
	public int GetAICarColor(int index)
	{
		if (_aiColors == null || _aiColors.Count < index) return 0;

		return _aiColors[index];
	}

	public void SetAICarColor(int index, int value)
	{
		if (_aiColors == null)
		{
			_aiColors = new List<int>(4);
		}

		if (_aiColors.Count <= index)
		{
			_aiColors.Add(value);
		}
		else
		{
			_aiColors[index] = value;
		}
	}
	#endregion
	#endregion

	#region Bool Values
	#region Properties
	public bool IsFirstVehicleShopVisit { get; set; }

	#region WeaponOwnership
	#region Front Weapons
	public bool OwnsMachineGun { get; set; }
	public bool OwnsRocketLauncher { get; set; }
	public bool OwnsOozeCannon { get; set; }
	public bool OwnsHomingRocketLauncher { get; set; }
	public bool OwnsFlamethrower { get; set; }
	public bool OwnsGrapplingHook { get; set; }
	#endregion

	#region Rear Weapons
	public bool OwnsOilSlicks { get; set; }
	public bool OwnsLandmines { get; set; }
	public bool OwnsNitroBooster { get; set; }
	public bool OwnsScatterJack { get; set; }
	public bool OwnsFirebomb { get; set; }
	public bool OwnsForcefield { get; set; }
	#endregion
	#endregion
	#endregion
	#endregion
}