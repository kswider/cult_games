﻿using UnityEngine;

namespace Resources
{
    [CreateAssetMenu]
    public class Place : ScriptableObject
    {
        public int id;
        public string engName;
        public string plName;
        public string type;
        public float latitude;
        public float longitude;
        public float scoreValue;
        public string description;
        public int curiosityQuizIndex;
        public int legendPuzzleIndex;
    }
}
