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
        // General attributes
        public string Species;
        public Type Type;

        // General parameters
        public float BaseSpeed;
        public float SensoryDistance;

        // Lifespan things
        public float BabyTime;
        public float AgeTime;

        public bool IsBaby;

        public MeterBar AgeBar;

        public float LifespanRemaining;

        public bool IsDying;

        // Mating things
        public Gender Gender;

        public float MateUrgency;
        public float MateTime;
        public float NeedToMatePercentage = 100;

        public bool LookForMate;
        public bool IsWaitingForMate;
        public bool FoundMate;

        public Animal CurrentMate;

        public MeterBar MateBar;

        public int BabyAverage;

        // Food things
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

        public Animal CurrentPredator;
        public bool IsWaitingToBeEaten;
        
        // AI things
        public NavMeshAgent meshAgent;
        public State CurrentState;
        public StateText StateText;

        // Movement things
        public float currentSpeed;

        // Animation things
        public Animator[] anim;

        //Others
        public Texture2D hoverCursor;
        public Texture2D autoCursor;
        public AudioSource audioSource;

        public void Init (
            Coord coord, string species, float baseSpeed, int maxViewDistance, float hungerTime, float thirstTime, 
            float mateUrgency, float mateTime, float lifespan, float babyTime, bool isBaby, int babyAverage) 
        {
            base.Init(coord);

            CreateValues(species, baseSpeed, maxViewDistance, hungerTime, thirstTime, 
            mateUrgency, mateTime, lifespan, babyTime, isBaby, babyAverage);
        }

        public void Init (
            Vector3 position, string species, float baseSpeed, int maxViewDistance, float hungerTime, float thirstTime, 
            float mateUrgency, float mateTime, float lifespan, float babyTime, bool isBaby, int babyAverage) 
        {
            transform.position = position;

            CreateValues(species, baseSpeed, maxViewDistance, hungerTime, thirstTime, 
            mateUrgency, mateTime, lifespan, babyTime, isBaby, babyAverage);
        }

        public void CreateValues (string species, float baseSpeed, int maxViewDistance, float hungerTime, float thirstTime, 
            float mateUrgency, float mateTime, float lifespan, float babyTime, bool isBaby, int babyAverage)
        {          
            hoverCursor = Camera.main.transform.parent.gameObject.GetComponent<FollowCamera>().hoverCursor;
            autoCursor = Camera.main.transform.parent.gameObject.GetComponent<FollowCamera>().autoCursor;
            audioSource = GetComponent<AudioSource>();

            IsDying = false;

            if(this is Predator)
            {
                this.Type = Type.Predator;
            }
            else{
                this.Type = Type.Prey;
            }
            this.Species = species;
            this.IsBaby = isBaby;

            anim = GetComponentsInChildren<Animator>();

            meshAgent = GetComponent<NavMeshAgent>();
            this.BaseSpeed = baseSpeed;
            meshAgent.speed = BaseSpeed;

            this.MaxViewDistance = maxViewDistance;

            CreateGender(babyAverage);

            CreateNeeds(hungerTime, thirstTime, mateUrgency, mateTime, lifespan, isBaby);

            if(!this.IsBaby)
            {
                this.BabyTime = 0;
                this.LookForMate = true;
            }
            if(this.IsBaby)
            {
                this.BabyTime = babyTime;
                MakeIntoBaby();
            }
                          
            StateText = transform.Find("UI/StateText").GetComponent<StateText>();
            SetState(new Roam(this));
        }

        public void MakeIntoBaby()
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            this.BaseSpeed = this.BaseSpeed - (this.BaseSpeed * .20f);
            this.MaxViewDistance = (int) this.MaxViewDistance - (int) (this.MaxViewDistance * .10f);
            this.ThirstTime = this.ThirstTime - (this.ThirstTime * .10f);
            this.HungerTime = this.HungerTime - (this.HungerTime * .10f);
            StartCoroutine(GrowUp());
        }

       IEnumerator GrowUp()
        {
            yield return new WaitForSeconds(this.BabyTime);

            this.LookForMate = true;
            this.IsBaby = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
            this.BaseSpeed = (this.BaseSpeed * 100) / 90;
            this.MaxViewDistance = (int) (this.MaxViewDistance * 100) / 90;
            this.ThirstTime = (this.ThirstTime * 100) / 90;
            this.HungerTime = (this.ThirstTime * 100) / 90;
            this.MateBar.SetMaxValue(this.MateTime);
        }

        public void CreateGender(int babyAverage)
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

            this.BabyAverage = babyAverage;
        }

        public void CreateNeeds(
            float hungerTime, float thirstTime, float mateUrgency, float mateTime, float lifespan, bool isBaby)
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
            this.IsWaitingForMate = false;

            this.AgeTime = this.BabyTime + lifespan;
            this.AgeBar = transform.Find("UI/AgeBar").GetComponent<MeterBar>();
            this.AgeBar.SetMaxValue(this.AgeTime);

            GameObject icon = transform.Find("UI/HeartIcon").gameObject;
            icon.SetActive(false);

            this.MateUrgency = mateUrgency;
            this.MateTime = mateTime;
            this.MateBar = transform.Find("UI/MateBar").GetComponent<MeterBar>();
            if(!this.IsBaby)
            {
                this.MateBar.SetMaxValue(this.MateTime);
            }
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
            if(!this.IsDying)
            {
                if (CurrentState != null)
                    CurrentState.OnStateExit();

                CurrentState = state;

                if (CurrentState != null)
                    CurrentState.OnStateEnter();
            }
        }

        IEnumerator RoamAround()
        {
            float waitLength = Random.Range(0f, 1.0f);
            SetState(new Wait(this));
            yield return new WaitForSeconds(waitLength);
            SetState(new Roam(this));
        }

        public void Feed()
        {
            this.HungerBar.DecreaseValue((this.FoodTarget.foodValue * this.HungerTime) / 100);
            if(this.FoodTarget is Plant)
            {
                Plant temp = this.FoodTarget as Plant;
                temp.Deactivate();
            }
            if(this.FoodTarget is Animal){
                Animal animal = this.FoodTarget as Animal;
                animal.DieFromEat();
            }
            SetState(new Roam(this));
        }

        public void ReachedFood()
        {   
            SetState(new Eat(this));
            if(this is Predator)
            {
                Animal animalFood = FoodTarget as Animal;
                animalFood.WaitToBeEaten();
            }
        }

        IEnumerator NotFoundFood()
        {
            float waitLength = Random.Range(0f, 1.0f);
            SetState(new Roam(this));
            yield return new WaitForSeconds(waitLength);
            this.LookForFood = true;
        }

        public void Drink()
        {
            this.ThirstBar.DecreaseValue((100 * this.ThirstTime) / 100);
            SetState(new Roam(this));
        }

        public void ReachedWater()
        {   
            SetState(new Drink(this));
        }

        IEnumerator NotFoundWater()
        {
            float waitLength = Random.Range(0f, 1.0f);
            SetState(new Roam(this));
            yield return new WaitForSeconds(waitLength);
            this.LookForWater = true;
        }

        public void FinishMating()
        {
            this.MateBar.DecreaseValue((100 * this.MateTime) / 100);
            this.CurrentMate = null;
            this.LookForMate = true;
            this.FoundMate = false;
            this.IsWaitingForMate = false;
            SetState(new Roam(this));
        }

        public void HaveBaby()
        {
            AnimalManager aM = Object.FindObjectOfType<AnimalManager>();
            int babyNum = (this.BabyAverage + this.CurrentMate.BabyAverage)/2;
            aM.CreateNew(this.Type, this.Species, this.coord, babyNum, transform.position);
        }

        public void ReachedMate()
        {   
            StartMating();
            this.CurrentMate.StartMating();
        }

        public void StartMating()
        {
            this.LookForMate = false;
            SetState(new Mate(this));
        }

        public void ActivateHeart()
        {
            GameObject icon = transform.Find("UI/HeartIcon").gameObject;
            icon.SetActive(true);
            this.FoundMate = true;
        }

        public void DeactivateHeart()
        {
            GameObject icon = transform.Find("UI/HeartIcon").gameObject;
            icon.SetActive(false);
        }

        IEnumerator NotFoundMate()
        {
            DeactivateHeart();
            float waitLength = Random.Range(0f, 1.0f);
            SetState(new Roam(this));
            yield return new WaitForSeconds(waitLength);
            this.LookForMate = true;
        }

        public void WaitForMate(Animal mate)
        {
            ActivateHeart();
            this.IsWaitingForMate = true;
            this.LookForMate = false;
            this.CurrentMate = mate;
            SetState(new Wait(this));
        }

        public void BeChased(Animal predator)
        {
            this.CurrentPredator = predator;
            StartCoroutine(RunAway());
        }

        IEnumerator RunAway()
        {
            yield return new WaitForSeconds(2);
            SetState(new RunAway(this));
        }

        public void StopRunningAway()
        {
            this.CurrentPredator = null;
            SetState(new Roam(this));
        }

        public void WaitToBeEaten()
        {
            this.IsWaitingToBeEaten = true;
            SetState(new BeEaten(this));
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

        public void DieFromEat()
        {
            SetState(new Die(this));
        }

        void OnMouseEnter()
        {
           Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
        }
       
        void OnMouseExit()
        {
           Cursor.SetCursor(autoCursor, Vector2.zero, CursorMode.Auto);
        }

        void OnMouseDown() 
        {
            audioSource.Play();
            for(int i = 0; i < anim.Length; i++)
            {
                anim[i].SetTrigger("clicked");
            }    
        }
    }
}