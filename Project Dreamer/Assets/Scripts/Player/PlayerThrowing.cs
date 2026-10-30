using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the player's ability to aim and throw objects.
/// Should be attached to the player character.
/// </summary>
public class PlayerThrowing : BaseThrowing
{
    [Header("Player")]

    [Tooltip("The maximum distance the player can throw items, in Unity units. Based on the a cylinder around the attached object's position, not considering _handOffset.")]
    [SerializeField] private float _throwRadius = 5.0f;

    [Tooltip("How far above themselves the player can throw items, in Unity units, relative to the attached object's postiion.")]
    [SerializeField] private float _throwHeightUp = 3.0f;

    [Tooltip("How far below themselves the player can throw items, in Unity units, relative to the attached object's postiion.")]
    [SerializeField] private float _throwHeightDown = 3.0f;

    [Tooltip("The material to add to an object to highlight it when it can be picked up.")]
    [SerializeField] private Material _pickupHighlightMaterial = null;

    [Tooltip("Layer mask defining which surfaces can be aimed at.")]
    [SerializeField] private LayerMask _targetableSurfaceMask = 1; //default to just layer 0

    [Header("Target Marker")]

    [Tooltip("The prefab to display as the target marker when aiming, if the throw is valid. The prefab should be oriented such that the +Z axis points into the surface the marker is on.")]
    [SerializeField] private GameObject _validMarkerPrefab = null;

    [Tooltip("The prefab to display as the target marker when aiming, if the throw is NOT valid. The prefab should be oriented such that the +Z axis points into the surface the marker is on.")]
    [SerializeField] private GameObject _invalidMarkerPrefab = null;

    [Tooltip("How much the target marker should \"hover\" over the surface it is on, to avoid z-fighting issues.")]
    [SerializeField] private float _targetMarkerHover = 0.01f;

    [Header("Miscellaneous")]

    [Tooltip("The camera, used for mouse position calculations. If unset, defaults to the main camera (Camera.main, determined on Start()).")]
    [SerializeField] private Camera _camera = null;

    private InputAction _pickupAction;

    private GameObject _activeMarker = null;
    private InputAction _aimAction;
    private InputAction _throwAction;

    private List<GameObject> _pickupableItems = new List<GameObject>();
    private GameObject _pickupTarget = null;

    protected override void Start()
    {
        base.Start();

        _aimAction = InputSystem.actions.FindAction("Aim");
        _throwAction = InputSystem.actions.FindAction("Throw");

        _pickupAction = InputSystem.actions.FindAction("Pick Up");

        //Make sure the action map is enabled
        _pickupAction.actionMap.Enable();

        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    private void Update()
    {
        //Aiming and throwing logic
        if (_aimAction.IsPressed())
        {
            RaycastHit? clickedInfo = GetClickedRaycast(Mouse.current.position.ReadValue(), _targetableSurfaceMask);
            if (clickedInfo.HasValue)
            {
                Vector3 targetPoint;
                Vector3 targetNormal;

                Vector3 clickedPoint = clickedInfo.Value.point;

                //Raycast to the clicked point to check for obstructions and range
                RaycastHit targetInfo;
                bool targetHit = Physics.Raycast(transform.position, clickedPoint - transform.position, out targetInfo, Mathf.Infinity, _targetableSurfaceMask, QueryTriggerInteraction.Ignore);

                Vector3 tryTarget;
                if (targetHit)
                {
                    tryTarget = targetInfo.point;
                }
                else
                {
                    tryTarget = clickedPoint;
                }

                //Cylindrical range check
                Vector2 displacementXZ = new Vector2(tryTarget.x, tryTarget.z) - new Vector2(transform.position.x, transform.position.z);
                float distanceXZ = displacementXZ.magnitude;
                float displacementY = tryTarget.y - transform.position.y;
                if (targetHit && distanceXZ <= _throwRadius && displacementY <= _throwHeightUp && -displacementY <= _throwHeightDown)
                {
                    targetPoint = tryTarget;
                    targetNormal = targetInfo.normal;
                }
                else
                {
                    //If the target is still out of range, clamp the range to a point within the cylinder,and raycast from there to the floor
                    float targetAngle;
                    if (distanceXZ == 0)
                    {
                        targetAngle = 90 * Mathf.Sign(displacementY);
                    }
                    else
                    {
                        targetAngle = Mathf.Atan2(displacementY, distanceXZ);
                    }

                    float tryDistance = (tryTarget - transform.position).magnitude;
                    float clampedDistance;
                    if (targetAngle > Mathf.Atan2(_throwHeightUp, _throwRadius)) //Breaches cylinder top
                    {
                        clampedDistance = tryDistance / displacementY * _throwHeightUp;
                    }
                    else if (targetAngle < Mathf.Atan2(-_throwHeightDown, _throwRadius)) //Breaches cylinder bottom
                    {
                        clampedDistance = tryDistance / -displacementY * _throwHeightDown;
                    }
                    else //Breaches cylinder side
                    {
                        clampedDistance = tryDistance / distanceXZ * _throwRadius;
                    }
                    Vector3 clampedPoint = transform.position + (tryTarget - transform.position).normalized * clampedDistance;
                    RaycastHit floorInfo;
                    bool floorHit = Physics.Raycast(clampedPoint, Vector3.down, out floorInfo, Mathf.Infinity, _targetableSurfaceMask, QueryTriggerInteraction.Ignore);
                    targetPoint = floorInfo.point;
                    targetNormal = floorInfo.normal;
                }


                if (_heldItem)
                {
                    //Prevent throwing at a wall that is facing away from the thrower
                    //This is done by comparing the surface normal to the vector from the hand to the target point
                    //If these vectors are facing the same way, the dot product will be positive
                    bool validCurve;

                    Vector3 dotLeft = clickedPoint - (transform.position);
                    //Ignore y component if positive (upwards) - throwing to a higher floor is possible due to gravity
                    dotLeft.y = Mathf.Min(dotLeft.y, 0);

                    Vector3 dotRight = targetNormal;

                    //dotRight.y = Mathf.Min(dotRight.y, 0);
                    if (Vector3.Dot(dotLeft, dotRight) > 0)
                    {
                        validCurve = false;
                        ResetThrow();
                    }
                    else
                    {
                        validCurve = CreateThrowCurve(targetPoint, targetNormal);
                    }

                    SetThrowMarker(targetPoint, targetNormal, validCurve);

                    if (validCurve && _throwAction.WasPressedThisFrame())
                    {
                        ThrowItem();
                    }
                }
                else
                {
                    ResetThrow();
                }                
            }
            else
            {
                ResetThrow();
            }
        }
        else
        {
            ResetThrow();
        }

        //Determining the nearest item, to highlight it
        if (_pickupableItems.Count > 0)
        {
            GameObject nearestItem = _pickupableItems[0];
            float nearestDistance = (_pickupableItems[0].transform.position - transform.position).magnitude;

            if (_pickupableItems.Count > 1)
            {
                for (int i = 1; i < _pickupableItems.Count; i++)
                {
                    GameObject checkItem = _pickupableItems[i];
                    float checkDistance = (checkItem.transform.position - transform.position).magnitude;

                    if (checkDistance < nearestDistance)
                    {
                        nearestItem = checkItem;
                        nearestDistance = checkDistance;
                    }
                }
            }

            if (_pickupTarget != nearestItem)
            {
                SetPickupTarget(nearestItem);
            }
        }
        else
        {
            SetPickupTarget(null);
        }

        //Picking up the highlighted item
        if (_pickupAction.WasPressedThisFrame() && _pickupTarget != null)
        {
            SetHeldItem(_pickupTarget);
        }
    }

    private void SetPickupTarget(GameObject target)
    {
        if (_pickupTarget != null)
        {
            Renderer oldPickupRenderer = _pickupTarget.GetComponent<Renderer>();
            List<Material> oldPickupMaterials = new List<Material>();
            oldPickupRenderer.GetSharedMaterials(oldPickupMaterials);
            oldPickupMaterials.Remove(_pickupHighlightMaterial);
            oldPickupRenderer.SetSharedMaterials(oldPickupMaterials);
        }

        _pickupTarget = target;

        if (_pickupTarget != null)
        {
            Renderer pickupRenderer = _pickupTarget.GetComponent<Renderer>();
            List<Material> pickupMaterials = new List<Material>();
            pickupRenderer.GetSharedMaterials(pickupMaterials);
            pickupMaterials.Add(_pickupHighlightMaterial);
            pickupRenderer.SetSharedMaterials(pickupMaterials);
        }
    }

    public void AddPickupByCollider(Collider other)
    {
        if (other.gameObject.GetComponent<ThrowableItem>() != null && other.gameObject != _heldItem)
        {
            _pickupableItems.Add(other.gameObject);
        }
    }

    public void RemovePickupByCollider(Collider other)
    {
        if (other.gameObject.GetComponent<ThrowableItem>() != null)
        {
            _pickupableItems.Remove(other.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AddPickupByCollider(other);
    }

    private void OnTriggerExit(Collider other)
    {
        RemovePickupByCollider(other);
    }

    public override void SetHeldItem(GameObject item)
    {
        _pickupableItems.Remove(item);
        base.SetHeldItem(item);
    }

    public override void DropItem()
    {
        _pickupableItems.Add(_heldItem);
        base.DropItem();
    }

    /// <summary>
    /// Returns the world position the player clicked on with the mouse,
    /// raycasting from it based on the script's selected camera.
    /// </summary>
    /// <param name="mousePosition">The position of the mouse on screen.</param>
    /// <param name="clickableLayers">A mask defining which layers will be checked.</param>
    /// <returns>The world position that was clicked on, or null if no object is found.</returns>
    protected RaycastHit? GetClickedRaycast(Vector2 mousePosition, LayerMask clickableLayers)
    {
        Ray screenRay = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(screenRay, out hitInfo, Mathf.Infinity, clickableLayers, QueryTriggerInteraction.Ignore))
        {
            return hitInfo;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Places the throwing target marker on a surface, facing a certain direction.
    /// The marker hovers slightly above the surface to avoid z-fighting issues.
    /// If a marker already exists, it will be removed in favor of the new one.
    /// </summary>
    /// <param name="position">Position of the throwing target marker</param>
    /// <param name="normal">Direction of the marker (for example, upwards if it's on the floor)</param>
    /// <param name="valid">Whether the throw marker should appear "valid" (normal) or "invalid" (tinted red)</param>
    private void SetThrowMarker(Vector3 position, Vector3 normal, bool valid = true)
    {
        if (_activeMarker)
        {
            Destroy(_activeMarker);
        }

        if (valid)
        {
            _activeMarker = Instantiate(_validMarkerPrefab);
        }
        else
        {
            _activeMarker = Instantiate(_invalidMarkerPrefab);
        }

        Vector3 finalPosition = position + normal * _targetMarkerHover;
        _activeMarker.transform.position = finalPosition;
        _activeMarker.transform.rotation = Quaternion.LookRotation(normal);
    }

    protected override void ThrowItem()
    {
        ThrowableItem throwable = _heldItem.GetComponent<ThrowableItem>();
        if (throwable != null)
        {
            throwable.QueueLandSound();
        }

        base.ThrowItem();
    }
    /// <summary>
    /// Removes the throwing target marker if one exists.
    /// </summary>
    public void RemoveThrowMarker()
    {
        if (_activeMarker)
        {
            Destroy(_activeMarker);
        }
    }

    /// <summary>
    /// Clears the throwing marker and curve, and hides the latter's line renderer.
    /// </summary>
    protected override void ResetThrow()
    {
        base.ResetThrow();
        RemoveThrowMarker();
    }
}
