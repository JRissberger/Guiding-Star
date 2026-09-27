using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class TrialPlayerMovement : MonoBehaviour
{
    // Player Vars
    PlayerInput playerInput;
    InputAction moveAction; 
    [Tooltip("0 makes y-axis dynamic with player and 1 keeps it static, 2 keeps it consistent with cardinal directions relative to camera (this becomes potentially confusing when camera is at quarter angles)")]
    [SerializeField] int moveType = 0;
    [SerializeField] float maxSpeed = 6f; 
    [SerializeField] float rotationSpeed = 10f;
    float speed = 0;
    Vector3 velocity;
    Vector3 moveDirection;

    // Y-axis movement
    [SerializeField] float bobDistance = 0.2f;
    [SerializeField] float bobSpeed = 2f;
    float baseHeight;

    // Camera
    public CinemachineCamera cam; 
    [Tooltip("this should be dynamic, to do later")]
    public Transform[] lookAts;
    Vector3 transPos;
    Vector3 lookAt;
    

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
        this.transform.position = new Vector3(this.transform.position.x, baseHeight + Mathf.Sin(Time.time * bobSpeed) * bobIntensity, this.transform.position.z);
        moveDirection = Vector3.zero;
        // Get player input
        Vector2 direction = moveAction.ReadValue<Vector2>();

        if (direction.magnitude > 0)
        {
            // Only change axis if camera is done transitioning
            if (cam.GetComponent<TrialCamera>().transitioning)
                lookAt = transPos;

            switch (moveType)
            {
                case 0:
                    if (!cam.GetComponent<TrialCamera>().transitioning)
                        lookAt = transPos = this.transform.position - cam.transform.position;

                    moveDirection = velocity = Movement01(direction, Vector3.zero, lookAt);
                    break;

                case 1:
                    if (!cam.GetComponent<TrialCamera>().transitioning)
                        lookAt = transPos = lookAts[(int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition].position - cam.transform.position;

                    moveDirection = velocity = Movement01(direction, Vector3.zero, lookAt);
                    break;

                case 2:
                    moveDirection = velocity = Movement2(direction, Vector3.zero);
                    break;

                default:
                    Debug.Log("There is no movement system #" + moveType + ". Try numbers 0 - 2");
                    break;
            }

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            // Smoothly rotate from current to target rotation
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float targetSpeed = moveDirection.magnitude * maxSpeed;                                     // Calculate target speed from input
        float speedDif = targetSpeed - speed;                                                       // How far we are from target speed
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? 5f : 7f;                               // Use acceleration or deceleration
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, 0.9f) * Mathf.Sign(speedDif);   // Non-linear acceleration
        speed += movement * Time.deltaTime;                                                         // Apply to current speed
        this.transform.position += velocity * speed * Time.deltaTime;
    }

    public Vector3 Movement01(Vector2 direction, Vector3 vel, Vector3 look)
    {
        look.y = 0;
        look = look.normalized;

        if (direction.y > 0)
            vel += look;
        if (direction.y < 0)
            vel -= look;
        if (direction.x > 0)
            vel += Vector3.Cross(cam.transform.position - lookAts[(int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition].position, Vector3.up);
        if (direction.x < 0)
            vel -= Vector3.Cross(cam.transform.position - lookAts[(int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition].position, Vector3.up);

        return vel.normalized;
    }

    public Vector3 Movement2(Vector2 direction, Vector3 vel)
    {
        if (!cam.GetComponent<TrialCamera>().transitioning)
            lookAt = transPos = lookAts[(int)cam.GetComponent<CinemachineSplineDolly>().CameraPosition].position - cam.transform.position;

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
            forward = new Vector3(Math.Sign(lookAt.z), 0, 0);
            horizontal = new Vector3(0, 0, -1 * Math.Sign(lookAt.z));
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

}