using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class FoodSpawner : MonoBehaviour
    {
        public GameObject[] FoodPlants;
        public int Quantity;

        public static Map plantFoodCoords;

        public float respawnTime;
        public GameObject UI;

        float maxScaleDeviation = .01f;

        public void SpawnInitialPopulation(int seed, List<Coord> walkableCoords, int size, int mapRegionSize)
        {
            plantFoodCoords = new Map (size, mapRegionSize);

            var spawnPrng = new System.Random (seed);
            var spawnCoords = new List<Coord>(walkableCoords);

            Transform foodHolder = new GameObject ("FoodAssets").transform;

            for(int i = 0; i < Quantity; i++)
            {
                if (spawnCoords.Count == 0) {
                    Debug.Log ("Ran out of empty tiles to spawn initial population");
                }else{
                    int spawnCoordIndex = spawnPrng.Next (0, spawnCoords.Count);
                    Coord coord = spawnCoords[spawnCoordIndex];
                    spawnCoords.RemoveAt(spawnCoordIndex);

                    float scale = 0.35f + ((float) spawnPrng.NextDouble () * 2 - 1) * maxScaleDeviation;
                    int foodPlantType = spawnPrng.Next(0, FoodPlants.Length);

                    GameObject newFoodPlant =  Instantiate(FoodPlants[foodPlantType]);
                    newFoodPlant.transform.localScale = Vector3.one * scale;
                    newFoodPlant.transform.parent = foodHolder;

                    GameObject ui = Instantiate(UI);
                    ui.transform.SetParent(newFoodPlant.transform);

                    Plant plantScript = newFoodPlant.AddComponent<Plant>();
                    plantScript.Init(coord, respawnTime);

                    plantFoodCoords.Add(plantScript, coord);
                }
            }

        }
    }
}