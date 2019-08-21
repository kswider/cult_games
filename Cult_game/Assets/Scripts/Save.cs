using System.Collections.Generic;

[System.Serializable]
public class Save
{
    [System.Serializable]
    public struct PlaceBlock
    {
        int placeId;
        int blockUntil;
    }

    public int generalScore = 0;
    public List<int> discoveredPlaces = new List<int>();
    public List<PlaceBlock> blockedPlaces = new List<PlaceBlock>();

}
