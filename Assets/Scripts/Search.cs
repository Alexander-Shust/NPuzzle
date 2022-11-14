using System;
using System.Collections.Generic;
using DataStructures.PriorityQueue;
using Enums;

public class Search
{
    public int VisitedCount { get; set; }
    public int HashSize => _hash.Count;

    private State _target;
    private PriorityQueue<State, int> _queue;
    private HashSet<string> _hash;

    public Search(State start, State target)
    {
        _queue = new PriorityQueue<State, int>(0);
        _target = target;
        _hash = new HashSet<string>();
        _queue.Insert(start, 0);
    }

    public State Astar()
    {
        _hash.Add(_queue.Top().String);
        while (_queue.Top() != null)
        {
            var state = _queue.Pop();
            ++VisitedCount;
            if (state.String.Equals(_target.String))
            {
                return state;
            }

            Expand(state);
        }

        return null;
    }

    private void Expand(State state)
    {
        var x = state.ZeroX;
        var y = state.ZeroY;
        var size = state.Size;
        var depth = state.Depth;

        if (x > 0)
        {
            var numbers = (int[,]) state.Numbers.Clone();
            (numbers[x - 1, y], numbers[x, y]) = (numbers[x, y], numbers[x - 1, y]);
            var newState = new State(size, numbers, x - 1, y, depth + 1, state);
            AddState(newState);
        }

        if (x < size - 1)
        {
            var numbers = (int[,]) state.Numbers.Clone();
            (numbers[x + 1, y], numbers[x, y]) = (numbers[x, y], numbers[x + 1, y]);
            var newState = new State(size, numbers, x + 1, y, depth + 1, state);
            AddState(newState);
        }

        if (y > 0)
        {
            var numbers = (int[,]) state.Numbers.Clone();
            (numbers[x, y - 1], numbers[x, y]) = (numbers[x, y], numbers[x, y - 1]);
            var newState = new State(size, numbers, x, y - 1, depth + 1, state);
            AddState(newState);
        }

        if (y < size - 1)
        {
            var numbers = (int[,]) state.Numbers.Clone();
            (numbers[x, y + 1], numbers[x, y]) = (numbers[x, y], numbers[x, y + 1]);
            var newState = new State(size, numbers, x, y + 1, depth + 1, state);
            AddState(newState);
        }
    }

    private void AddState(State newState)
    {
        if (!_hash.Contains(newState.String))
        {
            _queue.Insert(newState, newState.Depth - 1 + GetHeuristicDistance(newState));
            _hash.Add(newState.String);
        }
    }

    private int GetHeuristicDistance(State state)
    {
        return Settings.Heuristic switch
        {
            Heuristic.Misplaced => ErrorCount(state),
            Heuristic.Manhattan => Manhattan(state),
            Heuristic.LineConflicts => Manhattan(state) + LineConflicts(state),
            Heuristic.PatternDb => PatternDb(state),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private int PatternDb(State state)
    {
        if (state.Size != 4 || Settings.Type != PuzzleType.Soviet)
        {
            return Manhattan(state) + LineConflicts(state);
        }

        var singleArray = new int[16];
        var numbers = state.Numbers;
        for (var i = 0; i < 4; ++i)
        {
            for (var j = 0; j < 4; ++j)
            {
                singleArray[4 * i + j] = numbers[i, j];
            }
        }
        return PatternManager.GetHeuristic(singleArray);
    }

    private int ErrorCount(State state)
    {
        var count = 0;
        for (var i = 0; i < state.Size; ++i)
        {
            for (var j = 0; j < state.Size; ++j)
            {
                if (state.Numbers[i, j] != _target.Numbers[i, j] && state.Numbers[i, j] != 0)
                {
                    ++count;
                }
            }
        }
        return count;
    }

    private int Manhattan(State state)
    {
        var distance = 0;
        for (var i = 0; i < state.Size; ++i)
        {
            for (var j = 0; j < state.Size; ++j)
            {
                var current = state.Numbers[i, j];
                if (current == 0) continue;

                var isFound = false;
                for (var i2 = 0; i2 < state.Size; ++i2)
                {
                    for (var j2 = 0; j2 < state.Size; ++j2)
                    {
                        if (_target.Numbers[i2, j2] == current)
                        {
                            distance += Math.Abs(i2 - i) + Math.Abs(j2 - j);
                            isFound = true;
                            break;
                        }
                    }

                    if (isFound) break;
                }
            }
        }
        return distance;
    }

    private int LineConflicts(State state)
    {
        var distance = 0;
        for (var i = 0; i < state.Size; ++i)
        {
            distance += CountRowConflicts(state.Size, state.Numbers, i);
            distance += CountColumnConflicts(state.Size, state.Numbers, i);
        }
        return distance;
    }

    private int CountRowConflicts(int size, int[,] numbers, int index)
    {
        var count = 0;
        var pieces = new List<int>();
        for (var i = 0; i < size - 1; ++i)
        {
            if (pieces.Contains(i)) continue;
            for (var j = i + 1; j < size; ++j)
            {
                if (pieces.Contains(j) || numbers[i, index] == 0 || numbers[j, index] == 0) continue;
                var targetA = -1;
                var targetB = -1;
                for (var n = 0; n < size; ++n)
                {
                    if (_target.Numbers[n, index] == numbers[i, index]) targetA = n;
                    else if (_target.Numbers[n, index] == numbers[j, index]) targetB = n;
                }

                if (targetA >= 0 && targetB >= 0 && (i < j && targetA > targetB || i > j && targetA < targetB))
                {
                    count += 2;
                    pieces.Add(i);
                    pieces.Add(j);
                    break;
                }
            }
        }
        return count;
    }
    
    private int CountColumnConflicts(int size, int[,] numbers, int index)
    {
        var count = 0;
        var pieces = new List<int>();
        for (var i = 0; i < size - 1; ++i)
        {
            if (pieces.Contains(i)) continue;
            for (var j = i + 1; j < size; ++j)
            {
                if (pieces.Contains(j) || numbers[index, i] == 0 || numbers[index, j] == 0) continue;
                var targetA = -1;
                var targetB = -1;
                for (var n = 0; n < size; ++n)
                {
                    if (_target.Numbers[index, n] == numbers[index, i]) targetA = n;
                    else if (_target.Numbers[index, n] == numbers[index, j]) targetB = n;
                }

                if (targetA >= 0 && targetB >= 0 && (i < j && targetA > targetB || i > j && targetA < targetB))
                {
                    count += 2;
                    pieces.Add(i);
                    pieces.Add(j);
                    break;
                }
            }
        }
        return count;
    }
}