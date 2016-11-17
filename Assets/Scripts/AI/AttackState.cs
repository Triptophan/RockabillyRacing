using UnityEngine;
using System.Collections;

public sealed class AttackState : State
{
	private static AttackState _instance;

	private const int EXIT_STATE_VALUE = 35;

	private const float ATTACK_COOLDOWN_TIME = 3f;

	#region Properties
	public static State Instance
	{
		get
		{
			if (_instance == null)
				_instance = new AttackState();

			return _instance;
		}
	}
	#endregion

	public override void Enter(GameObject agent)
	{
		SteeringBehavior steeringBehavior = agent.GetComponent<SteeringBehavior>();
		if (steeringBehavior != null)
		{
			agent.GetComponent<SteeringBehavior>().PursuitOn();
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
		combatController.AttackFront();
	}

	public override void Exit(GameObject agent)
	{
		SteeringBehavior steeringBehavior = agent.GetComponent<SteeringBehavior>();
		if (steeringBehavior != null)
		{
			steeringBehavior.PursuitOff();
		}

		CombatController combatController = agent.GetComponent<CombatController>();
		if (combatController != null)
		{
			combatController.AttackCooldown = ATTACK_COOLDOWN_TIME;
		}
	}
}