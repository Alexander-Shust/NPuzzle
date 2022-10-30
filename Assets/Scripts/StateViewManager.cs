using System.Collections.Generic;
using UnityEngine;

public class StateViewManager : MonoBehaviour
{
    [SerializeField]
    private StateDisplay _currentState;

    [SerializeField]
    private StateDisplay _targetState;

    private List<int[,]> _states;
        
    private void Awake()
    {
        SetActive(false);
    }

    public void SetActive(bool isActive)
    {
        _currentState.gameObject.SetActive(isActive);
        _targetState.gameObject.SetActive(isActive);
    }

    public void SetStates(List<int[,]> states)
    {
        _states = states;
        _currentState.SetState(_states[0]);
        _targetState.SetState(_states[states.Count - 1]);
    }

    public void SetSize(int size)
    {
        _currentState.SetSize(size);
        _targetState.SetSize(size);
    }
}