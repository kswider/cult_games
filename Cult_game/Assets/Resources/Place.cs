using UnityEngine;

namespace ResourcesObjects
{
    [CreateAssetMenu]
    public class Place : ScriptableObject
    {
        public int id;
        public string engName;
        public string plName;
        public string imagePath;
        public string type;
        public float latitude;
        public float longitude;
        public int scoreValue;
        public string description;
        public string gameType;
        public string gameDifficulty;
    }
}
