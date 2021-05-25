using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class FindFood : State
    {
        public FindFood(Animal animal) : base(animal)
        {
            
        }

        public override void OnStateEnter()
        {
            Debug.Log("Found food!");
        }

        public override void Tick()
        {

        }

        public override void OnStateExit() 
        {

        }
    }
}
