using UnityEngine;

[RequireComponent(typeof(LapTracker))]
public class LapTextController : MonoBehaviour
{
	private LapTracker _lapTracker;

	private GameObject _myGameObject;

	public GUIText lapText;

	void Awake()
	{
		_myGameObject = gameObject;
		_lapTracker = _myGameObject.GetComponent<LapTracker>();
	}

	void Update()
	{
		UpdateLapGUI();
	}

	private void UpdateLapGUI()
	{
		lapText.text = string.Format("{0}\n{1}", _lapTracker.GetCurrentPlayerPlacementString(), _lapTracker.GetCurrentPlayerLapString());
	}
}