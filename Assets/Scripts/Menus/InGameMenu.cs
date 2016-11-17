using UnityEngine;

public class InGameMenu : MonoBehaviour
{
	private bool _showMenu = false;

	private MenuState _menuState;

	private const float MENU_WIDTH = 400f;
	private const float MENU_HEIGHT = 400f;
	private const float BUTTON_HEIGHT = 100f;
	private const float BUTTON_WIDTH = 350f;
	private const float BORDER_WIDTH = 25f;

	public GUISkin skin;

	void Start()
	{
		_menuState = MenuState.Base;
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			_showMenu = !_showMenu;
		}
	}

	void OnGUI()
	{
		GUI.matrix = GUIHelper.SetScale();
		GUI.skin = skin;

		GUIHelper.TogglePauseGame(_showMenu);

		if (_showMenu && GUIHelper.IsGamePaused)
		{
			switch (_menuState)
			{
				case MenuState.Base:
					RenderInGameMenu();
					break;
				case MenuState.Options:
					RenderOptionsMenu();
					break;
			}
		}
	}

	private void RenderInGameMenu()
	{
		GUI.BeginGroup(new Rect(GUIHelper.HalfScreenWidth - MENU_WIDTH / 2f, GUIHelper.HalfScreenHeight - MENU_HEIGHT / 2f, MENU_WIDTH, MENU_HEIGHT));

		GUI.Box(new Rect(0f, 0f, MENU_WIDTH, MENU_HEIGHT), "");

		if (GUI.Button(new Rect(BORDER_WIDTH, BORDER_WIDTH, BUTTON_WIDTH, BUTTON_HEIGHT), "Resume Game"))
		{
			_showMenu = false;
		}

		if (GUI.Button(new Rect(BORDER_WIDTH, BORDER_WIDTH * 2f + BUTTON_HEIGHT, BUTTON_WIDTH, BUTTON_HEIGHT), "Options"))
		{
			_menuState = MenuState.Options;
		}

		if (GUI.Button(new Rect(BORDER_WIDTH, BORDER_WIDTH * 3f + BUTTON_HEIGHT * 2f, BUTTON_WIDTH, BUTTON_HEIGHT), "Quit Race"))
		{
			QuitGame();
		}

		GUI.EndGroup();
	}

	private void RenderOptionsMenu()
	{

	}

	private void QuitGame()
	{
		LevelHelper.AbandonCurrentLevel();
		Application.LoadLevel(LevelHelper.MainMenu);
	}

	enum MenuState
	{
		Base,
		Options
	}
}