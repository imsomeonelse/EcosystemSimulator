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
            animal.StateText.UpdateText("BEING EATEN");

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetBool("isBeingEaten", true);
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
