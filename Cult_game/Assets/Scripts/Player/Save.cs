using System;
using System.Collections.Generic;

[Serializable]
public class Save
{
    [Serializable]
    public struct PlaceBlock
    {
        public int placeId;
        public DateTime blockUntil;
    }

    public int generalScore;
    public List<int> discoveredPlaces = new List<int>();
    public List<PlaceBlock> blockedPlaces = new List<PlaceBlock>();

}
