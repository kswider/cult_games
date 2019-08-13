using UnityEngine;

namespace Resources
{
    [CreateAssetMenu]
    public class Place : ScriptableObject
    {
        public int index;
        public string type;
        public float latitude;
        public float longitude;
        public float score_value;
        public string description;
        public int curiosity_quiz_index;
        public int legend_puzzle_index;
    }
}
