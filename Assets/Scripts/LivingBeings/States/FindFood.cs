using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class FindFood : State
    {
        Coord newCoord;
        Vector3 destination;
        LivingBeing closestFood;

        public FindFood(Animal animal) : base(animal)
        {
            
        }

        public override void OnStateEnter()
        {
            //Debug.Log("Finding Food");
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
                        
                        ReachedFood();
                    }
                }
            }

        }

        /* private void Find()
        {
            if(animal is Prey)
            {
                closestFood = FoodSpawner.plantFoodCoords.ClosestEntity(animal.coord, animal.MaxViewDistance);
                Plant plantFood = closestFood as Plant;
                if(!plantFood.isActive)
                {
                    animal.ReachedDestination();
                }
            }
            if(closestFood != null){
                SetDestination();
                animal.FoodTarget = closestFood;
            }
            else
            {
                animal.ReachedDestination();
            }
        } */

        private void Find()
        {
            Collider[] hitColliders = Physics.OverlapSphere(animal.transform.position, animal.MaxViewDistance);
            if(hitColliders.Length > 1)
            {
                closestFood = FindClosest(hitColliders);
                if(closestFood == null)
                {
                    NotFoundFood();
                }
                else{
                    if(closestFood is Plant){
                        Plant plantFood = closestFood as Plant;
                        if(!plantFood.isActive)
                        {
                            NotFoundFood();
                        }
                    }
                    SetDestination();
                    animal.FoodTarget = closestFood;
                }
            }else
            {
                NotFoundFood();
            }
        }

        private LivingBeing FindClosest(Collider[] objsFound)
        {  
            LivingBeing closest = objsFound[0].gameObject.GetComponent<LivingBeing>();

            if (GameObject.ReferenceEquals(animal.gameObject, objsFound[0].gameObject))
            {
                closest = objsFound[1].gameObject.GetComponent<Animal>();
            }

            float lowestDist = animal.MaxViewDistance;

            for(int i = 0; i < objsFound.Length; i++)
            {
                float dist = Vector3.Distance(objsFound[i].transform.position, animal.transform.position);

                if (dist < lowestDist && objsFound[i].gameObject.layer != LayerMask.NameToLayer("Animal"))
                {
                    lowestDist = dist;
                    closest = objsFound[i].gameObject.GetComponent<LivingBeing>();
                }
            }

            return closest;
        }

        private void SetDestination()
        {
            animal.currentSpeed = animal.BaseSpeed;

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetFloat("speed", animal.currentSpeed);
            }

            newCoord = closestFood.coord;
            destination = EnvironmentManager.tileCentres[newCoord.x, newCoord.y];

            animal.meshAgent.SetDestination(destination);

            UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
            animal.meshAgent.CalculatePath(destination, path);
            if (path.status == UnityEngine.AI.NavMeshPathStatus.PathPartial) {
                NotFoundFood();
            }
        }

        private void NotFoundFood()
        {
            animal.ReachedDestination();
        }

        private void ReachedFood()
        {
            animal.ReachedFood();
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
