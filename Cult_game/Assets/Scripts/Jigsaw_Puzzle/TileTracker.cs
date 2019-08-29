using System;
using UnityEngine;
using UnityEngine.UI;

public class TileTracker : MonoBehaviour
{
    private static float MOVING_THRESHOLD = 0.3f; // when touch is recognised as real dragging

    private GridController _gridController; 
    private float _statPosX;
    private float _statPosY;
    private Image _image;
    private Collider2D _ownCollider;
    private Collider2D _lastlyCollidedWith = null;
    private bool _insideBoard = true;

    private int _originTileKey;
    private int _currentTileKey;

    private float _deltaX; //diff beetween touch pos and tile pivot
    private float _deltaY;

    private void Awake()
    {
        _ownCollider = GetComponent<Collider2D>();
        
        GameObject gridHolder = GameObject.FindWithTag("GridHolder");
        _gridController = gridHolder.GetComponent<GridController>();
    }

    private void Update()
    {
        CheckTouchEvent();
    }

    private void CheckTouchEvent()
    {
        if (Input.touchCount <= 0) return;
        
        Touch touch = Input.GetTouch(0);
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

        if ((_gridController.DraggedTileKey != -1 || _ownCollider != Physics2D.OverlapPoint(touchPos)) &&
            _gridController.DraggedTileKey != _originTileKey) 
            return;
        
        switch (touch.phase)
        {
            case TouchPhase.Began:
                _deltaX = touchPos.x - transform.position.x;
                _deltaY = touchPos.y - transform.position.y;
                break;

            case TouchPhase.Moved:
                if(DetermineRealMovement(touchPos) && _insideBoard)
                {
                    _gridController.DraggedTileKey = _originTileKey;
                    transform.SetAsLastSibling();
                    transform.position = new Vector3(touchPos.x - _deltaX, touchPos.y - _deltaY, 0f);
                }
                break;

            case TouchPhase.Ended:
                if (_gridController.DraggedTileKey == -1)
                {          
                    RotateBy(90f);
                    _gridController.UpdateEfficiency();
                    _gridController.CheckProperPlacement();
                }
                else if (_gridController.DraggedTileKey == _originTileKey)
                {
                    if (_lastlyCollidedWith != null && _insideBoard)
                    {
                        _gridController.MarkNewPlacement(this, _lastlyCollidedWith.GetComponent<TileTracker>(),false);
                        _gridController.UpdateEfficiency();
                    }
                    else
                    {
                        RestoreStatPos();
                    }
                    _lastlyCollidedWith = null;
                    _insideBoard = true;
                    _gridController.DraggedTileKey = -1;
                }  
                break;
        }
    }

    void OnTriggerStay2D(Collider2D collidedWithCollider2D)
    {
        if (collidedWithCollider2D.CompareTag("Tile")) 
        {
            //Debug.Log("" + collidedWithCollider2D.GetComponent<TileTracker>().GetOriginKey());
            _lastlyCollidedWith = collidedWithCollider2D;

        }
    }
    void OnTriggerEnter2D(Collider2D collidedWithCollider2D)
    {
        if (!collidedWithCollider2D.CompareTag("Grid")) return;
        
        _insideBoard = false;
         RestoreStatPos();
    }

    private bool DetermineRealMovement(Vector2 touchPos)
    {
        return Math.Abs(Math.Abs(_statPosX) - Math.Abs(touchPos.x - _deltaX)) > MOVING_THRESHOLD ||
               Math.Abs(Math.Abs(_statPosY) - Math.Abs(touchPos.y - _deltaY)) > MOVING_THRESHOLD;
    }

    private void RestoreStatPos() {

        SetStationaryPosition(new Vector3(_statPosX, _statPosY, -1f));
    }
    public TileTracker SetOriginTileKey(int key)
    {
        _originTileKey = key;
        return this;
    }
    
    public TileTracker SetCurrentTileKey(int key)
    {
        _currentTileKey = key;
        return this;
    }
    public int GetcurrentTileKey()
    {
        return _currentTileKey;
    }
    public TileTracker SetStationaryPosition(Vector3 pos)
    {
        _statPosX = pos.x;
        _statPosY = pos.y;
        transform.localPosition = pos;
        return this;
    }
    public TileTracker RotateBy(float angle)
    {
        transform.Rotate(new Vector3(0f, 0f, angle), Space.Self);
        return this;
    }

    public TileTracker SetSprite(Sprite sprite)
    {
        _image = GetComponent<Image>();
        _image.sprite = sprite;
        return this;
    }

    public bool CheckProperPlace()
    {
        return _originTileKey == _currentTileKey;
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
        return new Vector3(_statPosX, _statPosY, -1f);
    }
}
