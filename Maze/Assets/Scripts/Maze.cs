using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
	[System.Serializable]
	public class Cell
	{
		public bool visited;
		public GameObject north; //1
		public GameObject east; //2 
		public GameObject west; //3 
		public GameObject south; //4
	}

	public GameObject wall;
	public float wallLength = 1.0f;
	public int xSize = 5;
	public int ySize = 5;
	private Vector3 initialPosition;
	private GameObject wallHolder;
	private Cell[] cells;
	private int currentCell = 0;
	private int totalCells;
	private int visitedCells = 0;
	private bool startedBuilding = false;
	private int currentNeighbour = 0;
	private List<int> lastCells;
	private int backingUp = 0;
	private int wallToBreak = 0;

	// Use this for initialization
	void Start () {
		CreateWalls ();
	}

	void CreateWalls()
	{
		wallHolder = new GameObject ();
		wallHolder.name = "Maze";

		initialPosition = new Vector3 ((-xSize / 2) + wallLength / 2, 0.0f, (-ySize / 2) + wallLength / 2);
		Vector3 myPos = initialPosition;
		GameObject tempWall;

		for (int i = 0; i < ySize ; i++) {
			for(int j = 0 ; j <= xSize ; j++){			
				myPos = new Vector3(initialPosition.x + (j*wallLength)-wallLength/2,0.0f,initialPosition.y + (i*wallLength)-wallLength/2);
				tempWall = Instantiate(wall,myPos,Quaternion.identity) as GameObject;
				tempWall.transform.parent = wallHolder.transform;
			}
		}

		for (int i = 0; i <= ySize ; i++) {
			for(int j = 0 ; j < xSize ; j++){			
				myPos = new Vector3(initialPosition.x + (j*wallLength),0.0f,initialPosition.y + (i*wallLength)-wallLength);
				tempWall = Instantiate(wall,myPos,Quaternion.Euler(0.0f,90.0f,0.0f)) as GameObject;
				tempWall.transform.parent = wallHolder.transform;
			}
		}
		CreateCells ();
	}

	void CreateCells()
	{
		lastCells = new List<int> ();
		lastCells.Clear ();
		totalCells = xSize * ySize;
		int children = wallHolder.transform.childCount;
		GameObject[] allWalls = new GameObject[children];
		cells = new Cell[xSize * ySize];
		int EWProcess = 0;
		int childProcess = 0;
		int termCount = 0;

		// Get All Children
		for (int i = 0; i < children; i++) {
			allWalls[i] = wallHolder.transform.GetChild(i).gameObject;	
		}

		//Assigns walls to cells
		for(int cellProcess = 0 ; cellProcess < cells.Length ; cellProcess++){

			
			if(termCount == xSize){
				EWProcess ++;
				termCount = 0;
			}

			cells[cellProcess] = new Cell();
			cells[cellProcess].east = allWalls[EWProcess];
			cells[cellProcess].south = allWalls[childProcess+(xSize+1)*(ySize)];

			EWProcess++;

			termCount++;
			childProcess++;
			cells[cellProcess].west = allWalls[EWProcess];
			cells[cellProcess].north = allWalls[(childProcess+(xSize+1)*(ySize))+xSize-1];

		}
		CreateMaze ();
	}

	void CreateMaze(){
		while (visitedCells < totalCells) {
			if(startedBuilding){
				GiveMeNeighbour();
				if(cells[currentNeighbour].visited == false && cells[currentCell].visited == true)
				{
					BreakWall();
					cells[currentNeighbour].visited = true;
					visitedCells++;
					lastCells.Add(currentCell);
					currentCell = currentNeighbour;
					if(lastCells.Count > 0)
					{
						backingUp = lastCells.Count - 1;
					}
				}
			}
			else {
				currentCell = Random.Range(0,totalCells);
				cells[currentCell].visited = true;
				visitedCells++;
				startedBuilding = true;
			}
		}
	}

	void BreakWall()
	{
		switch (wallToBreak) {
		case 1:
			Destroy(cells[currentCell].north);
			break;
		case 2:
			Destroy(cells[currentCell].east);
			break;
		case 3:
			Destroy(cells[currentCell].west);
			break;
		case 4:
			Destroy(cells[currentCell].south);
			break;
		}
	}

	void GiveMeNeighbour(){
		int length = 0;
		int[] neighbours = new int[4];
		int[] connectingWall = new int[4];
		int check = 0;
		check = ((currentCell + 1)/xSize);
		check -= 1;
		check *= xSize;
		check += xSize;

		//west
		if((currentCell+1 < totalCells) && (currentCell+1) != check){
			if(cells[currentCell + 1].visited == false){
				neighbours[length] = currentCell + 1;
				connectingWall[length] = 3;
				length++;
			}
		}

		//east
		if((currentCell-1 >= 0) && currentCell != check){
			if(cells[currentCell - 1].visited == false){
				neighbours[length] = currentCell - 1;
				connectingWall[length] = 2;
				length++;
			}
		}

		//North
		if(currentCell+xSize < totalCells){
			if(cells[currentCell + xSize].visited == false){
				neighbours[length] = currentCell + xSize;
				connectingWall[length] = 1;
				length++;
			}
		}

		//south
		if(currentCell - xSize >= 0){
			if(cells[currentCell - xSize].visited == false){
				neighbours[length] = currentCell - xSize;
				connectingWall[length] = 4;
				length++;
			}
		}

		if (length != 0) {
			int theChosenOne = Random.Range(0,length);
			currentNeighbour = neighbours[theChosenOne];
			wallToBreak = connectingWall[theChosenOne];
		}
		else{
			if(backingUp > 0)
			{
				currentCell = lastCells[backingUp];
				backingUp--;
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
