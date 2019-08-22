using System.Collections.Generic;
using Resources;
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
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _playerController = player.GetComponent<PlayerController>();
        }

        SetTypeBarToLookedType();
        UpdateTable();
    }

    public void UpdateTable()
    {
        ClearTable();

        List<Place> placesToShow = _playerController.places.FindAll(p => p.type.Equals(_playerController.LookedType));

        foreach (var place in placesToShow)
        {
            GameObject newPlace = Instantiate(entryPrefab, tableContent);
            newPlace.transform.Find("Name").GetComponent<Text>().text = place.engName;
            newPlace.transform.Find("Distance").GetComponent<Text>().text = "0m";
            newPlace.transform.Find("BTN_FOLLOW").GetComponent<Button>().onClick.AddListener(delegate
            {
                _playerController.selectedPlace = _playerController.places.Find(p => p.engName == place.engName);
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
        _playerController.LookedType = newType;
        SetTypeBarToLookedType();
    }

    private void SetTypeBarToLookedType()
    {
        switch (_playerController.LookedType)
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
