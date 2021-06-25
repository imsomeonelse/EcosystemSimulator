using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class Mate : State
    {
        float mateTime = 4.0f;
        float targetTime;

        public Mate(Animal animal) : base(animal)
        {
            //Debug.Log(animal + " is Mating");
        }

        public override void OnStateEnter()
        {
            targetTime = Time.time + mateTime;
            animal.IsWaitingForMate = false;

            for(int i = 0; i < animal.anim.Length; i++)
            {
                if(animal.anim[i] != null)
                {
                    animal.anim[i].SetBool("isMating", true);
                }
            }
        }

        public override void Tick()
        {
            if(Time.time >= targetTime)
            {
                if(animal.Gender == Gender.Female)
                {
                    animal.HaveBaby();
                }
                animal.FinishMating();
            }
        }

        public override void OnStateExit() 
        {
            animal.FoundMate = false;
            animal.LookForMate = true;
            animal.DeactivateHeart();

            for(int i = 0; i < animal.anim.Length; i++)
            {
                if(animal.anim[i] != null)
                {
                    animal.anim[i].SetBool("isMating", false);
                }
            }
        }
    }
}
