using System.Collections.Generic;
using UnityEngine;

public sealed class CombatSentryState : State
{
	private static CombatSentryState _instance;

	private const float VIEW_DISTANCE = 50f;

	private const float ATTACK_DOT = 0.85f;
	private const float DEFEND_DOT = -0.938f;

	private const float ATTACK_VALUE = 55f;
	private const float DEFEND_VALUE = 80f;

	private static GameObject[] targets;

	#region Properties
	public static State Instance
	{
		get
		{
			if (_instance == null)
				_instance = new CombatSentryState();

			return _instance;
		}
	}
	#endregion

	public override void Enter(GameObject agent)
	{
		if (targets == null || targets.Length == 0 || targets[0] == null)
		{
			GetTargets();
		}
	}

	public override void Execute(GameObject agent)
	{
		if (agent == null)
		{
			Exit(agent);
			return;
		}

		if (targets == null || targets.Length == 0 || targets[0] == null)
		{
			GetTargets();
		}

		List<GameObject> targetObjects = new List<GameObject>();

		for (int i = 0; i < targets.Length; i++)
		{
			if (targets[i].name != agent.name)
			{
				if (Vector3.Distance(targets[i].transform.position, agent.transform.position) <= VIEW_DISTANCE)
				{
					targetObjects.Add(targets[i]);
				}
			}
		}

		if (targetObjects.Count > 0)
		{
			GameObject target = targetObjects[Random.Range(0, targetObjects.Count - 1)];
			agent.GetComponent<SteeringBehavior>().currentTarget = target;

			Transform agentTransform = agent.transform;

			Vector3 agentPosition = agentTransform.position;
			Vector3 targetPosition = target.transform.position;
			Vector3 toTarget = targetPosition - agentPosition;

			float dot = Vector3.Dot(agentTransform.forward, Vector3.Normalize(toTarget));

			StateMachine stateMachine = agent.GetComponent<StateMachine>();
			CombatController combatController = agent.GetComponent<CombatController>();

			if (combatController != null)
			{
				combatController.AttackCooldown -= Time.deltaTime;
				combatController.DefendCooldown -= Time.deltaTime;
			}

			int test = Random.Range(0, 100);

			if (dot < DEFEND_DOT && toTarget.magnitude < 30f && combatController.DefendCooldown <= 0f)
			{
				if (test <= DEFEND_VALUE)
				{
					stateMachine.ChangeState(DefendState.Instance);
				}
			}
			else if (dot > ATTACK_DOT && toTarget.magnitude < 100f && combatController.AttackCooldown <= 0f)
			{
				if (test <= ATTACK_VALUE)
				{
					stateMachine.ChangeState(AttackState.Instance);
				}
			}
		}
	}

	public override void Exit(GameObject agent) { }

	private void GetTargets()
	{
		GameObject[] tempAIs = GameObject.FindGameObjectsWithTag(TagHelper.AI_AGENT);
		GameObject[] tempPlayers = GameObject.FindGameObjectsWithTag(TagHelper.PLAYER);

		targets = new GameObject[tempAIs.Length + tempPlayers.Length];

		int counter = 0;
		for (int i = 0; i < tempAIs.Length; i++)
		{
			targets[counter++] = tempAIs[i];
		}

		for (int i = 0; i < tempPlayers.Length; i++)
		{
			targets[counter++] = tempPlayers[i];
		}
	}
}