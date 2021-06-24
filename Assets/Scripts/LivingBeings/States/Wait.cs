using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class Wait : State
    {
        public Wait(Animal animal) : base(animal)
        {
            //Debug.Log("Waiting");
        }

        public override void OnStateEnter()
        {
            
        }

        public override void Tick()
        {

        }

        public override void OnStateExit() 
        {

        }
    }
}
