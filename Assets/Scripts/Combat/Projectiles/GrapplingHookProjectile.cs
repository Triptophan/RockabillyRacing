using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GrapplingHookProjectile : Projectile
{
	private const bool AMMO_USES_GRAVITY = false;

	private const float MAX_LIVE_TIME = 2f;
	private const float PROJECTILE_SPEED = 100f;
	private const float DAMAGE = 10f;
	private const float RETRACT_DURATION = 3f;
	private const float BOOST_PERCENTAGE = 70f;
	private const float SLOW_PERCENTAGE = 15f;

	private bool _attached;

	private GameObject _target;

	private Material _material;

	private AudioSource _audioSource;

	public AudioClip hookAttachedSound;

	public LineRenderer trail;

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();

		if (trail != null)
		{
			_material = trail.material;
		}
	}

	#region IProjectile Methods
	public override void Init()
	{
		MaxLiveTime = MAX_LIVE_TIME;
		ProjectileSpeed = PROJECTILE_SPEED;
		Damage = DAMAGE;

		AmmoUsesGravity = AMMO_USES_GRAVITY;

		_attached = false;

		base.Init();

		if (trail == null)
		{
			trail = new LineRenderer();
		}
	}

	public override void DoCollision(GameObject target)
	{
		if (target == Owner) return;

		GrapplingHookToken hook = target.AddComponent<GrapplingHookToken>();

		if (!hook) return;

		_attached = true;

		if (_audioSource != null)
		{
			_audioSource.Stop();

			_audioSource.clip = hookAttachedSound;

			_audioSource.Play();
		}

		hook.Duration = RETRACT_DURATION;
		hook.Target = target;
		hook.Owner = Owner;
		hook.BoostPercentage = BOOST_PERCENTAGE;
		hook.SlowPercentage = SLOW_PERCENTAGE;

		_target = target;

		_myGameObject.renderer.enabled = false;
	}
	#endregion

	void Update()
	{
		DrawTrail();
	}

	private void DrawTrail()
	{
		trail.SetVertexCount(2);

		Vector3 startPosition = Owner.transform.position + AmmoSpawnPoint.position;
		Vector3 endPosition = _attached ? _target.transform.position : _myGameObject.transform.position;

		trail.SetPosition(0, startPosition);
		trail.SetPosition(1, endPosition);

		float distance = Vector3.Distance(startPosition, endPosition);
		_material.SetTextureScale("_MainTex", new Vector2(distance * 2f, 1f));
	}
}

public class GrapplingHookToken : MonoBehaviour
{
	public GameObject Target { get; set; }
	public GameObject Owner { get; set; }

	public float BoostPercentage { get; set; }
	public float SlowPercentage { get; set; }

	public float Duration { get; set; }

	void Start()
	{
		Destroy(this, Duration);
	}

	void Update()
	{
		Vector3 maxBoost = Vector3.zero;
		float boostMagnitude = 0f;

		if (maxBoost == Vector3.zero)
		{
			maxBoost = (Target.transform.position - Owner.transform.position) * (1 + BoostPercentage / 100f);
			boostMagnitude = maxBoost.magnitude;
		}

		Vector3 ownerCurrentPosition = Owner.transform.position;
		Vector3 toTarget = Target.transform.position - ownerCurrentPosition;

		Vector3 newPosition = ownerCurrentPosition + (Vector3.Normalize(toTarget) * boostMagnitude);

		Owner.transform.position = Vector3.Lerp(Owner.transform.position, newPosition, 0.5f * Time.deltaTime);

		if (Target.rigidbody.drag == 0) { Target.rigidbody.drag = SlowPercentage; }
	}

	void OnDestroy()
	{
		Target.rigidbody.drag = 0f;
	}
}