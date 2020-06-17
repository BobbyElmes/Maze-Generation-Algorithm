using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//test script to demonstrate how the maze is created 
public class test : MonoBehaviour
{
    Walls render;
    Path a;
    // Start is called before the first frame update
    void Start()
    {
        float[,,] walls;
        a = new Path();
        int[] current = { -5, 0 };
        int[] goal = { 5, 0 };
        int[] size = { 10, 10 };
        float difficulty = 0.4f;
        string type = "circle";
        a.plotPath(difficulty, current, goal, size, type);
        List<int[]> path = a.getPath();
        
        int x = path.Count;
        Debug.Log("X: " + x);
        walls = new float[x, 2, 2];
        int i = 0;
        foreach (var coordinate in path)
        {
            Debug.Log(coordinate[0] + " , " + coordinate[1]);
            if (i < x - 1)
            {
                walls[i, 0, 0] = coordinate[0];
                walls[i, 0, 1] = coordinate[1];
                
            }
            if (i != 0)
            {
                walls[i - 1, 1, 0] = coordinate[0];
                walls[i - 1, 1, 1] = coordinate[1];
            }
            i++;
        }
        render = gameObject.GetComponent<Walls>();
        render.walls(walls, false);
        render.placeWalls(size, type);
       

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
