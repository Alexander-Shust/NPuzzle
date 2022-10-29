using TMPro;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _numberText;
    
    public int Number { get; private set; }

    public void SetNumber(int number)
    {
        Number = number;
        _numberText.text = number.ToString();
    }

    public void SetFontSize(float fontSize)
    {
        _numberText.fontSize = fontSize;
    }
}