using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//This class handles the generation of the maze path from a starting position to a goal 
public class Path 
{
    //"visited" keeps track of visited nodes
    List<int[]> visited = new List<int[]>();

    //"path" is the list of nodes on the path
    List<int[]> path = new List<int[]>();

    //"step" is the path length
    int step = 1;

    //"done" keeps track of whether the generation is complete
    bool done = false;

    //"plotPath" is a recursive function which creates a path from start to end using a difficulty 
    //between 0 and 1. start is the start point goal is the goal and size is the x*y of the maze
    public void plotPath(float difficulty, int [] current, int[] goal, int[] size, string type)
    {
        //loops until the maze is done
        while (done == false)
        {
            int difx, dify, direction = -1;
            bool[] canGo = new bool[] { true, true, true, true };
            int[] newCurrent = new int [2];
            newCurrent[0] = current[0];
            newCurrent[1] = current[1];

            //add current node to path
            visited.Add(current);

            difx = goal[0] - current[0];
            dify = goal[1] - current[1];

            //if at the goal, terminate
            if (difx == 0)
            {
                if (dify == 0)
                {
                    done = true;
                }
            }

            //else
            if (done != true)
            {
                //figure out where it is possible to move (left right up down)
                canGo = moveContains(type, current, canGo, size);
                bool deadEnd = true;
                for (int i = 0; i < canGo.Length; i++)
                {
                    if (canGo[i] == true)
                        deadEnd = false;
                }

                //if there isn't a possible move then return out of the current function call in the recursive loop
                if (deadEnd == true)
                {
                    return;
                }
                difx = goal[0] - current[0];
                dify = goal[1] - current[1];

                //pick a random move out of the possible moves 
                if (difx == 0)
                {
                    if (dify == 0)
                    {
                        done = true;
                    }
                    else
                    {
                        if (dify > 0)
                            direction = move(2, canGo, difficulty);
                        else
                            direction = move(3, canGo, difficulty);
                    }
                }
                else
                {
                    if (dify == 0)
                    {
                        if (difx > 0)
                            direction = move(0, canGo, difficulty);
                        else
                            direction = move(1, canGo, difficulty);
                    }
                    else
                    {
                        if (difx > 0 && dify > 0)
                            direction = move2(0, 2, canGo, difficulty);
                        if (difx > 0 && dify < 0)
                            direction = move2(0, 3, canGo, difficulty);
                        if (difx < 0 && dify > 0)
                            direction = move2(1, 2, canGo, difficulty);
                        if (difx < 0 && dify < 0)
                            direction = move2(1, 3, canGo, difficulty);
                    }
                }

                //move, then call "plotPath" with the new position
                if (direction != -1)
                {
                    if (direction == 0)
                        newCurrent[0] += step;
                    if (direction == 1)
                        newCurrent[0] -= step;
                    if (direction == 2)
                        newCurrent[1] += step;
                    if (direction == 3)
                        newCurrent[1] -= step;
                    plotPath(difficulty, newCurrent, goal, size, type);
                }
            }
            
        }

        //add the current position to the path
        if (type == "circle" || type == "donut" && done == true)
        {
            current[0] += size[0] / 2;
            current[1] += size[0] / 2;
            path.Add(new int[2]);
            int []temp1 = new int[2];
            temp1[0] = current[0];
            temp1[1] = current[1];
            path[path.Count-1] = temp1;
        }
        else
        {
            path.Add(current);
        }

    }

    //figures out which moves are possible, for example moving outside the maze isn't possible
    //as the main path can't go out of a maze (this is for SQUARE mazes)
    private bool[] possibleMoves(bool[] canGo, int[] current, int[] size)
    {
        int[] nextOne = new int[2];
        nextOne[0] = current[0];
        nextOne[1] = current[1];
        nextOne[0] = nextOne[0] + step;
        if (current[0] + step > size[0] || visited.Any(arr => arr.SequenceEqual(nextOne)))
            canGo[0] = false;
        nextOne[0] = current[0];
        nextOne[1] = current[1];
        nextOne[0] -= step;
        if (current[0] - step < 0 || visited.Any(arr => arr.SequenceEqual(nextOne)))
            canGo[1] = false;
        nextOne[0] = current[0];
        nextOne[1] = current[1];
        nextOne[1] += step;
        if (current[1] + step > size[1] || visited.Any(arr => arr.SequenceEqual(nextOne)))
            canGo[2] = false;
        nextOne[0] = current[0];
        nextOne[1] = current[1];
        nextOne[1] -= step;
        if (current[1] - step < 0 || visited.Any(arr => arr.SequenceEqual(nextOne)))
            canGo[3] = false;
        return canGo;
    }

    //called to decide which random move to take, given the probability difficulty
    //for when there is only one direction which takes the coordinate closer to the goal
    private int move(int x, bool[] canGo, float difficulty)
    {
        float rand = Random.Range(0.0f, 1.0f);
        float check = 0f;
        List<int> other = new List<int>();
        for(int i = 0; i < 4; i++)
        {
            if (i != x)
            {
                if (canGo[i] == true)
                    other.Add(i);
            }
        }
        float size = other.Count;


        if (canGo[x] == true)
        {
            check += 1-difficulty;
            if (rand <= check)
                return x;
            else
            {
                for (int i = 0; i < size; i++)
                {
                    check += (difficulty) * (1 / size);
                    if (rand <= check)
                        return other[i];
                }
            }
        }
        else
        {
            for(int i = 0; i < size; i++)
            {
                check += 1 / size;
                if (rand <= check)
                    return other[i];
            }

        }
        return -1;

    }

    //same as "move" but when there are two directions which take the 
    //path towards the goal (e.g. up and left)
    private int move2(int x, int y, bool[] canGo, float difficulty)
    {
        float rand = Random.Range(0.0f, 1.0f);
        float check = 0f;
        List<int> other = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if (i != x && i != y)
            {
                if(canGo[i] == true)
                    other.Add(i);
            }
        }
        float size = other.Count;

        if (canGo[x] == true || canGo[y] == true)
        {
            check += (1 - difficulty);
            if (rand <= check)
            {
                if (canGo[x] == true && canGo[y] == false)
                    return x;
                if (canGo[y] == true && canGo[x] == false)
                    return y;
                if(canGo[x] == true && canGo[y] == true)
                {
                    if (rand < check / 2)
                        return x;
                    else
                        return y;
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    check += (difficulty) * (1 / size);
                    if (rand <= check)
                    {
                        return (other[i]); ;
                        
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < size; i++)
            {
                check += 1 / size;
                if (rand <= check)
                    return other[i];
            }

        }
        return -1;

    }

    //getter
    public List<int[]> getPath()
    {
        return path;
    }

    //checks which moves are possible, redirecting to the correct function, given the shape of the maze
    // (i.e. a circle uses a different calculation to see if a coordinate is inside/outside of it than a square
    private bool[] moveContains(string type,int[] current, bool[] cango, int[] dimensions)
    {
        if(type == "square")
        {
            return possibleMoves(cango,current, dimensions);
        }
        if (type == "circle")
            return circleMoves(cango, current, dimensions[0]/2);
        //check both circles then cancel out to create donut
        if(type == "donut")
        {
            bool[] outer = new bool[4];
            bool[] both = new bool[4];
            bool[] inner = new bool[4];
            outer = circleMoves(cango, current, dimensions[0] / 2);
            for (int i = 0; i < 4; i++)
                both[i] = outer[i];
                inner = circleMoves(cango, current, dimensions[1] / 2);
            for (int i = 0; i < 4; i++)
            {
                if (inner[i] == true)
                {
                    both[i] = false;
                    
                }
            }
            return both;
        }
        if(type == "triangle")
        {
            return triangleMoves(cango, current, dimensions);
        }
            
        return cango;

    }

    //check which new coordinates are in circle
    private bool[] circleMoves(bool[] canGo,int[] current, int radius)
    {
        int[] nextOne = new int[2];
        nextOne[0] = current[0];
        nextOne[1] = current[1];
        nextOne[0] = nextOne[0] + step;
        if (Mathf.Sqrt(nextOne[0] * nextOne[0] + nextOne[1] * nextOne[1]) >= radius || visited.Any(arr => arr.SequenceEqual(nextOne)))
            canGo[0] = false;
        nextOne[0] -= step*2;
        if (Mathf.Sqrt(nextOne[0] * nextOne[0] + nextOne[1] * nextOne[1]) >= radius || visited.Any(arr => arr.SequenceEqual(nextOne)))
            canGo[1] = false;
        nextOne[0] += step;
        nextOne[1] += step;
        if (Mathf.Sqrt(nextOne[0] * nextOne[0] + nextOne[1] * nextOne[1]) >= radius || visited.Any(arr => arr.SequenceEqual(nextOne)))
            canGo[2] = false;
        nextOne[1] -= step * 2;
        if (Mathf.Sqrt(nextOne[0] * nextOne[0] + nextOne[1] * nextOne[1]) >= radius || visited.Any(arr => arr.SequenceEqual(nextOne)))
            canGo[3] = false;
        return canGo;
    }

    //check which new coordinates are in triangle
    private bool[] triangleMoves(bool[] canGo, int[] current, int[] size)
    {
        int[] nextOne = new int[2];
        nextOne[0] = current[0];
        nextOne[1] = current[1];
        nextOne[0] = nextOne[0] + step;
        //calculate if inside traingle
        if (nextOne[0] < (size[0]/2f)*(nextOne[1]/ (float)size[1]) || visited.Any(arr => arr.SequenceEqual(nextOne)) || nextOne[0] > size[0]- ((size[0] / 2f) * (nextOne[1] / (float)size[1])))
            canGo[0] = false;
        nextOne[0] = current[0];
        nextOne[1] = current[1];
        nextOne[0] -= step;
        if (nextOne[0] < (size[0] / 2f) * (nextOne[1] / (float)size[1]) || visited.Any(arr => arr.SequenceEqual(nextOne)) || nextOne[0] > size[0] - ((size[0] / 2f) * (nextOne[1] / (float)size[1])))
            canGo[1] = false;
        nextOne[0] = current[0];
        nextOne[1] = current[1];
        nextOne[1] += step;
        if (nextOne[0] < (size[0] / 2f) * (nextOne[1] / (float)size[1]) || nextOne[0] > size[0] - ((size[0] / 2f) * (nextOne[1] / (float)size[1])) || nextOne[1] > size[1])
        {
            canGo[2] = false;
        }
        else
        {
            if(visited.Any(arr => arr.SequenceEqual(nextOne)))
                canGo[2] = false;
        }
        nextOne[0] = current[0];
        nextOne[1] = current[1];
        nextOne[1] -= step;
        if (nextOne[0] < (size[0] / 2f) * (nextOne[1] / (float)size[1]) || visited.Any(arr => arr.SequenceEqual(nextOne)) || nextOne[0] > size[0] - ((size[0] / 2f) * (nextOne[1] / (float)size[1])) || nextOne[1] < 0f)
            canGo[3] = false;
        return canGo;
    }


}
