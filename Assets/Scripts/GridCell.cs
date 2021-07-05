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
    public int cellType{ get; private set;}
    public int xIndex  { get; set;}
    public int yIndex { get; set; }

    public bool isMatched = false;

    public Color[] colors;

    private void Start(){
        cellType = Random.Range(0, colors.Length);
        Image img = GetComponent<Image>();
        if (img != null) {
            img.color = colors[cellType];
        }
    }


    public void Init(){
        Text txt = GetComponentInChildren<Text>();
        if (txt != null)        {
            txt.text = xIndex + ", " + yIndex;
        }
    }



    public void OnPointerDown(PointerEventData eventData)
    {
       // print("My value is " + cellType);
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //print("wootwoot");
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




    public void PositionCell()
    {
        Vector2 pos = new Vector2(xIndex, yIndex) * GridController.grid.gridSpacing;
        if(xIndex % 2 == 0)
        {
            pos.y += GridController.grid.gridSpacing / 2;
        }
        RectTransform rt = (RectTransform)transform;
        rt.anchoredPosition = pos;

    }

}//End GridCell
