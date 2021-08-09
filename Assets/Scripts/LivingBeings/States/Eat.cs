using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class Eat : State
    {
        float eatTime = 2.0f;
        float targetTime;

        public Eat(Animal animal) : base(animal)
        {
            
        }

        public override void OnStateEnter()
        {
            //Debug.Log("Eating");
            animal.StateText.UpdateText("EATING");
            targetTime = Time.time + eatTime;

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetBool("isEating", true);
            }
        }

        public override void Tick()
        {
            if(Time.time >= targetTime)
            {
                animal.Feed();
            }
        }

        public override void OnStateExit() 
        {
            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetBool("isEating", false);
            }
        }
    }
}