using UnityEngine;

public sealed class Idle : State
{
	#region Private members
	private static Idle _instance;
	#endregion

	#region Properties
	public static State Instance
	{
		get
		{
			if (_instance == null)
				_instance = new Idle();

			return _instance;
		}
	}
	#endregion

	#region State Overrides
	public override void Enter(GameObject agent)
	{
		agent.rigidbody.Sleep();
		agent.rigidbody.velocity = Vector3.zero;
	}

	public override void Execute(GameObject agent) { }

	public override void Exit(GameObject agent) { }
	#endregion
}