using TMPro;
using UnityEngine;

public class NumberItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _number;

    public void SetNumber(int number)
    {
        _number.text = number.ToString();
    }
}