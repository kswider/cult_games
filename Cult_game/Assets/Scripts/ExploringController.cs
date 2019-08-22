using System;
using Resources;
using UnityEngine;
using UnityEngine.UI;

public class ExploringController : MonoBehaviour
{
    public GameObject compassArrow;
    
    private Text _nameText;
    private Text _distanceText;
    private Text _currentScore;
    private PlayerController _playerController;
    
    private Vector2 _localTarget;
    private Vector2 _currentPosition;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject firstLine = GameObject.Find("FirstLine");
        if (firstLine != null)
        {
            _nameText = firstLine.GetComponent<Text>();
        }
        GameObject secondLine = GameObject.Find("SecondLine");
        if (secondLine != null)
        {
            _distanceText = secondLine.GetComponent<Text>();
        }
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _playerController = player.GetComponent<PlayerController>();
        }
        GameObject score = GameObject.Find("Points");
        if (score != null)
        {
            _currentScore = score.GetComponent<Text>();
        }

        _currentPosition = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        if (_playerController.selectedPlace == null)
        {
            SetPlaceToNearest();
        }
        else
        {
            SetCustomPlace();
        }
        InvokeRepeating(nameof(UpdateNavigation), 0.5f, 0.5f);
    }
    
    public void SetPlaceToNearest()
    {
        SetPlaceAsLocal(FindNearestPlaceId());
        _nameText.text = "Nearest attraction: " + _playerController.selectedPlace.engName;
    }
    
    private void SetCustomPlace()
    {
        _localTarget = new Vector2(_playerController.selectedPlace.latitude, _playerController.selectedPlace.longitude);
        _nameText.text = "Selected attraction: " + _playerController.selectedPlace.engName;
    }

    public void AddPoints(int points)
    {
        _playerController.AddPoints(points);
    }

    private void UpdateNavigation()
    {
        _currentPosition = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        UpdateDistance();
        UpdateArrowDirection();
    }
    private void UpdateDistance()
    {
        float distance = DistanceFromCoordinates(_currentPosition, _localTarget);
        _distanceText.text = "Distance: " + Mathf.Round(distance) + "m";
    }
    private void UpdateArrowDirection()
    {
        float angle = AngleFromCoordinate(_currentPosition, _localTarget);
        compassArrow.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Round(angle));
    }

    private void SetPlaceAsLocal(int index)
    {
        _playerController.selectedPlace = _playerController.places.Find(p => p.id == index);
        _localTarget = new Vector2(_playerController.selectedPlace.latitude, _playerController.selectedPlace.longitude);
    }

    private int FindNearestPlaceId()
    {
        float minDistance = float.MaxValue;
        Place nearest = null;
        
        foreach (var place in _playerController.places)
        {
            if(_playerController.DiscoveredPlaces.Contains(place.id)) continue;
            
            Vector2 localTarget = new Vector2(place.latitude, place.longitude);
            float localDistance = DistanceFromCoordinates(_currentPosition, localTarget);
            if (!(localDistance < minDistance)) continue;
            minDistance = localDistance;
            nearest = place;
        }
        return nearest.id;
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
