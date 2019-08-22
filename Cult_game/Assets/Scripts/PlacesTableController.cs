using System;
using System.Collections.Generic;
using Resources;
using UnityEngine;
using UnityEngine.UI;

public class PlacesTableController : MonoBehaviour
{
    public GameObject entryPrefab;
    public Transform tableContent;
    
    private PlayerController _playerController;
    private Text _type;

    private List<GameObject> _shownPlaces = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _playerController = player.GetComponent<PlayerController>();
        }
        GameObject type = GameObject.Find("Type");
        if (type != null)
        {
            _type = type.GetComponent<Text>();
        }

        _type.text = _playerController.LookedType;
        UpdateTable();
    }

    public void UpdateTable()
    {
        foreach (var oldPlace in _shownPlaces)
        {
            Destroy(oldPlace);
        }
        
        List<Place> placesToShow = _playerController.places.FindAll(p => p.type.Equals(_playerController.LookedType));

        foreach (var place in placesToShow)
        {
            GameObject newPlace = Instantiate(entryPrefab, tableContent);
            newPlace.transform.Find("Name").GetComponent<Text>().text = place.engName;
            newPlace.transform.Find("Distance").GetComponent<Text>().text = "0m";
            newPlace.transform.Find("BTN_FOLLOW").GetComponent<Button>().onClick.AddListener(delegate
            {
                _playerController.selectedPlace = _playerController.places.Find(p => p.engName == place.engName); 
                Debug.Log("Target set to " + place.engName);
            });
            _shownPlaces.Add(newPlace);
        }
    }

    public void setLookedType(string type)
    {
        _playerController.LookedType = type;
        _type.text = type;
    }
}
