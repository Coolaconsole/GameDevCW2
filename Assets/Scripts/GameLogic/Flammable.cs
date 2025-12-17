using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof (Animator))]
public class Flammable : MonoBehaviour
{

    //if the current flammable object is on fire
    public bool isOnFire = false;
    //if the fire is a source it will never burn out
    public bool isSource = false;
    //how long the object will burn for before being destroyed
    public float burnTime = 5;
    //how long into burning that other flammable objects nearby will catch fire
    public float spreadTime = 3;
    //how many tiles away the fire will spread when spreadTime expires - recomend have equal to grid size
    public float fireRange = 1;
    //object to replace with when fully burnt - should just be a decal
    public GameObject burntObject;

    Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator> ();
        if (!isOnFire) {
            animator.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnFire)
        {
            if (spreadTime <= 0 || isSource)
            {
                spreadTime = 0;
                SpreadFire();
            }
            else
            {
                spreadTime -= Time.deltaTime;
            }

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

    public void SetOnfire()
    {
        if (!isOnFire)
        {
            isOnFire = true;
            animator.enabled = true;
            //animator.contro
            //spriteRenderer.sprite = onFireSprite;
        }
    }

    //Spead fire to nearby flammable objects
    void SpreadFire()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false; //may not want this
        Collider2D[] results = new Collider2D[5];
        Physics2D.OverlapCircle(transform.position, fireRange,contactFilter, results);

        foreach(Collider2D collider in results)
            {
            if (collider != null)
            {
                Flammable flammable = collider.GetComponentInParent<Flammable>();
                if (flammable != null)
                {
                    flammable.SetOnfire();
                }
            }
        }
    }

    //Destorys the object when the fire has fully burnt out
    void BurnOut()
    {
        if (burntObject != null)
        {
            Instantiate(burntObject,transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
