using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridController : MonoBehaviour {

    static public GridController grid; // singleton

    public float gridSpacing = 51;
    public int gridWidth = 4;
    public int gridHeight = 4;

    public GridCell cellPrefab;

    GridCell[,] cells;

    private void Start() {
        GridController.grid = this;

        BuildGrid(gridWidth, gridHeight);
    }

    private void Update() {
        
    }
    private void BuildGrid(int w, int h) {

        cells = new GridCell[w, h];

        for(int x = 0; x < w; x++) {
            for(int y = 0; y < h; y++) {

                GridCell cell = Instantiate(cellPrefab, transform);

                cell.xIndex = x;
                cell.yIndex = y;

                cell.PositionCell();

                cell.Init();

                cells[x, y] = cell;

            } // ends y loop
        } // ends x loop

    } // ends BuildGrid()

    public void TrySwap(GridCell cell, Direction dir) {
        //print("swap " + dir);

        GridCell cell2 = FindNeighborCell(cell, dir);

        if (cell2 == null) return; // no neighboring cell in that direction, do nothing...

        // TODO: swap the two cells

        // swap position in array:
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

        // TODO: check for matches

        // TODO: if no matches, unswap

    }
   

    private GridCell FindNeighborCell(GridCell cell, Direction dir) {

        if (cell == null) return null;

        if (dir == Direction.Up) return GetCellAt(cell.xIndex, cell.yIndex + 1);
        if (dir == Direction.Down) return GetCellAt(cell.xIndex, cell.yIndex - 1);

        if (cell.xIndex % 2 == 0) { // even column

            if (dir == Direction.LeftUp) return GetCellAt(cell.xIndex - 1, cell.yIndex + 1);
            if (dir == Direction.LeftDown) return GetCellAt(cell.xIndex - 1, cell.yIndex);
            if (dir == Direction.RightUp) return GetCellAt(cell.xIndex + 1, cell.yIndex + 1);
            if (dir == Direction.RightDown) return GetCellAt(cell.xIndex + 1, cell.yIndex);

        } else { // odd column

            if (dir == Direction.LeftUp) return GetCellAt(cell.xIndex - 1, cell.yIndex);
            if (dir == Direction.LeftDown) return GetCellAt(cell.xIndex - 1, cell.yIndex - 1);
            if (dir == Direction.RightUp) return GetCellAt(cell.xIndex + 1, cell.yIndex);
            if (dir == Direction.RightDown) return GetCellAt(cell.xIndex + 1, cell.yIndex - 1);
        }
        return null;
    }

    public GridCell GetCellAt(int x, int y) {

        if (x < 0) return null;
        if (y < 0) return null;
        if (x >= cells.GetLength(0)) return null;
        if (y >= cells.GetLength(1)) return null;

        return cells[x, y];
    }

} // ends GridController
