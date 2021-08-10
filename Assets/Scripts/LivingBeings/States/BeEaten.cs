using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class BeEaten : State
    {
        float targetTime;

        public BeEaten(Animal animal) : base(animal)
        {

        }

        public override void OnStateEnter()
        {
            if(!animal.IsDead)
            {
                animal.StateText.UpdateText("BEING EATEN");
            
                animal.currentSpeed = 0;
                animal.meshAgent.speed = 0;

                for(int i = 0; i < animal.anim.Length; i++)
                {
                    animal.anim[i].SetBool("isBeingEaten", true);
                }

                for(int i = 0; i < animal.anim.Length; i++)
                {
                    animal.anim[i].SetFloat("speed", 0);
                }
            }
        }

        public override void Tick()
        {

        }

        public override void OnStateExit() 
        {

        }
    }
}
