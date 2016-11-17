using UnityEngine;
using System.Collections;
using System.Text;

[RequireComponent(typeof(PlayerCombatController))]
[RequireComponent(typeof(DamageController))]
[RequireComponent(typeof(GUIText))]
public class AmmoHealthController : MonoBehaviour
{
	public GUIText GuiText;
	private PlayerCombatController _playerCombatController;
	private DamageController _damageController;
	private GameObject _myGameObject;

	void Start()
	{
		_myGameObject = gameObject;

		_playerCombatController = _myGameObject.GetComponent<PlayerCombatController>();
		_damageController = _myGameObject.GetComponent<DamageController>();
	}

	void Update()
	{
		if (GuiText != null)
		{
			GuiText.pixelOffset = new Vector2(Screen.width - 50f, Screen.height - 25f);

			UpdateGUI();
		}
	}

	public void UpdateGUI()
	{
		if (_damageController == null)
		{
			Debug.LogError(string.Format("Player {0} does not have a damage controller or one cannot be found.", _myGameObject.name));
		}

		if (_playerCombatController != null)
		{
			int frontAmmoRemaining = _playerCombatController.FrontWeaponAmmoRemaining;
			int rearAmmoRemaining = _playerCombatController.RearWeaponAmmoRemaining;

			StringBuilder text = new StringBuilder();
			text.AppendFormat("Health: {0}\n", _damageController.Health);

			if (frontAmmoRemaining >= 0 || rearAmmoRemaining >= 0)
			{
				text.Append("\nAmmo Remaining:\n");
			}

			if (frontAmmoRemaining >= 0)
			{
				text.AppendFormat("{0}: {1}\n", _playerCombatController.FrontWeaponName, frontAmmoRemaining);
			}

			if (rearAmmoRemaining >= 0)
			{
				text.AppendFormat("{0}: {1}", _playerCombatController.RearWeaponName, rearAmmoRemaining);
			}

			GuiText.text = text.ToString();
		}
	}
}
