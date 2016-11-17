using UnityEngine;
using System.Collections;
using System.Text;

public class MainMenu : MonoBehaviour
{
	public GUISkin skin;

	public TextMesh loadingText;
	public TextMesh raceToolTipText;
	public TextMesh garageToolTipText;
	public TextMesh dealershipToolTipText;

	private int numberOfDots;

	private AsyncOperation _nextRaceOperation;

	private bool _allowRaceHover;
	public bool IsQuitting;

	private GameObject _raceTextGameObject;
	private GameObject _garageTextGameObject;
	private GameObject _dealershipTextGameObject;

	private const float BUTTON_SHEET_WIDTH = 256f;
	private const float BUTTON_SHEET_HEIGHT = 768f;
	private const float BUTTON_CUTOUT_HEIGHT = 256f;
	private const float BORDER_PADDING = 25f;
	private const float BUTTON_HEIGHT = 60f;
	private const float BUTTON_WIDTH = 120f;

	private const string LOADING_TEXT = "Loading";

	void Start()
	{
		loadingText.gameObject.SetActive(false);

		_raceTextGameObject = raceToolTipText.gameObject;
		_garageTextGameObject = garageToolTipText.gameObject;
		_dealershipTextGameObject = dealershipToolTipText.gameObject;

		_allowRaceHover = true;
	}

	void OnGUI()
	{
		GUI.matrix = GUIHelper.SetScale();
		GUI.skin = skin;

		if (!IsQuitting)
		{
			RenderNextRaceButton();

			RenderGarageButton();

			RenderDealershipButton();
		}
		else
		{
			RenderQuitConfirm();
		}
	}

	#region Private Render Methods
	private void RenderNextRaceButton()
	{
		Rect raceButtonRect = new Rect(GUIHelper.HalfScreenWidth - BUTTON_SHEET_WIDTH * 2f, GUIHelper.HalfScreenHeight - BUTTON_CUTOUT_HEIGHT / 2f, BUTTON_SHEET_WIDTH, BUTTON_CUTOUT_HEIGHT);
		GUI.BeginGroup(raceButtonRect);
		if (GUI.Button(new Rect(0, 0, BUTTON_SHEET_WIDTH, BUTTON_SHEET_HEIGHT),"", skin.customStyles[0]))
		{
			_allowRaceHover = false;

			loadingText.gameObject.SetActive(true);
			_raceTextGameObject.SetActive(false);
			_garageTextGameObject.SetActive(false);
			_dealershipTextGameObject.SetActive(false);
			
			LevelHelper.GoToNextLevel();
		}
		GUI.EndGroup();

		if (_allowRaceHover && raceButtonRect.Contains(Event.current.mousePosition))
		{
			_raceTextGameObject.SetActive(true);
			raceToolTipText.text = "Begin next race.";
		}
		else
		{
			_raceTextGameObject.SetActive(false);
		}
	}

	private void RenderGarageButton()
	{
		Rect garageButtonRect = new Rect(GUIHelper.HalfScreenWidth - BUTTON_SHEET_WIDTH / 2f, GUIHelper.HalfScreenHeight - BUTTON_CUTOUT_HEIGHT / 2f, BUTTON_SHEET_WIDTH, BUTTON_CUTOUT_HEIGHT);
		GUI.BeginGroup(garageButtonRect);
		if (GUI.Button(new Rect(0, -BUTTON_CUTOUT_HEIGHT * 2, BUTTON_SHEET_WIDTH, BUTTON_SHEET_HEIGHT), "", skin.customStyles[0]))
		{
			Application.LoadLevel(LevelHelper.WeaponShop);
		}
		GUI.EndGroup();

		if (garageButtonRect.Contains(Event.current.mousePosition))
		{
			_garageTextGameObject.SetActive(true);
			garageToolTipText.text = "Upgrade your vehicle performance.";
		}
		else
		{
			_garageTextGameObject.SetActive(false);
		}
	}

	private void RenderDealershipButton()
	{
		Rect dealershipButtonRect = new Rect(GUIHelper.HalfScreenWidth + BUTTON_SHEET_WIDTH * 1f, GUIHelper.HalfScreenHeight - BUTTON_CUTOUT_HEIGHT / 2f, BUTTON_SHEET_WIDTH, BUTTON_CUTOUT_HEIGHT);
		GUI.BeginGroup(dealershipButtonRect);
		if (GUI.Button(new Rect(0, -BUTTON_CUTOUT_HEIGHT, BUTTON_SHEET_WIDTH, BUTTON_SHEET_HEIGHT), "", skin.customStyles[0]))
		{
			Application.LoadLevel(LevelHelper.VehicleShop);
		}
		GUI.EndGroup();

		if (dealershipButtonRect.Contains(Event.current.mousePosition))
		{
			_dealershipTextGameObject.SetActive(true);
			dealershipToolTipText.text = "Purchase a new ride.";
		}
		else
		{
			_dealershipTextGameObject.SetActive(false);
		}
	}

	private void RenderQuitConfirm()
	{
		float boxWidth = 350f;
		float boxHeight = 150f;

		GUI.BeginGroup(new Rect(GUIHelper.HalfScreenWidth - boxWidth / 2f, GUIHelper.HalfScreenHeight - boxHeight / 2f, boxWidth, boxHeight));

		GUI.Box(new Rect(0f, 0f, boxWidth, boxHeight), "You really leaving?");

		float buttonYOffset = boxHeight - BORDER_PADDING - BUTTON_HEIGHT;
		if (GUI.Button(new Rect(BORDER_PADDING, buttonYOffset, BUTTON_WIDTH, BUTTON_HEIGHT), "Yes"))
		{
			Application.Quit();
		}

		if (GUI.Button(new Rect(boxWidth - BORDER_PADDING - BUTTON_WIDTH, buttonYOffset, BUTTON_WIDTH, BUTTON_HEIGHT), "No"))
		{
			IsQuitting = false;
		}

		GUI.EndGroup();
	}
	#endregion
}