using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AbilityScore))]
public class VehicleMovement : MonoBehaviour
{
	#region Constants
	private float SLIPPING_EXTREMUM_SLIP = 0.01f;
	private float SLIPPING_EXTREMUM_VALUE = 1f;
	private float SLIPPING_ASYMPTOTE_SLIP = 0.04f;
	private float SLIPPING_ASYMPTOTE_VALUE = 0.6f;
	private float SLIPPING_STIFFNESS = 5f;
	private float STEERING_DRAG_BUFFER = 5000f;
	#endregion

	#region Private Members
	private AbilityScore _abilityScores;

	private Wheel[] _wheels;

	private float _currentMotorTorque;
	private float _boostMultiplier;

	private GameObject _myGameObject;

	private Transform _myTransform;

	private Rigidbody _myRigidbody;

	private Vector3 _centerOfMassDefault;

	private WheelFriction _sidewaysSlippingFriction;

	private bool _waitingForPositionUpdate;
	#endregion

	#region Public Members
	public Drivetrain drivetrain;

	public Transform[] frontWheels;
	public Transform[] rearWheels;

	public WheelFriction forwardFriction;
	public WheelFriction sidewaysFriction;

	public Suspension suspension;

	public Transform centerOfMass;

	public Vector3 LastPosition { get; set; }

	public bool IsOverCrashVelocityThreshold
	{
		get
		{
			return _myRigidbody.velocity.sqrMagnitude > 9f;
		}
	}
	public bool canDrive;

	public float throttle;
	public float steering;
	#endregion

	public void Awake()
	{
		_myGameObject = gameObject;
		_myTransform = _myGameObject.transform;
		_myRigidbody = _myGameObject.rigidbody;

		_sidewaysSlippingFriction = new WheelFriction()
		{
			extremumSlip = SLIPPING_EXTREMUM_SLIP,
			extremumValue = SLIPPING_EXTREMUM_VALUE,
			asymptoteSlip = SLIPPING_ASYMPTOTE_SLIP,
			asymptoteValue = SLIPPING_ASYMPTOTE_VALUE,
			stiffness = SLIPPING_STIFFNESS
		};

		_waitingForPositionUpdate = false;

		canDrive = false;
	}

	public void Start()
	{
		_abilityScores = GetComponent<AbilityScore>();

		_wheels = new Wheel[frontWheels.Length + rearWheels.Length];

		if (_abilityScores != null)
		{
			drivetrain.maxSpeed = ConversionHelper.ConvertMilesPerHourToMetersPerSecond(drivetrain.maxSpeed * _abilityScores.TopSpeedMultiplier);
		}

		SetupWheelColliders();

		SetupCenterOfMass();
	}

	public void Update()
	{
		PlayEngineSound();
	}

	public void FixedUpdate()
	{
		CalculateMotorTorque();

		ApplySteering();

		UpdateWheelGraphics();

		if (canDrive)
		{
			ApplyThrottle();

			ApplyBoost();
		}

		if (!_waitingForPositionUpdate)
		{
			StartCoroutine("UpdatePosition", 5f);
		}
	}

	#region Start Methods
	private void SetupWheelColliders()
	{
		int wheelCount = 0;

		foreach (Transform wheelTransform in frontWheels)
		{
			_wheels[wheelCount++] = SetupWheel(wheelTransform, true);
		}

		foreach (Transform wheelTransform in rearWheels)
		{
			_wheels[wheelCount++] = SetupWheel(wheelTransform, false);
		}
	}

	private Wheel SetupWheel(Transform wheelTransform, bool isFrontWheel)
	{
		GameObject go = new GameObject(wheelTransform.name + " Collider");
		go.transform.position = wheelTransform.position;
		go.transform.parent = wheelTransform;
		go.transform.rotation = wheelTransform.rotation;

		WheelCollider wc = (WheelCollider)go.AddComponent(typeof(WheelCollider));
		wc.mass = 2f;
		wc.forwardFriction = SetupFriction(forwardFriction);
		wc.sidewaysFriction = SetupFriction(sidewaysFriction);

		wc.suspensionDistance = suspension.range;

		JointSpring js = wc.suspensionSpring;
		js.damper = suspension.damper;

		if (isFrontWheel)
			js.spring = suspension.frontSpring;
		else
			js.spring = suspension.rearSpring;

		wc.suspensionSpring = js;

		Wheel wheel = new Wheel();
		wheel.collider = wc;
		wheel.wheelMesh = (Transform)wheelTransform.GetComponentsInChildren(typeof(Transform))[1];

		wheel.collider.radius = wheel.wheelMesh.renderer.bounds.size.y / 2;

		wheel.isSteerWheel = isFrontWheel;

		return wheel;
	}

	private WheelFrictionCurve SetupFriction(WheelFriction data)
	{
		WheelFrictionCurve wfc = new WheelFrictionCurve();
		wfc.extremumSlip = data.extremumSlip;
		wfc.extremumValue = data.extremumValue;
		wfc.asymptoteSlip = data.asymptoteSlip;
		wfc.asymptoteValue = data.asymptoteValue;
		wfc.stiffness = data.stiffness;

		return wfc;
	}

	private void SetupCenterOfMass()
	{
		if (centerOfMass != null)
			_myRigidbody.centerOfMass = centerOfMass.localPosition;

		_centerOfMassDefault = _myRigidbody.centerOfMass;
	}
	#endregion

	#region Update Methods
	private void UpdateWheelGraphics()
	{
		foreach (Wheel w in _wheels)
		{
			if (w.isSteerWheel)
			{
				Vector3 euler = w.wheelMesh.localEulerAngles;
				euler.y = (steering * drivetrain.turningRadius) + (w.wheelMesh.name == "FR Tire" ? 90f : 270f);

				w.wheelMesh.localEulerAngles = euler;
			}

			w.wheelMesh.Rotate(0, 0, w.collider.rpm * 6 * Time.deltaTime);
		}
	}

	private void PlayEngineSound()
	{
		AudioSource mySource = gameObject.GetComponent<AudioSource>();
		if (mySource != null)
		{
			float adjustment = (0.7f + (_myRigidbody.velocity.magnitude / drivetrain.maxSpeed) * 0.3f);
			float lerpAdjustment = Mathf.Lerp(mySource.pitch, adjustment, 0.5f);
			mySource.pitch = Mathf.Clamp(lerpAdjustment, 0.7f, 1f);
		}
	}
	#endregion

	#region FixedUpdate Methods
	private void CalculateMotorTorque()
	{
		_currentMotorTorque = drivetrain.horsePower * 5252f * throttle * _abilityScores.AccelerationMultiplier / drivetrain.averageRPM;
	}

	private void ApplyThrottle()
	{
		foreach (Wheel w in _wheels)
		{
			w.collider.motorTorque = _currentMotorTorque / 4f;
		}

		if (!_myRigidbody.isKinematic)
			_myRigidbody.velocity = Vector3.ClampMagnitude(_myRigidbody.velocity, drivetrain.maxSpeed);
	}

	private void ApplySteering()
	{
		float turningRadius = _abilityScores.CorneringMultiplier / Mathf.Sin((90f - (steering * drivetrain.turningRadius)) * Mathf.Deg2Rad);
		float turnFactor = (drivetrain.turningRadius / 20f) + 1f;
		float turningSpeed = Mathf.Clamp(_myTransform.InverseTransformDirection(_myRigidbody.velocity).z / turningRadius, -turnFactor, turnFactor);

		_myRigidbody.angularDrag = (steering != 0f && throttle > 0f ? STEERING_DRAG_BUFFER : 0f);
		_myRigidbody.centerOfMass = new Vector3(_centerOfMassDefault.x + steering, _centerOfMassDefault.y, _centerOfMassDefault.z);

		_myTransform.RotateAround(_myTransform.position + _myTransform.right * turningRadius * steering, _myTransform.up, turningSpeed * Mathf.Rad2Deg * Time.deltaTime * steering);
	}

	private void ApplyBoost()
	{
		if (_boostMultiplier > 0f)
		{
			foreach (Wheel w in _wheels)
			{
				w.collider.motorTorque = w.collider.motorTorque * _boostMultiplier;
			}

			Vector3 force = Vector3.forward * 1000f;
			Vector3 position = _myTransform.localPosition + _myTransform.forward * 1.5f + _myTransform.up * 0.25f;

			_myRigidbody.AddForceAtPosition(force, position);
		}
	}

	private IEnumerator UpdatePosition(float delay)
	{
		_waitingForPositionUpdate = true;

		yield return new WaitForSeconds(delay);

		LastPosition = _myTransform.position;
		_waitingForPositionUpdate = false;
	}
	#endregion

	#region External Methods
	public void StartSlip()
	{
		foreach (Wheel w in _wheels)
		{
			w.collider.sidewaysFriction = SetupFriction(_sidewaysSlippingFriction);
		}
	}

	public void StopSlip(float duration)
	{
		StartCoroutine("RemoveSlip", duration);
	}

	public void StartBoost(float boostMultiplier, float duration)
	{
		_boostMultiplier = boostMultiplier;

		drivetrain.maxSpeed *= _boostMultiplier;
		drivetrain.horsePower *= _boostMultiplier;

		StartCoroutine("RemoveBoost", duration);
	}

	private IEnumerator RemoveSlip(float duration)
	{
		yield return new WaitForSeconds(duration);

		foreach (Wheel w in _wheels)
		{
			w.collider.sidewaysFriction = SetupFriction(sidewaysFriction);
		}
	}

	private IEnumerator RemoveBoost(float duration)
	{
		yield return new WaitForSeconds(duration);

		drivetrain.maxSpeed /= _boostMultiplier;
		drivetrain.horsePower /= _boostMultiplier;
	}

	public void ToggleWheelColliders(bool enabled)
	{
		if (_wheels == null) return;

		foreach (Wheel w in _wheels)
		{
			Vector3 euler = w.wheelMesh.localEulerAngles;
			euler.y = (w.wheelMesh.name == "FR Tire" || w.wheelMesh.name == "RR Tire") ? 90f : 270f;

			w.wheelMesh.localEulerAngles = euler;
			if (w.collider != null)
			{
				w.collider.enabled = enabled;
			}
		}
	}
	#endregion
}

#region Vehicle Specific classes
[System.Serializable]
public class Drivetrain
{
	public float horsePower;
	public float averageRPM;
	public float maxSpeed;
	public float turningRadius;
}

public class Wheel
{
	public WheelCollider collider;
	public Transform wheelMesh;
	public bool isSteerWheel;
}

[System.Serializable]
public class WheelFriction
{
	public float extremumSlip;
	public float extremumValue;
	public float asymptoteSlip;
	public float asymptoteValue;
	public float stiffness;
}

[System.Serializable]
public class Suspension
{
	public float range;
	public float damper;
	public float frontSpring;
	public float rearSpring;
}
#endregion