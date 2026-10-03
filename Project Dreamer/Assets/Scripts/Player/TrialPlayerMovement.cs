using System;
using System.Linq;
using Unity.AppUI.UI;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.Windows;
using static UnityEngine.UI.Image;

public class TrialPlayerMovement : MonoBehaviour
{
    // Player Vars
    PlayerInput playerInput;
    InputAction moveAction;
    [Tooltip("0 makes y-axis dynamic with player, 1 keeps it consistent with cardinal directions relative to camera (this becomes potentially confusing when camera is at quarter angles)")]
    [SerializeField] int moveType = 0;
    [SerializeField] float maxSpeed = 6f;
    [SerializeField] float rotationSpeed = 10f;
    float speed = 0;
    Vector3 velocity;
    Vector3 moveDirection;
    Vector2 prevInput = Vector2.zero;

    // Y-axis movement
    [SerializeField] float bobDistance = 0.2f;
    [SerializeField] float bobSpeed = 2f;
    float baseHeight;

    // Camera
    public CinemachineCamera cam;
    [Tooltip("this should be dynamic, to do later")]
    public Transform[] lookAts;
    Vector3 lookAt;
    Vector3 transPos;
    int checkKeyUp = 0;
    bool transedAxis = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        baseHeight = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // Apply bobbing movement
        this.transform.position = new Vector3(this.transform.position.x, baseHeight + Mathf.Sin(Time.time * bobSpeed) * bobDistance, this.transform.position.z);
        moveDirection = Vector3.zero;
        // Get player input
        Vector2 direction = moveAction.ReadValue<Vector2>();

        if (direction.magnitude > 0)
        {
            // Only change axis if camera is halfway through transitioning
            if (cam.GetComponent<TrialCamera>().halfTrans && checkKeyUp == 0)
                checkKeyUp = 1;

            switch (moveType)
            {
                case 0:
                    //if (!cam.GetComponent<TrialCamera>().halfTrans)
                    lookAt = this.transform.position - cam.transform.position;

                    moveDirection = velocity = Movement0(direction, Vector3.zero, lookAt);
                    break;

                case 1:
                    moveDirection = velocity = Movement1(direction, Vector3.zero);
                    break;

                default:
                    Debug.Log("There is no movement system #" + moveType + ". Try 0 or 1");
                    break;
            }

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            // Smoothly rotate from current to target rotation
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        }

        //if (!CollisionCheck(velocity))
        //{
        float targetSpeed = moveDirection.magnitude * maxSpeed;                                     // Calculate target speed from input
        float speedDif = targetSpeed - speed;                                                       // How far we are from target speed
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? 5f : 7f;                               // Use acceleration or deceleration
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, 0.9f) * Mathf.Sign(speedDif);   // Non-linear acceleration
        speed += movement * Time.deltaTime;                                                         // Apply to current speed
        this.transform.position += velocity * speed * Time.deltaTime;
        //}
        prevInput = direction;
    }

    public Vector3 Movement0(Vector2 direction, Vector3 vel, Vector3 look)
    {
        look.y = 0;
        look = look.normalized;

        if (direction.y > 0)
            vel += look;
        if (direction.y < 0)
            vel -= look;
        if (direction.x > 0)
            vel += Vector3.Cross(cam.transform.position - lookAts[(int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition].position, Vector3.up).normalized;
        if (direction.x < 0)
            vel -= Vector3.Cross(cam.transform.position - lookAts[(int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition].position, Vector3.up).normalized;

        return vel.normalized;
    }

    public Vector3 Movement1(Vector2 direction, Vector3 vel)
    {
        int splineDirection = Math.Sign(cam.GetComponent<TrialCamera>().startPos - cam.GetComponent<TrialCamera>().goal);
        if (splineDirection < 0) splineDirection = 0;

        int pos;
        // On key down after halfway
        if (checkKeyUp > 0 && prevInput.magnitude < direction.magnitude)
        {
            checkKeyUp = 0;
            pos = (int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition;

            if (cam.GetComponent<TrialCamera>().transitioning)
            {
                checkKeyUp = -1;
                transedAxis = true;
                pos += splineDirection;
            }

            lookAt = transPos = lookAts[pos].position - (Vector3)cam.GetComponent<CinemachineSplineDolly>().Spline.Spline[pos].Position;
        }
        // past halfway transitioning but key has not changed or has changed movement axis but camera is still transitioning, do not change the set axis
        else if (checkKeyUp > 0 || transedAxis && cam.GetComponent<TrialCamera>().transitioning)
        {
            lookAt = transPos;
        }
        else
        {
            checkKeyUp = 0;
            transedAxis = false;
            pos = (int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition;
            if (cam.GetComponent<TrialCamera>().transitioning) pos += splineDirection;
            lookAt = transPos = lookAts[pos].position - (Vector3)cam.GetComponent<CinemachineSplineDolly>().Spline.Spline[pos].Position;
            Debug.Log((Vector3)cam.GetComponent<CinemachineSplineDolly>().Spline.Spline[pos].Position);
        }

        lookAt.y = 0;
        Vector3 forward;
        Vector3 horizontal;

        // Align movement axis with camera direction locked to closest x/z axes
        if (Mathf.Max(Mathf.Abs(lookAt.x), Mathf.Abs(lookAt.z)) == Mathf.Abs(lookAt.z))
        {
            forward = new Vector3(0, 0, Math.Sign(lookAt.z));
            horizontal = new Vector3(Math.Sign(lookAt.z), 0, 0);
        }
        else
        {
            forward = new Vector3(Math.Sign(lookAt.x), 0, 0);
            horizontal = new Vector3(0, 0, -1 * Math.Sign(lookAt.x));
        }

        // set x/z movement
        if (direction.y > 0)
            vel += forward;
        if (direction.y < 0)
            vel -= forward;
        if (direction.x > 0)
            vel += horizontal;
        if (direction.x < 0)
            vel -= horizontal;

        return vel.normalized;
    }

    /*public bool CollisionCheck(Vector3 direction)
    {
        Physics.SphereCast(transform.position, 3f, direction, out RaycastHit hitInfo, 10f, this.layer);
        return;
    }*/
}