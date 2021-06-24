using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimalManagement;
using TerrainGeneration;

public class EnvironmentManager : MonoBehaviour
{
    const int mapRegionSize = 10;

    public int seed;

    public static List<Coord> walkableCoords;
    public static List<Coord> waterCoords;
    public static bool[, ] walkable;
    public static Vector3[, ] tileCentres;
    public static int size;

    static System.Random prng;
    public static TerrainGeneration.TerrainData terrainData;

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
        waterCoords = terrainData.waterCoords;

        walkableCoords = GetComponent<FloraSpawner>().SpawnFlora(seed, terrainData, walkable, tileCentres);

        GetComponent<FoodSpawner>().SpawnInitialPopulation(seed, walkableCoords, size, mapRegionSize);

        GetComponent<AnimalManager>().SpawnInitialPopulation(walkableCoords);

    }

    /// Get random neighbour tile, weighted towards those in similar direction as currently facing
    public static Coord GetRandomWalkable() 
    {
        return walkableCoords[Random.Range(0, walkableCoords.Count)];
    }

    public static Coord GetClosestWater(Vector3 currPos, float maxViewDistance)
    {
        Coord closestWater = new Coord();
        float lowestDist = maxViewDistance;

        foreach(Coord water in waterCoords)
        {
            Vector3 waterPos = tileCentres[water.x, water.y];
            float dist = Vector3.Distance(currPos, waterPos);

            if (dist < lowestDist && dist < maxViewDistance)
            {
                lowestDist = dist;
                closestWater = water;
            }
        }

        return closestWater;
    }
}
