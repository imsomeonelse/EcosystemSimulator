using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class RunAway : State
    {
        Coord newCoord;
        Vector3 destination;
        
        public RunAway(Animal animal) : base(animal)
        {

        }

        public override void OnStateEnter()
        {
            animal.StateText.UpdateText("RUNNING AWAY");

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetFloat("speed", animal.currentSpeed);
            }
            
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
                        
                        SetDestination();
                    }
                }
            }

        }

        private void SetDestination()
        {
            animal.currentSpeed = animal.BaseSpeed * 3;
            animal.meshAgent.speed = animal.BaseSpeed * 3;

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetFloat("speed", animal.currentSpeed);
            }

            newCoord = EnvironmentManager.GetRandomWalkable();
            destination = EnvironmentManager.tileCentres[newCoord.x, newCoord.y];

            animal.meshAgent.SetDestination(destination);

            UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
            animal.meshAgent.CalculatePath(destination, path);
            if (path.status == UnityEngine.AI.NavMeshPathStatus.PathPartial) {
                SetDestination();
            }
        }

        public override void OnStateExit() 
        {

        }
    }
}
