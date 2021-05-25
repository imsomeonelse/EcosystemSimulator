using UnityEngine;

namespace TerrainGeneration {
    [System.Serializable]
    public class Biome
    {
        [Range (0, 1)]
        public float height;
        public Color startCol;
        public Color endCol;
        public int numSteps;
    }
}