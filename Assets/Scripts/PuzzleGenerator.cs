using System;

public static class PuzzleGenerator
{
    public static int[,] Generate(int size, PuzzleType type = PuzzleType.Snail, bool solvable = true)
    {
        var startPosition = type switch
        {
            PuzzleType.Snail => GenerateSnailPosition(size),
            PuzzleType.Soviet => GenerateSovietPosition(size),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        if (!solvable)
        {
            if (startPosition[0, 0] == 0 || startPosition[0, 1] == 0)
            {
                
            }
            else
            {
                
            }
        }
        
        return startPosition;
    }

    private static int[,] GenerateSovietPosition(int size)
    {
        var puzzle = new int[size, size];
        for (var i = 0; i < size * size; ++i)
        {
            puzzle[i / size, i % size] = i + 1;
        }

        puzzle[size - 1, size - 1] = 0;
        return puzzle;
    }

    private static int[,] GenerateSnailPosition(int size)
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