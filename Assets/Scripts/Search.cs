﻿using System.Collections.Generic;
using DataStructures.PriorityQueue;
using UnityEngine;

public class Search
{
    public int VisitedCount { get; set; }

    private State _target;
    private PriorityQueue<State, int> _queue;
    private HashSet<string> _hash;

    public Search(State start, State target)
    {
        _queue = new PriorityQueue<State, int>(0);
        _target = target;
        _hash = new HashSet<string>();
        _queue.Insert(start, 3);
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
                Debug.LogError(_hash.Count);
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
            var newState = new State(size, numbers, x - 1, y, depth + 1);
            AddState(newState);
        }

        if (x < size - 1)
        {
            var numbers = (int[,]) state.Numbers.Clone();
            (numbers[x + 1, y], numbers[x, y]) = (numbers[x, y], numbers[x + 1, y]);
            var newState = new State(size, numbers, x + 1, y, depth + 1);
            AddState(newState);
        }

        if (y > 0)
        {
            var numbers = (int[,]) state.Numbers.Clone();
            (numbers[x, y - 1], numbers[x, y]) = (numbers[x, y], numbers[x, y - 1]);
            var newState = new State(size, numbers, x, y - 1, depth + 1);
            AddState(newState);
        }

        if (y < size - 1)
        {
            var numbers = (int[,]) state.Numbers.Clone();
            (numbers[x, y + 1], numbers[x, y]) = (numbers[x, y], numbers[x, y + 1]);
            var newState = new State(size, numbers, x, y + 1, depth + 1);
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
        return ErrorCount(state);
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
}