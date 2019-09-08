using ResourcesObjects;
using UnityEngine;
using UnityEngine.UI;

public class InspirationalLearningController : MonoBehaviour
{
    public Canvas parentCanvas;
    public Text engName;
    public Text plName;
    public Text type;
    public Text points;
    public Text distance;
    public Text description;
    public Transform placePic;
    
    private PlayerController _playerController;
    private Vector2 _placePosition;

    private JigsawGrid _gridIMG;
    private Place _shownPlace;
    
    void Start()
    {
        _playerController = Utilities.FindPlayerController();
        _shownPlace = _playerController.Places.Find(p => p.id == _playerController.CurrentPlayedPlaceId);

        engName.text = _shownPlace.engName;
        plName.text = "Polish name: " + _shownPlace.plName;
        type.text = "Type: " + _shownPlace.type;
        points.text = "Points: " + _shownPlace.scoreValue;
        description.text = _shownPlace.description;
        _placePosition = new Vector2(_shownPlace.latitude, _shownPlace.longitude);
        
        LoadImage();
        
        InvokeRepeating(nameof(UpdateNavigation), 0.5f, 1f);
    }
    
    private void UpdateNavigation()
    {
        Vector2 playerPosition = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        
        float distanceValue = Geometry.DistanceFromCoordinates(playerPosition, _placePosition);
        distance.text = "Distance: " + Mathf.Round(distanceValue) + "m";
    }

    private void LoadImage()
    {
        Vector2Int gridDifficulty = GridController.DifficultyToBounds(_shownPlace.gameDifficulty);
        _gridIMG = new JigsawGrid(gridDifficulty, _shownPlace.imagePath, _shownPlace.gameDifficulty, parentCanvas);
        GridHelper.SetPosition(_gridIMG, placePic.transform);
        GridHelper.ScaleGrid(_gridIMG, new Vector3(0.5f, 0.5f, 0.5f));
        GridHelper.TurnOffFlamesAtAll(_gridIMG);
        GridHelper.TurnOffPhysics(_gridIMG);
    }
}
