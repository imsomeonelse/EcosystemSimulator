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
            animal.StateText.UpdateText("FINDING FOOD");
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
                        float dist = Vector3.Distance(animal.FoodTarget.transform.position, animal.transform.position);
                        if(dist <= 10)
                        {
                            ReachedFood();
                        }
                        else
                        {
                            NotFoundFood();
                        }
                    }
                }
            }
            if(animal is Predator)
            {
                Animal prey = animal.FoodTarget as Animal;

                if(prey.IsWaitingToBeEaten == true && prey.CurrentPredator != animal)
                {
                    NotFoundFood();
                }
            }
        }

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
                        animal.StateText.UpdateText("WALKING TO FOOD");
                        Plant plantFood = closestFood as Plant;
                        if(!plantFood.isActive)
                        {
                            NotFoundFood();
                        }
                    }
                    else{
                        animal.StateText.UpdateText("CHASING PREY");
                    }
                    animal.FoodTarget = closestFood;
                    SetDestination();
                }
            }else
            {
                NotFoundFood();
            }
        }

        private LivingBeing FindClosest(Collider[] objsFound)
        {  
            LivingBeing closest = null;

            float lowestDist = animal.MaxViewDistance;

            for(int i = 0; i < objsFound.Length; i++)
            {
                float dist = Vector3.Distance(objsFound[i].transform.position, animal.transform.position);

                if(animal is Prey)
                {
                    if (dist < lowestDist && objsFound[i].gameObject.layer != LayerMask.NameToLayer("Animal"))
                    {
                        lowestDist = dist;
                        closest = objsFound[i].gameObject.GetComponent<LivingBeing>();
                    }
                }
                if(animal is Predator)
                {
                    if (dist < lowestDist && objsFound[i].GetComponent<Animal>()!= null && objsFound[i].GetComponent<Animal>().Type == Type.Prey)
                    {
                        //Debug.Log(animal + " found food: " + objsFound[i].GetComponent<Animal>());
                        lowestDist = dist;
                        closest = objsFound[i].gameObject.GetComponent<LivingBeing>();
                    }
                }
            }

            return closest;
        }

        private void SetDestination()
        {
            if(animal is Predator)
            {
                animal.currentSpeed = animal.BaseSpeed * 3;
                animal.meshAgent.speed = animal.BaseSpeed * 3;
            }
            else
            {
                animal.currentSpeed = animal.BaseSpeed;
                animal.meshAgent.speed = animal.BaseSpeed;
            }

            for(int i = 0; i < animal.anim.Length; i++)
            {
                animal.anim[i].SetFloat("speed", animal.currentSpeed);
            }

            destination = animal.FoodTarget.transform.position;

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
