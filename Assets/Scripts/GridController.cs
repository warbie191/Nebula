using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;





public class GridController : MonoBehaviour
{

    static public GridController grid; //singleton

    public float gridSpacing = 51;
    public int gridWidth = 4;
    public int gridHeight = 4;

    public GridCell cellPrefab;
    GridCell[,] cells;
    
    private void Start(){
        GridController.grid = this;
        BuildGrid(gridWidth, gridHeight);
    }

    
    private void Update(){
        
    }


    private void BuildGrid(int w, int h){
        cells = new GridCell[w, h];
        for (int x = 0; x < w; x++){   
            for (int y = 0; y < h; y++){

                GridCell cell = Instantiate(cellPrefab, transform);

                cell.xIndex = x;
                cell.yIndex = y;
                cell.PositionCell();
                cell.Init();

                cells[x, y] = cell;
                //cell.cellType = Random.Range(0, 10);


            }//end y loop
        }//end x loop

    }//BuildGrid


    public void TrySwap(GridCell cell, Direction dir)
    {
        //print("swap" + dir);

        GridCell cell2 = FindNeighborCell(cell, dir);

        if (cell2 == null) return; //no neighbor in that direction do nothing


        cells[cell.xIndex, cell.yIndex] = cell2;
        cells[cell2.xIndex, cell2.yIndex] = cell;

        int tempX = cell.xIndex;
        int tempY = cell.yIndex;

        cell.xIndex = cell2.xIndex;
        cell.yIndex = cell2.yIndex;

        cell2.xIndex = tempX;
        cell2.yIndex = tempY;

        cell.PositionCell();
        cell2.PositionCell();

       

        

        if(CheckForMatches()){
           Popmatches();
        }
        else{

            //unswap but no chance to animate
            cell2.xIndex = cell.xIndex;
            cell2.yIndex = cell.yIndex;
            cell.xIndex = tempX;
            cell.yIndex = tempY;
            cells[cell.xIndex, cell.yIndex] = cell;
            cells[cell2.xIndex, cell2.yIndex] = cell2;

            cell.PositionCell();
            cell2.PositionCell();

            
        }
        //TODO check for matches, unswap
    }

    private void Popmatches()
    {
        for (int x = 0; x < cells.GetLength(0); x++) //one column at a time
        {
            int amountMatchThisColumn = 0;

            for (int y = 0; y < cells.GetLength(1); y++)
            {
                if (cells[x, y].isMatched)
                {
                    amountMatchThisColumn ++;

                    int newYIndex = cells.GetLength(1) - amountMatchThisColumn;
                    cells[x, y].yIndex = newYIndex;
                    cells[x, newYIndex] = cells[x, y]; //move cell
                    cells[x, newYIndex].PositionCell();
                    cells[x, newYIndex].Respawn();
                }
                else
                {
                    if(amountMatchThisColumn > 0) { 
                    //move cell down
                    int newYIndex = cells[x, y].yIndex - amountMatchThisColumn;

                    cells[x, y].yIndex = newYIndex;
                    cells[x, newYIndex] = cells[x, y]; //move cell
                    cells[x, newYIndex].PositionCell();
                    }
                }

            }
        }
    }

    private GridCell FindNeighborCell(GridCell cell, Direction dir)
    {
        if (cell == null) return null;

        if (dir == Direction.Up) return GetCellAt(cell.xIndex, cell.yIndex + 1);
        if (dir == Direction.Down) return GetCellAt(cell.xIndex, cell.yIndex - 1);

        if (cell.xIndex % 2 == 0) // even column
        {
           

            if (dir == Direction.LeftUp) return GetCellAt(cell.xIndex - 1, cell.yIndex + 1);
            if (dir == Direction.LeftDown) return GetCellAt(cell.xIndex - 1, cell.yIndex);
            if (dir == Direction.RightUp) return GetCellAt(cell.xIndex + 1, cell.yIndex + 1);
            if (dir == Direction.RightDown) return GetCellAt(cell.xIndex + 1, cell.yIndex);

        }
        else //odd column
        {
            if (dir == Direction.LeftUp) return GetCellAt(cell.xIndex - 1, cell.yIndex);
            if (dir == Direction.LeftDown) return GetCellAt(cell.xIndex - 1, cell.yIndex - 1);
            if (dir == Direction.RightUp) return GetCellAt(cell.xIndex + 1, cell.yIndex);
            if (dir == Direction.RightDown) return GetCellAt(cell.xIndex + 1, cell.yIndex - 1);

        }
        return null;
    }

    public GridCell GetCellAt(int x, int y){
        if (x < 0) return null;
        if (y < 0) return null;
        if (x >= cells.GetLength(0)) return null;
        if (y >= cells.GetLength(1)) return null;


        return cells[x, y];
    }


    public bool CheckForMatches()
    {
        bool foundMatches = false;
        for(int x =0; x < cells.GetLength(0); x++)
        {
            for(int y=0; y < cells.GetLength(1); y++)
            {
                //check for matches up
               if(CheckNeighborForMatch(cells[x, y], Direction.Up) >= 3) foundMatches = true;
                //check for matches up and right
               if (CheckNeighborForMatch(cells[x, y], Direction.RightUp) >= 3) foundMatches = true;
                //check for matches down and right
               if (CheckNeighborForMatch(cells[x, y], Direction.RightDown) >= 3) foundMatches = true;

            }
        }
        return foundMatches;
    }
  
    private int CheckNeighborForMatch(GridCell cell, Direction dir, int lenght = 1)
    {
        GridCell cell2 = FindNeighborCell(cell, dir);
        if (cell2 !=null && cell.cellType == cell2.cellType){
            //match
            int total = CheckNeighborForMatch(cell2, dir, lenght+1);
            
            if (total >= 3)
            {
                cell.isMatched = true;
                cell2.isMatched = true;
            }

            return total;
        }
        else {
            //no match
            return lenght;
        }

    }



}//end
