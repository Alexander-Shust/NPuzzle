using System.Collections.Generic;
using UnityEngine;

public class StateDisplay : MonoBehaviour
{
    [SerializeField]
    private RectTransform _panel;

    [SerializeField]
    private GameObject _numberPrefab;
    
    private int _size;
    private List<NumberItem> _numbers;

    public void SetState(int[,] state)
    {
        for (var i = 0; i < _size; ++i)
        {
            for (var j = 0; j < _size; ++j)
            {
                _numbers[i * _size + j].SetNumber(state[i, j]);
            }
        }
    }

    public void SetSize(int size)
    {
        _size = size;
        var rect = _panel.rect;
        var numberWidth = rect.width / size;
        var numberHeight = rect.height / size;
        _numbers = new List<NumberItem>();
        for (var i = 0; i < _size * _size; ++i)
        {
            var position = new Vector2(i % _size * numberWidth, rect.height - i / size * numberHeight);
            var numberGo = Instantiate(_numberPrefab, position, Quaternion.identity, _panel);
            var numberItem = numberGo.GetComponent<NumberItem>();
            numberItem.SetSize(numberWidth, numberHeight);
            _numbers.Add(numberItem);
        }
    }
}