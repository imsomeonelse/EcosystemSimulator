using UnityEngine;

namespace TerrainGeneration {
    public class TerrainData {
        public int size;
        public Vector3[, ] tileCentres;
        public bool[, ] walkable;
        public bool[, ] shore;

        public TerrainData (int size) {
            this.size = size;
            tileCentres = new Vector3[size, size];
            walkable = new bool[size, size];
            shore = new bool[size, size];
        }
    }
}