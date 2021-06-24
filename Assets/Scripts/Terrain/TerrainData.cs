using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TerrainGeneration {
    public class TerrainData {
        const int mapRegionSize = 10;
        
        public int size;
        public Vector3[, ] tileCentres;
        public bool[, ] walkable;
        public bool[, ] shore;
        public List<Coord> waterCoords;

        public TerrainData (int size) {
            this.size = size;
            tileCentres = new Vector3[size, size];
            walkable = new bool[size, size];
            shore = new bool[size, size];
            waterCoords = new List<Coord> ();
        }
    }
}