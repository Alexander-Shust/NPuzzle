using TMPro;
using UnityEngine;

public class NumberItem : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rect;
    
    [SerializeField]
    private TMP_Text _number;

    public void SetNumber(int number)
    {
        _number.text = number.ToString();
    }

    public void SetSize(float width, float height)
    {
        _rect.sizeDelta = new Vector2(width, height);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}