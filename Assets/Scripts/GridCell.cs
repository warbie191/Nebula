using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public enum Direction
{
    RightDown,
    Down,
    LeftDown,
    LeftUp,
    Up,
    RightUp
}
public class GridCell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
    public CellType cellType { get; private set;}

    public bool isMatched = false;

    public Color[] colors;


    /// <summary>
    /// The pixel position that the cell wants to be at for animation
    /// </summary>
    private Vector2 targetPosition;
    private Vector2 velocity;

    private void UpdateSprite() {
        Image img = GetComponent<Image>();
        if (img != null) {
            int n = (int)cellType;
            if (n < 0) n = 0;
            if (n >= colors.Length) n = colors.Length - 1;
            img.color = colors[n];
        }
    }
    public void Spawn(CellType cellType) {
        isMatched = false;
        RectTransform rt = (RectTransform)transform;
        rt.localScale = Vector3.one;
        velocity = Vector2.zero;
        SetCellType(cellType);
    }

    public void SpawnRandom() {
        Spawn(RandCellType());
    }
    public void AnimSlideToTarget() {
        RectTransform rt = (RectTransform)transform;
        Vector2 dis = targetPosition - rt.anchorMin;
        rt.anchorMax = rt.anchorMin = rt.anchorMin + dis * .05f;
    }
    public bool AnimFallToTarget() {
        RectTransform rt = (RectTransform)transform;
        if (rt.anchorMin.y <= targetPosition.y) return true;

        velocity -= new Vector2(0, 10f) * Time.deltaTime;
        Vector2 newPos = rt.anchorMin + velocity * Time.deltaTime;
        if (newPos.y < targetPosition.y) newPos.y = targetPosition.y;
        rt.anchorMax = rt.anchorMin = newPos;
        return false;
    }
    public void AnimChangeScale() {
        if (isMatched)
        {
            RectTransform rt = (RectTransform)transform;
            rt.localScale += (Vector3.zero - rt.localScale) * .05f;
        }
    }
    public static CellType RandCellType() {
        return (CellType)Random.Range(1, System.Enum.GetNames(typeof(CellType)).Length);
    }
    public void SetCellType(CellType cellType) {
        this.cellType = cellType;
        UpdateSprite();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
       if(GridController.grid.isEditMode) {

            if(cellType == CellType.None) {
                SetCellType(RandCellType());
            } else {
                SetCellType(CellType.None);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        if (GridController.grid.isEditMode) return;

        Vector2 cellPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 vectorToMouse = eventData.position - cellPos;

        if (vectorToMouse.magnitude > 30)        {
            float radians = Mathf.Atan2(vectorToMouse.y, vectorToMouse.x);
            float degrees = radians * Mathf.Rad2Deg;
           

            if (degrees >= 0 && degrees < 60) GridController.grid.TrySwap(this, Direction.RightUp);
            if (degrees >= 60 && degrees < 120) GridController.grid.TrySwap(this, Direction.Up);
            if (degrees >= 120 && degrees < 180) GridController.grid.TrySwap(this, Direction.LeftUp);
            if (degrees >= -180 && degrees < -120) GridController.grid.TrySwap(this, Direction.LeftDown);
            if (degrees >= -120 && degrees < -60) GridController.grid.TrySwap(this, Direction.Down);
            if (degrees >= -60 && degrees < 0) GridController.grid.TrySwap(this, Direction.RightDown);
        }

    }//ends OnPointerUP()


    public void PositionCell(GridPosition gridPos, bool snapToTarget = false)
    {
        Text txt = GetComponentInChildren<Text>();

        // set text:
        if (txt != null)  txt.text = gridPos.x + ", " + gridPos.y;

        // get a copy of the gridPos as a Vector2
        Vector2 pos = gridPos.vec2;

        //pos.x += .5f;
        //pos.y += .5f;

        // if we're in even column, slide add .5 to x
        if (gridPos.x % 2 == 0) pos.y += 0.5f;

        // scale by the current grid-spacing
        pos *= GridController.grid.gridSpace;

        // set target (pixel) position
        targetPosition = pos;

        // snap to target position
        //if (snapToTarget) (transform as RectTransform).anchoredPosition = targetPosition;
        if (snapToTarget) {
            (transform as RectTransform).anchorMin = targetPosition;
            (transform as RectTransform).anchorMax = targetPosition;
        }
        
        velocity = Vector2.zero;
    }

}//End GridCell
