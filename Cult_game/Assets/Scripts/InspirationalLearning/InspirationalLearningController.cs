using System.Collections;
using System.Collections.Generic;
using ResourcesObjects;
using UnityEngine;
using UnityEngine.UI;

public class InspirationalLearningController : MonoBehaviour
{
    public Text engName;
    public Text plName;
    public Text type;
    public Text points;
    public Text distance;
    public Text description;
    public Image placePic;

    private PlayerController _playerController;
    private Vector2 _placePosition;
    
    void Start()
    {
        _playerController = Utilities.FindPlayer();
        Place shownPlace = _playerController.places.Find(p => p.id == _playerController.CurrentPlayedPlaceId);

        engName.text = shownPlace.engName;
        plName.text = "Polish name: " + shownPlace.plName;
        type.text = "Type: " + shownPlace.type;
        points.text = "Points: " + shownPlace.scoreValue;
        description.text = shownPlace.description;
        
        _placePosition = new Vector2(shownPlace.latitude, shownPlace.longitude);
        InvokeRepeating(nameof(UpdateNavigation), 0.5f, 1f);
    }
    
    private void UpdateNavigation()
    {
        Vector2 playerPosition = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        
        float distanceValue = Geometry.DistanceFromCoordinates(playerPosition, _placePosition);
        distance.text = "Distance: " + Mathf.Round(distanceValue) + "m";
    }
}
