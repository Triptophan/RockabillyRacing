using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicController : MonoBehaviour
{
	#region Inspector Variables
	public AudioClip[] titleMusic;
	public AudioClip[] menuMusic;
	public AudioClip[] vehicleShopMusic;
	public AudioClip[] weaponShopMusic;
	public AudioClip[] raceMusic;

	public MusicType musicType;
	#endregion

	private AudioClip[] _musicLibrary;

	private GameObject _myGameObject;

	void Awake()
	{
		_myGameObject = gameObject;

		AssignMusicLibrary();
	}

	public void PlayMusic()
	{
		if (_musicLibrary.Length == 0) return;

		AudioSource source = _myGameObject.GetComponent<AudioSource>();
		if (source != null)
		{
			source.clip = _musicLibrary[Random.Range(0, _musicLibrary.Length -1)];
			source.loop = true;
			source.Play();
		}
	}

	private void AssignMusicLibrary()
	{
		switch (musicType)
		{
			case MusicType.Menu:
				_musicLibrary = menuMusic;
				break;
			case MusicType.Race:
				_musicLibrary = raceMusic;
				break;
			case MusicType.Title:
				_musicLibrary = titleMusic;
				break;
			case MusicType.VehicleShop:
				_musicLibrary = vehicleShopMusic;
				break;
			case MusicType.WeaponShop:
				_musicLibrary = weaponShopMusic;
				break;
		}
	}
}