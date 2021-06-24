using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    [System.Serializable]
    public struct PreyManager{
        [SerializeField]
        private GameObject _Prefab;
        public GameObject Prefab{
            get
            {
                return _Prefab;
            }

            set
            {
                _Prefab = value;
            }
        }

        [SerializeField]
        private string _Species;
        public string Species{
            get
            {
                return _Species;
            }

            set
            {
                _Species = value;
            }
        }

        [SerializeField]
        private int _Quantity;
        public int Quantity{
            get
            {
                return _Quantity;
            }

            set
            {
                _Quantity = value;
            }
        }

        public float BabyTime;
        public float AdultTime;
        public float BaseSpeed;
        public int MaxViewDistance;
        public float HungerTime;
        public float ThirstTime;
        public float MateTime;
        public float MateUrgency;
        public int NumBabyAverage;
    }

    [System.Serializable]
    public struct PredatorManager{
        [SerializeField]
        private GameObject _Prefab;
        public GameObject Prefab{
            get
            {
                return _Prefab;
            }

            set
            {
                _Prefab = value;
            }
        }

        [SerializeField]
        private string _Species;
        public string Species{
            get
            {
                return _Species;
            }

            set
            {
                _Species = value;
            }
        }

        [SerializeField]
        private int _Quantity;
        public int Quantity{
            get
            {
                return _Quantity;
            }

            set
            {
                _Quantity = value;
            }
        }

        public float BabyTime;
        public float AdultTime;
        public float BaseSpeed;
        public int MaxViewDistance;
        public float HungerTime;
        public float ThirstTime;
        public float MateTime;
        public float MateUrgency;
        public int NumBabyAverage;
    }

    [System.Serializable]
    public class AnimalManager : MonoBehaviour
    {
        [Header("Prey")]
        [SerializeField]
        private PreyManager[] _Prey;

        [Header("Predator")]
        [SerializeField]
        private PredatorManager[] _Predator;

        List<Animal> _AnimalList = new List<Animal>();

        public Counter PredatorCounter;
        public Counter PreyCounter;

        private int NumPredators = 0;
        private int NumPrey = 0;

        public void SpawnInitialPopulation(List<Coord> walkableCoords)
        {
            var spawnCoords = new List<Coord>(walkableCoords);

            foreach (PreyManager pMan in _Prey)
            {
                this.NumPrey += pMan.Quantity;
                int numToSpawn = pMan.Quantity;
                for(int i = 0; i < numToSpawn; i++)
                {
                    if (spawnCoords.Count == 0) {
                        Debug.Log ("Ran out of empty tiles to spawn initial population");
                    }else{
                        int spawnCoordIndex = Random.Range(0, spawnCoords.Count);
                        Coord coord = spawnCoords[spawnCoordIndex];
                        spawnCoords.RemoveAt(spawnCoordIndex);

                        GameObject newPrey =  Instantiate(pMan.Prefab);
                        newPrey.name = pMan.Species + i.ToString();
                        Prey preyScript = newPrey.AddComponent<Prey>();
                        preyScript.Init(
                            coord, pMan.Species, pMan.BaseSpeed, pMan.MaxViewDistance, pMan.HungerTime, pMan.ThirstTime, 
                            pMan.MateUrgency, pMan.MateTime, pMan.AdultTime, pMan.BabyTime, false, pMan.NumBabyAverage);

                        _AnimalList.Add(preyScript);
                    }
                }
            }

            foreach (PredatorManager pMan in _Predator)
            {
                this.NumPredators += pMan.Quantity;
                int numToSpawn = pMan.Quantity;
                for(int i = 0; i < numToSpawn; i++)
                {
                    if (spawnCoords.Count == 0) {
                        Debug.Log ("Ran out of empty tiles to spawn initial population");
                    }else{
                        int spawnCoordIndex = Random.Range(0, spawnCoords.Count);
                        Coord coord = spawnCoords[spawnCoordIndex];
                        spawnCoords.RemoveAt(spawnCoordIndex);

                        GameObject newPredator =  Instantiate(pMan.Prefab);
                        newPredator.name = pMan.Species + i.ToString();
                        Predator predatorScript = newPredator.AddComponent<Predator>();
                        predatorScript.Init(
                            coord, pMan.Species, pMan.BaseSpeed, pMan.MaxViewDistance, pMan.HungerTime, pMan.ThirstTime, 
                            pMan.MateUrgency, pMan.MateTime, pMan.AdultTime, pMan.BabyTime, false, pMan.NumBabyAverage);
                            
                        _AnimalList.Add(predatorScript);
                    }
                }
            }

            UpdateUI();
        }

        public void CreateNew(Type type, string species, Coord coord, int quantity)
        {
            if(type == Type.Prey)
            {
                foreach(PreyManager pMan in _Prey)
                {
                    if(pMan.Species == species)
                    {
                        for(int i = 0; i < quantity; i++)
                        {
                            GameObject newPrey =  Instantiate(pMan.Prefab);
                            Prey preyScript = newPrey.AddComponent<Prey>();
                            preyScript.Init(
                                coord, pMan.Species, pMan.BaseSpeed, pMan.MaxViewDistance, pMan.HungerTime, pMan.ThirstTime, 
                                pMan.MateUrgency, pMan.MateTime, pMan.AdultTime, pMan.BabyTime, true, pMan.NumBabyAverage);

                            AddPrey();
                        }
                    }
                }
            }
            else
            {
                foreach(PredatorManager pMan in _Predator)
                {
                    if(pMan.Species == species)
                    {
                        for(int i = 0; i < quantity; i++)
                        {
                            GameObject newPredator =  Instantiate(pMan.Prefab);
                            Predator predatorScript = newPredator.AddComponent<Predator>();
                            predatorScript.Init(
                                coord, pMan.Species, pMan.BaseSpeed, pMan.MaxViewDistance, pMan.HungerTime, pMan.ThirstTime, 
                                pMan.MateUrgency, pMan.MateTime, pMan.AdultTime, pMan.BabyTime, true, pMan.NumBabyAverage);

                            AddPredator();
                        }
                    }
                }
            }
        }

        public void UpdateUI()
        {
            PredatorCounter.UpdateValue(this.NumPredators.ToString());
            PreyCounter.UpdateValue(this.NumPrey.ToString());
        }

        public void AddPredator()
        {
            this.NumPredators += 1; 
            UpdateUI();
        }

        public void AddPrey()
        {
            this.NumPrey += 1;  
            UpdateUI();
        }

        public void RemovePredator()
        {
            this.NumPredators -= 1; 
            UpdateUI();
        }

        public void RemovePrey()
        {
            this.NumPrey -= 1;  
            UpdateUI();
        }
    }
}
