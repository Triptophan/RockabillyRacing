using System.Collections.Generic;
using UnityEngine;

public static class LevelHelper
{
    public static int TitleMenu { get { return (int)Levels.TitleMenu; } }
    public static int MainMenu { get { return (int)Levels.MainMenu; } }
    public static int VehicleShop { get { return (int)Levels.VehicleShop; } }
    public static int WeaponShop { get { return (int)Levels.WeaponShop; } }
    public static int WinnersCircle { get { return (int)Levels.WinnersCircle; } }
    public static int Square { get { return (int)Levels.Square; } }
    public static int LBend { get { return (int)Levels.LBend; } }
    public static int Plus { get { return (int)Levels.Plus; } }
    public static int LBendDrop { get { return (int)Levels.LBendDrop; } }
    public static int SquareNight { get { return (int)Levels.SquareNight; } }
    public static int PlusNight { get { return (int)Levels.PlusNight; } }
    public static int LBendNight { get { return (int)Levels.LBendNight; } }
    public static int LBendDropNight { get { return (int)Levels.LBendDropNight; } }

	public static List<int> TrackLevelList
	{
		get
		{
			List<int> levelList = new List<int>
			{
				Square,
				LBend,
				Plus,
				LBendDrop,
				SquareNight,
				LBendNight,
				PlusNight,
				LBendDropNight
			};

			return levelList;
		}
	}

	public static void ResetPlayedLevel()
	{
		PlayerData data = new PlayerData();
		data.LastLevelLoaded = -1;
		data.Save();
	}

	public static void GoToNextLevel()
	{
		Application.LoadLevel(GetNextLevelValue());
	}

	public static void AbandonCurrentLevel()
	{
		PlayerData data = new PlayerData();
		data.LastLevelLoaded--;
		data.Save();
	}

	private static int GetNextLevelValue()
	{
        List<int> levelList = TrackLevelList;
        PlayerData data = new PlayerData();
        int lastLevelIndex = levelList.IndexOf(data.LastLevelLoaded) + 1;

        if (lastLevelIndex >= levelList.Count) lastLevelIndex = 0;

		return (int)Levels.Square;
	}

    private enum Levels
    {
        TitleMenu = 0,
        MainMenu,
        VehicleShop,
        WeaponShop,
        WinnersCircle,
        Square,
        LBend,
        Plus,
        LBendDrop,
        SquareNight,
        LBendNight,
        PlusNight,
        LBendDropNight
    }
}