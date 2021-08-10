using UnityEngine;
using System.Collections;
 
public class FlyCamera : MonoBehaviour {
    [SerializeField]
    float _WalkingSpeed = 20.0f;
    [SerializeField]
    float _RunningSpeed = 50.0f;
    [SerializeField]
    float _LookSpeed = 10.0f;
    [SerializeField]
    float _ZoomSpeed = 500.0f;

    float _LookXLimit = 50.0f;

    private CharacterController characterController;
    private Camera playerCamera;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    LineRenderer lineRenderer;

    private void Awake()
    {
        characterController = gameObject.AddComponent<CharacterController>();
        playerCamera = gameObject.transform.GetChild(0).gameObject.GetComponent<Camera>();

        lineRenderer = gameObject.GetComponent<LineRenderer>();

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Update()
    {
        Move();
        Rotate();
        Fly();
        Zoom();
    }

    private void Move(){

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = (isRunning ? _RunningSpeed : _WalkingSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? _RunningSpeed : _WalkingSpeed) * Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

    }

    private void Rotate(){

        rotationX += -Input.GetAxis("Mouse Y") * _LookSpeed;
        rotationX = Mathf.Clamp(rotationX, -_LookXLimit, _LookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _LookSpeed, 0);

    }

    private void Fly(){
        Vector3 goUp = new Vector3(0, 0, 0);
        bool spacebarPressed = Input.GetKeyDown(KeyCode.Space);

        if(Input.GetKey(KeyCode.Space)){
            goUp.y = goUp.y + _WalkingSpeed * Time.deltaTime;
            characterController.Move(goUp);
        }

        if(Input.GetKey(KeyCode.X)){
            goUp.y = goUp.y - _WalkingSpeed * Time.deltaTime;
            characterController.Move(goUp);
        }
    }

    private void Zoom(){
        characterController.Move(playerCamera.transform.forward * Time.deltaTime * Input.mouseScrollDelta.y * _ZoomSpeed);
    }
}