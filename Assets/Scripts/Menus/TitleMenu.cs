using UnityEngine;

public class TitleMenu : MonoBehaviour
{
	private const float BUTTON_SHEET_WIDTH = 256f;
	private const float BUTTON_SHEET_HEIGHT = 384f;
	private const float BUTTON_CUTOUT_HEIGHT = 128f;

	private const int PLAYER_STARTING_CASH = 25000;

	private MusicController _musicController;

	public GUISkin rockabillyRacingSkin;

    void Awake()
    {
        GameObject musicControllerObject = GameObjectHelper.MusicController;
        if (musicControllerObject != null)
        {
            _musicController = GetComponent<MusicController>();
            GameObject.DontDestroyOnLoad(_musicController);
        }
    }

    void Start()
    {
        if (_musicController != null)
        {
            _musicController.PlayMusic();
        }
    }

	void OnGUI()
	{
		GUI.matrix = GUIHelper.SetScale();
		GUI.skin = rockabillyRacingSkin;

		RenderTitleMenu();
	}

	private void RenderTitleMenu()
	{
		float groupWidth = GUIHelper.HalfScreenWidth - BUTTON_SHEET_WIDTH / 2f;
		float groupGroupHeight = GUIHelper.HalfScreenHeight;
		PlayerData data = new PlayerData();

		GUI.BeginGroup(new Rect(groupWidth, groupGroupHeight, BUTTON_SHEET_WIDTH, BUTTON_CUTOUT_HEIGHT));
		if (GUI.Button(new Rect(0, 0, BUTTON_SHEET_WIDTH, BUTTON_SHEET_HEIGHT), ""))
		{
			data.Clear();
			data.IsFirstVehicleShopVisit = true;
			data.PlayerCash = PLAYER_STARTING_CASH;
			data.PlayerName = "";
			data.Save();
			WeaponHelper.ResetWeaponOwnership();
			AbilityScoreHelper.ResetAbilityScores();
			WeaponHelper.ResetWeaponUpgrades();
			LevelHelper.ResetPlayedLevel();
			Application.LoadLevel(LevelHelper.VehicleShop);
		}
		GUI.EndGroup();

		if (!data.IsFirstVehicleShopVisit)
		{
			GUI.BeginGroup(new Rect(groupWidth, groupGroupHeight + BUTTON_CUTOUT_HEIGHT, BUTTON_SHEET_WIDTH, BUTTON_CUTOUT_HEIGHT));
			if (GUI.Button(new Rect(0, -BUTTON_CUTOUT_HEIGHT - 2f, BUTTON_SHEET_WIDTH, BUTTON_SHEET_HEIGHT), ""))
			{
				Application.LoadLevel(LevelHelper.MainMenu);
			}
			GUI.EndGroup();
		}

		GUI.BeginGroup(new Rect(groupWidth, groupGroupHeight + (BUTTON_CUTOUT_HEIGHT * 2f), BUTTON_SHEET_WIDTH, BUTTON_CUTOUT_HEIGHT));
		if (GUI.Button(new Rect(0, -BUTTON_CUTOUT_HEIGHT * 2f - 2f, BUTTON_SHEET_WIDTH, BUTTON_SHEET_HEIGHT), ""))
		{
			Application.Quit();
			Destroy(_musicController);
		}
		GUI.EndGroup();
	}
}