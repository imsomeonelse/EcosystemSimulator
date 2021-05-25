using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimalManagement;
using TerrainGeneration;

public class EnvironmentManager : MonoBehaviour
{
    public int seed;

    public static List<Coord> walkableCoords;
    public static bool[, ] walkable;
    public static Vector3[, ] tileCentres;
    static int size;

    static System.Random prng;
    TerrainGeneration.TerrainData terrainData;

    static Coord[,][] walkableNeighboursMap;

    void Start()
    {
        prng = new System.Random(seed);

        Init();
    }

    private void Init(){
        var terrainGenerator = FindObjectOfType<TerrainGenerator> ();
        terrainData = terrainGenerator.Generate ();

        tileCentres = terrainData.tileCentres;
        walkable = terrainData.walkable;
        size = terrainData.size;

        walkableCoords = GetComponent<FloraSpawner>().SpawnFlora(seed, terrainData, walkable, tileCentres);

        GetComponent<FoodSpawner>().SpawnInitialPopulation(seed, walkableCoords);

        GetComponent<AnimalManager>().SpawnInitialPopulation(seed, walkableCoords);

    }

    /// Get random neighbour tile, weighted towards those in similar direction as currently facing
    public static Coord GetRandomWalkable() 
    {
        return walkableCoords[prng.Next(walkableCoords.Count)];
    }
}
