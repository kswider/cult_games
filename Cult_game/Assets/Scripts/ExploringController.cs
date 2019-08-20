using System;
using System.Collections.Generic;
using Resources;
using UnityEngine;
using UnityEngine.UI;

public class ExploringController : MonoBehaviour
{
    public GameObject compassArrow;
    public GameObject selectedObjectName;
    public GameObject selectedObjectDistance;

    public List<Place> places;
    private Place _selectedPlace;
    
    private Text _nameText;
    private Text _distanceText;

    private Vector2 _localTarget;
    
    private Vector2 _currentPosition;

    private float _angle;
    // Start is called before the first frame update
    void Start()
    {
        _nameText = selectedObjectName.GetComponent<Text>();
        _distanceText = selectedObjectDistance.GetComponent<Text>();
        
        _currentPosition = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        SetPlaceToNearest();
        
        InvokeRepeating(nameof(UpdateArrowDirection), 0.5f, 0.5f);
    }
    
    void UpdateArrowDirection()
    {
        _currentPosition = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        _distanceText.text = "Distance: " + Mathf.Round(DistanceFromCoordinates(_currentPosition, _localTarget)) + "m";
        _angle = AngleFromCoordinate(_currentPosition, _localTarget);
        compassArrow.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Round(_angle));
    }

    private float DistanceFromCoordinates(Vector2 coord1, Vector2 coord2)
    {
        //sauce: https://stackoverflow.com/questions/639695/how-to-convert-latitude-or-longitude-to-meters
        double R = 6378.137; // Radius of earth in KM
        double dLat = coord2.x * Math.PI / 180 - coord1.x * Math.PI / 180;
        double dLon = coord2.y * Math.PI / 180 - coord1.y * Math.PI / 180;
        double a = Math.Sin(dLat/2) * Math.Sin(dLat/2) +
                Math.Cos(coord1.x * Math.PI / 180) * Math.Cos(coord2.x * Math.PI / 180) *
                Math.Sin(dLon/2) * Math.Sin(dLon/2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
        double d = R * c;
        return (float)d * 1000; // meters
    }
    
    private float AngleFromCoordinate(Vector2 coord1, Vector2 coord2)
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

    public void SetCustomPlace(int index)
    {
        SetPlaceAsLocal(index);
        _nameText.text = "Selected attraction: " + _selectedPlace.engName;
    }
    
    public void SetPlaceToNearest()
    {
        SetPlaceAsLocal(FindNearestPlaceIndex());
        _nameText.text = "Nearest attraction: " + _selectedPlace.engName;
    }

    private void SetPlaceAsLocal(int index)
    {
        _selectedPlace = places.Find(p => p.id == index);
        _localTarget = new Vector2(_selectedPlace.latitude, _selectedPlace.longitude);
    }

    private int FindNearestPlaceIndex()
    {
        float minDistance = float.MaxValue;
        Place nearest = null;
        
        foreach (var place in places)
        {
            Vector2 localTarget = new Vector2(place.latitude, place.longitude);
            float localDistance = DistanceFromCoordinates(_currentPosition, localTarget);
            if (localDistance < minDistance)
            {
                nearest = place;
            }
        }
        return nearest.id;
    }
}
