using System;
using Resources;
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
    private Place _targettedPlace;

    private bool _promptExists;
    
    // Start is called before the first frame update
    private void Start()
    {
        _promptExists = false;
        
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _playerController = player.GetComponent<PlayerController>();
        }

        _playerPosition = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        if (_playerController.selectedPlace == null)
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
        _targettedPlace = FindNearestPlace();
        SetPlaceAsLocalTarget(_targettedPlace);
        nameText.text = "Nearest attraction: " + _targettedPlace.engName;
    }
    
    private void SetTargetToCustomPlace()
    {
        SetPlaceAsLocalTarget(_playerController.selectedPlace);
        _targettedPlace = _playerController.selectedPlace;
        nameText.text = "Selected attraction: " + _targettedPlace.engName;
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
        float distance = DistanceFromCoordinates(_playerPosition, _localTargetPosition);
        distanceText.text = "Distance: " + Mathf.Round(distance) + "m";
        if (distance < _playerController.DistanceThreshold && _promptExists == false)
        {
            _promptExists = true;
            CreateDiscoveryPrompt();
        }
    }
    private void UpdateArrowDirection()
    {
        float angle = AngleFromCoordinate(_playerPosition, _localTargetPosition);
        compassArrow.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Round(angle));
    }

    private void CreateDiscoveryPrompt()
    {
        GameObject newDiscoveryPrompt = Instantiate(discoveryPromptPrefab, background);
        newDiscoveryPrompt.transform.Find("SecondLine").GetComponent<Text>().text = _targettedPlace.engName;

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
            _playerController.DiscoveredPlaces.Add(_targettedPlace.id);
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
            float localDistance = DistanceFromCoordinates(_playerPosition, localTarget);
            if (!(localDistance < minDistance)) continue;
            minDistance = localDistance;
            nearest = place;
        }
        return nearest;
    }
    
    private static float DistanceFromCoordinates(Vector2 coord1, Vector2 coord2)
    {
        //sauce: https://stackoverflow.com/questions/639695/how-to-convert-latitude-or-longitude-to-meters
        const double R = 6378.137; // Radius of earth in KM
        double dLat = coord2.x * Math.PI / 180 - coord1.x * Math.PI / 180;
        double dLon = coord2.y * Math.PI / 180 - coord1.y * Math.PI / 180;
        double a = Math.Sin(dLat/2) * Math.Sin(dLat/2) +
                Math.Cos(coord1.x * Math.PI / 180) * Math.Cos(coord2.x * Math.PI / 180) *
                Math.Sin(dLon/2) * Math.Sin(dLon/2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
        double d = R * c;
        return (float)d * 1000; // meters
    }
    
    private static float AngleFromCoordinate(Vector2 coord1, Vector2 coord2)
    {
        //sauce: https://stackoverflow.com/questions/3932502/calculate-angle-between-two-latitude-longitude-points
        double dLon = coord2.y - coord1.y;

        double y = Math.Sin(dLon) * Math.Cos(coord2.x);
        double x = Math.Cos(coord1.x) * Math.Sin(coord2.x) - Math.Sin(coord1.x)
                   * Math.Cos(coord2.x) * Math.Cos(dLon);

        double brng = Math.Atan2(y, x);

        brng = brng * 180 / Math.PI;
        brng = (brng + 360) % 360;
        brng = 360 - brng; // count degrees counter-clockwise - remove to make clockwise

        return (float)brng;
    }
}
