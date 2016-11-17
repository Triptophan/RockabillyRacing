using UnityEngine;
using System.Collections;

public static class GUIHelper
{
	public const float NATIVE_WIDTH = 1920f;
	public const float NATIVE_HEIGHT = 1200f;

	public static bool IsGamePaused { get { return Time.timeScale == 0f; } }

	public static float HalfScreenWidth { get { return NATIVE_WIDTH / 2f; } }
	public static float HalfScreenHeight { get { return NATIVE_HEIGHT / 2f; } }

	public static void TogglePauseGame(bool pause)
	{
		Time.timeScale = pause ? 0f : 1f;
	}

	public static Matrix4x4 SetScale()
	{
		float rx = Screen.width / NATIVE_WIDTH;
		float ry = Screen.height / NATIVE_HEIGHT;

		Vector3 tScale = new Vector3(rx, ry, 1f);

		return Matrix4x4.TRS(Vector3.zero, Quaternion.identity, tScale);
	}
}
