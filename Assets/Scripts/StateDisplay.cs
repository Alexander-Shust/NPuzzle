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

    private void Clear()
    {
        if (_numbers == null) return;
        
        foreach (var number in _numbers)
        {
            number.Destroy();
        }

        _numbers = null;
    }

    public void SetSize(int size)
    {
        Clear();
        _size = size;
        var rect = (Vector2) _panel.position;
        var numberWidth = 500.0f / _size;
        var numberHeight = 500.0f / _size;
        var startX = rect.x - 250.0f + numberWidth / 2;
        var startY = rect.y - 250.0f - numberHeight / 2;
        _numbers = new List<NumberItem>();
        for (var i = 0; i < _size * _size; ++i)
        {
            var position = new Vector2(startX + i % _size * numberWidth, startY + 500.0f - i / _size * numberHeight);
            var numberGo = Instantiate(_numberPrefab, position, Quaternion.identity, _panel);
            var numberItem = numberGo.GetComponent<NumberItem>();
            numberItem.SetSize(numberWidth, numberHeight);
            _numbers.Add(numberItem);
        }
    }
}