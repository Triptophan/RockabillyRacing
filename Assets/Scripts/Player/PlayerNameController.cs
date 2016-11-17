using UnityEngine;

public class PlayerNameController : MonoBehaviour
{
	public string PlayerName;

	void Start()
	{
		if (transform.parent.tag == TagHelper.PLAYER)
		{
			PlayerData data = new PlayerData();

			PlayerName = data.PlayerName;

			SetName();
		}
	}

	void Update()
	{
		transform.LookAt(transform.position + Vector3.forward + Vector3.right + -Vector3.up);
	}

	private void SetName()
	{
		TextMesh text = GetComponent<TextMesh>();

		if (text != null)
		{
			text.text = PlayerName;
		}
	}
}
