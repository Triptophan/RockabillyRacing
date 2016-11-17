using UnityEngine;

public class PlayerCamera : MonoBehaviour 
{	
	public bool IsAttached { get; set; }
	
	public Transform cameraTarget;
	
	private Transform _myTransform; 
	
	private Vector3 _offset;
	
	private float _maxSpeed = 53.6448f;
		
	public void Awake()
	{
		_myTransform = transform;
		IsAttached = true;
	}
	
	void Start () 
	{
		_offset = new Vector3(-15f, 20f, -15f);
	}
	
	void Update () 
	{
		if(cameraTarget == null) return;
		
		if(IsAttached)
		{
			_myTransform.position = Vector3.MoveTowards(cameraTarget.position, cameraTarget.position + _offset, _maxSpeed);
		}
	}
}
