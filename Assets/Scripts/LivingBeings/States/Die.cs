using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class Die : State
    {
        public Die(Animal animal) : base(animal)
        {
            
        }

        public override void OnStateEnter()
        {
            if(!animal.IsDead)
            {
                animal.StateText.UpdateText("DYING");

                for(int i = 0; i < animal.anim.Length; i++)
                {
                    animal.anim[i].SetBool("isDead", true);
                }
                animal.IsDying = true;
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
