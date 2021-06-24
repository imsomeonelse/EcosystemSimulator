using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class Die : State
    {
        public Die(Animal animal) : base(animal)
        {
            //Debug.Log("Dying");
        }

        public override void OnStateEnter()
        {
            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetBool("isDead", true);
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
