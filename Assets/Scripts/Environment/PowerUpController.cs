using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpController : MonoBehaviour
{
	private const int MAX_POWERUPS = 10;

	private const float AMMO_RANDOM_RANGE = 3f;
	private const float MONEY_RANDOM_RANGE = 3f;

	private GameObject _frontAmmoPowerUp;
	private GameObject _rearAmmoPowerUp;
	private GameObject _moneyPowerUp;
	private GameObject _healthPowerUp;

	private Vector3 p1;
	private Vector3 p2;
	private Vector3 p3;

	public List<Vector3> _validVertices;
	public List<int> _validTriangles;
	public List<Vector3> _validNormals;

	public Transform trackTransform;

	public bool powerUpsOn;

	private Mesh _trackMesh;

	public float powerUpSpawnInterval = 2f;
	
	public int CurrentPowerUpCount { get; set; }

	void Start()
	{
		if (!powerUpsOn) return;

		CurrentPowerUpCount = 0;

		if (trackTransform == null)
		{
			Debug.LogError("There needs to be a track transform assigned to the Power Up Controller Component of the Game Controller.");
			return;
		}

		_trackMesh = trackTransform.GetComponent<MeshFilter>().mesh;

		_frontAmmoPowerUp = ResourcesHelper.FrontAmmoPowerUp;
		_rearAmmoPowerUp = ResourcesHelper.RearAmmoPowerUp;
		_moneyPowerUp = ResourcesHelper.MoneyPowerUp;
		_healthPowerUp = ResourcesHelper.HealthPowerUp;

		CacheSpawnVertices();

		StartCoroutine(ScheduleAmmoSpawn());
	}

	private void CacheSpawnVertices()
	{
		_validVertices = new List<Vector3>();
		_validNormals = new List<Vector3>();

		Vector3[] vertices = _trackMesh.vertices;
		int[] triangleIndices = _trackMesh.triangles;
		Vector3[] normals = _trackMesh.normals;

		for (int i = 0; i < triangleIndices.Length; i = i + 3)
		{
			int T1 = triangleIndices[i];
			int T2 = triangleIndices[i + 1];
			int T3 = triangleIndices[i + 2];

			Vector3 P1 = vertices[T1];
			Vector3 P2 = vertices[T2];
			Vector3 P3 = vertices[T3];

			Vector3 N1 = normals[T1];
			Vector3 N2 = normals[T2];
			Vector3 N3 = normals[T3];

			Vector3 faceNormal = (N1 + N2 + N3) / 3;
			float dot = Vector3.Dot(faceNormal, Vector3.up);
			float heightDifference = ((P1 + P2 + P3) / 3).y - trackTransform.position.y;
			//Use this test to determine the default y value of the track's transform as a child.
			//if(dot == 1f) Debug.Log(string.Format("normaldot: {3} triangle.y: {0} transform.y: {1} difference: {2}", ((P1 + P2 + P3) / 3).y, trackTransform.position.y, heightDifference, dot));
			if (dot ==  1f && heightDifference < 0.5f)
			{
				_validVertices.Add(P1);
				_validVertices.Add(P2);
				_validVertices.Add(P3);

				_validNormals.Add(N1);
				_validNormals.Add(N2);
				_validNormals.Add(N3);
			}
		}
	}

	private IEnumerator ScheduleAmmoSpawn()
	{
		if (CurrentPowerUpCount < MAX_POWERUPS)
		{
			DeployPowerUp();
		}

		yield return new WaitForSeconds(powerUpSpawnInterval);

		StartCoroutine(ScheduleAmmoSpawn());
	}

	private void GetMeshSpawn()
	{
		int index = Random.Range(0, (_validVertices.Count - 1) / 3);

		p1 = _validVertices[index * 3];
		p2 = _validVertices[index * 3 + 1];
		p3 = _validVertices[index * 3+ 2];

		//Debug.Log(string.Format("p1: {0} p2: {1} p3: {2}", p1, p2, p3));
	}

	private Vector3 GetAmmoSpawnPoint()
	{
		GetMeshSpawn();

		return (p1 + p2 + p3) / 3;
	}

	private void DeployPowerUp()
	{
		if (CurrentPowerUpCount < MAX_POWERUPS)
		{
			float test = Random.Range(0f, 10f);
			Vector3 position = GetAmmoSpawnPoint();
			GameObject powerUp;

			if (test > AMMO_RANDOM_RANGE)
			{
				if (test > AMMO_RANDOM_RANGE + MONEY_RANDOM_RANGE)
				{
					powerUp = (GameObject)GameObject.Instantiate(_healthPowerUp, position, Quaternion.identity);

				}
				else
				{
					powerUp = (GameObject)GameObject.Instantiate(_moneyPowerUp, position, Quaternion.identity);
				}
			}
			else
			{
				float ammoTest = Random.Range(1, 2);
				GameObject _ammoPowerUp = ammoTest / 2 == 1 ? _frontAmmoPowerUp : _rearAmmoPowerUp;
				powerUp = (GameObject)GameObject.Instantiate(_ammoPowerUp, position, Quaternion.identity);
			}

			powerUp.transform.position += Vector3.up * 0.1f;
			powerUp.name = string.Format("{0}{1}", powerUp.name, powerUp.GetInstanceID());

			CurrentPowerUpCount++;
		}
	}

	void OnDrawGizmos()
	{
		if (p1 != Vector3.zero && p2 != Vector3.zero && p3 != Vector3.zero)
		{
			Gizmos.DrawLine(p1, p2);
			Gizmos.DrawLine(p2, p3);
			Gizmos.DrawLine(p3, p1);
		}
	}

	public void KillPowerUp()
	{
		StartCoroutine(DoDestroyPowerUp());
	}

	private IEnumerator DoDestroyPowerUp()
	{
		yield return new WaitForSeconds(3f);

		CurrentPowerUpCount--;
	}
}