using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileTracker : MonoBehaviour
{
    private static float MOVING_THRESHOLD = 0.3f; // when touch is recognised as real dragging

    private GridController gridController; 
    private float statPosX;
    private float statPosY;
    private UnityEngine.UI.Image image;
    private Collider2D ownCollider;
    private Collider2D lastlyCollidedWith;
    private bool insideBoard;

    private int originTileKey;
    private int currentTileKey;

    private float deltaX; //diff beetween touch pos and tile pivot
    private float deltaY;

    void Awake()
    { 
        
        this.ownCollider = this.GetComponent<Collider2D>();
        this.lastlyCollidedWith = null;
        this.insideBoard = true;
        GameObject gridHolder = GameObject.FindWithTag("GridHolder");
        this.gridController = gridHolder.GetComponent<GridController>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTouchEvent();

    }

    private void CheckTouchEvent()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            if (((this.gridController.CheckDraggingTile() == -1) && this.ownCollider == Physics2D.OverlapPoint(touchPos)) || (this.gridController.CheckDraggingTile() == this.originTileKey)){

                switch (touch.phase)
                {

                    case TouchPhase.Began:
                        this.deltaX = touchPos.x - this.transform.position.x;
                        this.deltaY = touchPos.y - this.transform.position.y;
                        break;

                    case TouchPhase.Moved:

                        if(DetermineRealMovement(touchPos) && this.insideBoard)
                        {
                            this.gridController.SetDraggingTile(this.originTileKey);
                            this.transform.SetAsLastSibling();
                            this.transform.position = new Vector3(touchPos.x - this.deltaX, touchPos.y - this.deltaY, 0f);
                        }
                        break;

                    case TouchPhase.Ended:

                        if (this.gridController.CheckDraggingTile() == -1)
                        {          
                            this.RotateBy(90f);
                            this.gridController.updateEfficiency();
                            this.gridController.CheckProperPlacement();
                        }
                        else if (this.gridController.CheckDraggingTile() == this.originTileKey)
                        {
                            if (this.lastlyCollidedWith != null && this.insideBoard)
                            {
                                this.gridController.MarkNewPlacement(this, this.lastlyCollidedWith.GetComponent<TileTracker>(),false);
                                this.gridController.updateEfficiency();
                            }
                            else
                            {
                                this.RestoreStatPos();
                            }
                            this.lastlyCollidedWith = null;
                            this.insideBoard = true;
                            this.gridController.SetDraggingTile(-1);
                        }  
                        break;
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D collidedWithCollider2D)
    {
        if (collidedWithCollider2D.CompareTag("Tile")) 
        {
            //Debug.Log("" + collidedWithCollider2D.GetComponent<TileTracker>().GetOriginKey());
            this.lastlyCollidedWith = collidedWithCollider2D;

        }
    }
    void OnTriggerEnter2D(Collider2D collidedWithCollider2D)
    {
         if (collidedWithCollider2D.CompareTag("Grid"))
        {
            this.insideBoard = false;
            RestoreStatPos();
        }
    }

    private bool DetermineRealMovement(Vector2 touchPos)
    {  
        if ((Math.Abs(Math.Abs(this.statPosX) - Math.Abs(touchPos.x - this.deltaX)) > MOVING_THRESHOLD)||
            (Math.Abs(Math.Abs(this.statPosY) - Math.Abs(touchPos.y - this.deltaY)) > MOVING_THRESHOLD))
        {
            return true;
        }
        return false;
    }

    private void RestoreStatPos() {

        this.SetStationaryPosition(new Vector3(this.statPosX, this.statPosY, -1f));
    }
    public TileTracker SetOriginTileKey(int key)
    {
        this.originTileKey = key;
        return this;
    }
    public int GetOriginKey()
    {
        return this.originTileKey;
    }
    public TileTracker SetCurrentTileKey(int key)
    {
        this.currentTileKey = key;
        return this;
    }
    public int GetcurrentTileKey()
    {
        return this.currentTileKey;
    }
    public TileTracker SetStationaryPosition(Vector3 pos)
    {
        this.statPosX = pos.x;
        this.statPosY = pos.y;
        this.transform.localPosition = pos;
        return this;
    }
    public TileTracker RotateBy(float angle)
    {
        this.transform.Rotate(new Vector3(0f, 0f, angle), Space.Self);
        return this;
    }

    public TileTracker SetSprite(Sprite sprite)
    {
        this.image = this.GetComponent<UnityEngine.UI.Image>();
        this.image.sprite = sprite;
        return this;
    }

    public bool CheckProperPlace()
    {
        if(originTileKey == currentTileKey)
        {
           return true;
        }
        return false; 
    }
    public int CheckNeededRotationsAmount()
    {
        int rotZ = (int)this.transform.eulerAngles.z;
        if (rotZ == 0f)
            return 0;

        return (int)( 4f - (this.transform.eulerAngles.z / 90f));
    }

    public Vector3 GetStationaryPosition()
    {
        return new Vector3(this.statPosX, this.statPosY, -1f);
    }
}
