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

        //General parameters
        public float BaseSpeed;
        public float SensoryDistance;

        //Lifespan things
        public float BabyTime;
        public float AgeTime;

        public bool IsBaby;

        public MeterBar AgeBar;

        public float LifespanRemaining;

        //Mating things
        public Gender Gender;

        public float MateUrgency;
        public float MateTime;
        public float NeedToMatePercentage = 100;

        public bool LookForMate;
        public bool IsWaitingForMate;

        public Animal CurrentMate;

        public MeterBar MateBar;

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

        public bool LookForFood;
        public bool LookForWater;
        
        //AI things
        public NavMeshAgent meshAgent;

        //Movement things
        public float currentSpeed;

        //Animation things
        public Animator[] anim;


        public Animal(string species){
            Species = species;
        }

        public void Init (
            Coord coord, float baseSpeed, int maxViewDistance, float hungerTime, float thirstTime, 
            float mateUrgency, float mateTime, float lifespan, float babyTime, bool isBaby) 
        {
            base.Init(coord);

            anim = GetComponentsInChildren<Animator>();

            meshAgent = GetComponent<NavMeshAgent>();
            this.BaseSpeed = baseSpeed;
            meshAgent.speed = BaseSpeed;

            this.MaxViewDistance = maxViewDistance;

            CreateGender();

            CreateNeeds(hungerTime, thirstTime, mateUrgency, mateTime, lifespan, babyTime, isBaby);
             
            SetState(new Roam(this));
        }

        public void CreateGender()
        {
            int gender = Random.Range(0, 2);
            
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

        public void CreateNeeds(
            float hungerTime, float thirstTime, float mateUrgency, float mateTime, float lifespan, float babyTime, bool isBaby)
        {
            this.HungerTime = hungerTime;
            this.ThirstTime = thirstTime;

            this.HungerBar = transform.Find("UI/HungerBar").GetComponent<MeterBar>();
            this.ThirstBar = transform.Find("UI/ThirstBar").GetComponent<MeterBar>();

            this.HungerBar.SetMaxValue(this.HungerTime);
            this.ThirstBar.SetMaxValue(this.ThirstTime);

            this.LookForFood = true;
            this.LookForWater = true;

            this.IsBaby = isBaby;

            if(!isBaby)
            {
                babyTime = 0;
                this.LookForMate = true;
            }
            this.IsWaitingForMate = false;

            this.AgeTime = babyTime + lifespan;
            this.AgeBar = transform.Find("UI/AgeBar").GetComponent<MeterBar>();
            this.AgeBar.SetMaxValue(this.AgeTime);

            GameObject icon = transform.Find("UI/HeartIcon").gameObject;
            icon.SetActive(false);

            this.MateUrgency = mateUrgency;
            this.MateTime = mateTime;
            this.MateBar = transform.Find("UI/MateBar").GetComponent<MeterBar>();
            this.MateBar.SetMaxValue(this.MateTime);
        }

        public void Update()
        {
            CurrentState.Tick();

            ManageNeeds();
        }

        public void ManageNeeds()
        {
            this.HungerPercentage = this.HungerBar.GetValue() * 100 / this.HungerTime;
            this.ThirstPercentage = this.ThirstBar.GetValue() * 100 / this.ThirstTime;
            this.LifespanRemaining = this.AgeTime - (this.AgeBar.GetValue() * 100 / this.AgeTime);
            this.NeedToMatePercentage = this.MateBar.GetValue() * 100 / this.MateTime;

            if(
                this.NeedToMatePercentage > this.MateUrgency && 
                CurrentState is Roam && 
                this.LookForMate && 
                !this.IsWaitingForMate && 
                this.ThirstPercentage < 50 && 
                this.HungerPercentage < 50
            )
            {
                SetState(new FindMate(this));
            }

            if(this.ThirstPercentage > this.HungerPercentage)
            {
                if(this.ThirstPercentage > 15 && CurrentState is Roam && this.LookForWater)
                {
                    SetState(new FindWater(this));
                }
                
            }
            else
            {
                if(this.HungerPercentage > 10 && CurrentState is Roam && LookForFood)
                {
                    SetState(new FindFood(this));
                }
            }

            if(this.HungerPercentage >= 100 || this.ThirstPercentage >= 100 || this.LifespanRemaining <= 0)
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
            float waitLength = Random.Range(0f, 2.0f);
            SetState(new Wait(this));
            yield return new WaitForSeconds(waitLength);
            SetState(new Roam(this));
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

        public void ReachedFood()
        {   
            SetState(new Eat(this));
        }

        IEnumerator NotFoundFood()
        {
            float waitLength = Random.Range(1f, 2.9f);
            SetState(new Roam(this));
            yield return new WaitForSeconds(waitLength);
            this.LookForFood = true;
        }

        public void ReachedWater()
        {   
            SetState(new Drink(this));
        }

        IEnumerator NotFoundWater()
        {
            float waitLength = Random.Range(1f, 2.9f);
            SetState(new Roam(this));
            yield return new WaitForSeconds(waitLength);
            this.LookForWater = true;
        }

        public void ReachedMate()
        {   
            SetState(new Mate(this));
        }

        public void ActivateHeart()
        {
            GameObject icon = transform.Find("UI/HeartIcon").gameObject;
            icon.SetActive(true);
        }

        IEnumerator NotFoundMate()
        {
            float waitLength = Random.Range(1f, 2.9f);
            SetState(new Roam(this));
            yield return new WaitForSeconds(waitLength);
            this.LookForMate = true;
        }

        public void WaitForMate()
        {
            ActivateHeart();
            this.IsWaitingForMate = true;
            SetState(new Wait(this));
        }

        public void ReachedDestination()
        {
            if(CurrentState is FindFood)
            {
                this.LookForFood = false;
                StartCoroutine(NotFoundFood());
            }
            if(CurrentState is FindWater){
                this.LookForWater = false;
                StartCoroutine(NotFoundWater());
            }
            if(CurrentState is FindMate){
                this.LookForMate = false;
                StartCoroutine(NotFoundMate());
            }
            else
            {
                StartCoroutine(RoamAround());
            }
        }
    }
}