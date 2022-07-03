using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public BodyPart bodyPart;
    CircleCollider2D circleCollider;

    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CheckIfGrounded()
    {
        // ~ TODO 
        bool _isGrounded = false;
        //https://docs.unity3d.com/ScriptReference/Physics2D.CircleCast.html
        return _isGrounded;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != bodyPart.gameObject)
            bodyPart.isGrounded = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != bodyPart.gameObject)
            bodyPart.isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != bodyPart.gameObject)
            bodyPart.isGrounded = false;
    }
}
