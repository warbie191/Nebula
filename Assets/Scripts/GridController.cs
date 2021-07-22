using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public struct GridPosition {
    static public GridPosition Zero = new GridPosition(0);
    public int x;
    public int y;
    public override string ToString() {
        return $"({x}, {y})";
    }
    public Vector2 vec2 {
        get {
            return new Vector2(x, y);
        }
    }
    public GridPosition(int xy) {
        this.x = xy;
        this.y = xy;
    }
    public GridPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }
}
public class GridController : MonoBehaviour {

    static public GridController grid; //singleton

    /// <summary>
    /// This class holds all of the classes that make up the GridController's finite state machine (FSM).
    /// This class is static, which means we cannot instantiate it.
    /// </summary>
    public static class States {
        /// <summary>
        /// The base-clase for the states we'll use in GridController's state machine.
        /// This class is abstract, which means we cannot instantiate it, but we CAN
        /// instantiate sub-classes of this class.
        /// </summary>
        public abstract class State {
            protected bool doneAnimating {  get { return animTimer > animTime; } }
            protected float animTime = .25f;
            private float animTimer = 0;
            public bool acceptsInput { get; protected set; }
            /// <summary>
            /// We will call this every game-tick.
            /// It is marked as virtual, which means sub-classes can override it.
            /// </summary>
            /// <returns>This function is meant to return the State that the state machine
            /// should switch to next. A value of `null` means that the state machine should NOT switch.</returns>
            public virtual State Update() {
                if (animTime > 0f) {
                    animTimer += Time.deltaTime;

                    if (animTimer >= animTime) return new Idle();
                }
                return null;
            }
            /// <summary>
            /// This will be called once when this state becomes the active state in the FSM.
            /// </summary>
            public virtual void OnStart() { }
            /// <summary>
            /// This will be called once when this state is no longer the active state in the FSM.
            /// </summary>
            public virtual void OnEnd() { }
        }
        /// <summary>
        /// This is the Idle state. In this state, the board can receive input from the player.
        /// </summary>
        public class Idle : State {
            bool hasChecked = false;
            public Idle() {
                animTime = 0f;
                acceptsInput = true;
            }
            public override State Update() {
                if (!hasChecked) {
                    hasChecked = true;
                }

                return null;
            }
        }
        public class Swapping : State {
            protected bool checkForMatches = true;
            public Swapping(bool checkForMatches = true) {
                this.checkForMatches = checkForMatches;
            }
            public override State Update() {
                for (int x = 0; x < grid.cells.GetLength(0); x++) // for each column ...
                {
                    for (int y = 0; y < grid.cells.GetLength(1); y++) // go up the column (from 0), one gem at a time
                    {
                        grid.cells[x, y].AnimSlideToTarget();
                    }
                }
                base.Update();
                if (doneAnimating) {
                    if (checkForMatches) { // unswapping does NOT check for matches
                        if (grid.CheckForMatches()) {
                            return new Popping();
                        } else {
                            return new Unswapping();
                        }
                    }
                    return new Idle();
                }
                return null;
            }
        }
        public class Unswapping : Swapping {
            public Unswapping() : base(false) {

            }
            public override void OnStart() {
                grid.UndoSwap();
            }
        }
        public class Popping : State {

            public Popping() {
                animTime = .5f;
            }

            public override State Update() {
                // animate the cells to shrink:
                for (int x = 0; x < grid.cells.GetLength(0); x++) // for each column ...
                {
                    for (int y = 0; y < grid.cells.GetLength(1); y++) // go up the column (from 0), one gem at a time
                    {
                        grid.cells[x, y].AnimChangeScale();
                    }
                }

                base.Update();
                if(doneAnimating) {
                    grid.Popmatches();
                    return new Falling();
                }
                return null;
            }
        }
        public class Falling : State {
            public Falling() {
                animTime = 1.5f;
            }
            public override State Update() {
                for (int x = 0; x < grid.cells.GetLength(0); x++) // for each column ...
                {
                    for (int y = 0; y < grid.cells.GetLength(1); y++) // go up the column (from 0), one gem at a time
                    {
                        grid.cells[x, y].AnimFallToTarget();
                    }
                }
                return base.Update();
            }
        }
    }

    public Canvas parentUI;

    public float gridSpacing = 51;
    public int gridWidth = 4;
    public int gridHeight = 4;

    public GridCell cellPrefab;
    GridCell[,] cells;

    /// <summary>
    /// This holds a reference to the current state of the GridController's FSM.
    /// </summary>
    States.State boardState;

    GridPosition lastSwapA;
    GridPosition lastSwapB;


    private void Start() {
        GridController.grid = this;
        BuildGrid(gridWidth, gridHeight);
    }

    private void Update() {
        if (boardState == null) boardState = new States.Idle(); // if the FSM has no state, set it to Idle
        ChangeStates(boardState.Update());

    }
    public bool AcceptsInput() {
        if (boardState == null) return false;
        return boardState.acceptsInput;
    }
    private void ChangeStates(States.State newState) {
        if (newState == null) return;
        if (boardState != null) boardState.OnEnd();
        boardState = newState;
        boardState.OnStart();
    }

    private void BuildGrid(int w, int h) {
        cells = new GridCell[w, h];
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {

                // spawn UI game object:
                GridCell cell = Instantiate(cellPrefab, parentUI.transform);

                // store cell in grid:
                cells[x, y] = cell;

                // position it on the screen:
                cell.PositionCell(new GridPosition(x, y), true);


            }//end y loop
        }//end x loop

    }//BuildGrid

    public GridPosition? LookupCellPosition(GridCell cell) {
        if (cell == null) return null;
        for (int x = 0; x < cells.GetLength(0); x++) {
            for (int y = 0; y < cells.GetLength(1); y++) {

                if (cell == cells[x, y]) return new GridPosition(x, y);

            }//end y loop
        }//end x loop
        return null;
    }


    public void TrySwap(GridCell cell, Direction dir)
    {
        if (!AcceptsInput()) return;
        if (cell == null) return;

        GridPosition cell1Pos = (GridPosition)LookupCellPosition(cell);
        GridPosition cell2Pos = FindNeighborCell(cell1Pos, dir);
        GridCell cell2 = LookupCell(cell2Pos);

        if (cell == null || cell2 == null) return;

        // swap them:
        cells[cell1Pos.x, cell1Pos.y] = cell2;
        cells[cell2Pos.x, cell2Pos.y] = cell;

        // notify them:
        cell.PositionCell(cell2Pos);
        cell2.PositionCell(cell1Pos);

        lastSwapA = cell1Pos;
        lastSwapB = cell2Pos;

        // change state to Swapping:
        ChangeStates(new States.Swapping(true));
    }
    public void UndoSwap() {

        GridCell cell = LookupCell(lastSwapA);
        GridCell cell2 = LookupCell(lastSwapB);

        // swap them:
        cells[lastSwapA.x, lastSwapA.y] = cell2;
        cells[lastSwapB.x, lastSwapB.y] = cell;

        // notify them:
        cell.PositionCell(lastSwapB);
        cell2.PositionCell(lastSwapA);

        //lastSwapA = lastSwapB;
        //lastSwapB = cell2Pos;

        // change state to Swapping:
        ChangeStates(new States.Swapping(false));
    }

    private void Popmatches()
    {

        GridCell[,] temp = new GridCell[cells.GetLength(0), cells.GetLength(1)];

        for (int x = 0; x < cells.GetLength(0); x++) // for each column ...
        {

            int amountMatchThisColumn = 0; // track the number of matches

            for (int y = 0; y < cells.GetLength(1); y++) // go up the column (from 0), one gem at a time
            {
                GridCell cell = cells[x, y];
                if (cell.isMatched) // cell is matched:
                {
                    amountMatchThisColumn ++;

                    int newYIndex = cells.GetLength(1) - amountMatchThisColumn;

                    temp[x, newYIndex] = cell; //move cell

                }
                else // cell is NOT matched:
                {
                    int newYIndex = y - amountMatchThisColumn;
                    temp[x, newYIndex] = cell; //move cell
                    cell.PositionCell(new GridPosition(x, newYIndex));
                }
            }
            int secondCount = 0;
            for (int y = 0; y < temp.GetLength(1); y++) // go up the column (from 0), one gem at a time
            {
                GridCell cell = temp[x, y];
                if (cell.isMatched) // cell is matched:
                {
                    secondCount++;
                    cell.Respawn();
                    cell.PositionCell(new GridPosition(x, cells.GetLength(1) + secondCount), true);
                    cell.PositionCell(new GridPosition(x, y));
                    
                }
            }
        }

        cells = temp;

    }

    private GridPosition FindNeighborCell(GridPosition pos, Direction dir)
    {

        if (dir == Direction.Up) return new GridPosition(pos.x, pos.y + 1);
        if (dir == Direction.Down) return new GridPosition(pos.x, pos.y - 1);

        if (pos.x % 2 == 0) // even column
        {
            if (dir == Direction.LeftUp) return new GridPosition(pos.x - 1, pos.y + 1);
            if (dir == Direction.LeftDown) return new GridPosition(pos.x - 1, pos.y);
            if (dir == Direction.RightUp) return new GridPosition(pos.x + 1, pos.y + 1);
            if (dir == Direction.RightDown) return new GridPosition(pos.x + 1, pos.y);
        }
        else //odd column
        {
            if (dir == Direction.LeftUp) return new GridPosition(pos.x - 1, pos.y);
            if (dir == Direction.LeftDown) return new GridPosition(pos.x - 1, pos.y - 1);
            if (dir == Direction.RightUp) return new GridPosition(pos.x + 1, pos.y);
            if (dir == Direction.RightDown) return new GridPosition(pos.x + 1, pos.y - 1);

        }
        print("uh oh...");
        return pos; // this shouldn't happen
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
                GridPosition pos = new GridPosition(x, y);
                //check for matches up
               if(CheckNeighborForMatch(pos, Direction.Up) >= 3) foundMatches = true;
                //check for matches up and right
               if (CheckNeighborForMatch(pos, Direction.RightUp) >= 3) foundMatches = true;
                //check for matches down and right
               if (CheckNeighborForMatch(pos, Direction.RightDown) >= 3) foundMatches = true;

            }
        }
        return foundMatches;
    }
  
    private int CheckNeighborForMatch(GridPosition pos, Direction dir)
    {
        List<GridCell> cellsInMatch = new List<GridCell>();
        GridCell cell = LookupCell(pos);
        cellsInMatch.Add(cell);

        int lengthOfMatch = 1;
        while(cell != null && lengthOfMatch < 12) {

            GridPosition cell2Pos = FindNeighborCell(pos, dir);
            GridCell cell2 = LookupCell(cell2Pos);
            if(cell2 == null) break;
            if(cell2.cellType != cell.cellType) break;
            
            cellsInMatch.Add(cell2);
            lengthOfMatch++;
            cell = cell2;
            pos = cell2Pos;
        }
        if (lengthOfMatch >= 3)
        {
            foreach (GridCell c in cellsInMatch) c.isMatched = true;
        }
        //print(pos + " (type " + cell.cellType + ") dir: " + dir + " results: " + lengthOfMatch);
        return lengthOfMatch;
    }

    private GridCell LookupCell(GridPosition pos) {

        if (pos.x < 0) return null;
        if (pos.y < 0) return null;
        if (pos.x >= cells.GetLength(0)) return null;
        if (pos.y >= cells.GetLength(1)) return null;

        return cells[pos.x, pos.y];
    }
}//end
