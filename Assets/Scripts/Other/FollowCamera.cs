using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimalManagement;

public class FollowCamera : MonoBehaviour
{
    float doubleClickDelay = 0.2f;
    float previousClick = 0f;

    bool isFollowing = false;
    Transform animalFollowing;

    CharacterController characterController;
    Camera playerCamera;
    Vector3 defaultPos = new Vector3(0f, 3.5f, -5f);
    Vector3 defaultRot = new Vector3(20f, 0f, 0f);

    // Start is called before the first frame update
    private void Start() 
    {
        characterController = gameObject.GetComponent<CharacterController>();
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
                            animalFollowing = hit.transform;
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
            Vector3 newPos = animalFollowing.position + defaultPos;
            playerCamera.transform.position = newPos;
        }
    }

    void StartFollow()
    {
        Debug.Log("henlo");
        Vector3 refPos = Vector3.zero;
        Quaternion newRot = animalFollowing.rotation * Quaternion.Euler(defaultRot);
        Vector3 newPos = animalFollowing.position + defaultPos;
        //playerCamera.transform.rotation =  newRot;
        playerCamera.transform.position = newPos;
    }
}
