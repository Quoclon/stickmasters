using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    public Body body;

    void Start()
    {
        // Setup to Avoid collisions with self
        AvoidInternalCollisions();

        // Used if we want to ignore whole layers
        //Physics2D.IgnoreLayerCollision(3, 6);
    }

    // Avoid colliding with Players own Collisions (i.e. own weapons/bodyParts)
    public void AvoidInternalCollisions()
    {
        var colliders = body.colliders;

        for (int i = 0; i < colliders.Length; i++)
        {
            for (int k = i + 1; k < colliders.Length; k++)
            {
                Physics2D.IgnoreCollision(colliders[i], colliders[k]);
            }
        }
    }

    // ~ Is this just a redundency incase the above doesn't work?  Adds items if missed?
    private void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log("Collision Test");

        if (coll.gameObject.tag == "Player")
        {
            Debug.Log("Player Collision");
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), coll.gameObject.GetComponent<Collider2D>());
        }
    } 
}
