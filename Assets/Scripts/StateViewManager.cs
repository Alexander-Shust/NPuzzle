using UnityEngine;

public class StateViewManager : MonoBehaviour
{
    [SerializeField]
    private StateDisplay _currentState;

    [SerializeField]
    private StateDisplay _targetState;

    private void Awake()
    {
        _currentState.gameObject.SetActive(false);
        _targetState.gameObject.SetActive(false);
    }
}