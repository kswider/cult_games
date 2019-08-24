﻿using System;
using ResourcesObjects;
using UnityEngine;
using UnityEngine.UI;

public class ExploringController : MonoBehaviour
{
    public GameObject compassArrow;
    public Text nameText;
    public Text distanceText;
    public Transform background;
    public GameObject discoveryPromptPrefab;

    private PlayerController _playerController;

    private Vector2 _localTargetPosition;
    private Vector2 _playerPosition;
    private Place _targetedPlace;

    private bool _promptExists;
    
    // Start is called before the first frame update
    private void Start()
    {
        _promptExists = false;
        
        _playerController = Utilities.FindPlayer();

        _playerPosition = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        if (_playerController.Settings.SelectedPlace == null)
        {
            SetTargetToNearestPlace();
        }
        else
        {
            SetTargetToCustomPlace();
        }
        InvokeRepeating(nameof(UpdateNavigation), 0.5f, 0.5f);
    }
    
    public void SetTargetToNearestPlace()
    {
        _playerController.Settings.SelectedPlaceId = -1;
        _playerController.Settings.SelectedPlace = null;
        _targetedPlace = FindNearestPlace();
        SetPlaceAsLocalTarget(_targetedPlace);
        nameText.text = "Nearest attraction: " + _targetedPlace.engName;
    }
    
    private void SetTargetToCustomPlace()
    {
        SetPlaceAsLocalTarget(_playerController.Settings.SelectedPlace);
        _targetedPlace = _playerController.Settings.SelectedPlace;
        nameText.text = "Selected attraction: " + _targetedPlace.engName;
    }
    
    private void SetPlaceAsLocalTarget(Place place)
    {
        _localTargetPosition = new Vector2(place.latitude, place.longitude);
    }

    private void UpdateNavigation()
    {
        _playerPosition = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        UpdateDistance();
        UpdateArrowDirection();
    }
    private void UpdateDistance()
    {
        float distance = Geometry.DistanceFromCoordinates(_playerPosition, _localTargetPosition);
        distanceText.text = "Distance: " + Mathf.Round(distance) + "m";
        
        if (!(distance < _playerController.Settings.DistanceThreshold) || _promptExists) return;
        _promptExists = true;
        CreateDiscoveryPrompt();
    }
    private void UpdateArrowDirection()
    {
        float angle = Geometry.AngleFromCoordinate(_playerPosition, _localTargetPosition);
        compassArrow.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Round(angle));
    }

    private void CreateDiscoveryPrompt()
    {
        GameObject newDiscoveryPrompt = Instantiate(discoveryPromptPrefab, background);
        newDiscoveryPrompt.transform.Find("SecondLine").GetComponent<Text>().text = _targetedPlace.engName;

        String gameType = "???"; //TODO
        String difficulty = "???"; //TODO

        newDiscoveryPrompt.transform.Find("FourthLine").GetComponent<Text>().text =
            "Game type: " + gameType + "\nDifficulty: " + difficulty;
        newDiscoveryPrompt.transform.Find("Buttons/BTN_NO").GetComponent<Button>().onClick.AddListener(delegate
        {
            // TODO set block time to prevent prompt spam
            _promptExists = false;
            Destroy(newDiscoveryPrompt);
        });
        newDiscoveryPrompt.transform.Find("Buttons/BTN_YES").GetComponent<Button>().onClick.AddListener(delegate
        {
            // TODO sceneController -> move to valid scene according to gameType.
            _playerController.DiscoveredPlaces.Add(_targetedPlace.id);
            SetTargetToNearestPlace();
            _promptExists = false;
            Destroy(newDiscoveryPrompt);
        });
    }
    
    private Place FindNearestPlace()
    {
        float minDistance = float.MaxValue;
        Place nearest = null;
        
        foreach (var place in _playerController.places)
        {
            if(_playerController.DiscoveredPlaces.Contains(place.id)) continue;
            
            Vector2 localTarget = new Vector2(place.latitude, place.longitude);
            float localDistance = Geometry.DistanceFromCoordinates(_playerPosition, localTarget);
            if (!(localDistance < minDistance)) continue;
            minDistance = localDistance;
            nearest = place;
        }
        return nearest;
    }
}
