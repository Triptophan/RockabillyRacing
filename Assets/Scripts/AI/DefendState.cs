using UnityEngine;
using System.Collections;

public sealed class DefendState : State
{
	private static DefendState _instance;

	private const int EXIT_STATE_VALUE = 20;

	private const float DEFEND_COOLDOWN_TIME = 1.5f;

	#region Properties
	public static State Instance
	{
		get
		{
			if (_instance == null)
				_instance = new DefendState();

			return _instance;
		}
	}
	#endregion

	public override void Enter(GameObject agent)
	{
		SteeringBehavior steeringBehavior = agent.GetComponent<SteeringBehavior>();
		if (steeringBehavior != null)
		{
			steeringBehavior.EvadeOn();
		}
	}

	public override void Execute(GameObject agent)
	{
		StateMachine stateMachine = agent.GetComponent<StateMachine>();

		int test = Random.Range(0, 100);

		if (test < EXIT_STATE_VALUE)
		{
			stateMachine.ChangeState(CombatSentryState.Instance);
			return;
		}

		CombatController combatController = agent.GetComponent<CombatController>();
		combatController.AttackRear();
	}

	public override void Exit(GameObject agent)
	{
		SteeringBehavior steeringBehavior = agent.GetComponent<SteeringBehavior>();
		if (steeringBehavior != null)
		{
			steeringBehavior.EvadeOff();
		}

		CombatController combatController = agent.GetComponent<CombatController>();
		if (combatController != null)
		{
			combatController.DefendCooldown = DEFEND_COOLDOWN_TIME;
		}
	}
}