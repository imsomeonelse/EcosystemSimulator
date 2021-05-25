using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AnimalManagement{
    enum Type{
        Prey,
        Predator
    }

    public class Animal : LivingBeing
    {
        public string Species;

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

        public LivingBeing FoodTarget;
        public Coord WaterTarget;

        public float Hunger = 0;
        public float Thirst = 0;
        
        //AI things
        public NavMeshAgent meshAgent;

        //Movement things
        public float currentSpeed;

        //Animation things
        public Animator[] anim;


        public Animal(string species){
            Species = species;
        }

        public void Init (Coord coord, float baseSpeed) {
            base.Init(coord);
            this.BaseSpeed = baseSpeed;

            prng = new System.Random(0);

            anim = GetComponentsInChildren<Animator>();

            meshAgent = GetComponent<NavMeshAgent>();
            meshAgent.speed = BaseSpeed;
             
            SetState(new Roam(this));
        }

        public void Update(){
            CurrentState.Tick();
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

        public void ReachedDestination()
        {
            StartCoroutine(RoamAround());
        }

        public void StartDying()
        {
            for(int i = 0; i < anim.Length; i++)
            {
                anim[i].SetBool("isDead", true);
            }
        }
    }
}