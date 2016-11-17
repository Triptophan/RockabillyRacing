using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class VehicleShopMenu : MonoBehaviour
{
	#region Variables
	#region Inspector Variables
	public Texture[] buttonTextures;

	public Material[] carMaterials;

	public GUISkin skin;

	public TextMesh carCostText;
	public TextMesh carNameText;
	#endregion

	#region Private Members
	private int _selectedOption;
	private int _oldOption = 0;

	private string playerName = "";

	private bool _colorChanged = false;
	private bool _renderingConfirmation = false;

	private float _continueButtonWidth;

	private MeshRenderer _meshRenderer;

	private PlayerData _data;
	#endregion

	#region Private Static Variables
	private static List<string> _AINames = new List<string>() { "Lou", "Johnny", "Chet", "Elvis", "Butch" };

	private static List<int> VEHICLE_COSTS = new List<int>() { 25000 };
	#endregion

	#region Constants
	private const float LABEL_WIDTH = 400f;
	private const float BORDER_PADDING = 250f;
	private const float LINE_HEIGHT = 100f;
	
	private const float ARROW_BUTTON_WIDTH = 100f;
	private const float SELECTION_BUTTON_WIDTH = 100f;
	private const int COLOR_CHANGE_COST = 5000;
	#endregion
	#endregion

	#region Unity Methods
	void Awake()
	{
		_meshRenderer = GetComponent<MeshRenderer>();

		_data = new PlayerData();
	}

	void Start()
	{
		_selectedOption = _data.IsFirstVehicleShopVisit ? 0 : _data.CarColor;
		_oldOption = _selectedOption;

		playerName = _data.PlayerName;
	}

	void Update()
	{
		if (_meshRenderer != null)
		{
			if (_meshRenderer.material != carMaterials[_selectedOption])
			{
				_meshRenderer.material = carMaterials[_selectedOption];
			}
		}

		_colorChanged = _oldOption != _selectedOption && !_data.IsFirstVehicleShopVisit;
	}

	void OnGUI()
	{
		GUI.matrix = GUIHelper.SetScale();
		GUI.skin = skin;
		_continueButtonWidth = _colorChanged ? 200f : 250f;

		RenderColorOptions();

		RenderCarTypeOptions();

		RenderPlayerNameEntry();

		RenderContinueButton();

		RenderPlayerCashStatus();

		RenderColorChangeConfirmationDialog();
	}
	#endregion

	#region Menu Methods
	private void RenderPlayerCashStatus()
	{
		string cashString = string.Format("Cash: {0:C0}", _data.PlayerCash);
		GUI.Label(new Rect(GUIHelper.NATIVE_WIDTH - LABEL_WIDTH - BORDER_PADDING, BORDER_PADDING, LABEL_WIDTH, LINE_HEIGHT), cashString);
	}

	private void RenderCarTypeOptions()
	{
		if (_renderingConfirmation) return;

		string carTypeText = "2 Door Coupe";
		string carTypeCost = string.Format("Cost: {0:C0}", VEHICLE_COSTS[0]);

		carNameText.gameObject.SetActive(_data.IsFirstVehicleShopVisit);
		carCostText.gameObject.SetActive(_data.IsFirstVehicleShopVisit);

		carNameText.text = carTypeText;
		carCostText.text = carTypeCost;
	}

	private void RenderColorOptions()
	{
		if (_data.PlayerCash < COLOR_CHANGE_COST || _renderingConfirmation) return;

		float xOffset = 300f;
		string colorPickerText = _data.IsFirstVehicleShopVisit ? "Select Color" : string.Format("Change Color - ${0}", COLOR_CHANGE_COST);

		GUI.Label(new Rect(BORDER_PADDING, xOffset, LABEL_WIDTH, LINE_HEIGHT), colorPickerText);
		_selectedOption = GUI.SelectionGrid(new Rect(BORDER_PADDING, LINE_HEIGHT / 2f + xOffset, LABEL_WIDTH / 2f, 450),
											_selectedOption,
											GetCarColorButtons(),
											1);
	}

	public void RenderColorChangeConfirmationDialog()
	{
		if (!_renderingConfirmation) return;

		float buttonWidth = 200f;
		float dialogWidth = buttonWidth * 2f + BORDER_PADDING * 3f;
		float dialogHeight = 200f;
		float buttonHeight = 100f;
		float buttonYOffset = dialogHeight - (BORDER_PADDING + buttonHeight - 10f);

		GUI.BeginGroup(new Rect(GUIHelper.HalfScreenWidth - dialogWidth / 2f, GUIHelper.HalfScreenHeight - dialogHeight / 2f, dialogWidth, dialogHeight));

		GUI.Box(new Rect(0f, 0f, dialogWidth, dialogHeight), "That looks great! But, are you sure?");

		if (GUI.Button(new Rect(BORDER_PADDING, buttonYOffset, buttonWidth, buttonHeight), "Yes!"))
		{
			_data.PlayerCash -= COLOR_CHANGE_COST;

			SaveAndContinue();
		}

		if (GUI.Button(new Rect(dialogWidth - (BORDER_PADDING + buttonWidth), buttonYOffset, buttonWidth, buttonHeight), "No way!"))
		{
			_selectedOption = _oldOption;
			_renderingConfirmation = false;
		}

		GUI.EndGroup();
	}

	private void RenderPlayerNameEntry()
	{
		if (!_data.IsFirstVehicleShopVisit) return;

		GUI.Label(new Rect(BORDER_PADDING, BORDER_PADDING, LABEL_WIDTH, LINE_HEIGHT), "Your Name:");
		GUI.SetNextControlName("PlayerName");
		playerName = GUI.TextField(new Rect(BORDER_PADDING, LINE_HEIGHT, LABEL_WIDTH, LINE_HEIGHT / 2f), playerName, 20);
		if (GUI.GetNameOfFocusedControl() == "")
		{
			GUI.FocusControl("PlayerName");
		}
	}

	private void RenderContinueButton()
	{
		float buyButtonXOffset = GUIHelper.NATIVE_WIDTH - _continueButtonWidth - BORDER_PADDING;
		float buttonYOffset = GUIHelper.NATIVE_HEIGHT - LINE_HEIGHT - BORDER_PADDING;
		float backButtonWidth = 200f;
		if (_colorChanged)
		{
			if (GUI.Button(new Rect(buyButtonXOffset - backButtonWidth - BORDER_PADDING, buttonYOffset, backButtonWidth, LINE_HEIGHT), "Revert"))
			{
				_selectedOption = _oldOption;
			}
		}

		string buttonText = _data.IsFirstVehicleShopVisit ? "Continue" : _colorChanged ? "Buy" : "Main Menu";
		if (!string.IsNullOrEmpty(playerName))
		{
			if (GUI.Button(new Rect(buyButtonXOffset, buttonYOffset, _continueButtonWidth, LINE_HEIGHT), buttonText))
			{
				_renderingConfirmation = _colorChanged && !_data.IsFirstVehicleShopVisit;

				if (!_renderingConfirmation)
				{
					SaveAndContinue();
				}
			}
		}
	}

	private GUIContent[] GetCarColorButtons()
	{
		GUIContent[] buttonContent = new GUIContent[buttonTextures.Length];

		for (int i = 0; i < buttonTextures.Length; i++)
		{
			buttonContent[i] = new GUIContent(GetColorPaddedString(buttonTextures[i].name), buttonTextures[i]);
		}

		return buttonContent;
	}

	private string GetColorPaddedString(string rawString)
	{
		rawString = rawString.Replace("CarButton", "");

		int rawStringLength = rawString.Trim().Length;
		int paddingAdd = 10 - rawStringLength;

		System.Text.StringBuilder newString = new System.Text.StringBuilder();

		for (int i = 0; i < paddingAdd; i++)
		{
			newString.Append(" ");
		}
		newString.Append(rawString);

		return newString.ToString();
	}
	#endregion

	#region Post-Menu Methods
	private void SaveAndContinue()
	{
		if (_data.IsFirstVehicleShopVisit)
		{
			SetAIColors();
			SetAINames();

			_data.PlayerName = string.IsNullOrEmpty(playerName) ? "Player" : playerName;
			_data.PlayerCash = _data.PlayerCash - VEHICLE_COSTS[0];
			_data.PlayerVehicleType = (int)VehicleType.Coupe;
			_data.IsFirstVehicleShopVisit = false;
		}

		_data.CarColor = _selectedOption;
		_data.Save();
		Application.LoadLevel(LevelHelper.MainMenu);
	}

	private void SetAIColors()
	{
		List<int> usedMaterialIndexes = new List<int>();

		while (usedMaterialIndexes.Count < 3)
		{
			int selectedIndex = Random.Range(0, carMaterials.Length - 1);

			if (!usedMaterialIndexes.Contains(selectedIndex))
			{
				usedMaterialIndexes.Add(selectedIndex);
			}
		}

		for (int i = 0; i < usedMaterialIndexes.Count; i++)
		{
			_data.SetAICarColor(i, usedMaterialIndexes[i]);
		}
	}

	private void SetAINames()
	{
		List<string> usedNames = new List<string>();

		while (usedNames.Count < 3)
		{
			string name = _AINames[Random.Range(0, _AINames.Count - 1)];
			if (!usedNames.Contains(name))
			{
				usedNames.Add(name);
			}
		}

		for (int i = 0; i < usedNames.Count; i++)
		{
			_data.SetAIName(i, usedNames[i]);
		}
	}
	#endregion
}