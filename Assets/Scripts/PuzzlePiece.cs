using TMPro;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _numberText;

    public void SetNumber(int number)
    {
        _numberText.text = number.ToString();
    }

    public void SetFontSize(float fontSize)
    {
        _numberText.fontSize = fontSize;
    }
}