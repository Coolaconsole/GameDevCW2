using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Flammable : MonoBehaviour
{
    [Header("Fire Settings")]
    public bool isOnFire = false;
    public bool isSource = false;
    public float burnTime = 5;
    public float spreadTime = 3;
    public float fireRange = 1;

    [Header("Visuals")]
    // This is the "Burnt Tree" prefab in your screenshot
    public GameObject burntObject; 
    // This is the "Flame" child object in your screenshot
    public GameObject fireOverlay; 

    private Animator animator;


    /// Initializes the component and ensures the overlay is in the correct state.

    void Start()
    {
        animator = GetComponent<Animator>();

        // Ensure the fire visual matches our initial isOnFire state
        if (fireOverlay != null)
        {
            fireOverlay.SetActive(isOnFire);
        }

        // If not on fire, we disable the animator on the main object
        if (!isOnFire)
        {
            animator.enabled = false;
        }
    }

    /// Monitors the burn and spread timers.
 
    void Update()
    {
        if (isOnFire)
        {
            // Logic for spreading fire to neighbors
            if (spreadTime <= 0 || isSource)
            {
                spreadTime = 0;
                SpreadFire();
            }
            else
            {
                spreadTime -= Time.deltaTime;
            }

            // Logic for burning out and being destroyed
            if (!isSource)
            {
                if (burnTime <= 0)
                {
                    burnTime = 0;
                    BurnOut();
                }
                else
                {
                    burnTime -= Time.deltaTime;
                }
            }
        }
    }


    /// Activates the fire state and shows the animated overlay.
  
    public void SetOnfire()
    {
        // Only trigger if not already burning
        if (!isOnFire)
        {
            isOnFire = true;
            
            // Enable the main object's animator (if you use it for the log itself)
            animator.enabled = true;

            // This activates the "Flame" child object so it becomes visible
            if (fireOverlay != null)
            {
                fireOverlay.SetActive(true);
                
                // If the Flame child has its own Animator, make sure it's enabled
                Animator overlayAnim = fireOverlay.GetComponent<Animator>();
                if (overlayAnim != null)
                {
                    overlayAnim.enabled = true;
                }
            }
        }
    }


    /// Uses a circle overlap to find and ignite nearby flammable objects.

    void SpreadFire()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;
        Collider2D[] results = new Collider2D[10];
        Physics2D.OverlapCircle(transform.position, fireRange, contactFilter, results);

        foreach (Collider2D collider in results)
        {
            if (collider != null)
            {
                Flammable flammable = collider.GetComponentInParent<Flammable>();
                if (flammable != null && !flammable.isOnFire)
                {
                    flammable.SetOnfire();
                }
            }
        }
    }

    /// Replaces the object with the burnt version and destroys this one.

    void BurnOut()
    {
        if (burntObject != null)
        {
            Instantiate(burntObject, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}