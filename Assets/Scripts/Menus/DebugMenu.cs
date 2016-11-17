using UnityEngine;
using System.Collections;

public class DebugMenu : MonoBehaviour
{
	private bool _showMenu = false;
	private bool _paused = false;

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.BackQuote))
		{
			_showMenu = !_showMenu;
		}
	}

	void OnGUI()
	{
		if (Debug.isDebugBuild && _showMenu)
		{
			GUI.Box(new Rect(10, 160, 200, 200), "Debug Menu");
			GUI.Label(new Rect(20, 185, 100, 30), "Load Levels");

			if (GUI.Button(new Rect(20, 210, 160, 20), "Level 1"))
			{
				Time.timeScale = 0f;
				KillAllPlayers();
				Application.LoadLevel(LevelHelper.Square);
			}

			if (GUI.Button(new Rect(20, 270, 160, 20), "Pause"))
			{
				_paused = !_paused;
				Time.timeScale = _paused ? 0f : 1f;
			}
		}
	}

	private void KillAllPlayers()
	{
		GameObject[] ais = GameObject.FindGameObjectsWithTag(TagHelper.AI_AGENT);
		foreach (GameObject ai in ais)
		{
			StateMachine stateMachine = ai.GetComponent<StateMachine>();
			if (stateMachine != null)
			{
				stateMachine.ChangeState(null);
				stateMachine.ChangeGlobalState(null);
			}
			Destroy(ai, 1f);
		}

		GameObject player = GameObject.FindWithTag(TagHelper.PLAYER);
		DestroyImmediate(player);
	}
}