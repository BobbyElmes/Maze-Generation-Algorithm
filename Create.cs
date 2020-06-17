using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//This class controls the whole maze generation and wall generation process
public class Create : MonoBehaviour
{
    Walls render;
    Path a;
    Maze maze;
    public GameObject finish;
    bool[,] xWalls;
    bool[,] yWalls;
    public GameObject player;
    public GameObject goalPos;
    public AudioClip tunes;
    public AudioSource source;
    public int[] goal, current;
    public string type = "square";
    static public int[] size = { 30, 30 };
    private bool eris;

    //creates a maze of the given shape, size and difficulty
    public void create(string shape)
    {
        type = shape;
        a = new Path();
        int difficulty;
        int[] order = new int [2];
        int[] difValues = new int[2];
        if (type == "square")
        {
            difValues = squareSize();
            size[0] = difValues[0];
            size[1] = difValues[0];
            order = randomStart();
            
            
            
        }
        else
        {
            if(type == "triangle")
            {
                difValues = triSize();
                size[0] = difValues[0];
                size[1] = difValues[0];
                order = randomTriStart();
               
            }
            else
            {
                if(type == "circle")
                {
                    difValues = circSize();
                    size[0] = difValues[0];
                    size[1] = difValues[0];
                    order = randomCircStart();
                    
                }
                if(type == "donut")
                {
                    difValues = donutSize();
                    size[0] = difValues[0];
                    size[1] = difValues[2];
                    order = randomCircStart();
                    
                }
            }
        }

        current = new int[] { order[0], order[1] };
        goal = new int[] { order[2], order[3] };
        difficulty = difValues[1];

        source.time = Data.audioTime;
        if (source.time < 416)
            eris = true;
        else
        {
            goalPos.GetComponent<Goal>().changeTune("tune:\n joyhauser - landa");
            eris = false;
        }
        source.Play();


        //calculate difficulty values
        float pathDifficulty = 0.2f + 0.003f*difficulty;
        float deadEnds = 0.2f*difficulty;
        float connectivity = 12.05f - 0.12f*difficulty;

        a.plotPath(pathDifficulty, current, goal, size, type);
        
        
        List<int[]> path = a.getPath();

        //plot main path
        while (path.Count == 0)
        {
            a = new Path();
            a.plotPath(pathDifficulty, current, goal, size, type);
            path = a.getPath();
            Debug.Log("TRY AGAIN");
        }


        if (type == "circle" || type == "donut")
        {
            for (int i = 0; i < 2; i++)
            {
                goal[i] += size[0] / 2;
            }
        }

        //plot rest of maze
        maze = new Maze();
        if (type == "donut")
        {
            int[] temp = new int[2];
            temp[0] = size[0];
            temp[1] = size[0];
            maze.initialise(path, temp, type);

        }
        else
        {
            maze.initialise(path, size, type);
        }
        maze.plotDeadEnds(deadEnds,size,connectivity);
        List<List<int[]>> paths = maze.getPaths();
            render = gameObject.GetComponent<Walls>();

        if(type == "circle")
            size[1] = size[0];

        

        //set player position
        player.GetComponent<player>().loadPos(current);

        //set goal position
        finish.transform.position = new Vector3(goal[0], goal[1], 0);

        
        

        //plot the maze walls
        plotWalls(size, paths);
    }

    //initializes an array where every wall in the grid of the maze is filled, then 
    //traverses the maze paths to remove walls to create the full maze data struture, ready for
    //the "Walls" class to utilize
    void plotWalls(int[] sizeT, List<List<int[]>> paths)
    {
        int[] tempSize = new int[2];
        tempSize[0] = sizeT[0] + 1;
        tempSize[1] = sizeT[1] + 1;
        if(type == "donut")
            tempSize[1] = sizeT[0] + 1;
        yWalls = new bool[tempSize[0], tempSize[1] + 1];
        xWalls = new bool[tempSize[0] + 1, tempSize[1]];

        for (int i = 0; i < tempSize[0]; i++)
            for (int c = 0; c < tempSize[1]+1; c++)
                yWalls[i, c] = true;
        for (int i = 0; i < tempSize[0] + 1; i++)
            for (int c = 0; c < tempSize[1]; c++)
                xWalls[i, c] = true;

        int xdif, ydif;
        render = gameObject.GetComponent<Walls>();
        foreach (var route in paths)
        {
            int i = 0;
            int[] previous = new int[2];
            foreach (var coordinate in route)
            {
                xdif = 0;
                ydif = 0;
                
                if (i == 0)
                {
                    previous[0] = coordinate[0];
                    previous[1] = coordinate[1];
                }
                else
                {
                    xdif = coordinate[0] - previous[0];
                    ydif = coordinate[1] - previous[1];
                    if (xdif != 0)
                    {
                        if (xdif > 0)
                            xWalls[previous[0] + 1, previous[1]] = false;
                        else
                            xWalls[previous[0], previous[1]] = false;
                    }
                    else
                    {
                        if (ydif > 0)
                            yWalls[previous[0], previous[1]+1] = false;
                        else
                            yWalls[previous[0], previous[1]] = false;
                    }
                    previous[0] = coordinate[0];
                    previous[1] = coordinate[1];
                }
                i++;

            }
        }


        int count = 0;
        for (int i = 0; i < tempSize[0]; i++)
        {
            for (int c = 0; c < tempSize[1] + 1; c++)
            {
                if (yWalls[i, c] == true)
                    count++;
            }
        }
        float [,,]walls = new float[count, 2, 2];

        count = 0;
        for (int i = 0; i < tempSize[0]; i++)
        {
            for (int c = 0; c < tempSize[1] + 1; c++)
            {
                if (yWalls[i, c] == true)
                {

                    walls[count, 0, 0] = i - 0.5f;
                    walls[count, 0, 1] = c - 0.5f;
                    walls[count, 1, 0] = i + 0.5f;
                    walls[count, 1, 1] = c - 0.5f;
                    count++;
                }
            }
        }
        render.walls(walls, false);
        render.placeWalls(size, type);


        count = 0;
        for (int i = 0; i < tempSize[0] + 1; i++)
        {
            for (int c = 0; c < tempSize[1]; c++)
            {
                if (xWalls[i, c] == true)
                    count++;
            }
        }
        walls = new float[count, 2, 2];

        count = 0;
        for (int i = 0; i < tempSize[0] + 1; i++)
        {
            for (int c = 0; c < tempSize[1]; c++)
            {
                if (xWalls[i, c] == true)
                {

                    walls[count, 0, 0] = i - 0.5f;
                    walls[count, 0, 1] = c - 0.5f;
                    walls[count, 1, 0] = i - 0.5f;
                    walls[count, 1, 1] = c + 0.5f;
                    count++;
                }
            }
        }
        render.walls(walls, false);
        render.placeWalls(size, type) ;
        
    }

    //figures out a random starting and goal position (in opposite corners) in a square maze
    private int[] randomStart()
    {
        int[] order = new int[4] { 0, 0, size[0], size[1] };
        int rand = UnityEngine.Random.Range(1, 5);
        if (rand == 1 || rand == 3)
        {
            order[0] = order[2];
            order[2] = 0;
        }
        if(rand == 2 || rand == 3)
        {
            order[1] = order[3];
            order[3] = 0;
        }
        return order;
    }

    //figures out a random starting and goal position (from either of 2 corners to the top) in a triangle maze
    private int[] randomTriStart()
    {
        int[] order = new int[4] { 0, 0, size[0]/2, size[1] };
        int rand = UnityEngine.Random.Range(1, 3);
        if (rand == 1)
        {
            order[0] = size[0];
        }
        return order;
    }

    //figures out a random starting and goal position (at opposite ends) in a circle or ring shaped maze
    private int[] randomCircStart()
    {
        int[] order = new int[4] { 0, 0, size[0] / 2, size[1] };
        int rand = UnityEngine.Random.Range(1, 5);
        if (rand == 1)
        {
            order[0] = (size[0]/2)-1;
            order[1] = 0;
            order[2] = -((size[0] / 2)-1);
            order[3] = 0;
        }
        if (rand == 2)
        {
            order[0] = 0;
            order[1] = (size[0] / 2)-1 ;
            order[2] = 0;
            order[3] = -((size[0] / 2)-1 ) ;
        }
        if (rand == 3)
        {
            order[0] = -((size[0] / 2) -1);
            order[1] = 0;
            order[2] = (size[0] / 2)-1 ;
            order[3] = 0;
        }
        if (rand == 4)
        {
            order[0] = 0;
            order[1] = -((size[0] / 2) -1 ) ;
            order[2] = 0;
            order[3] = (size[0] / 2)-1;
        }
        
        return order;
    }

    public string getType()
    {
        return type;
    }

    public int[] getSize()
    {
        return size;
    }

    //hard coded size and difficulty value for given difficulty string value
    //you can use these or you can make your own difficulty values
    public int[] squareSize()
    {
        int[] difValues = new int[2];
        if (Data.difficulty == "hard")
        {
            difValues[0] = 25;
            difValues[1] = 90;
        }
        else
        {
            if (Data.difficulty == "medium")
            {
                difValues[0] = 15;
                difValues[1] = 75;
            }
            else
            {
                if (Data.difficulty == "easy")
                {
                    difValues[0] = 10;
                    difValues[1] = 60;
                }
            }

        }

        return difValues;
    }

    //hard coded triangle difficulty/size values
    public int[] triSize()
    {
        int[] difValues = new int[2];
        if (Data.difficulty == "hard")
        { 
            difValues[0] = 36;
            difValues[1] = 90;
        }
        else
        {
            if (Data.difficulty == "medium")
            {
                difValues[0] = 20;
                difValues[1] = 75;
            }
            else
            {
                if(Data.difficulty == "easy")
                {
                    difValues[0] = 10;
                    difValues[1] = 60;
                }
            }
        }

        return difValues;
    }

    //hard coded circle difficulty/size values
    public int[] circSize()
    {
        int[] difValues = new int[2];
        if (Data.difficulty == "hard")
        {
            difValues[0] = 25;
            difValues[1] = 90;
        }
        else
        {
            if (Data.difficulty == "medium")
            {
                difValues[0] = 15;
                difValues[1] = 75;
            }
            else
            {
                if (Data.difficulty == "easy")
                {
                    difValues[0] = 10;
                    difValues[1] = 60;
                }
            }
        }

        return difValues;
    }

    //hard coded ring difficulty/size values
    public int[] donutSize()
    {
        int[] difValues = new int[3];

        if (Data.difficulty == "hard")
        {
            difValues[0] = 30;
            difValues[1] = 90;
            difValues[2] = 15;
        }
        else
        {  
            if (Data.difficulty == "medium")
            {
                difValues[0] = 20;
                difValues[1] = 60;
                difValues[2] = 10;
            }
            else
            {
                if (Data.difficulty == "easy")
                {
                    difValues[0] = 15;
                    difValues[1] = 90;
                    difValues[2] = 7;
                }
            }
        }

        return difValues;
    }

    //says which song is playing
    void Update()
    {
        if (source.time > 758f)
            source.time = 29.76f;
        if (source.time > 416f && eris == true) {
            goalPos.GetComponent<Goal>().changeTune("tune: joyhauser\n - landa");
            eris = false;
        }
        if(source.time < 416 && eris == false)
        {
            eris = true;
            goalPos.GetComponent<Goal>().changeTune("tune: ilija\n djokovic - eris");
        }
    }

    public void saveTime()
    {
        Data.audioTime = source.time;
    }

    public void pauseSong()
    {
        source.Pause();
    }

    public void playSong()
    {
        source.Play();
    }

}
