using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Player Vars
    Rigidbody rb;
    private InputAction moveAction;
    private int moveType = 1;
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float transitionSpeed = 6f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float accelRate = 1f;
    private float speed = 0;
    private float currentMaxSpeed = 10f;
    private Vector3 velocity;
    private Vector3 moveDirection;

    LayerMask corporealMask;
    public bool corporeal;

    // Y-axis movement
    [SerializeField] float bobDistance = 0.2f;
    [SerializeField] float bobSpeed = 2f;
    private float baseHeight;

    // Camera
    private CinemachineCamera cam;
    public List<Trigger> triggers;
    private Vector3 lookAt;
    //private Vector3 tempRot;
    private Vector3 prevRot = Vector3.zero;
    private int waitingForKeyUp = -1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = FindFirstObjectByType<CinemachineCamera>();
        prevRot = new Vector3(0, 0, -1);
        moveAction = InputSystem.actions.FindAction("Move");
        baseHeight = this.transform.position.y;
        corporealMask = LayerMask.GetMask("Barriers", "Phaseable");
        triggers.Add(cam.GetComponent<CameraController>().knots[0].trigger);
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
        float targetSpeed = 0;

        // Get player input
        if (moveAction.IsPressed())
        {
            targetSpeed = currentMaxSpeed;
            Vector2 direction = moveAction.ReadValue<Vector2>();


            Quaternion targetRotation;
            switch (moveType)
            {
                case 0:
                    lookAt = this.transform.position - cam.transform.position;

                    moveDirection = velocity = Movement0(direction, Vector3.zero, lookAt);

                    targetRotation = Quaternion.LookRotation(moveDirection);
                    // Smoothly rotate from current to target rotation
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    break;

                case 1:
                    moveDirection = velocity = Movement1(direction, Vector3.zero);

                    if ((moveDirection - this.transform.forward).magnitude > 0.01)
                    {
                        targetRotation = Quaternion.LookRotation(moveDirection);
                        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    }
                    break;

                case 2:
                    moveDirection = velocity = new Vector3(direction.x, 0, direction.y);

                    targetRotation = Quaternion.LookRotation(moveDirection);
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    break;

                case 3:
                    moveDirection = velocity = Movement3(direction);

                    break;

                default:
                    Debug.Log("There is no movement system #" + moveType + ". Try 1 or 3");
                    break;
            }

        }
        else if (waitingForKeyUp == 0) { waitingForKeyUp = 1; }

        float speedDif = targetSpeed - speed;                                                       // How far we are from target speed
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, 0.9f) * Mathf.Sign(speedDif);   // Non-linear acceleration
        speed += movement * Time.deltaTime;                                                         // Apply to current speed
        rb.linearVelocity = velocity * speed * Time.deltaTime;
    }

    /// <summary>
    /// DEPRECATED W and S moves along camera forward, right and left dependent to perpendicular
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="vel"></param>
    /// <param name="look"></param>
    /// <returns></returns>
    public Vector3 Movement0(Vector2 direction, Vector3 vel, Vector3 look)
    {
        look.y = 0;
        look = look.normalized;

        if (direction.y > 0)
            vel += look;
        if (direction.y < 0)
            vel -= look;
        if (direction.x > 0)
            vel += cam.transform.right;
        if (direction.x < 0)
            vel -= cam.transform.right;

        return vel.normalized;
    }

    /// <summary>
    /// Switch axes on key up after halfway through transition, axes are dependent on trigger box z-axis
    /// </summary>
    /// <param name="direction">The player input vector, x is W/S, y is A/D</param>
    /// <param name="vel"></param>
    /// <returns></returns>
    public Vector3 Movement1(Vector2 direction, Vector3 vel)
    {
        Vector3 forward = prevRot.normalized;
        Vector3 horizontal = Vector3.Cross(Vector3.up, prevRot).normalized;
        Vector3 vv = forward;

        foreach (Trigger t in triggers)
        {
            // If this is the trigger we're in && the camera is at least halfway to it &&
            // the player's forward vector isn't already aligned with the forward vector of the trigger
            // Then adjust the forward vector
            if (cam.GetComponent<CameraController>().goal == t.knot && Math.Abs(t.knot - cam.GetComponent<CinemachineSplineDolly>().CameraPosition) <= 0.5 && forward != t.forwardMovement)
            {
                vv = t.forwardMovement;
                break;
            }
        }

        // if we need to transition, start listening for key up
        if (waitingForKeyUp == -1 && forward != vv) { waitingForKeyUp = 0; }
        // if W/S have been released, start listening for them to be pressed again
        if (waitingForKeyUp == 0 && direction.y == 0) { waitingForKeyUp = 1; }
        // if W or S has been pressed again, change the axes
        if (waitingForKeyUp == 1 && Math.Abs(direction.y) > 0)
        {
            forward = vv;
            horizontal = Vector3.Cross(Vector3.up, vv);
            //Debug.Log("reassigned axis to " + vv);
            waitingForKeyUp = -1;
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

        prevRot = forward;


        return vel.normalized;
    }

    /// <summary>
    /// DEPRECATED Uses the forward camera vector for W and S and perpendicular to the camera for A and D
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Vector3 Movement3(Vector2 direction)
    {
        lookAt = cam.transform.forward;
        lookAt.y = 0;
        lookAt = lookAt.normalized;

        Vector3 vel = Vector3.zero;

        if (direction.y > 0)
            vel += lookAt;
        if (direction.y < 0)
            vel -= lookAt;
        if (direction.x > 0)
            vel -= Vector3.Cross(lookAt, Vector3.up).normalized;
        if (direction.x < 0)
            vel += Vector3.Cross(lookAt, Vector3.up).normalized;

        if (direction.y >= -0.5)
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(vel.normalized), rotationSpeed * Time.deltaTime);

        return vel.normalized;
    }

    public void SetToTransitioningSpeed(bool transitioning)
    {
        if (transitioning)
            currentMaxSpeed = transitionSpeed;
        else
            currentMaxSpeed = maxSpeed;
    }

    public void OnTriggerEnter(Collider collider)
    {
        GameObject g = collider.gameObject;
        if (collider.TryGetComponent<Trigger>(out Trigger t))
        {
            if (!triggers.Contains(t))
                triggers.Add(t);
        }
    }
    public void OnTriggerExit(Collider collider)
    {
        if (collider.TryGetComponent<Trigger>(out Trigger t))
            triggers.Remove(t);
    }
}