using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace AnimalManagement{
    public class Roam : State
    {
        Coord newCoord;
        Vector3 destination;

        public Roam(Animal animal) : base(animal)
        {
            
        }

        public override void OnStateEnter()
        {
            Debug.Log("Roaming");
            SetDestination();
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

                        //Debug.Log("Prev coord: " + animal.previousCoord.ToString() + " Current coord: " + animal.coord.ToString());
                        
                        StopWalking();
                    }
                }
            }
        }

        private void SetDestination()
        {
            animal.currentSpeed = animal.BaseSpeed;

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetFloat("speed", animal.currentSpeed);
            }

            newCoord = EnvironmentManager.GetRandomWalkable();
            destination = EnvironmentManager.tileCentres[newCoord.x, newCoord.y];

            animal.meshAgent.SetDestination(destination);

            NavMeshPath path = new NavMeshPath();
            animal.meshAgent.CalculatePath(destination, path);
            if (path.status == NavMeshPathStatus.PathPartial) {
                StopWalking();
            }
        }

        private void StopWalking()
        {
            animal.ReachedDestination();
        }

        public override void OnStateExit() 
        {
            animal.currentSpeed = 0;

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetFloat("speed", animal.currentSpeed);
            }
        }
    }
}