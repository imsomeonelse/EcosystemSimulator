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
                        
                        StopWalking();
                    }
                }
            }

        }

        private void Find()
        {
            if(animal is Prey)
            {
                closestWater = animal.waterCoords[animal.coord.x, animal.coord.y];
            }
            if(closestWater != null){
                SetDestination();
                animal.WaterTarget = closestWater;
            }
            else
            {
                animal.ReachedDestination();
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
                Debug.Log("partial");
                StopWalking();
            } */
        }

        private void StopWalking()
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
