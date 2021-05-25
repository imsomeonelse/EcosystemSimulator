using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TerrainGeneration;

public class FloraSpawner : MonoBehaviour
{
    [Header("Trees")]
    [SerializeField]
    private GameObject[] _TreePrefabs;
    [SerializeField]
    [Range (0, 1)]
    private float _TreeProbability;
    [Header("Other Fauna")]
    [SerializeField]
    private GameObject[] _FaunaPrefabs;
    [SerializeField]
    [Range (0, 1)]
    private float _FaunaProbability;

    public List<Coord> SpawnFlora(int seed, TerrainGeneration.TerrainData terrainData, bool[, ] walkable, Vector3[, ] tileCentres)
    {
        float maxRot = 4;
        float maxScaleDeviation = .05f;
        var spawnPrng = new System.Random (seed);
        var prng = new System.Random(seed);

        Transform environmentHolder = new GameObject ("EnvironmentAssets").transform;
        List<Coord> walkableCoords = new List<Coord> ();

        for (int y = 0; y < terrainData.size; y++) {
            for (int x = 0; x < terrainData.size; x++) {
                if (walkable[x, y]) {
                    if(prng.NextDouble() * 100 < _TreeProbability) 
                    {

                        // Randomize rot/scale
                        float rotX = Mathf.Lerp (-maxRot, maxRot, (float) spawnPrng.NextDouble ());
                        float rotZ = Mathf.Lerp (-maxRot, maxRot, (float) spawnPrng.NextDouble ());
                        float rotY = (float) spawnPrng.NextDouble () * 360f;
                        Quaternion rot = Quaternion.Euler (rotX, rotY, rotZ);

                        float scale = 0.6f + ((float) spawnPrng.NextDouble () * 2 - 1) * maxScaleDeviation;

                        int treeType = prng.Next(0, _TreePrefabs.Length);

                        // Spawn
                        GameObject tree = Instantiate (_TreePrefabs[treeType], tileCentres[x, y], rot);
                        tree.transform.parent = environmentHolder;
                        tree.transform.localScale = Vector3.one * scale;
                        var aiObstacle = tree.AddComponent<NavMeshObstacle>();
                        aiObstacle.carving = true;

                        walkable[x, y] = false;

                    }else {

                        walkableCoords.Add (new Coord (x, y));
                        
                    }

                    if(prng.NextDouble() * 100 < _FaunaProbability)
                    {

                        // Randomize rot/scale
                        float rotX = Mathf.Lerp (-maxRot, maxRot, (float) spawnPrng.NextDouble ());
                        float rotZ = Mathf.Lerp (-maxRot, maxRot, (float) spawnPrng.NextDouble ());
                        float rotY = (float) spawnPrng.NextDouble () * 360f;
                        Quaternion rot = Quaternion.Euler (rotX, rotY, rotZ);

                        float scale = 0.6f + ((float) spawnPrng.NextDouble () * 2 - 1) * maxScaleDeviation;

                        int assetType = prng.Next(0, _FaunaPrefabs.Length);

                        // Spawn
                        GameObject fauna = Instantiate (_FaunaPrefabs[assetType], tileCentres[x, y], rot);
                        fauna.transform.parent = environmentHolder;
                        fauna.transform.localScale = Vector3.one * scale;
                        var aiObstacle = fauna.AddComponent<NavMeshObstacle>();
                        aiObstacle.carving = true;

                        walkable[x, y] = false;

                    }else {

                        walkableCoords.Add (new Coord (x, y));
                        
                    }

                }
            }
        }

        return walkableCoords;
    }
}
