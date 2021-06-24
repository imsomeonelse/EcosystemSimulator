using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class FindWater : State
    {
        Coord newCoord;
        Vector3 destination;
        Coord closestWater;

        public FindWater(Animal animal) : base(animal)
        {
            
        }

        public override void OnStateEnter()
        {
            Debug.Log("Finding Water");
            Find();
        }

        public override void Tick()
        {
             // Check if we've reached the destination
            if (!animal.meshAgent.pathPending)
            {
                if (animal.meshAgent.remainingDistance <= animal.meshAgent.stoppingDistance)
                {
                    if(!animal.meshAgent.hasPath || animal.meshAgent.velocity.sqrMagnitude == 0f)
                    {
                        animal.coord = newCoord;
                        
                        ReachedWater();
                    }
                }
            }

        }

        private void Find()
        {
            closestWater = EnvironmentManager.GetClosestWater(animal.transform.position, animal.MaxViewDistance);
            if(closestWater.x != 0 && closestWater.y != 0){
                SetDestination();
                animal.WaterTarget = closestWater;
            }
            else
            {
                NotFoundWater();
            }
        }

        private void SetDestination()
        {
            animal.currentSpeed = animal.BaseSpeed;

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetFloat("speed", animal.currentSpeed);
            }

            newCoord = closestWater;
            destination = EnvironmentManager.tileCentres[newCoord.x, newCoord.y];

            animal.meshAgent.SetDestination(destination);

            UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
            animal.meshAgent.CalculatePath(destination, path);
            /* if (path.status == UnityEngine.AI.NavMeshPathStatus.PathPartial) {
                NotFoundWater();
            } */
        }

        private void NotFoundWater()
        {
            animal.ReachedDestination();
        }

        private void ReachedWater()
        {
            animal.ReachedWater();
        }

        public override void OnStateExit() 
        {
            animal.currentSpeed = 0;

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetFloat("speed", 0);
            }

        }
    }
}
