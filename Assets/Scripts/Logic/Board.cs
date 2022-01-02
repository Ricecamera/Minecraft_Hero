using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Board : MonoBehaviour
{
    [Serializable]
    public class Grid {
        public Vector2 Position { get; private set;}
        public bool isVacant;

        //Assignment constructor.
        public Grid (Vector2 position) {
            this.Position = position;
            isVacant = true;
        }
    }


    [SerializeField]
    private int boxSize = 5, spaceSize = 2;

    public int columns;             //Number of columns in our game board.
    public int rows;                //Number of rows in our game board.

    //A list of possible locations to place objects.
    public List<Grid> gridPositions = new List<Grid>();

    void Start()
    {
        // Get boundary of play area
        float xBound = Mathf.Floor(CameraManager.instance.Bound.x);
        float zBound = Mathf.Floor(CameraManager.instance.Bound.z);

        // Calculate width and height of play area
        float screenWidth = xBound * 2;
        float screenHeight = zBound * 2;

        columns = (int) (screenWidth) / (boxSize + spaceSize);
        rows = (int) (screenHeight) / (boxSize + spaceSize);

        // Add screen size by spaceSize to remove excess spacing from multiply by columns and rows
        float padX = (screenWidth - (boxSize + spaceSize) * columns + spaceSize)/2;
        float padZ = (screenHeight - (boxSize + spaceSize) * rows + spaceSize)/2;

        float startingX = -xBound + padX + boxSize / 2;
        float startingZ = -zBound + padZ + boxSize / 2;
        InitialiseList(startingX, startingZ);
    }

    private void InitialiseList(float startingX, float startingZ) {
        //Clear our list gridPositions.
        gridPositions.Clear();
        
        for (int x = 0; x < columns; x++) {
            for (int z = 0; z < rows; z++) {
                float xPos = startingX + x * (boxSize + spaceSize);
                float zPos = startingZ + z * (boxSize + spaceSize);

                Vector2 pos = new Vector2(xPos, zPos);
                Grid grid = new Grid(pos);
                gridPositions.Add(grid);
            }
        }
    }
}
