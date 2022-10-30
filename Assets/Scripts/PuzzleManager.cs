using JetBrains.Annotations;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField]
    private PuzzleType _puzzleType;

    [SerializeField]
    private Solvable _solvable;
    
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
    private int[,] _goal;

    private void Start()
    {
        var boardLocalScale = _board.localScale;
        _boardWidth = boardLocalScale.x;
        _boardHeight = boardLocalScale.z;
        _pieces = new GameObject[_boardSize * _boardSize];
        
        InitBoard();
    }

    private bool IsSolvable(int[,] pieces)
    {
        var zeroX = _boardSize - 1;
        var zeroY = _boardSize - 1;
        for (var i = 0; i < _boardSize * _boardSize; ++i)
        {
            if (pieces[i / _boardSize, i % _boardSize] == 0)
            {
                zeroX = i / _boardSize;
                zeroY = i % _boardSize;
                break;
            }
        }

        for (var i = zeroX; i < _boardSize - 1; ++i)
        {
            (pieces[i, zeroY], pieces[i + 1, zeroY]) = (pieces[i + 1, zeroY], pieces[i, zeroY]);
        }

        zeroX = _boardSize - 1;

        for (var i = zeroY; i < _boardSize - 1; ++i)
        {
            (pieces[zeroX, i], pieces[zeroX, i + 1]) = (pieces[zeroX, i + 1], pieces[zeroX, i]);
        }

        var errors = 0;
        for (var i = 1; i < _boardSize * _boardSize; ++i)
        {
            for (var j = 0; j < _boardSize * _boardSize; ++j)
            {
                var x = j / _boardSize;
                var y = j % _boardSize;
                if (pieces[x, y] == i) break;
                if (pieces[x, y] > i)
                {
                    ++errors;
                }
            }
        }

        return errors % 2 == 0;
    }
    
    

    private void ClearBoard()
    {
        foreach (var go in _pieces)
        {
            if (go != null)
            {
                Destroy(go);
            }
        }
    } 
    
    [UsedImplicitly]
    public void InitBoard()
    {
        ClearBoard();
        var size = _boardSize;
        var pieces = PuzzleGenerator.Generate(size, _puzzleType, _solvable);
        _goal = _puzzleType switch
        {
            PuzzleType.Snail => PuzzleGenerator.GenerateSnailPosition(size),
            _ => PuzzleGenerator.GenerateSovietPosition(size)
        };
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

        var isStraightSolvable = IsSolvable((int[,]) pieces.Clone());
        var isSolvable = isStraightSolvable && _puzzleType == PuzzleType.Soviet;
        isSolvable |= isStraightSolvable && _puzzleType == PuzzleType.Snail && _boardSize % 2 == 0;
        isSolvable |= !isStraightSolvable && _puzzleType == PuzzleType.Snail && _boardSize % 2 == 1;
        if (isSolvable)
        {
            var moves = Solver.Solve(_boardSize, pieces, _goal);
            Debug.LogError($"Solved in {moves.Count} moves");
        }
        else
        {
            Debug.LogError("Unsolvable");
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            Up();
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            Down();
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            Left();
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            Right();
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            InitBoard();
        }
    }

    [UsedImplicitly]
    public void Up()
    {
        var target = _emptyPosition - _boardSize;
        if (target < 0) return;
        
        var pieceGo = _pieces[target];
        var position = pieceGo.transform.position;
        position.z -= _pieceSize + _spacing;
        pieceGo.transform.position = position;
        SwapWithEmpty(target);
    }

    [UsedImplicitly]
    public void Down()
    {
        var target = _emptyPosition + _boardSize;
        if (target >= _boardSize * _boardSize) return;
        
        var pieceGo = _pieces[target];
        var position = pieceGo.transform.position;
        position.z += _pieceSize + _spacing;
        pieceGo.transform.position = position;
        SwapWithEmpty(target);
    }

    [UsedImplicitly]
    public void Left()
    {
        if (_emptyPosition % _boardSize == 0) return;
        var target = _emptyPosition - 1;
        
        var pieceGo = _pieces[target];
        var position = pieceGo.transform.position;
        position.x += _pieceSize + _spacing;
        pieceGo.transform.position = position;
        SwapWithEmpty(target);
    }

    [UsedImplicitly]
    public void Right()
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
        if (IsVictory())
        {
            Debug.LogError("Victory");
            InitBoard();
        }
    }

    private bool IsVictory()
    {
        for (var i = 0; i < _boardSize * _boardSize; ++i)
        {
            var x = i / _boardSize;
            var y = i % _boardSize;
            if (_pieces[i] == null)
            {
                if (_goal[x, y] == 0) continue;
                return false;
            }
            var piece = _pieces[i].GetComponent<PuzzlePiece>();
            
            if (piece.Number != _goal[x, y]) return false;
        }
        return true;
    }
}
