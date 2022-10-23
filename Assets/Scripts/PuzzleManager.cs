using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField]
    private Transform _board;

    [SerializeField]
    private GameObject _piecePrefab;
    
    [SerializeField]
    private int _boardSize = 4;

    [SerializeField]
    private float _spacing = 0.1f;

    private float _boardWidth;
    private float _boardHeight;
    private float _pieceSize;
    
    private void Start()
    {
        var boardLocalScale = _board.localScale;
        _boardWidth = boardLocalScale.x;
        _boardHeight = boardLocalScale.z;
        _pieceSize = (Mathf.Min(_boardWidth, _boardHeight) - _spacing * (_boardSize - 1)) / _boardSize;
        var fillOrigin = new Vector3((_pieceSize -_boardWidth) / 2, 0.0f, (_pieceSize -_boardHeight) / 2);
        
        for (var i = 0; i < _boardSize; ++i)
        {
            for (var j = 0; j < _boardSize; ++j)
            {
                var piecePosition = fillOrigin + new Vector3(i * (_pieceSize + _spacing), 1, j * (_pieceSize + _spacing));
                var pieceGo = Instantiate(_piecePrefab, piecePosition, Quaternion.identity, _board);
                pieceGo.transform.localScale *= _pieceSize / Mathf.Min(_boardHeight, _boardWidth);
            }
        }
    }
}
