using System.Collections.Generic;
using ResourcesObjects;
using UnityEngine;
using UnityEngine.UI;

public class PlacesTableController : MonoBehaviour
{
    public GameObject entryPrefab;
    public Transform tableContent;
    public Text type;

    private PlayerController _playerController;
    
    private List<GameObject> _shownPlaces = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        _playerController = Utilities.FindPlayer();

        SetTypeBarToLookedType();
        UpdateTable();
    }

    public void UpdateTable()
    {
        ClearTable();

        List<Place> placesToShow = _playerController.places.FindAll(p => p.type.Equals(_playerController.Settings.LookedType));
        
        Vector2 currentPosition = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        
        foreach (var place in placesToShow)
        {
            Vector2 placePosition = new Vector2(place.latitude, place.longitude);
            
            GameObject newPlace = Instantiate(entryPrefab, tableContent);
            newPlace.transform.Find("Name").GetComponent<Text>().text = place.engName;
            newPlace.transform.Find("Distance").GetComponent<Text>().text = 
                ((int)Geometry.DistanceFromCoordinates(currentPosition, placePosition)) + "m";
            newPlace.transform.Find("Name/IsVisited").gameObject.SetActive(
                _playerController.DiscoveredPlaces.Contains(place.id)
            );
            newPlace.transform.Find("BTN_FOLLOW").GetComponent<Button>().onClick.AddListener(delegate
            {
                _playerController.Settings.SelectedPlace = _playerController.places.Find(p => p.engName == place.engName);
                _playerController.Settings.SaveSettings();
            });
            _shownPlaces.Add(newPlace);
        }
    }

    private void ClearTable()
    {
        foreach (var place in _shownPlaces)
        {
            Destroy(place);
        }
    }

    public void SetLookedType(string newType)
    {
        _playerController.Settings.LookedType = newType;
        _playerController.Settings.SaveSettings();
        SetTypeBarToLookedType();
    }

    private void SetTypeBarToLookedType()
    {
        switch (_playerController.Settings.LookedType)
        {
            case "PLACE":
                type.text = "Places";
                break;
            case "CURIOSITY":
                type.text = "Curiosities";
                break;
            case "LEGEND":
                type.text = "Legends";
                break;
            default:
                Debug.Log("Not recognized type");
                break;
        }
    }
}
