using UnityEngine;

public class MainMenuExitButton : MonoBehaviour
{
	public MainMenu _mainMenuComponent;

	void OnMouseUp()
	{
		if (_mainMenuComponent != null)
		{
			_mainMenuComponent.IsQuitting = true;
		}
	}
}