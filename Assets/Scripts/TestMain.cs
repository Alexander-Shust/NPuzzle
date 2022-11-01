using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TestMain : MonoBehaviour
{
    private void Start()
    {
        var start2X2 = new[,]
        {
            {2, 3},
            {0, 1}
        };

        var bad3X3 = new[,] 
        {   
            {8,6,7},
            {2,5,4},
            {3,0,1}
        };

        var easy3X3 = new[,]
        {
            {4, 1, 3},
            {2, 0, 6},
            {7, 5, 8}
        };

        var start4X4 = new[,] 
        {     
            {5,10,14,7},
            {8,3,6,1},
            {15,0,12,9},
            {2,11,4,13}
        };
        
        var easy4X4 = new[,] 
        {     
            {12,1,3,4},
            {2,13,14,5},
            {11,10,8,6},
            {9,15,7,0}
        };
        
        var target2X2 = new[,]
        {
            {1, 2},
            {3, 0}
        };

        var target3X3 = new[,] 
        {    
            {1,2,3},
            {4,5,6},
            {7,8,0}
        };

        var target4X4 = new[,] 
        {    
            {1,2,3,4},
            {5,6,7,8},
            {9,10,11,12},
            {13,14,15,0}
        };

        var start2 = new State(2, start2X2, 1, 0, 0);
        var start3 = new State(3, bad3X3, 2, 1, 0);
        var easy3 = new State(3, easy3X3, 1, 1, 0);
        var easy4 = new State(4, easy4X4, 0, 3, 0);
        var start4 = new State(4, start4X4, 2, 1, 0);
        var target2 = new State(2, target2X2, 1, 1, 0);
        var target3 = new State(3, target3X3, 2, 2, 0);
        var target4 = new State(4, target4X4, 3, 3, 0);

        var watch = new Stopwatch();
        var search = new Search(easy4, target4);
                            

        watch.Start();
        var node = search.Astar();
        watch.Stop();
        
        Debug.LogError($"States visited {search.VisitedCount}");
        Debug.LogError($"Elapsed {watch.ElapsedMilliseconds} milliseconds");
        Debug.LogError($"Node at depth {node.Depth}");
    }
}