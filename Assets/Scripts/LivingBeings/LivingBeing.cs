using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class LivingBeing : MonoBehaviour
    {
        [HideInInspector]
        public Coord coord;
        [HideInInspector]
        public int mapIndex;
        public int foodValue = 70;

        protected bool dead;

        public virtual void Init (Coord coord) 
        {
            this.coord = coord;
            transform.position = EnvironmentManager.tileCentres[coord.x, coord.y];
        }

        public virtual void Deactivate() { }

        public virtual void Die ()
        {
            if (!dead) {
                dead = true;

                AnimalManager aM = Object.FindObjectOfType<AnimalManager>();

                if(this is Predator)
                {
                    aM.RemovePredator();
                }
                if(this is Prey)
                {
                    aM.RemovePrey();
                }

                aM.AddDeath();
                
                Destroy (gameObject);
            }
        }

    }
}