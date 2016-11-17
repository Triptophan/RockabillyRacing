using UnityEngine;
using System.Collections;

public static class GameObjectHelper
{
	public static bool IsACar(GameObject obj)
	{
		return obj.tag == TagHelper.AI_AGENT || obj.tag == TagHelper.PLAYER;
	}

	public static bool IsInFront(Vector3 targetA, Vector3 targetB, Vector3 direction)
	{
		float product = (targetA.x - targetB.x) * direction.x +
						(targetA.y - targetB.y) * direction.y +
						(targetA.z - targetB.z) * direction.z;

		return product > 0f;
	}

	public static GameObject AmmoHealthGUIObject { get { return GameObject.Find("AmmoHealthGUI"); } }

	public static GameObject MainCamera { get { return GameObject.Find("Main Camera"); } }

	public static GameObject MusicController { get { return GameObject.Find("MusicController"); } }
}