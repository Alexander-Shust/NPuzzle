using System.Collections.Generic;
using System.Diagnostics;
using Enums;

public static class Solver
{
    private static int[,] _puzzle;
    private static int[,] _goal;
    private static int _size;

    public static Solution Solve(int[,] puzzle, int[,] goal, PuzzleType type)
    {
        var states = new List<int[,]>();
        _puzzle = (int[,]) puzzle.Clone();
        _goal = (int[,]) goal.Clone();
        _size = Settings.Size;
        
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
        
        var goalZeroX = type == PuzzleType.Soviet ? _size - 1 : _size / 2;
        var goalZeroY = type == PuzzleType.Soviet ? _size - 1 : _size / 2 + _size % 2 - 1;
        
        var timer = new Stopwatch();
        var start = new State(_size, _puzzle, zeroX, zeroY, 0);
        var target = new State(_size, _goal, goalZeroX, goalZeroY, 0);
        var search = new Search(start, target);
        
        timer.Start();
        var final = search.Astar();
        timer.Stop();
        
        var current = final;
        while (current.String != start.String)
        {
            states.Add(current.Numbers);
            current = current.Previous;
        }
        states.Add(start.Numbers);
        states.Reverse();
        var result = new Solution
        {
            States = states,
            Moves = final.Depth,
            Time = timer.ElapsedMilliseconds,
            Visited = search.VisitedCount,
            HashSize = search.HashSize
        };
        return result;
    }
}