using UnityEngine;

public class Breakable : MonoBehaviour
{

    //object to replace with when broken - should just be a decal
    public GameObject brokenObject;

    public void Break()
    {
        if (brokenObject != null)
        {
            Instantiate(brokenObject, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
