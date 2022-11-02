using System.Collections.Generic;
using System.Diagnostics;
using Enums;

public static class Solver
{
    private static int[,] _puzzle;
    private static int[,] _goal;
    private static int _size;

    public static List<int[,]> Solve(int[,] puzzle, int[,] goal, PuzzleType type)
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
        
        var watch = new Stopwatch();
        var start = new State(_size, puzzle, zeroX, zeroY, 0);
        var target = new State(_size, goal, goalZeroX, goalZeroY, 0);
        var search = new Search(start, target);
                            

        watch.Start();
        var final = search.Astar();
        watch.Stop();
        
        UnityEngine.Debug.LogError($"States visited {search.VisitedCount}");
        UnityEngine.Debug.LogError($"Elapsed {watch.ElapsedMilliseconds} milliseconds");
        UnityEngine.Debug.LogError($"Node at depth {final.Depth}");
        
        states.Add(final.Numbers);
        return states;
    }
}