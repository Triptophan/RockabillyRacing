using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponShopMenu : MonoBehaviour
{
	#region Private Members
	private ShopMenuState _menuState;

	private PlayerData _data;

	private const float _borderPadding = 25f;
	#endregion

	#region Public Members
	public GUISkin skin;

	public Texture2D[] performanceUpgradeIcons;
	public Texture2D[] frontWeaponUpgradeIcons;
	public Texture2D[] rearWeaponUpgradeIcons;
	public Texture2D tierBarGraphImage;
	public Texture2D weaponGroupImage;
	public Texture2D weaponTierGraphImage;
	public Texture2D weaponEquippedImage;
	public Texture2D purchaseDisabledImage;
	#endregion
	
	#region Unity Methods
	void Awake()
	{
		_menuState = ShopMenuState.PerformanceUpgrades;

		_data = new PlayerData();
	}

	void OnGUI()
	{
		GUI.matrix = GUIHelper.SetScale();
		GUI.skin = skin;

		float _menuBoxWidth = GUIHelper.NATIVE_WIDTH - (_borderPadding * 2.5f);
		float _menuBoxHeight = GUIHelper.NATIVE_HEIGHT - (_borderPadding * 2f);

		GUI.BeginGroup(new Rect(_borderPadding, _borderPadding * 2f, _menuBoxWidth, _menuBoxHeight));
		GUI.Box(new Rect(0, 0, _menuBoxWidth, _menuBoxHeight), "Garage Menu"); 

		float cashLabelWidth = 300f;
		string cashString = string.Format("Cash: ${0}", _data.PlayerCash);
		GUI.Label(new Rect(_menuBoxWidth - cashLabelWidth, 20f, cashLabelWidth, 25f), cashString, skin.customStyles[(int)Style.CashLabel]);

		RenderUpgradeOptionTabs();

		switch (_menuState)
		{
			case ShopMenuState.PerformanceUpgrades:
				RenderPerformanceUpgradeMenu();
				break;
			case ShopMenuState.WeaponUpgrades:
				RenderWeaponUpgradeMenu();
				break;
		}

		GUI.EndGroup();
	}
	#endregion

	private void RenderUpgradeOptionTabs()
	{
		const float sheetWidth = 256f;
		const float cutoutHeight = 64f;
		const float sheetHeight = 128f;
		float xOffset = GUIHelper.HalfScreenWidth - sheetWidth - 7.5f;
		float yOffset = 75f;
		Rect performanceButtonRect = new Rect(0f, -cutoutHeight, sheetWidth, sheetHeight);
		GUI.BeginGroup(new Rect(xOffset, yOffset, sheetWidth, cutoutHeight));
		if (_menuState == ShopMenuState.PerformanceUpgrades)
		{
			GUI.Label(performanceButtonRect, skin.customStyles[(int)Style.UpgradeButtons].active.background);
		}
		else
		{
			if (GUI.Button(performanceButtonRect, "", skin.customStyles[(int)Style.UpgradeButtons]))
			{
				_menuState = ShopMenuState.PerformanceUpgrades;
			}
		}
		GUI.EndGroup();

		Rect upgradeButtonRect = new Rect(0f, 0f, sheetWidth, sheetHeight);
		GUI.BeginGroup(new Rect(xOffset + sheetWidth + 15f, yOffset, sheetWidth, cutoutHeight));
		if (_menuState == ShopMenuState.WeaponUpgrades)
		{
			GUI.Label(upgradeButtonRect, skin.customStyles[(int)Style.UpgradeButtons].active.background);
		}
		else
		{
			if (GUI.Button(upgradeButtonRect, "", skin.customStyles[(int)Style.UpgradeButtons]))
			{
				_menuState = ShopMenuState.WeaponUpgrades;
			}
		}
		GUI.EndGroup();

		RenderContinueButton();
	}

	private void RenderPerformanceUpgradeMenu()
	{
		List<UpgradeMenuItemArgs> argsList = new List<UpgradeMenuItemArgs>();

		argsList.Add(new UpgradeMenuItemArgs(performanceUpgradeIcons[0], "Engine\nUpgrade", "Top Speed", _data.TopSpeedAbilityPoints,
											UpgradeType.TopSpeed, AbilityScoreHelper.GetCorneringTrainingCost(_data.CorneringAbilityPoints)));

		argsList.Add(new UpgradeMenuItemArgs(performanceUpgradeIcons[1], "Transmission\nUpgrade", "Acceleration", _data.AccelerationAbilityPoints,
											UpgradeType.Acceleration, AbilityScoreHelper.GetAccelerationTrainingCost(_data.AccelerationAbilityPoints)));

		argsList.Add(new UpgradeMenuItemArgs(performanceUpgradeIcons[2], "Tire\nUpgrade", "Cornering", _data.CorneringAbilityPoints,
											UpgradeType.Cornering, AbilityScoreHelper.GetCorneringTrainingCost(_data.CorneringAbilityPoints)));

		argsList.Add(new UpgradeMenuItemArgs(performanceUpgradeIcons[3], "Armor\nUpgrade", "Armor", _data.ArmorAbilityPoints,
											UpgradeType.Armor, AbilityScoreHelper.GetArmorTrainingCost(_data.ArmorAbilityPoints)));

		for(int i = 0; i < argsList.Count; i++)
		{
			RenderPerformanceUpgradeMenuItem(i, argsList[i]);
		}
	}

	private void RenderPerformanceUpgradeMenuItem(int argIndex, UpgradeMenuItemArgs args)
	{
		float width = 950f;
		float height = 300f;
		float xOffset = GUIHelper.HalfScreenWidth - width / 2f;
		float yOffset = 125f;

		GUI.BeginGroup(new Rect(xOffset, yOffset + (args.Icon.height * argIndex), width, height));

		GUI.Label(new Rect(0f, 0f, args.Icon.width, args.Icon.height), args.Icon);
		GUI.Label(new Rect(0f, 0f, args.Icon.width, args.Icon.height), args.IconText, skin.customStyles[(int)Style.IconText]);
		GUI.Label(new Rect(args.Icon.width, 25f, 50f, 20f), args.UpgradeName);

		float oneBarHeight = tierBarGraphImage.height / 10f;
		GUI.BeginGroup(new Rect(args.Icon.width, 75f, tierBarGraphImage.width, oneBarHeight));
		GUI.Label(new Rect(0f, 0f - (oneBarHeight * (args.AbilityPoints - 1)), tierBarGraphImage.width, tierBarGraphImage.height), tierBarGraphImage);
		GUI.EndGroup();

		VehicleType vehicleType = (VehicleType)_data.PlayerVehicleType;
		float purchaseButtonTop = 140f;
		GUIStyle purchaseButtonStyle = skin.customStyles[(int)Style.PurchaseButton];
		float purchaseButtonWidth = purchaseButtonStyle.normal.background.width;
		float purchaseButtonHeight = purchaseButtonStyle.normal.background.height;

		if (!VehicleHelper.IsAtMax(args.UpgradeType, vehicleType) && _data.PlayerCash >= args.AbilityCost)
		{
			if (GUI.Button(new Rect(args.Icon.width, purchaseButtonTop, purchaseButtonWidth, purchaseButtonHeight), "", purchaseButtonStyle))
			{
				VehicleHelper.PurchaseUpgrade(args.UpgradeType, vehicleType, args.AbilityCost);
			}
		}
		else
		{
			GUI.Label(new Rect(args.Icon.width, purchaseButtonTop, purchaseButtonWidth, purchaseButtonHeight), purchaseDisabledImage);
		}

		GUI.Label(new Rect(width - 200f, purchaseButtonTop, 200f, purchaseButtonHeight), string.Format("${0}", args.AbilityCost.ToString()));

		GUI.EndGroup();
	}

	private void RenderWeaponUpgradeMenu()
	{
		float width = weaponGroupImage.width;
		float height = weaponGroupImage.height + 50f;
		float xOffset = GUIHelper.HalfScreenWidth - width / 2f;
		float yOffset = 150f;

		GUI.BeginGroup(new Rect(xOffset, yOffset, width, height));
		GUI.Label(new Rect(0f, 0f, width, 40f), "Front Mounted Weapons:", skin.customStyles[4]);
		GUI.Box(new Rect(0f, 50f, width, height), weaponGroupImage);
		RenderFrontWeaponUpgradeItems();
		GUI.EndGroup();

		GUI.BeginGroup(new Rect(xOffset, yOffset + height, width, height));
		GUI.Label(new Rect(0f, 0f, width, 40f), "Rear Mounted Weapons:", skin.customStyles[4]);
		GUI.Box(new Rect(0f, 50f, width, height), weaponGroupImage);
		RenderRearWeaponUpgradeItems();
		GUI.EndGroup();
	}

	private void RenderFrontWeaponUpgradeItems()
	{
		List<WeaponMenuItemArgs<FrontWeaponType>> argsList = new List<WeaponMenuItemArgs<FrontWeaponType>>();
		Rect offsetRect;
		float border = _borderPadding * 1.5f;
		float sectionWidth = frontWeaponUpgradeIcons[0].width + weaponTierGraphImage.width + weaponEquippedImage.width + _borderPadding;
		float sectionHeight = frontWeaponUpgradeIcons[0].height / 2f;
		
		offsetRect = new Rect(_borderPadding, border, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<FrontWeaponType>(offsetRect, frontWeaponUpgradeIcons[0], "Machine Gun", _data.OwnsMachineGun,
					_data.MachineGunAbilityPoints, FrontWeaponType.MachineGun));

		offsetRect = new Rect(sectionWidth + _borderPadding, border, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<FrontWeaponType>(offsetRect, frontWeaponUpgradeIcons[1], "Rockets", _data.OwnsRocketLauncher,
					_data.RocketLauncherAbilityPoints, FrontWeaponType.RocketLauncher));

		offsetRect = new Rect(sectionWidth * 2f + border, border, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<FrontWeaponType>(offsetRect, frontWeaponUpgradeIcons[2], "Ooze Cannon", _data.OwnsOozeCannon,
					_data.OozeGunAbilityPoints, FrontWeaponType.OozeGun));

		offsetRect = new Rect(_borderPadding, sectionHeight, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<FrontWeaponType>(offsetRect, frontWeaponUpgradeIcons[3], "Flamethrower", _data.OwnsFlamethrower,
					_data.FlameThrowerAbilityPoints, FrontWeaponType.Flamethrower));

		offsetRect = new Rect(sectionWidth + _borderPadding, sectionHeight, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<FrontWeaponType>(offsetRect, frontWeaponUpgradeIcons[4], "Hook", _data.OwnsGrapplingHook,
					_data.GrapplingHookAbilityPoints, FrontWeaponType.GrapplingHook));

		offsetRect = new Rect(sectionWidth * 2f + border, sectionHeight, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<FrontWeaponType>(offsetRect, frontWeaponUpgradeIcons[5], "Heat Seekers", _data.OwnsHomingRocketLauncher,
					_data.HomingRocketLauncherAbilityPoints, FrontWeaponType.HomingRocketLauncher));

		foreach (WeaponMenuItemArgs<FrontWeaponType> arg in argsList)
		{
			RenderWeaponUpgradeItem(arg);
		}
	}

	private void RenderRearWeaponUpgradeItems()
	{
		List<WeaponMenuItemArgs<RearWeaponType>> argsList = new List<WeaponMenuItemArgs<RearWeaponType>>();
		Rect offsetRect;
		float border = _borderPadding * 1.5f;
		float sectionWidth = frontWeaponUpgradeIcons[0].width + weaponTierGraphImage.width + weaponEquippedImage.width + _borderPadding;
		float sectionHeight = frontWeaponUpgradeIcons[0].height / 2f;

		offsetRect = new Rect(_borderPadding, border, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<RearWeaponType>(offsetRect, rearWeaponUpgradeIcons[0], "Oil Slick", _data.OwnsOilSlicks,
					_data.OilSlickAbilityPoints, RearWeaponType.OilSlick));

		offsetRect = new Rect(sectionWidth + _borderPadding, border, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<RearWeaponType>(offsetRect, rearWeaponUpgradeIcons[1], "Firebomb", _data.OwnsFirebomb,
					_data.FireBombAbilityPoints, RearWeaponType.Firebomb));

		offsetRect = new Rect(sectionWidth * 2f + border, border, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<RearWeaponType>(offsetRect, rearWeaponUpgradeIcons[2], "Nitrous", _data.OwnsNitroBooster,
					_data.NitroBoosterAbilityPoints, RearWeaponType.NitroBooster));

		offsetRect = new Rect(_borderPadding, sectionHeight, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<RearWeaponType>(offsetRect, rearWeaponUpgradeIcons[3], "Landmine", _data.OwnsLandmines,
					_data.LandMineAbilityPoints, RearWeaponType.Landmines));

		offsetRect = new Rect(sectionWidth + _borderPadding, sectionHeight, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<RearWeaponType>(offsetRect, rearWeaponUpgradeIcons[4], "Scatterjacks", _data.OwnsScatterJack,
					_data.ScatterJackAbilityPoints, RearWeaponType.ScatterJack));

		offsetRect = new Rect(sectionWidth * 2f + border, sectionHeight, sectionWidth, sectionHeight);
		argsList.Add(new WeaponMenuItemArgs<RearWeaponType>(offsetRect, rearWeaponUpgradeIcons[5], "Force Field", _data.OwnsForcefield,
					_data.ForceFieldAbilityPoints, RearWeaponType.ForceField));

		foreach (WeaponMenuItemArgs<RearWeaponType> arg in argsList)
		{
			RenderWeaponUpgradeItem(arg);
		}
	}

	private void RenderWeaponUpgradeItem(WeaponMenuItemArgs<FrontWeaponType> arg)
	{
		float doubleBorder = _borderPadding * 2f;
		float weaponItemWidth = arg.Icon.width + weaponTierGraphImage.width;
		// Item Group Start
		GUI.BeginGroup(arg.ContainerRect);
		// Weapon Icon & Label
		GUI.BeginGroup(new Rect(0f, 0f, weaponItemWidth, arg.Icon.height / 2f));
		float iconOffset = arg.OwnsWeapon ? 0f : arg.Icon.height / 2f;
		GUI.Label(new Rect(0f, 0f - iconOffset, arg.Icon.width, arg.Icon.height), arg.Icon);
		GUI.Label(new Rect(0f, 0f, arg.Icon.width, arg.Icon.height / 2f), arg.IconText, skin.customStyles[(int)Style.IconText]);
		GUI.EndGroup();

		float oneBarHeight = weaponTierGraphImage.height / 4f;
		// Ammo upgrade level indicator
		GUI.BeginGroup(new Rect(_borderPadding + arg.Icon.width, doubleBorder, weaponTierGraphImage.width + 20f, oneBarHeight));
		GUI.Label(new Rect(0f, 0f - (oneBarHeight * (arg.AbilityLevel)), weaponTierGraphImage.width + 20f, weaponTierGraphImage.height + 2f), weaponTierGraphImage);
		GUI.EndGroup();

		float equipButtonWidth = skin.customStyles[(int)Style.EquipButton].normal.background.width;
		float equipButtonHeight = skin.customStyles[(int)Style.EquipButton].normal.background.height;
		// Equip Button
		GUI.BeginGroup(new Rect(_borderPadding + arg.Icon.width + weaponTierGraphImage.width, doubleBorder - 3f, equipButtonWidth, equipButtonHeight));
		Rect equipButtonRect = new Rect(0f, 0f, equipButtonWidth, equipButtonHeight);
		if (arg.OwnsWeapon)
		{
			if (_data.PlayerFrontWeapon == (int)arg.WeaponType)
			{
				GUI.Label(equipButtonRect, weaponEquippedImage);
			}
			else
			{
				if (GUI.Button(equipButtonRect, "", skin.customStyles[(int)Style.EquipButton]))
				{
					_data.PlayerFrontWeapon = (int)arg.WeaponType;
				}
			}
		}
		GUI.EndGroup();

		GUIStyle purchaseButtonStyle = skin.customStyles[(int)Style.PurchaseButton];
		Texture2D purchaseButtonImage = purchaseButtonStyle.normal.background;
		float purchaseButtonWidth = purchaseButtonImage.width;
		float purchaseButtonHeight = purchaseButtonImage.height;
		int cost = arg.OwnsWeapon ? WeaponHelper.GetAmmoCost(arg.WeaponType) : WeaponHelper.GetWeaponCost(arg.WeaponType);
		GUI.BeginGroup(new Rect(_borderPadding + arg.Icon.width, doubleBorder + oneBarHeight, purchaseButtonWidth, purchaseButtonHeight));
		Rect purchaseButtonRect = new Rect(0f, 0f, purchaseButtonWidth, purchaseButtonHeight);
		if (!WeaponHelper.IsAtMax(arg.WeaponType))
		{
			if (_data.PlayerCash >= cost)
			{
				if (GUI.Button(purchaseButtonRect, "", purchaseButtonStyle))
				{
					WeaponHelper.PurchaseUpgrade(arg.OwnsWeapon, arg.WeaponType, cost);
				}
			}
			else
			{
				GUI.Label(purchaseButtonRect, purchaseDisabledImage);
			}
		}
		else
		{
			GUI.Label(purchaseButtonRect, "     MAXED"); 
		}
		GUI.EndGroup();

		if (!WeaponHelper.IsAtMax(arg.WeaponType))
		{
			GUI.Label(new Rect(_borderPadding + arg.Icon.width + purchaseButtonWidth + 10f, doubleBorder + oneBarHeight + 5f, purchaseButtonWidth + 50f, purchaseButtonHeight), string.Format("${0}", cost));
		}
		GUI.EndGroup();
	}

	private void RenderWeaponUpgradeItem(WeaponMenuItemArgs<RearWeaponType> arg)
	{
		float doubleBorder = _borderPadding * 2f;
		float weaponItemWidth = arg.Icon.width + weaponTierGraphImage.width;
		// Item Group Start
		GUI.BeginGroup(arg.ContainerRect);
		// Weapon Icon & Label
		GUI.BeginGroup(new Rect(0f, 0f, weaponItemWidth, arg.Icon.height / 2f));
		float iconOffset = arg.OwnsWeapon ? 0f : arg.Icon.height / 2f;
		GUI.Label(new Rect(0f, 0f - iconOffset, arg.Icon.width, arg.Icon.height), arg.Icon);
		GUI.Label(new Rect(0f, 0f, arg.Icon.width, arg.Icon.height / 2f), arg.IconText, skin.customStyles[(int)Style.IconText]);
		GUI.EndGroup();

		float oneBarHeight = weaponTierGraphImage.height / 4f;
		// Ammo upgrade level indicator
		GUI.BeginGroup(new Rect(_borderPadding + arg.Icon.width, doubleBorder, weaponTierGraphImage.width + 20f, oneBarHeight));
		GUI.Label(new Rect(0f, 0f - (oneBarHeight * (arg.AbilityLevel)), weaponTierGraphImage.width + 20f, weaponTierGraphImage.height + 2f), weaponTierGraphImage);
		GUI.EndGroup();

		float equipButtonWidth = skin.customStyles[(int)Style.EquipButton].normal.background.width;
		float equipButtonHeight = skin.customStyles[(int)Style.EquipButton].normal.background.height;
		// Equip Button
		GUI.BeginGroup(new Rect(_borderPadding + arg.Icon.width + weaponTierGraphImage.width, doubleBorder - 3f, equipButtonWidth, equipButtonHeight));
		Rect equipButtonRect = new Rect(0f, 0f, equipButtonWidth, equipButtonHeight);
		if (arg.OwnsWeapon)
		{
			if (_data.PlayerRearWeapon == (int)arg.WeaponType)
			{
				GUI.Label(equipButtonRect, weaponEquippedImage);
			}
			else
			{
				if (GUI.Button(equipButtonRect, "", skin.customStyles[(int)Style.EquipButton]))
				{
					_data.PlayerRearWeapon = (int)arg.WeaponType;
				}
			}
		}
		GUI.EndGroup();

		GUIStyle purchaseButtonStyle = skin.customStyles[(int)Style.PurchaseButton];
		Texture2D purchaseButtonImage = purchaseButtonStyle.normal.background;
		float purchaseButtonWidth = purchaseButtonImage.width;
		float purchaseButtonHeight = purchaseButtonImage.height;
		int cost = arg.OwnsWeapon ? WeaponHelper.GetAmmoCost(arg.WeaponType) : WeaponHelper.GetWeaponCost(arg.WeaponType);
		GUI.BeginGroup(new Rect(_borderPadding + arg.Icon.width, doubleBorder + oneBarHeight, purchaseButtonWidth, purchaseButtonHeight));
		Rect purchaseButtonRect = new Rect(0f, 0f, purchaseButtonWidth, purchaseButtonHeight);
		if (!WeaponHelper.IsAtMax(arg.WeaponType))
		{
			if (_data.PlayerCash >= cost)
			{
				if (GUI.Button(purchaseButtonRect, "", purchaseButtonStyle))
				{
					WeaponHelper.PurchaseUpgrade(arg.OwnsWeapon, arg.WeaponType, cost);
				}
			}
			else
			{
				GUI.Label(purchaseButtonRect, purchaseDisabledImage);
			}
		}
		else
		{
			GUI.Label(purchaseButtonRect, "     MAXED");
		}
		GUI.EndGroup();

		if (!WeaponHelper.IsAtMax(arg.WeaponType))
		{
			GUI.Label(new Rect(_borderPadding + arg.Icon.width + purchaseButtonWidth + 10f, doubleBorder + oneBarHeight + 5f, purchaseButtonWidth + 50f, purchaseButtonHeight), string.Format("${0}", cost));
		}
		GUI.EndGroup();
	}

	private void RenderContinueButton()
	{
		float continueButtonWidth = skin.customStyles[3].normal.background.width;
		float continueButtonHeight = skin.customStyles[3].normal.background.height;
		float xOffset = GUIHelper.NATIVE_WIDTH - _borderPadding * 3f - continueButtonWidth;
		float yOffset = GUIHelper.NATIVE_HEIGHT - _borderPadding * 3f - continueButtonHeight;

		if (GUI.Button(new Rect(xOffset, yOffset, continueButtonWidth, continueButtonHeight), "", skin.customStyles[3]))
		{
			_data.Save();
			Application.LoadLevel(LevelHelper.MainMenu);
		}
	}

	#region Enums
	protected enum ShopMenuState
	{
		PerformanceUpgrades,
		WeaponUpgrades
	}

	private enum Style
	{
		CashLabel,
		UpgradeButtons,
		PurchaseButton,
		ContinueButton,
		WeaponGroupLabel,
		EquipButton,
		IconText
	}
	#endregion

	private class UpgradeMenuItemArgs
	{
		public Texture2D Icon { get; set; }

		public string IconText { get; set; }
		public string UpgradeName { get; set; }

		public int AbilityPoints { get; set; }
		public int AbilityCost { get; set; }

		public UpgradeType UpgradeType { get; set; }

		public UpgradeMenuItemArgs(Texture2D icon, string iconText, string upgradeName, int abilityPoints, UpgradeType upgradeType, int abilityCost)
		{
			Icon = icon;
			IconText = iconText;
			UpgradeName = upgradeName;
			AbilityPoints = abilityPoints;
			UpgradeType = upgradeType;
			AbilityCost = abilityCost;
		}
	}

	private class WeaponMenuItemArgs<T>
	{
		public Rect ContainerRect { get; set; }

		public Texture2D Icon { get; set; }

		public T WeaponType { get; set; }

		public bool OwnsWeapon { get; set; }

		public string IconText { get; set; }

		public int AbilityLevel { get; set; }

		public WeaponMenuItemArgs(Rect containerRect, Texture2D icon, string iconText, bool ownsWeapon, int abilityLevel, T weaponType)
		{
			ContainerRect = containerRect;
			Icon = icon;
			IconText = iconText;
			OwnsWeapon = ownsWeapon;
			AbilityLevel = abilityLevel;
			WeaponType = weaponType;
		}
	}
}