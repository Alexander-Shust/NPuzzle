using System;

public class State : IComparable<State>
{
    public int Size { get; }
    public int[,] Numbers { get; }
    public int ZeroX { get; }
    public int ZeroY { get; }
    public int Distance { get; set; }
    public int Depth { get; set; }
    public string String { get; }

    public State(int size, int[,] numbers, int zeroX, int zeroY, int depth)
    {
        Size = size;
        Numbers = (int[,]) numbers.Clone();
        ZeroX = zeroX;
        ZeroY = zeroY;
        String = string.Empty;
        Depth = depth;
        for (var i = 0; i < Size; ++i)
        {
            for (var j = 0; j < Size; ++j)
            {
                String += Numbers[i, j] + "-";
            }
        }

        // String = String.TrimEnd('-');
    }

    public int CompareTo(State other)
    {
        if (Distance > other.Distance) return 1;
        if (Distance < other.Distance) return -1;
        return 0;
    }
}