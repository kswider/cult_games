using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ResourcesObjects;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    //Save
    public int Score { get; set; }
    public List<int> DiscoveredPlaces { get; set; }
    public List<Save.PlaceBlock> BlockedPlaces { get; set; }
    public int CurrentPlayedPlaceId { get; set; }
    
    //Database
    public List<Place> Places { get; set; }
    
    //PlayerSettings
    public Settings Settings { get; private set; }

    private void Start()
    {
        ReadPlaces();
        LoadGame();
        
        Settings = new Settings();
        Place selected = Places.Find(p => p.id == Settings.SelectedPlaceId);
        if (selected != null)
        {
            Settings.SelectedPlace = selected;
        }
    }

    public void SaveGame()
    {
        ClearPlaceBlocks();
        Save save = CreateSaveGameObject();
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();
        
        Debug.Log("Game Saved");
    }
    
    public void DeleteSave()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            File.Delete(Application.persistentDataPath + "/gamesave.save");
        }
        Score = 0;
        DiscoveredPlaces = new List<int>();
        BlockedPlaces = new List<Save.PlaceBlock>();
        Debug.Log("Game Data Deleted");
    }

    public void AddPoints(int points)
    {
        Score += points;
    }

    private void ReadPlaces()
    {
        Place[] regular = Resources.LoadAll<Place>("Places");
        Place[] curiosities = Resources.LoadAll<Place>("Curiosities");
        Place[] legends = Resources.LoadAll<Place>("Legends");

        Places = regular.Concat(curiosities).Concat(legends).ToList();
    }
    
    private Save CreateSaveGameObject()
    {
        return new Save
        {
            generalScore = Score
            , discoveredPlaces = DiscoveredPlaces
            , blockedPlaces = BlockedPlaces
        };
    }

    private void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            Score = save.generalScore;
            DiscoveredPlaces = save.discoveredPlaces;
            BlockedPlaces = save.blockedPlaces;
            
            Debug.Log("Game Loaded");
            ClearPlaceBlocks();
        }
        else
        {
            Score = 0;
            DiscoveredPlaces = new List<int>();
            BlockedPlaces = new List<Save.PlaceBlock>();
            Debug.Log("No game saved!");
        }
    }

    private void ClearPlaceBlocks()
    {
        var copy = BlockedPlaces.ToList();
        foreach (var block in copy.Where(block => block.blockUntil.CompareTo(DateTime.Now) <= 0))
        {
            BlockedPlaces.Remove(block);
        }
    }
}
