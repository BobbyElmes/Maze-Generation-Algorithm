using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//class used to plot the maze walls
public class Walls : MonoBehaviour
{
	public float[,,] wallSet;
	public GameObject myPrefab;
	public float[,,] allRGB = new float[10,2, 3];
	int currentMaze = 9;
	private SpriteRenderer rend;
	private Sprite green, black;
	private Color wallColour;
	private float[] RGB = new float[3];
	private string shape;
	public List<GameObject> wallObjects = new List<GameObject>();

	// Use this for initialization
	void Start () {
		rend = myPrefab.GetComponent<SpriteRenderer>();
		green = Resources.Load<Sprite>("green");
		black = Resources.Load<Sprite>("wall");
		rend.sprite = black;
		hardCodedMazeColours();
		currentMaze = Random.Range(1, 9);
	}

	//initialize some wall values
	public void walls(float[,,] x, bool y){
		wallSet = new float [x.Length/4,2,2];
		for(int i = 0; i < x.Length/4; i++)
        {
			wallSet[i, 0, 0] = x[i, 0, 0];
			wallSet[i, 0, 1] = x[i, 0, 1];
			wallSet[i, 1, 0] = x[i, 1, 0];
			wallSet[i, 1, 1] = x[i, 1, 1];
		}
		if (y == true)
		{
			rend.sprite = green;
			myPrefab.GetComponent<BoxCollider2D>().isTrigger = true;
		}
		else
		{
			rend.sprite = black;
			myPrefab.GetComponent<BoxCollider2D>().isTrigger = false; 
		}
	}

	//places each wall in the correct position in the maze
	public void placeWalls(int[] size, string type){
		float distance, difx, dify;
		float angle;
		shape = type;
		for(int i = 0; i < wallSet.GetLength(0); i++){
		//distance between coordinates
			difx = wallSet[i, 0, 0] - wallSet[i, 1, 0];
			dify = wallSet[i, 0, 1] - wallSet[i, 1, 1];
			//length of wall
			distance = Mathf.Sqrt (Mathf.Pow (difx, 2) + Mathf.Pow (dify, 2));
			//angle of rotation
			if (difx != 0)
			{
				if (difx < 0)
					angle = 0f;
				else
					angle = 180f;
			}
			else
			{
				if (dify < 0)
					angle = 90f;
				else
					angle = 270f;
			}

			//calculate colour of wall, given shape
			if (rend.sprite == black)
			{ 
				if (shape != "donut")
				{
					interpolate(size, wallSet[i, 0, 0], wallSet[i, 0, 1]);
				}
				else
				{
					int[] tempSize = new int[2];
					tempSize[0] = size[0];
					tempSize[1] = size[0];
					interpolate(tempSize, wallSet[i, 0, 0], wallSet[i, 0, 1]);
				}
				wallColour = new Color(RGB[0], RGB[1], RGB[2], 1f);
				rend.color = wallColour;
				
			}
			float[] coord = new float[2] { wallSet[i, 0, 0], wallSet[i, 0, 1] };

			//used to check if wall is in certain shapes 
			bool place = true;
			if (shape == "triangle")
				place = isTriangle(size, coord, angle);
			if (shape == "circle")
				place = isCircle(size[0] / 2, coord, angle);
			if (shape == "donut")
			{
				bool place1 = isDonut(size, coord, angle);
				place = isCircle(size[0] / 2, coord, angle);
				if (place1 == true)
					place = false;
			}
			//place wall by insantiating a new wall prefab
			if (place == true)
			{
				GameObject newObject;
				newObject = Instantiate(myPrefab, new Vector3(wallSet[i, 0, 0], wallSet[i, 0, 1], 0), Quaternion.Euler(0, 0, angle));
				
				wallObjects.Add(newObject);
				newObject.GetComponent<SpriteRenderer>().sortingOrder = wallObjects.Count;
				newObject.transform.localScale = new Vector3(0.061f, 0.0175f, 1f);
			}
		}
	}


	//interpolates corner to corner between 2 colours
	public void interpolate(int[] size, float x, float y)
    {
		
		RGB[0] = allRGB[currentMaze, 0, 0] * (1 - (((x / size[0]) + (y / size[1])) / 2f)) + (allRGB[currentMaze, 1, 0]) * ((((x / size[0]) + (y / size[1])) / 2f));
		RGB[1] = allRGB[currentMaze, 0, 1] * (1 - (((x / size[0]) + (y / size[1])) / 2f)) + (allRGB[currentMaze, 1, 1]) * ((((x / size[0]) + (y / size[1])) / 2f));
		RGB[2] = allRGB[currentMaze, 0, 2] * (1 - (((x / size[0]) + (y / size[1])) / 2f)) + (allRGB[currentMaze, 1, 2]) * ((((x / size[0]) + (y / size[1])) / 2f));
		
	}


	//checks if wall is in the circle radius
	public bool isCircle(int radius, float[] coord, float angle)
    {
		float[] temp = new float[2];
		float valueX = 0.5f;
		float valueY = 0.5f;
		temp[0] = coord[0];
		temp[1] = coord[1];
		if (temp[0] < radius && temp[1] > radius)
		{
			if ((int)angle == 90)
			{
				temp[0] += valueX;
				temp[1] += valueY;
			}
			if ((int)angle == 0 )
			{
				temp[0] += valueX;
				temp[1] -= valueY;
			}
		}
		if(temp[0] < radius && temp[1] < radius)
        {
			if ((int)angle == 90)
			{
				temp[0] += valueX;
				temp[1] += valueY;
			}
			if ((int)angle == 0)
			{
				temp[0] += valueX;
				temp[1] += valueY;
			}
		}
		if (temp[0] > radius && temp[1] < radius)
        {
			if ((int)angle == 90)
			{
				temp[0] -= valueX;
				temp[1] += valueY;
			}
			if ((int)angle == 0)
			{
				temp[0] += valueX;
				temp[1] += valueY;
			}
		}
		if (temp[0] > radius && temp[1] > radius)
		{
			if ((int)angle == 90)
			{
				temp[0] -= valueX;
				temp[1] += valueY;
			}
			if ((int)angle == 0)
			{
				temp[0] += valueX;
				temp[1] -= valueY;
			}
		}
		temp[0] -= radius;
		temp[1] -= radius;
		if (temp[0] == radius)
			return false;
		if ((temp[1]) == radius)
			return false;
		if (temp[0] == -radius)
			return false;
		if (temp[1] == -radius)
			return false;
		if (Mathf.Sqrt(temp[0] * temp[0] + temp[1] * temp[1]) > radius)
			return false;
		return true;
    }

	//checks if wall is in an outer circle radius but not in an inner circle radius
	public bool isDonut(int[] size, float[] coord, float angle)
	{
		float[] temp = new float[2];
		float valueX = 0.5f;
		float valueY = 0.5f;
		int radius;
		radius = size[0] / 2;
		temp[0] = coord[0];
		temp[1] = coord[1];

		if (temp[0] < radius && temp[1] > radius)
		{
			if ((int)angle == 90)
			{
				temp[0] -= valueX;
				temp[1] += valueY;
			}
			if ((int)angle == 0)
			{
				temp[0] += valueX;
				temp[1] += valueY;
			}
		}
		if (temp[0] < radius && temp[1] < radius)
		{
			if ((int)angle == 90)
			{
				temp[0] -= valueX;
				temp[1] += valueY;
			}
			if ((int)angle == 0)
			{
				temp[0] += valueX;
				temp[1] -= valueY;
			}
		}
		if (temp[0] > radius && temp[1] < radius)
		{
			if ((int)angle == 90)
			{
				temp[0] += valueX;
				temp[1] += valueY;
			}
			if ((int)angle == 0)
			{
				temp[0] += valueX;
				temp[1] -= valueY;
			}
		}
		if (temp[0] > radius && temp[1] > radius)
		{
			if ((int)angle == 90)
			{
				temp[0] += valueX;
				temp[1] += valueY;
			}
			if ((int)angle == 0)
			{
				temp[0] += valueX;
				temp[1] += valueY;
			}
		}
		temp[0] -= radius;
		temp[1] -= radius;
		
		if (Mathf.Sqrt(temp[0] * temp[0] + temp[1] * temp[1]) >= size[1] / 2)
			return false;
		return true;
	}


	//checks if wall is inside the triangle
	public bool isTriangle(int[] size, float[] coord, float angle)
    {
		float middle = size[0] / 2f;
		if ((int)angle == 90)
		{
				if (coord[0] + 0.5f < (middle * ((coord[1] + 0.5f) / (float)size[1])) || coord[0] - 0.5f > size[0] - middle * (((coord[1] + 0.5f) / (float)(size[1]))))
				{
					return false;
				}
			
		}
		if((int)angle == 0)
        {
				if (coord[0] + 0.5f < (middle * ((coord[1] - 0.5f) / (float)size[1])) || coord[0] + 0.5f > size[0] - middle * (((coord[1] - 0.5f) / (float)(size[1]))))
				{
					return false;
				}
		}
		return true;
    }


	//different colour schemes for the walls
	void hardCodedMazeColours()
    {
		int i = 0;
		//colour 1
		allRGB[i, 0, 0] = 1f;
		allRGB[i, 0, 1] = 0f;
		allRGB[i, 0, 2] = 0.216f;

		allRGB[i, 1, 0] = 1f;
		allRGB[i, 1, 1] = 0f;
		allRGB[i, 1, 2] = 0.216f;
		

		i++;

		//colour 2
		allRGB[i, 0, 0] = 0f;
		allRGB[i, 0, 1] = 0.455f;
		allRGB[i, 0, 2] = 1f;

		allRGB[i, 1, 0] = 1f;
		allRGB[i, 1, 1] = 0.012f;
		allRGB[i, 1, 2] = 0.325f;

		i++;

		//colour 3
		allRGB[i, 0, 0] = 0f;
		allRGB[i, 0, 1] = 1f;
		allRGB[i, 0, 2] = 0.416f;

		allRGB[i, 1, 0] = 1f;
		allRGB[i, 1, 1] = 0.718f;
		allRGB[i, 1, 2] = 0f;

		i++;

		//colour 4
		allRGB[i, 0, 0] = 1f;
		allRGB[i, 0, 1] = 1f;
		allRGB[i, 0, 2] = 0f;

		allRGB[i, 1, 0] = 1f;
		allRGB[i, 1, 1] = 0f;
		allRGB[i, 1, 2] = 0f;

		i++;

		//colour 5
		allRGB[i, 0, 0] = 0.533f;
		allRGB[i, 0, 1] = 0f;
		allRGB[i, 0, 2] = 1f;

		allRGB[i, 1, 0] = 0f;
		allRGB[i, 1, 1] = 1f;
		allRGB[i, 1, 2] = 1f;

		i++;

		//colour 6
		allRGB[i, 0, 0] = 1f;
		allRGB[i, 0, 1] = 1f;
		allRGB[i, 0, 2] = 1f;

		allRGB[i, 1, 0] = 1f;
		allRGB[i, 1, 1] = 1f;
		allRGB[i, 1, 2] = 1f;

		i++;

		//colour 7
		allRGB[i, 0, 0] = 1f;
		allRGB[i, 0, 1] = 0.4f;
		allRGB[i, 0, 2] = 0f;

		allRGB[i, 1, 0] = 0.666f;
		allRGB[i, 1, 1] = 0f;
		allRGB[i, 1, 2] = 1f;

		i++;

		//colour 8
		allRGB[i, 0, 0] = 0f;
		allRGB[i, 0, 1] = 0.651f;
		allRGB[i, 0, 2] = 1f;

		allRGB[i, 1, 0] = 0f;
		allRGB[i, 1, 1] = 1f;
		allRGB[i, 1, 2] = 0.4f;

		i++;

		//colour 9
		allRGB[i, 0, 0] = 1f;
		allRGB[i, 0, 1] = 1f;
		allRGB[i, 0, 2] = 0f;

		allRGB[i, 1, 0] = 1f;
		allRGB[i, 1, 1] = 1f;
		allRGB[i, 1, 2] = 0f;

		i++;

		//colour 10
		allRGB[i, 0, 0] = 1f;
		allRGB[i, 0, 1] = 0.353f;
		allRGB[i, 0, 2] = 0.431f;

		allRGB[i, 1, 0] = 1f;
		allRGB[i, 1, 1] = 0.353f;
		allRGB[i, 1, 2] = 0.431f;

		i++;
	}
}
