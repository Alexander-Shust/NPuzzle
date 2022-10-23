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

    private GameObject[] _pieces;
    private int _emptyPosition;

    private void Start()
    {
        var boardLocalScale = _board.localScale;
        _boardWidth = boardLocalScale.x;
        _boardHeight = boardLocalScale.z;
        var puzzle = GeneratePuzzle(_boardSize);
        InitBoard(_boardSize, puzzle);
        Down();
        Right();
        Down();
        Left();
    }

    public void InitBoard(int size, int[,] pieces)
    {
        _pieces = new GameObject[size * size];
        _pieceSize = (Mathf.Min(_boardWidth, _boardHeight) - _spacing * (size - 1)) / size;
        var fontSize = 36 / size;
        var fillOrigin = new Vector3((_pieceSize -_boardWidth) / 2, 0.0f, (_boardHeight - _pieceSize) / 2);
        
        for (var i = 0; i < size; ++i)
        {
            for (var j = 0; j < size; ++j)
            {
                if (pieces[i, j] == 0)
                {
                    _emptyPosition = i * size + j;
                    continue;
                }
                
                var piecePosition = fillOrigin + new Vector3(j * (_pieceSize + _spacing), 1, -i * (_pieceSize + _spacing));
                var pieceGo = Instantiate(_piecePrefab, piecePosition, Quaternion.identity, _board);
                pieceGo.transform.localScale *= _pieceSize / Mathf.Min(_boardHeight, _boardWidth);
                _pieces[i * size + j] = pieceGo;

                var piece = pieceGo.GetComponent<PuzzlePiece>();
                piece.SetNumber(pieces[i, j]);
                piece.SetFontSize(fontSize);
            }
        }
    }

    private void Up()
    {
        var target = _emptyPosition - _boardSize;
        if (target < 0) return;
        
        var pieceGo = _pieces[target];
        var position = pieceGo.transform.position;
        position.z -= _pieceSize + _spacing;
        pieceGo.transform.position = position;
        SwapWithEmpty(target);
    }

    private void Down()
    {
        var target = _emptyPosition + _boardSize;
        if (target >= _boardSize * _boardSize) return;
        
        var pieceGo = _pieces[target];
        var position = pieceGo.transform.position;
        position.z += _pieceSize + _spacing;
        pieceGo.transform.position = position;
        SwapWithEmpty(target);
    }

    private void Left()
    {
        if (_emptyPosition % _boardSize == 0) return;
        var target = _emptyPosition - 1;
        
        var pieceGo = _pieces[target];
        var position = pieceGo.transform.position;
        position.x += _pieceSize + _spacing;
        pieceGo.transform.position = position;
        SwapWithEmpty(target);
    }

    private void Right()
    {
        var target = _emptyPosition + 1;
        if (target % _boardSize == 0) return;
        
        var pieceGo = _pieces[target];
        var position = pieceGo.transform.position;
        position.x -= _pieceSize + _spacing;
        pieceGo.transform.position = position;
        SwapWithEmpty(target);
    }

    private void SwapWithEmpty(int target)
    {
        _pieces[_emptyPosition] = _pieces[target];
        _pieces[target] = null;
        _emptyPosition = target;
    }

    public int[,] GeneratePuzzle(int size)
    {
        var puzzle = new int[size, size];
        for (var i = 0; i < size * size; ++i)
        {
            puzzle[i / size, i % size] = i;
        }
        return puzzle;
    }
}
