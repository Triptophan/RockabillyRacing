using UnityEngine;
using System.Collections.Generic;

public class WinnersCircle : MonoBehaviour 
{	
	private List<GameObject> _winners;
	
	public GameObject[] winners;
	
	void Awake()
	{
		_winners = new List<GameObject>();
	}
	
	void Start()
	{
		DontDestroyOnLoad(this);
	}
	
	public void AddWinner(GameObject car)
	{
		_winners.Add(car);
		
		winners = _winners.ToArray();
	}
	
	public void DisableWinnersCircleWinners()
	{
		foreach(GameObject car in winners)
		{
			car.SetActive(false);
		}
	}
}