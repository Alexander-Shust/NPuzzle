using System;
using System.IO;
using System.Text.RegularExpressions;
using Enums;
using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField]
    private SettingsManager _settingsManager;

    [SerializeField]
    private ResultManager _resultManager;

    [SerializeField]
    private Transform _board;

    [SerializeField]
    private Button _solveButton;

    [SerializeField]
    private Button _moveButton;

    [SerializeField]
    private Transform _buttons;

    [SerializeField]
    private TMP_Text _unsolvable;

    [SerializeField]
    private StateViewManager _viewManager;

    [SerializeField]
    private GameObject _piecePrefab;

    [SerializeField]
    private float _spacing = 0.1f;
    
    [SerializeField]
    private float _moveSpeed = 2.0f;
    
    private PuzzleType _puzzleType;
    
    private int _boardSize = 3;
    private Solvable _solvable = Solvable.Yes;
    
    private float _boardWidth;
    private float _boardHeight;
    private float _pieceSize;

    private GameObject[] _pieces;
    private int _emptyPosition;
    private int[,] _goal;

    private Solution _solution;
    private int _solutionStep;
    private bool _isFinished;
    private bool _isActive;
    private bool _isMoving;
    private Vector3 _targetPosition;
    private Transform _movingTransform;

    private void Awake()
    {
        _pieces = new GameObject[_boardSize * _boardSize];
        var boardLocalScale = _board.localScale;
        _boardWidth = boardLocalScale.x;
        _boardHeight = boardLocalScale.z;
        _solveButton.gameObject.SetActive(false);
        _moveButton.gameObject.SetActive(false);
        _unsolvable.gameObject.SetActive(false);
        PatternManager.LoadHeuristics();
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
        _buttons.gameObject.SetActive(true);
        _board.gameObject.SetActive(true);
        _solveButton.gameObject.SetActive(false);
        _moveButton.gameObject.SetActive(false);
        _unsolvable.gameObject.SetActive(false);
        _viewManager.SetActive(false);
        _resultManager.SetActive(false);
        _isFinished = false;
    } 
    
    [UsedImplicitly]
    public void InitBoard()
    {
        ClearBoard();
        _settingsManager.UpdateSettings();
        _boardSize = Settings.Size;
        _puzzleType = Settings.Type;
        _solvable = Settings.Solvable;
        
        var size = _boardSize;
        var pieces = PuzzleGenerator.Generate(size, _puzzleType, _solvable);
        _goal = _puzzleType switch
        {
            PuzzleType.Snail => PuzzleGenerator.GenerateSnailPosition(size),
            PuzzleType.Soviet => PuzzleGenerator.GenerateSovietPosition(size),
            _ => throw new ArgumentOutOfRangeException()
        };
        CreatePieces(size, pieces);
    }

    private void CreatePieces(int size, int[,] pieces)
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

        var isStraightSolvable = IsSolvable((int[,]) pieces.Clone());
        var isSolvable = isStraightSolvable && _puzzleType == PuzzleType.Soviet;
        isSolvable |= isStraightSolvable && _puzzleType == PuzzleType.Snail && _boardSize % 2 == 0;
        isSolvable |= !isStraightSolvable && _puzzleType == PuzzleType.Snail && _boardSize % 2 == 1;
        if (isSolvable)
        {
            _solveButton.gameObject.SetActive(true);
        }
        else
        {
            _unsolvable.gameObject.SetActive(true);
        }
    }

    [UsedImplicitly]
    public void Solve()
    {
        var pieces = new int[_boardSize, _boardSize];
        for (var i = 0; i < _boardSize * _boardSize; ++i)
        {
            var x = i / _boardSize;
            var y = i % _boardSize;
            var piece = _pieces[i];
            pieces[x, y] = piece == null ? 0 : piece.GetComponent<PuzzlePiece>().Number;
        }

        var solution = Solver.Solve(pieces, _goal, _puzzleType);
        
        _board.gameObject.SetActive(false);
        _buttons.gameObject.SetActive(false);
        _solveButton.gameObject.SetActive(false);
        _moveButton.gameObject.SetActive(true);
        _isFinished = true;
        _solutionStep = 0;
        _solution = solution;
        
        _viewManager.SetActive(true);
        _viewManager.SetSize(_boardSize);
        _viewManager.SetStates(solution.States);
        
        _resultManager.SetActive(true);
        _resultManager.SetMoveCount(solution.Moves);
        _resultManager.SetElapsedTime(solution.Time);
        _resultManager.SetVisitedCount(solution.Visited);
        _resultManager.SetHashSize(solution.HashSize);
    }

    private void Update()
    {
        if (!_isFinished)
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

        else if (!_isMoving && _isActive)
        {
            if (++_solutionStep <= _solution.Moves)
            {
                var nextStep = _solution.States[_solutionStep];
                var zeroX = _emptyPosition / _boardSize;
                var zeroY = _emptyPosition % _boardSize;
                var newZeroX = zeroX;
                var newZeroY = zeroY;
                for (var i = 0; i < _boardSize * _boardSize; ++i)
                {
                    if (nextStep[i / _boardSize, i % _boardSize] == 0)
                    {
                        newZeroX = i / _boardSize;
                        newZeroY = i % _boardSize;
                        break;
                    }
                }

                if (newZeroX == zeroX)
                {
                    if (newZeroY > zeroY)
                    {
                        Right();
                    }
                    else
                    {
                        Left();
                    }
                }
                else if (newZeroX > zeroX)
                {
                    Down();
                }
                else
                {
                    Up();
                }
            }
            else
            {
                _isFinished = false;
                _isActive = false;
            }
        }

        if (!_isMoving) return;

        var currentPosition = _movingTransform.position;
        var direction = (_targetPosition - currentPosition).normalized;
        currentPosition += direction * (Time.deltaTime * _pieceSize * _moveSpeed);
        if ((_targetPosition - currentPosition).magnitude < 0.01f)
        {
            currentPosition = _targetPosition;
            _isMoving = false;
        }

        _movingTransform.position = currentPosition;
    }

    private void Move(Transform pieceTransform, Vector3 targetPosition)
    {
        _isMoving = true;
        _movingTransform = pieceTransform;
        _targetPosition = targetPosition;
    }

    [UsedImplicitly]
    public void Up()
    {
        var target = _emptyPosition - _boardSize;
        if (_isMoving || target < 0) return;
        
        var pieceGo = _pieces[target];
        var position = pieceGo.transform.position;
        position.z -= _pieceSize + _spacing;
        Move(pieceGo.transform, position);
        SwapWithEmpty(target);
    }

    [UsedImplicitly]
    public void Down()
    {
        var target = _emptyPosition + _boardSize;
        if (_isMoving || target >= _boardSize * _boardSize) return;
        
        var pieceGo = _pieces[target];
        var position = pieceGo.transform.position;
        position.z += _pieceSize + _spacing;
        Move(pieceGo.transform, position);
        SwapWithEmpty(target);
    }

    [UsedImplicitly]
    public void Left()
    {
        if (_isMoving || _emptyPosition % _boardSize == 0) return;
        var target = _emptyPosition - 1;
        
        var pieceGo = _pieces[target];
        var position = pieceGo.transform.position;
        position.x += _pieceSize + _spacing;
        Move(pieceGo.transform, position);
        SwapWithEmpty(target);
    }

    [UsedImplicitly]
    public void Right()
    {
        var target = _emptyPosition + 1;
        if (_isMoving || target % _boardSize == 0) return;
        
        var pieceGo = _pieces[target];
        var position = pieceGo.transform.position;
        position.x -= _pieceSize + _spacing;
        Move(pieceGo.transform, position);
        SwapWithEmpty(target);
    }

    private void SwapWithEmpty(int target)
    {
        _pieces[_emptyPosition] = _pieces[target];
        _pieces[target] = null;
        _emptyPosition = target;
    }

    [UsedImplicitly]
    public void DisplaySolution()
    {
        _board.gameObject.SetActive(true);
        _viewManager.SetActive(false);
        _moveButton.gameObject.SetActive(false);
        _isActive = true;
    }

    [UsedImplicitly]
    public void LoadFile()
    {
        var filePath = EditorUtility.OpenFilePanel("Select File", Settings.PuzzleDirectory, "txt");
        var lines = File.ReadAllLines(filePath);
        var sizeLineNumber = -1;
        var size = 0;
        for (var i = 0; i < lines.Length; ++i)
        {
            var line = lines[i];
            if (line == string.Empty || line[0] == '#') continue;
            
            var content = line.Split('#')[0];
            var numbers = content.Split(' ');
            if (numbers.Length == 1)
            {
                if (int.TryParse(numbers[0], out size))
                {
                    sizeLineNumber = i;
                    break;
                }
            }
        }

        if (sizeLineNumber == -1) return;
        var puzzle = new int[size, size];
        var filledLines = 0;
        for (var i = sizeLineNumber + 1; i < lines.Length; ++i)
        {
            var line = lines[i];
            if (line == string.Empty || line[0] == '#') continue;
            
            var content = Regex.Replace(line.Split('#')[0], "\\s+", " ");
            var numbers = content.Trim(' ').Split(' ');
            if (numbers.Length == size)
            {
                for (var j = 0; j < size; ++j)
                {
                    if (int.TryParse(numbers[j], out var number))
                    {
                        puzzle[filledLines, j] = number;
                    }
                    else
                    {
                        return;
                    }
                }

                ++filledLines;
            }
            else
            {
                return;
            }
        }

        if (filledLines != size) return;
        
        ClearBoard();
        _boardSize = size;
        _settingsManager.UpdateSettings();
        _puzzleType = Settings.Type;
        _goal = _puzzleType switch
        {
            PuzzleType.Snail => PuzzleGenerator.GenerateSnailPosition(size),
            PuzzleType.Soviet => PuzzleGenerator.GenerateSovietPosition(size),
            _ => throw new ArgumentOutOfRangeException()
        };
        CreatePieces(size, puzzle);
    }
}
