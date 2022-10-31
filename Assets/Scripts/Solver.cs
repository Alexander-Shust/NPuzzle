using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Solver
{
    private static int[,] _puzzle;
    private static int[,] _goal;
    private static int _size;
    
    public static List<int[,]> Solve(int size, int[,] puzzle, int[,] goal)
    {
        var states = new List<int[,]>();
        _puzzle = (int[,]) puzzle.Clone();
        _goal = (int[,]) goal.Clone();
        _size = size;
        
        var zeroX = _size - 1;
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

        // var iterations = 0;
        var possibleMoves = new List<Move>();
        while (!IsEqual(_puzzle, _goal))
        {
            // if (iterations++ > 1000000)
            // {
            //     Debug.LogError(iterations);
            //     break;
            // }
            possibleMoves.Clear();
            if (zeroX > 0) possibleMoves.Add(Move.Up);
            if (zeroX < size - 1) possibleMoves.Add(Move.Down);
            if (zeroY > 0) possibleMoves.Add(Move.Left);
            if (zeroY < size - 1) possibleMoves.Add(Move.Right);

            // if (moves.Count > 0)
            // {
            //     var lastMove = moves[moves.Count - 1];
            //     possibleMoves.Remove(ReverseMove(lastMove));
            // }

            var tempMoves = possibleMoves.ToArray();
            
            foreach (var move in tempMoves)
            {
                var possibleState = GetVariantState(move, zeroX, zeroY);
                if (states.Any(state => IsEqual(state, possibleState)))
                {
                    possibleMoves.Remove(move);
                    // break;
                }
            }
            
            if (possibleMoves.Count == 0)
            {
                states.RemoveAt(states.Count - 1);
                continue;
            }

            var chosenMove = possibleMoves[0];
            var chosenCount = GetVariantScore(chosenMove, zeroX, zeroY, out var chosenX, out var chosenY);
            
            for (var i = 1; i < possibleMoves.Count; ++i)
            {
                var move = possibleMoves[i];
                var count = GetVariantScore(move, zeroX, zeroY, out var targetX, out var targetY);
                if (count < chosenCount)
                {
                    chosenCount = count;
                    chosenX = targetX;
                    chosenY = targetY;
                }
            }
            states.Add((int[,]) _puzzle.Clone());
            (_puzzle[zeroX, zeroY], _puzzle[chosenX, chosenY]) = (_puzzle[chosenX, chosenY], _puzzle[zeroX, zeroY]);
            zeroX = chosenX;
            zeroY = chosenY;
        }
        states.Add((int[,]) _puzzle.Clone());
        return states;
    }

    private static int[,] GetVariantState(Move move, int zeroX, int zeroY)
    {
        var targetX = move switch
        {
            Move.Up => zeroX - 1,
            Move.Down => zeroX + 1,
            _ => zeroX
        };
        var targetY = move switch
        {
            Move.Right => zeroY + 1,
            Move.Left => zeroY - 1,
            _ => zeroY
        };
        var variant = (int[,]) _puzzle.Clone();
        (variant[zeroX, zeroY], variant[targetX, targetY]) = (variant[targetX, targetY], variant[zeroX, zeroY]);
        return variant;
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
        (variant[zeroX, zeroY], variant[targetX, targetY]) = (variant[targetX, targetY], variant[zeroX, zeroY]);
        return ErrorCount(variant);
    }

    private static int ErrorCount(int[,] variant)
    {
        var count = 0;
        for (var i = 0; i < _size * _size; ++i)
        {
            var x = i / _size;
            var y = i % _size;
            if (variant[x, y] != _goal[x, y] && variant[x, y] != 0) 
            { 
                ++count;
            }
        }

        return count;
    }

    private static bool IsEqual(int[,] state1, int[,] state2)
    {
        for (var i = 0; i < _size; ++i)
        {
            for (var j = 0; j < _size; ++j)
            {
                if (state1[i, j] != state2[i, j]) return false;
            }
        }
        return true;
    }

    private static Move ReverseMove(Move move)
    {
        return move switch
        {
            Move.Up => Move.Down,
            Move.Right => Move.Left,
            Move.Down => Move.Up,
            Move.Left => Move.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(move), move, null)
        };
    }
}