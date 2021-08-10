using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimalManagement;

public class FollowCamera : MonoBehaviour
{
    float doubleClickDelay = 0.2f;
    float previousClick = 0f;

    public bool isFollowing = false;
    Transform animalFollowing;

    Camera playerCamera;

    // Start is called before the first frame update
    private void Start() 
    {
        playerCamera = gameObject.transform.GetChild(0).gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isFollowing)
        {
            if (Input.GetMouseButtonDown(0)){
                float gap = Time.time - previousClick;
                if( gap < doubleClickDelay ){
                    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit)){
                        GameObject gameObject = hit.transform.gameObject;
                        LivingBeing livingBeing = gameObject.GetComponent<LivingBeing>();
                        if(livingBeing is Animal)
                        {
                            animalFollowing = hit.transform.Find("CameraTarget").transform;
                            isFollowing = true;
                            StartFollow();
                        }
                    }
                }
                previousClick = Time.time;
            }
        }
        else
        {
            if(animalFollowing == null)
            {
                isFollowing = false;
            }
            if (Input.GetMouseButtonDown(0)){
                float gap = Time.time - previousClick;
                if( gap < doubleClickDelay ){
                    isFollowing = false;
                }
                previousClick = Time.time;
            }
            playerCamera.transform.LookAt(animalFollowing);
            transform.position = animalFollowing.position;
            playerCamera.transform.rotation = animalFollowing.rotation;
        }
    }

    void StartFollow()
    {
        playerCamera.transform.LookAt(animalFollowing);
        transform.position = Vector3.Lerp(transform.position, animalFollowing.position, Time.deltaTime);
        playerCamera.transform.rotation = Quaternion.Lerp(transform.rotation, animalFollowing.rotation, Time.deltaTime);
    }
}
