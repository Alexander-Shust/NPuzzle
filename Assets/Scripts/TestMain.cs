using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TestMain : MonoBehaviour
{
    private void Start()
    {
        var bad3X3 = new[,] 
        {   
            {8,6,7},
            {2,5,4},
            {3,0,1}
        };

        var easy3X3 = new[,]
        {
            {1, 0, 3},
            {4, 2, 6},
            {7, 5, 8}
        };

        var initConfig4x4 = new[,] 
        {     
            {5,10,14,7},
            {8,3,6,1},
            {15,0,12,9},
            {2,11,4,13}
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

        var start = new State(3, bad3X3, 2, 1, 0);
        var easy = new State(3, easy3X3, 0, 1, 0);
        var target = new State(3, target3X3, 2, 2, 0);    

        var watch = new Stopwatch();
        var search = new Search(easy, target);
                            

        watch.Start();
        var node = search.Astar();
        watch.Stop();
        
        Debug.LogError($"States visited {search.VisitedCount}");
        Debug.LogError($"Elapsed {watch.ElapsedMilliseconds} milliseconds");
        Debug.LogError($"Node at depth {node.Depth}");
    }
}