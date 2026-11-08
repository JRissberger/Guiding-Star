using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevelPhysics2D;

public class PlayerCorporeality : MonoBehaviour
{
    [Tooltip("The material to apply to the player when they are hiding in a shadow.")]
    [SerializeField] private Material hideMaterial = null;

    [Tooltip("How far the player can travel (in Unity units) after using a shadow, before becoming corporeal again.")]
    [SerializeField] private float shadowDistance = 5.0f;

    private Material defaultMaterial;

    private int SHADOW_LAYER = 8;

    private bool hiding = false;
    private float ghostDistance = 0.0f;
    public float defaultCorporealityDistance = 15f;
    private List<GameObject> touchingShadows = new List<GameObject>();
    private Renderer rend;
    private InputAction hideAction;
    private InputAction debugTestAction;

    private void Start()
    {
        rend = GetComponent<Renderer>();

        if (hideMaterial == null)
        {
            Debug.LogWarning("PlayerCorporeality script on object " + gameObject.name + " does not have hideMaterial assigned! The player may look strange when hiding in a shadow.");
        }

        defaultMaterial = rend.material;
        //TEMPORARY: Use debug key 3 to hide, and debug key 4 to "use" incorporeality
        hideAction = InputSystem.actions.FindAction("Debug 3");
        debugTestAction = InputSystem.actions.FindAction("Debug 4");
    }

    private void Update()
    {
        if (debugTestAction.WasPressedThisFrame())
        {
            if (IsCorporeal())
            {
                Debug.Log("The player is corporeal! Action failed.");
            }
            else
            {
                Debug.Log("The player is incorporeal! Action succeeded; making the player corporeal.");
                SetCorporeal();
            }
        }
    }

    public void OnShadowEnter(ShadowTrigger other)
    {
        touchingShadows.Add(other.gameObject);
        SetHiding(true);
        Debug.Log("setting hiding");
    }

    public void OnShadowExit(ShadowTrigger other)
    {
        touchingShadows.Remove(other.gameObject);
        if (touchingShadows.Count == 0)
        {
            SetIncorporeal(defaultCorporealityDistance);
            SetHiding(false);
            Debug.Log("setting incorporeal");
        }
    }

    /// <summary>
    /// Makes the player incorporeal until they have moved the given distance.
    /// </summary>
    /// <param name="distance">The total distance the player can move before becoming corporeal again</param>
    public void SetIncorporeal(float distance)
    {
        ghostDistance = distance;
        SetCorporealInternal(false);
    }

    /// <summary>
    /// Sets the player to be corporeal immediately.
    /// </summary>
    public void SetCorporeal()
    {
        SetCorporealInternal(true);
    }

    /// <summary>
    /// Subtracts the given value from the player's corporeality distance "meter."
    /// If this brings it to 0, the player will become corporeal.
    /// </summary>
    /// <param name="amount">The amount to subtract</param>

    public void SubtractDistance(float amount)
    {
        ghostDistance -= amount;
        if (ghostDistance <= 0)
        {
            ghostDistance = 0;
            SetCorporealInternal(true);
        }
    }

    /// <summary>
    /// Gets whether the player is corporeal.
    /// </summary>
    /// <returns>True if the player is corporeal (default). False if not.</returns>
    public bool IsCorporeal()
    {
        return ghostDistance == 0.0f;
    }

    /// <summary>
    /// Gets whether the player is currently hiding in a shadow.
    /// </summary>
    /// <returns>True if the player is hiding, false if not</returns>
    public bool IsHiding()
    {
        return hiding;
    }

    /// <summary>
    /// Updates values based on the player's corporeality.
    /// Called by public methods when the player becomes (in)corporeal.
    /// </summary>
    /// <param name="corporeal">True for corporeal, false for incorporeal</param>
    private void SetCorporealInternal(bool corporeal)
    {
        if (corporeal)
        {
            Physics.IgnoreLayerCollision(2, 7, false);
            ghostDistance = 0;
            rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, 1f);
        }
        else
        {
            Physics.IgnoreLayerCollision(2, 7, true);
            rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, 0.4f);
        }
    }

    /// <summary>
    /// Sets whether the player is currently hiding in a shadow.
    /// Currently this toggles the player's movement, updates the player's renderer,
    /// and, if the player is becoming hidden, makes them incorporeal.
    /// </summary>
    /// <param name="isHiding">True for hiding, false for not hiding</param>
    public void SetHiding(bool isHiding)
    {
        //TODO:
        //Currently hiding just disables the player movement script which is a bit janky.
        //At some point we should add a method within movement to disable it more cleanly.
        if (isHiding == true && hiding == false) //Entering a shadow
        {
            //rend.material = hideMaterial;
            //GetComponent<PlayerMovement>().enabled = false;
        }
        else if (isHiding == false && hiding == true) //Leaving a shadow
        {
            //rend.material = defaultMaterial;
            //GetComponent<PlayerMovement>().enabled = true;
            //SetIncorporeal(shadowDistance);
        }

        hiding = isHiding;
    }
}
