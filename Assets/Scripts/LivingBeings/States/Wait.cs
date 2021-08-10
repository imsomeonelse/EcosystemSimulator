using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class Wait : State
    {
        public Wait(Animal animal) : base(animal)
        {

        }

        public override void OnStateEnter()
        {
            animal.StateText.UpdateText("WAITING");
            
            animal.currentSpeed = 0;
            animal.meshAgent.speed = 0;

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetFloat("speed", 0);
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
