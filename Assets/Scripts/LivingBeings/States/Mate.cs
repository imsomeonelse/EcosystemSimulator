using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class Mate : State
    {
        public Mate(Animal animal) : base(animal)
        {
            Debug.Log("Mating");
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
