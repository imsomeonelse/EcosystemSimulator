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

        protected bool dead;

        public virtual void Init (Coord coord) {
            this.coord = coord;
            transform.position = EnvironmentManager.tileCentres[coord.x, coord.y];
        }

        public virtual void Die () {
            if (!dead) {
                dead = true;
                Destroy (gameObject);
            }
        }

    }
}