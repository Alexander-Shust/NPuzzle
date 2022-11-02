using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class StateViewManager : MonoBehaviour
{
    [SerializeField]
    private StateDisplay _currentState;

    [SerializeField]
    private StateDisplay _targetState;

    private List<int[,]> _states;

    private int _currentIndex;

    private int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            _currentIndex = value;
            UpdateStates();
        }
    }

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
        if (states.Count < 2) return;

        _states = states;
        CurrentIndex = 0;
    }

    private void UpdateStates()
    {
        _currentState.SetState(_states[CurrentIndex]);
        _targetState.SetState(_states[CurrentIndex + 1]);
    }

    [UsedImplicitly]
    public void NextState()
    {
        if (_states.Count <= CurrentIndex + 2) return;

        ++CurrentIndex;
    }

    [UsedImplicitly]
    public void PreviousState()
    {
        if (CurrentIndex <= 0) return;

        --CurrentIndex;
    }

    public void SetSize(int size)
    {
        _currentState.SetSize(size);
        _targetState.SetSize(size);
    }
}