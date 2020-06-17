using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//This class handles the generation of dead ends and connecting paths
public class Maze 
{
    //array of the size of the maze, true if a node is unvisited O(1) access time
    bool [,] unvisited;

    //list of paths (dead end paths & connecting paths)
    List<List<int[]>> paths;

    //list for the current path being made
    List<int[]> link = new List<int[]>();

    //step size for a path
    int step = 1;

    //number of possible moves (left/down/up/right)
    int length = 4;

    //used for placing points in the maze (not implemented in this version, in the final game)
    int[,] points;

    //shape of the maze (square,circle,triangle,ring)
    string shape;

    //initializes the relevant variables
    public void initialise(List<int[]> route, int[] size, string type) 
    {
        shape = type;
        //This section of code initializes the visited/unvisited cells 
        unvisited = new bool[size[0]+1, size[1]+1];
        for(int i = 0; i < size[0]+1; i++)
            for(int c = 0; c < size[1]+1; c++)
            {
                unvisited[i, c] = true;
            }

        foreach (var coordinate in route)
        {
            unvisited[coordinate[0], coordinate[1]] = false;
        }

        paths = new List<List<int[]>>();
        paths.Add(route); 
    }

    //this function plots dead ends, and then connecting paths
    public void plotDeadEnds(float deadEndDifficulty, int[] size, float connectivity)
    {
        //calculations for the number of dead ends, the number of connecting paths, and the max dead end length
        //for a given deadend difficulty and connectivity value and maze size
        int numberOf = (int)Mathf.Round(((size[0] + size[1]) / 4f) * deadEndDifficulty);
        int numberOfConnecting = (int)Mathf.Round(((size[0] + size[1]) / 4f) * connectivity);
        int maxLength = (int)Mathf.Round(((size[0] + size[1]) / 2f) * deadEndDifficulty);
        int[] start = new int[2];
        bool walk = false;
        int z = 0;

        //create dead ends
        List<int[]> deadEnds = new List<int[]>();
        //create up the the total number of dead ends (& connecting paths at the same time)
        for (int i = 0; i < numberOf; i++)
        {
            bool found = false;
            int rand1=0, rand2=0;
            walk = false;

            //if there is an unvisited node in the maze, create dead end
            for (int x = 0; x < size[0] + 1; x++)
                for (int c = 0; c < size[1] + 1; c++)
                {
                    if (unvisited[x, c] == true)
                        walk = true;
                }
            if (walk == true)
            {
                //find a random visited node with a neighbouring unvisited node
                while (found == false)
                {
                    rand1 = Random.Range(0, unvisited.GetLength(0));
                    rand2 = Random.Range(0, unvisited.GetLength(1));
                    if (unvisited[rand1, rand2] == false)
                    {
                        start[0] = rand1;
                        start[1] = rand2;
                        if (neighbouring(start, size) == true && !paths[0].Contains(start))
                            found = true;
                    }
                }
                //plot the dead end, given this visited node and add it to the list of paths
                start[0] = rand1;
                start[1] = rand2;
                int x = deadEnd(start, size, maxLength);
                deadEnds.Add(paths.Last().Last());
            }

            //create connecting paths
            if(z < numberOfConnecting)
            {
                walk = false;
                int p = 0;
                int k;

                //if there is an unvisited node, you can create a connecting path
                for (k = 0; k < size[0] + 1; k++)
                {
                    p = 0;
                    for (p = 0; p < size[1] + 1; p++)
                    {
                        if (unvisited[k, p] == true)
                        {
                            if (shape == "triangle")
                            {
                                int[] coord = new int[2];
                                coord[0] = k;
                                coord[1] = p;
                                if (inTriangle(coord, size) == true)
                                {
                                    walk = true;
                                }
                            }
                            else
                            {
                                walk = true;
                            }
                        }
                    }
                }

                //if you can create a connecting path, do it
                if (walk == true)
                {
                    bool find = false;
                    rand1 = 0; rand2 = 0;
                    //find a starting node for the connecting path
                    while (find == false)
                    {
                        rand1 = Random.Range(0, unvisited.GetLength(0));
                        rand2 = Random.Range(0, unvisited.GetLength(1));
                        if (unvisited[rand1, rand2] == false)
                        {
                            start[0] = rand1;
                            start[1] = rand2;
                            if (neighbouring(start, size) == true && !paths[0].Contains(start))
                            {
                                find = true;
                            }
                        }

                    }
                    //create the connecting path
                    link = new List<int[]>();
                    bool x = randomWalk(start, size, deadEnds, true, -1);
                    
                    //add the connecting path to the list of paths, being careful
                    //to add the path and not a reference to the path
                    if (x == true)
                    {
                        if (shape == "triangle")
                        {
                            int[] prev = new int[2];
                            int f = 0;
                            int dif = 0;
                            bool ad = true;
                            foreach (var coord in link)
                            {
                                if (coord[0] == 10 && coord[1] == 19)
                                {
                                    Debug.Log("TING");
                                    foreach (var d in link)
                                        Debug.Log(d[0] + " : " + d[1]);
                                }
                                    if (f > 0)
                                {
                                    dif = coord[0] - prev[0];
                                    dif += coord[1] - prev[1];
                                    if (dif != 1 && dif != -1)
                                        ad = false;
                                }
                                prev = coord;
                                f++;
                            }
                            if (ad == true)
                            {
                                paths.Add(new List<int[]>());
                                foreach (var d in link) {
                                    int []pos = new int[2];
                                    pos[0] = d[0];
                                    pos[1] = d[1];
                                    paths[paths.Count - 1].Add(pos);

                                }
                            }
                        }
                        else
                        {
                            paths.Add(new List<int[]>());
                            foreach (var d in link)
                            {
                                int[] pos = new int[2];
                                pos[0] = d[0];
                                pos[1] = d[1];
                                paths[paths.Count - 1].Add(pos);

                            }
                        }
                    }
                    z++;
                }
            }
        }

    }

    //checks if the current coordinate is within a triangle
    private bool inTriangle(int[] current, int[] size)
    {
        if (current[0] > (size[0] - (size[0] / 2f * (current[1] / (float)size[1]))) || current[0] < ((size[0] / 2f * (current[1] / (float)size[1]))))
            return false;
        return true;
    }


    //creates a single dead end, given the starting position, size of the maze, and maxLength
    public int deadEnd(int[] start, int[] size, int maxLength)
    {
        bool done = false;
        int[] current = new int[2];
        current[0] = start[0];
        current[1] = start[1];
        bool[] moves;
        length = 4;
        List<int[]> path = new List<int[]>();
        int choice, move = 0, currentLength = 0;
        int x = 1, firstMove = -1;

        while (done == false)
        {
            moves = new bool[] { true, true, true, true };
            length = 4;
            //mark current square as visited
            unvisited[current[0], current[1]] = false;

            moves = possibleMoves(moves, current, size, shape);
            
            //if no possible moves
            if (length == 0)
            {
                done = true;
            }
            else
            {
                //pick a random move
                choice = Random.Range(0, length);
                int tempcount = 0;
                //find it
                for (int i = 0; i < 4; i++)
                {
                    if (moves[i] == true)
                    {
                        if (tempcount == choice)
                        {
                            move = i;
                        }
                        tempcount++;
                    }
                }
                //add current square to path
                int[] temp = new int[2];
                temp[0] = current[0];
                temp[1] = current[1];
                path.Add(temp);

                //move one step
                if (move == 0)
                    current[0] = current[0] + step;
                if (move == 1)
                    current[0] = current[0] - step;
                if (move == 2)
                    current[1] = current[1] + step;
                if (move == 3)
                    current[1] = current[1] - step;
                
                currentLength++;

                if (currentLength == maxLength)
                {
                        done = true;
                }


            }
            x++;

        }
        if (currentLength > 0)
        {
            int[] temp = new int[2];
            temp[0] = current[0];
            temp[1] = current[1];

            path.Add(temp);
            paths.Add(path);
        }

        return currentLength;
    }

    //checks if there is a neighbouring unvisited node to the given coordinate
    public bool neighbouring(int []current, int[] size)
    {
        bool neighbour = false;
        int[] temp = new int[2];
        temp[0] = current[0];
        temp[1] = current[1];
        temp[0] += 1;
        if (temp[0] <= size[0])
            if (unvisited[temp[0], temp[1]] == true)
            {
                neighbour = true;
            }
        temp[0] -= 2;

        if (temp[0] >= 0)
            if (unvisited[temp[0], temp[1]] == true)
            {
                neighbour = true;
            }
        temp[0] += 1;
        temp[1] += 1;

        if (temp[1] <= size[1])
            if (unvisited[temp[0], temp[1]] == true)
            {
                neighbour = true;
            }
        temp[1] -= 2;

        if (temp[1] >= 0)
            if( unvisited[temp[0], temp[1]] == true)
            {
                neighbour = true;
            }
        return neighbour;
    }

    //walks from one visited non-dead end node to another, creating a connecting path
    public bool randomWalk(int[] current, int[] size, List<int[]> deadEnds, bool first, int ban)
    {
        bool done = false;
        int move;
        //while not done
        while (done == false) {
            move = 0;
            //if done
            if (first != true && unvisited[current[0], current[1]] == false){
                done = true;
            }
            //else
            else
            {
                unvisited[current[0], current[1]] = false;
                bool[] moves = new bool[] { true, true, true, true };
                int[] temp = new int[2];
                temp[0] = current[0];
                temp[1] = current[1];

                //find potential moves
                moves = possibleRandomMoves(temp, deadEnds, size, moves, ban);

                int length = 0;
                for(int i = 0; i < 4; i++)
                {
                    if (moves[i] == true)
                        length++;
                }
                //if there are not any possible moves
                if (length == 0)
                {
                    deadEnds.Add(current);
                    break;
                }
                //else, move
                else
                {
                    int choice = Random.Range(0, length);
                    int tempcount = -1;
                    //pick random move out of possible moves
                    for (int i = 0; i < 4; i++)
                    {
                        if (moves[i] == true)
                            tempcount++;
                        if (tempcount == choice && moves[i] == true)
                        {
                            move = i;
                        }
                    }
                    //move
                    temp[0] = current[0];
                    temp[1] = current[1];
                    if (move == 0)
                    {
                        temp[0]++;
                        done = randomWalk(temp, size, deadEnds, false, move);
                    }

                    if (move == 1)
                    {
                        temp[0]--;
                        done = randomWalk(temp, size, deadEnds, false, move);
                    }

                    if (move == 2)
                    {
                        temp[1]++;
                        done = randomWalk(temp, size, deadEnds, false, move);
                    }

                    if (move == 3)
                    {
                        temp[1]--;
                        done = randomWalk(temp, size, deadEnds, false, move);
                    }



                }

            }

        }
        //add current node to the current connecting path
        if (done == true)
            link.Add(current);
        return done;
                    
    }

    //calcualtes possible dead end moves for a given maze shape (square, circle, triangle, ring)
    private bool[] possibleMoves(bool[] moves, int[] current, int[] size, string type)
    {
        if (type == "square")
        {
            //check all possible moves 
            if (current[0] + step > size[0] || unvisited[current[0] + 1, current[1]] == false)
            {
                moves[0] = false;
                length -= 1;
            }
            if (current[0] - step < 0 || unvisited[current[0] - step, current[1]] == false)
            {
                moves[1] = false;
                length -= 1;
            }
            if (current[1] + step > size[1] || unvisited[current[0], current[1] + step] == false)
            {
                moves[2] = false;
                length -= 1;
            }
            if (current[1] - step < 0 || unvisited[current[0], current[1] - step] == false)
            {
                moves[3] = false;
                length -= 1;
            }
        }

        if(type == "circle" || type == "donut")
        {
            int[] temp = new int[2];
            temp[0] = current[0];
            temp[1] = current[1];
            temp[0] -= size[0] / 2;
            temp[1] -= size[0] / 2;
            if (Mathf.Sqrt((temp[0] + step)* (temp[0] + step)+ temp[1] * temp[1]) >= size[0]/2 || unvisited[current[0] + 1, current[1]] == false)
            {
                moves[0] = false;
                length -= 1;
            }
            if (Mathf.Sqrt((temp[0] - step) * (temp[0] - step) + temp[1] * temp[1]) >= size[0]/2 || unvisited[current[0] - step, current[1]] == false)
            {
                moves[1] = false;
                length -= 1;
            }
            if (Mathf.Sqrt((temp[1] + step) * (temp[1] + step) + temp[0] * temp[0]) >= size[0]/2 || unvisited[current[0], current[1] + step] == false)
            {
                moves[2] = false;
                length -= 1;
            }
            if (Mathf.Sqrt((temp[1] - step) * (temp[1] - step) + temp[0] * temp[0]) >= size[0]/2 || unvisited[current[0], current[1] - step] == false)
            {
                moves[3] = false;
                length -= 1;
            }
            //check inner circle
            if (type == "donut")
            {
                temp = new int[2];
                temp[0] = current[0];
                temp[1] = current[1];
                temp[0] -= size[0] / 2;
                temp[1] -= size[0] / 2;
                if (Mathf.Sqrt((temp[0] + step) * (temp[0] + step) + temp[1] * temp[1]) <= size[1] / 2)
                {
                    if (moves[0] == true)
                    {
                        moves[0] = false;
                        length -= 1;
                    }
                }
                if (Mathf.Sqrt((temp[0] - step) * (temp[0] - step) + temp[1] * temp[1]) <= size[1] / 2)
                {
                    if (moves[1] == true)
                    {
                        moves[1] = false;
                        length -= 1;
                    }
                }
                if (Mathf.Sqrt((temp[1] + step) * (temp[1] + step) + temp[0] * temp[0]) <= size[1] / 2)
                {
                    if (moves[2] == true)
                    {
                        moves[2] = false;
                        length -= 1;
                    }
                }
                if (Mathf.Sqrt((temp[1] - step) * (temp[1] - step) + temp[0] * temp[0]) <= size[1] / 2)
                {
                    if (moves[3] == true)
                    {
                        moves[3] = false;
                        length -= 1;
                    }
                }

            }
        }

        if(type == "triangle")
        {
            //check all possible moves 
            if (current[0] + step > (size[0] - (size[0]/2f *(current[1]/(float)size[1]))) || unvisited[current[0] + 1, current[1]] == false)
            {
                moves[0] = false;
                length -= 1;
            }
            if (current[0] - step < (size[0] / 2f * (current[1] / (float)size[1])) || unvisited[current[0] - step, current[1]] == false)
            {
                moves[1] = false;
                length -= 1;
            }
            if (current[1] + step > size[1] || unvisited[current[0], current[1] + step] == false || current[0] > size[0] - (size[0] / 2f * ((current[1]+step) / (float)size[1])) || current[0] < (size[0] / 2f * ((current[1]+step) / (float)size[1])))
            {
                moves[2] = false;
                length -= 1;
            }
            if (current[1] - step < 0 || unvisited[current[0], current[1] - step] == false || current[0] > size[0] - (size[0] / 2f * ((current[1] - step) / (float)size[1])) || current[0] < (size[0] / 2f * ((current[1] - step) / (float)size[1])))
            {
                moves[3] = false;
                length -= 1;
            }
        }
        

        return moves;
    }

    //calcualtes possible connecting moves for a given maze shape (square, circle, triangle, ring), I should really condense all of these functions into one neater one
    private bool[] possibleRandomMoves(int[] temp, List<int[]> deadEnds, int[] size, bool[] moves, int ban)
    {
        if (shape == "square")
        {
            temp[0] += 1;

            if (temp[0] > size[0] || deadEnds.Contains(temp) || paths[0].Contains(temp))
                moves[0] = false;
            temp[0] -= 2;
            if (temp[0] < 0 || deadEnds.Contains(temp) || paths[0].Contains(temp))
                moves[1] = false;
            temp[0] += 1;
            temp[1] += 1;
            if (temp[1] > size[1] || deadEnds.Contains(temp) || paths[0].Contains(temp))
                moves[2] = false;
            temp[1] -= 2;
            if (temp[1] < 0 || deadEnds.Contains(temp) || paths[0].Contains(temp))
                moves[3] = false;
            if (ban != -1)
                moves[ban] = false;
        }

        if(shape == "circle" || shape == "donut")
        {
            temp[0] += 1;
            //take away radius to shift circle to origin
            temp[0] -= size[0] / 2;
            temp[1] -= size[0] / 2;

            if (Mathf.Sqrt(temp[0]*temp[0]+temp[1]*temp[1]) >= size[0]/2 || deadEnds.Contains(temp) || paths[0].Contains(temp))
                moves[0] = false;
            temp[0] -= 2;
            if (Mathf.Sqrt(temp[0] * temp[0] + temp[1] * temp[1]) >= size[0]/2 || deadEnds.Contains(temp) || paths[0].Contains(temp))
                moves[1] = false;
            temp[0] += 1;
            temp[1] += 1;
            if (Mathf.Sqrt(temp[0] * temp[0] + temp[1] * temp[1]) >= size[0]/2 || deadEnds.Contains(temp) || paths[0].Contains(temp))
                moves[2] = false;
            temp[1] -= 2;
            if (Mathf.Sqrt(temp[0] * temp[0] + temp[1] * temp[1]) >= size[0]/2 || deadEnds.Contains(temp) || paths[0].Contains(temp))
                moves[3] = false;
            if (ban != -1)
                moves[ban] = false;
            if(shape == "donut")
            {
                temp[1] += 1;
                temp[0] += 1;
                if (Mathf.Sqrt(temp[0] * temp[0] + temp[1] * temp[1]) <= size[1] / 2 )
                    moves[0] = false;
                temp[0] -= 2;
                if (Mathf.Sqrt(temp[0] * temp[0] + temp[1] * temp[1]) <= size[1] / 2 )
                    moves[1] = false;
                temp[0] += 1;
                temp[1] += 1;
                if (Mathf.Sqrt(temp[0] * temp[0] + temp[1] * temp[1]) <= size[1] / 2 )
                    moves[2] = false;
                temp[1] -= 2;
                if (Mathf.Sqrt(temp[0] * temp[0] + temp[1] * temp[1]) <= size[1] / 2 )
                    moves[3] = false;
            }
        }
        if(shape == "triangle")
        {
            temp[0] += 1;

            if (temp[0] > (size[0] - ((size[0]/2f)*(temp[1]/(float)size[1]))) || deadEnds.Contains(temp) || paths[0].Contains(temp))
                moves[0] = false;
            temp[0] -= 2;
            if (temp[0] < ((size[0] / 2f) * (temp[1] / (float)size[1])) || deadEnds.Contains(temp) || paths[0].Contains(temp))
                moves[1] = false;
            temp[0] += 1;
            temp[1] += 1;
            if (temp[1] > size[1] || deadEnds.Contains(temp) || paths[0].Contains(temp) || temp[0] > size[0] - ((size[0] / 2f) * (temp[1] / (float)size[1])) || temp[0] < ((size[0] / 2f) * (temp[1] / (float)size[1])))
                moves[2] = false;
            temp[1] -= 2;
            if (temp[1] < 0 || deadEnds.Contains(temp) || paths[0].Contains(temp) || temp[0] > size[0] - ((size[0] / 2f) * (temp[1] / (float)size[1])) || temp[0] < ((size[0] / 2f) * (temp[1] / (float)size[1])))
                moves[3] = false;
            if (ban != -1)
                moves[ban] = false;
        }
        return moves;
    }

    //getter
    public List<List<int[]>> getPaths()
    {
        return paths;
    }

    //getter for points in the full game
    public int[,] getPoints()
    {
        return points;
    }

    //sets the number of points for a maze game (only called in the full game)
    public int findMaxPoints(int sizex)
    {
        if (shape == "square"|| shape == "circle"){
            if (sizex < 10)
                return 1;
            if (sizex < 15)
                return 3;
            if (sizex < 20)
                return 6;
            if (sizex < 30)
                return 15;
            if (sizex < 61)
                return 30;
        }
        else
        {
            if (sizex < 10)
                return 1;
            if (sizex < 15)
                return 2;
            if (sizex < 20)
                return 3;
            if (sizex < 30)
                return 7;
            if (sizex < 61)
                return 25;
        }
        return 0;
    }

}
