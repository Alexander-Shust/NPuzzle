﻿public class State
{
    public int Size { get; }
    public int[,] Numbers { get; }
    public int ZeroX { get; }
    public int ZeroY { get; }
    public int Depth { get; }
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
                String += Numbers[i, j] + ",";
            }
        }
    }
}