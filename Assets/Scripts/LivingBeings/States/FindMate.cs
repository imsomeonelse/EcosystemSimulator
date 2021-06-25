using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalManagement{
    public class FindMate : State
    {
        Vector3 destination;
        Animal closestMate;

        public FindMate(Animal animal) : base(animal)
        {
            //Debug.Log("Finding mate");
        }

        public override void OnStateEnter()
        {
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
                        animal.coord = closestMate.coord;
                        
                        ReachedMate();
                    }
                }
            }

            if(closestMate == null)
            {
                NotFoundMate();
            }
        }

        private void Find()
        {
            Collider[] hitColliders = Physics.OverlapSphere(animal.transform.position, animal.MaxViewDistance);
            if(hitColliders.Length > 1)
            {
                closestMate = FindClosest(hitColliders);
                if(closestMate == null)
                {
                    NotFoundMate();
                }
                else
                {
                    animal.ActivateHeart();
                    closestMate.WaitForMate(animal);
                    SetDestination();
                    animal.CurrentMate = closestMate;
                }
            }else
            {
                NotFoundMate();
            }
        }

        private Animal FindClosest(Collider[] objsFound)
        {  
            Animal closest = objsFound[0].gameObject.GetComponent<Animal>();

            if (GameObject.ReferenceEquals(animal.gameObject, objsFound[0].gameObject))
            {
                closest = objsFound[1].gameObject.GetComponent<Animal>();
            }

            float lowestDist = animal.MaxViewDistance;

            for(int i = 0; i < objsFound.Length; i++)
            {
                float dist = Vector3.Distance(objsFound[i].transform.position, animal.transform.position);

                GameObject temp = objsFound[i].gameObject;

                if (dist < lowestDist && 
                    temp.layer == LayerMask.NameToLayer("Animal") && 
                    !GameObject.ReferenceEquals(animal.gameObject, temp) &&
                    temp.GetComponent<Animal>().Gender != animal.Gender &&
                    !temp.GetComponent<Animal>().FoundMate &&
                    animal.Species == temp.GetComponent<Animal>().Species &&
                    !temp.GetComponent<Animal>().IsBaby
                )
                {
                    lowestDist = dist;
                    closest = temp.GetComponent<Animal>();
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

            destination = closestMate.transform.position;

            animal.meshAgent.SetDestination(destination);

            UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
            animal.meshAgent.CalculatePath(destination, path);
            if (path.status == UnityEngine.AI.NavMeshPathStatus.PathPartial) {
                NotFoundMate();
            }
        }

        private void NotFoundMate()
        {
            animal.ReachedDestination();
        }

        private void ReachedMate()
        {
            animal.ReachedMate();
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
