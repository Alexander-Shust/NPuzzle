using Enums;
using TMPro;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown _heuristic;

    [SerializeField]
    private TMP_Dropdown _size;

    [SerializeField]
    private TMP_Dropdown _type;

    [SerializeField]
    private TMP_Dropdown _solvable;
    
    public void UpdateSettings()
    {
        Settings.Heuristic = (Heuristic) _heuristic.value;
        Settings.Type = (PuzzleType) _type.value;
        Settings.Size = _size.value + 2;
        Settings.Solvable = (Solvable) _solvable.value;
    }
}