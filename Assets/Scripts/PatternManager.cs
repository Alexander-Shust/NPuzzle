using System.IO;

public static class PatternManager
{
    private static readonly string PatternFile = Settings.PuzzleDirectory + "\\patterndb.bin";
    private static byte[] _patternDb;

    private struct TilePattern
    {
        public int[] Tiles;
        public int TileCount;
        public long Offset;
    }

    private static readonly TilePattern FirstPattern = new TilePattern
    {
        Tiles = new [] {1, 5, 6, 9, 10, 13},
        TileCount = 6,
        Offset = 0
    };
    
    private static readonly TilePattern SecondPattern = new TilePattern
    {
        Tiles = new [] {7, 8, 11, 12, 14, 15},
        TileCount = 6,
        Offset = 16777216
    };
    
    private static readonly TilePattern ThirdPattern = new TilePattern
    {
        Tiles = new [] {2, 3, 4},
        TileCount = 3,
        Offset = 33554432
    };

    private static readonly TilePattern[] Patterns =
    {
        FirstPattern,
        SecondPattern,
        ThirdPattern
    };

    public static void LoadHeuristics()
    {
        _patternDb = File.ReadAllBytes(PatternFile);
    }

    public static int GetHeuristic(int[] board)
    {
        var heuristic = 0;
        for (var i = 0; i < 3; i++)
        {
            var index = GetPatternIndex(board, Patterns[i]);
            heuristic += _patternDb[index + Patterns[i].Offset];
        }
        return heuristic;
    }

    private static int GetPatternIndex(int[] board, TilePattern pattern)
    {
        var index = 0;
        var k = 1;
        for (var i = 0; i < pattern.TileCount; i++)
        {
            for (var j = 0; j < 16; j++)
            {
                if (pattern.Tiles[i] == board[j])
                {
                    index += j * k;
                    k *= 16;
                    break;
                }
            }
        }
        return index;
    }
}