using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class Drink : State
    {
        float drinkTime = 1.0f;
        float targetTime;

        public Drink(Animal animal) : base(animal)
        {
            
        }

        public override void OnStateEnter()
        {
            Debug.Log("Drinking");
            targetTime = Time.time + drinkTime;

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetBool("isEating", true);
            }
        }

        public override void Tick()
        {
            if(Time.time >= targetTime)
            {
                animal.Drink();
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