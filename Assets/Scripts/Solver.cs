using System.Collections.Generic;
using UnityEngine;

public static class Solver
{
    private static int[,] _puzzle;
    private static int[,] _goal;
    private static int _size;
    
    public static List<Move> Solve(int size, int[,] puzzle, int[,] goal)
    {
        var moves = new List<Move>();
        _puzzle = puzzle;
        _goal = goal;
        _size = size;var zeroX = _size - 1;
        var zeroY = _size - 1;
        for (var i = 0; i < _size * _size; ++i)
        {
            if (puzzle[i / _size, i % _size] == 0)
            {
                zeroX = i / _size;
                zeroY = i % _size;
                break;
            }
        }
        var possibleMoves = new List<Move>();
        while (!IsEqual())
        {
            possibleMoves.Clear();
            if (zeroX > 0) possibleMoves.Add(Move.Up);
            if (zeroX < size - 1) possibleMoves.Add(Move.Down);
            if (zeroY > 0) possibleMoves.Add(Move.Left);
            if (zeroY < size - 1) possibleMoves.Add(Move.Right);

            var chosenMove = possibleMoves[Random.Range(0, possibleMoves.Count)];
            var chosenCount = GetVariantScore(chosenMove, zeroX, zeroY, out var chosenX, out var chosenY);
            foreach (var move in possibleMoves)
            {
                var count = GetVariantScore(move, zeroX, zeroY, out var targetX, out var targetY);
                if (count < chosenCount)
                {
                    chosenCount = count;
                    chosenX = targetX;
                    chosenY = targetY;
                    chosenMove = move;
                }
            }
            (_puzzle[zeroX, zeroY], _puzzle[chosenX, chosenY]) = (_puzzle[chosenX, chosenY], _puzzle[zeroX, zeroY]);
            zeroX = chosenX;
            zeroY = chosenY;
            moves.Add(chosenMove);
            
        }
        return moves;
    }

    private static int GetVariantScore(Move move, int zeroX, int zeroY, out int targetX, out int targetY)
    {
        targetX = move switch
        {
            Move.Up => zeroX - 1,
            Move.Down => zeroX + 1,
            _ => zeroX
        };
        targetY = move switch
        {
            Move.Right => zeroY + 1,
            Move.Left => zeroY - 1,
            _ => zeroY
        };
        var variant = (int[,]) _puzzle.Clone();
        return ErrorCount(variant);
    }

    private static int ErrorCount(int[,] variant)
    {
        var count = 0;
        for (var i = 0; i < _size * _size; ++i)
        {
            for (var j = 0; j < _size * _size; ++j)
            {
                var x = j / _size;
                var y = j % _size;
                if (variant[x, y] == _goal[x, y]) break;
                if (variant[x, y] > _goal[x, y])
                {
                    ++count;
                }
            }
        }

        return count;
    }

    private static bool IsEqual()
    {
        for (var i = 0; i < _size; ++i)
        {
            for (var j = 0; j < _size; ++j)
            {
                if (_puzzle[i, j] != _goal[i, j]) return false;
            }
        }
        return true;
    }
}