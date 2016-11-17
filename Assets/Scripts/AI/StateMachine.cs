using UnityEngine;

public class StateMachine : MonoBehaviour
{
	#region private members
	private State _previousState;
	private State _previousGlobalState;

	private State _currentState;
	private State _currentGlobalState;
	#endregion

	#region StateMachine methods
	public void Update()
	{
		if (_currentState != null) _currentState.Execute(gameObject);
		if (_currentGlobalState != null) _currentGlobalState.Execute(gameObject);
	}

	public void ChangeState(State newState)
	{
		_previousState = _currentState;

		if (_currentState != null) _currentState.Exit(gameObject);

		_currentState = newState;

		if (_currentState != null) _currentState.Enter(gameObject);
	}

	public void ChangeGlobalState(State newState)
	{
		_previousGlobalState = _currentState;

		if (_currentGlobalState != null) _currentGlobalState.Exit(gameObject);

		_currentGlobalState = newState;

		if (_currentGlobalState != null) _currentGlobalState.Enter(gameObject);
	}

	public void RevertToPreviousState()
	{
		if (_previousState != null)
			ChangeState(_previousState);
	}

	public void RevertToPreviousGlobalState()
	{
		if (_previousGlobalState != null)
			ChangeGlobalState(_previousGlobalState);
	}

	public bool IsInState(State state)
	{
		return _currentState.GetType() == typeof(State);
	}

	public bool IsInGlobalState(State state)
	{
		return _currentGlobalState.GetType() == typeof(State);
	}
	#endregion
}
