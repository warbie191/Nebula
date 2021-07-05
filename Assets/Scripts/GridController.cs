using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour {

    public int gridWidth = 4;
    public int gridHeight = 4;

    public GridCell cellPrefab;

    GridCell[,] cells;

    private void Start() {
        BuildGrid(gridWidth, gridHeight);
    }

    private void Update() {
        
    }
    private void BuildGrid(int w, int h) {

        cells = new GridCell[w, h];

        for(int x = 0; x < w; x++) {
            for(int y = 0; y < h; y++) {

                GridCell cell = Instantiate(cellPrefab, transform);
                RectTransform rt = (RectTransform) cell.transform;
                Vector2 pos = new Vector2(x, y) * 51;
                rt.anchoredPosition = pos;

            } // ends y loop
        } // ends x loop

    } // ends BuildGrid()
} // ends GridController
