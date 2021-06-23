using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace AnimalManagement{
    public enum Type{
        Prey,
        Predator
    }

    public enum Gender{
        Female,
        Male
    }

    public class Animal : LivingBeing
    {
        public string Species;
        public Type Type;

        public State CurrentState;

        //Randomization things
        static System.Random prng;

        //General parameters
        public float BaseSpeed;
        public float SensoryDistance;

        //Lifespan things
        public float LifespanTime;
        public float StarvationTime;
        public float DehydrationTime;

        //Food things
        public string FoodSource;

        public int MaxViewDistance;

        public LivingBeing FoodTarget;
        public Coord WaterTarget;

        public float HungerPercentage = 100;
        public float ThirstPercentage = 100;

        public float HungerTime = 10;
        public float ThirstTime = 10;

        public MeterBar HungerBar;
        public MeterBar ThirstBar;

        public Coord[,] waterCoords;
        
        //AI things
        public NavMeshAgent meshAgent;

        //Movement things
        public float currentSpeed;

        //Animation things
        public Animator[] anim;

        //Mating things
        public Gender Gender;


        public Animal(string species){
            Species = species;
        }

        public void Init (Coord coord, float baseSpeed, int maxViewDistance, float hungerTime, float thirstTime, float mateUrgency) 
        {
            base.Init(coord);

            prng = new System.Random();

            anim = GetComponentsInChildren<Animator>();

            meshAgent = GetComponent<NavMeshAgent>();
            this.BaseSpeed = baseSpeed;
            meshAgent.speed = BaseSpeed;

            this.MaxViewDistance = maxViewDistance;

            CreateGender();

            CreateNeeds(hungerTime, thirstTime, mateUrgency);
            CreateNearestWater();
             
            SetState(new Roam(this));
        }

        public void CreateGender()
        {
            int gender = prng.Next(0, 2);
            
            this.Gender = gender == 0 ? Gender.Female : Gender.Male;

            GameObject maleIcon = transform.Find("UI/MaleIcon").gameObject;
            GameObject femaleIcon = transform.Find("UI/FemaleIcon").gameObject;

            if(gender == 0)
            {
                maleIcon.SetActive(false);
            }
            else
            {
                femaleIcon.SetActive(false);
            }
        }

        public void CreateNeeds(float hungerTime, float thirstTime, float mateUrgency)
        {
            this.HungerTime = hungerTime;
            this.ThirstTime = thirstTime;

            this.HungerBar = GetComponentsInChildren<MeterBar>()[0];
            this.ThirstBar = GetComponentsInChildren<MeterBar>()[1];

            HungerBar.SetMaxValue(this.HungerTime);
            ThirstBar.SetMaxValue(this.ThirstTime);
        }

        public void Update()
        {
            CurrentState.Tick();

            ManageNeeds();
        }

        public void ManageNeeds()
        {
            this.HungerPercentage = this.HungerBar.GetHunger() * 100 / this.HungerTime;
            this.ThirstPercentage = this.ThirstBar.GetHunger() * 100 / this.ThirstTime;

            if(this.HungerPercentage > 10 && (CurrentState is Roam || CurrentState is Wait))
            {
                SetState(new FindFood(this));
            }

            if(this.ThirstPercentage > 15 && (CurrentState is Roam || CurrentState is Wait))
            {
                SetState(new FindWater(this));
            }

            if(this.HungerPercentage >= 100 || this.ThirstPercentage >= 100)
            {
                SetState(new Die(this));
            }
        }

        public void SetState(State state)
        {
            if (CurrentState != null)
                CurrentState.OnStateExit();

            CurrentState = state;

            if (CurrentState != null)
                CurrentState.OnStateEnter();
        }

        IEnumerator RoamAround()
        {
            float waitLength = (float)(prng.Next(0, 2) + prng.NextDouble());
            SetState(new Wait(this));
            yield return new WaitForSeconds(waitLength);
            SetState(new Roam(this));
        }

        public void ReachedFood()
        {   
            SetState(new Eat(this));
        }

        public void ReachedWater()
        {   
            SetState(new Drink(this));
        }

        public void Feed()
        {
            this.HungerBar.DecreaseValue((this.FoodTarget.foodValue * this.HungerTime) / 100);
            SetState(new Roam(this));
            if(this.FoodTarget is Plant)
            {
                Plant temp = this.FoodTarget as Plant;
                temp.Deactivate();
            }
            else{
                this.FoodTarget.Die();
            }
        }

        public void Drink()
        {
            this.ThirstBar.DecreaseValue((100 * this.ThirstTime) / 100);
            SetState(new Roam(this));
        }

        public void ReachedDestination()
        {
            StartCoroutine(RoamAround());
        }

        private void CreateNearestWater()
        {
            List<Coord> viewOffsets = new List<Coord> ();
            int viewRadius = this.MaxViewDistance;
            int sqrViewRadius = viewRadius * viewRadius;
            for (int offsetY = -viewRadius; offsetY <= viewRadius; offsetY++) {
                for (int offsetX = -viewRadius; offsetX <= viewRadius; offsetX++) {
                    int sqrOffsetDst = offsetX * offsetX + offsetY * offsetY;
                    if ((offsetX != 0 || offsetY != 0) && sqrOffsetDst <= sqrViewRadius) {
                        viewOffsets.Add (new Coord (offsetX, offsetY));
                    }
                }
            }

            viewOffsets.Sort ((a, b) => (a.x * a.x + a.y * a.y).CompareTo (b.x * b.x + b.y * b.y));
            Coord[] viewOffsetsArr = viewOffsets.ToArray ();
            waterCoords = new Coord[EnvironmentManager.size, EnvironmentManager.size];
            for (int y = 0; y < EnvironmentManager.terrainData.size; y++) {
                for (int x = 0; x < EnvironmentManager.terrainData.size; x++) {
                    bool foundWater = false;
                    if (EnvironmentManager.walkable[x, y]) {
                        for (int i = 0; i < viewOffsets.Count; i++) {
                            int targetX = x + viewOffsetsArr[i].x;
                            int targetY = y + viewOffsetsArr[i].y;
                            if (targetX >= 0 && targetX < EnvironmentManager.size && targetY >= 0 && targetY < EnvironmentManager.size) {
                                if (EnvironmentManager.terrainData.shore[targetX, targetY]) {
                                    if (EnvironmentUtility.TileIsVisibile (x, y, targetX, targetY)) {
                                        waterCoords[x, y] = new Coord (targetX, targetY);
                                        foundWater = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!foundWater) {
                        waterCoords[x, y] = Coord.invalid;
                    }
                }
            }
        }
    }
}