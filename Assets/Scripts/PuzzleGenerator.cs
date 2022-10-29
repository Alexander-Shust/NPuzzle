using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class PuzzleGenerator
{
    public static int[,] Generate(int size, PuzzleType type = PuzzleType.Soviet, bool solvable = true)
    {
        var start = type switch
        {
            PuzzleType.Snail => GenerateSnailPosition(size),
            PuzzleType.Soviet => GenerateSovietPosition(size),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var zeroX = type == PuzzleType.Soviet ? size - 1 : size / 2;
        var zeroY = type == PuzzleType.Soviet ? size - 1 : size / 2 + size % 2 - 1;

        var possibleMoves = new List<Move>();
        for (var i = 0; i < 2001; ++i)
        {
            possibleMoves.Clear();
            if (zeroX > 0) possibleMoves.Add(Move.Up);
            if (zeroX < size - 1) possibleMoves.Add(Move.Down);
            if (zeroY > 0) possibleMoves.Add(Move.Left);
            if (zeroY < size - 1) possibleMoves.Add(Move.Right);

            var move = possibleMoves[Random.Range(0, possibleMoves.Count)];
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
            (start[zeroX, zeroY], start[targetX, targetY]) = (start[targetX, targetY], start[zeroX, zeroY]);
            zeroX = targetX;
            zeroY = targetY;
        }

        if (!solvable)
        {
            if (start[0, 0] == 0 || start[0, 1] == 0)
            {
                (start[size - 1, size - 2], start[size - 1, size - 1]) = (start[size - 1, size - 1], start[size - 1, size - 2]);
            }
            else
            {
                (start[0, 0], start[0, 1]) = (start[0, 1], start[0, 0]);
            }
        }
        
        return start;
    }

    public static int[,] GenerateSovietPosition(int size)
    {
        var puzzle = new int[size, size];
        for (var i = 0; i < size * size; ++i)
        {
            puzzle[i / size, i % size] = i + 1;
        }

        puzzle[size - 1, size - 1] = 0;
        return puzzle;
    }

    public static int[,] GenerateSnailPosition(int size)
    {
        var puzzle = new int[size, size];
        var i = 1;
        for (var round = 0; round < size / 2; ++round)
        {
            if (round == size / 2 - 1 && size % 2 == 0)
            {
                puzzle[round, round] = i++;
                puzzle[round, size - round - 1] = i++;
                puzzle[size - round - 1, size - round - 1] = i;
                break;
            }

            for (var j = round; j < size - round - 1; ++j)
            {
                puzzle[round, j] = i++;
            }

            for (var j = round; j < size - round - 1; ++j)
            {
                puzzle[j, size - round - 1] = i++;
            }

            for (var j = size - round - 1; j > round; --j)
            {
                puzzle[size - round - 1, j] = i++;
            }

            for (var j = size - round - 1; j > round; --j)
            {
                puzzle[j, round] = i++;
            }
        }

        return puzzle;
    }
}

public enum PuzzleType
{
    Snail,
    Soviet
}