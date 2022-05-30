using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalForce : MonoBehaviour
{
    public Direction forceDirection;
    //public float forceAmount;
    public float upForce;
    public float downForce;
    public float horizontalForce;

    private Rigidbody2D rb;
    private Vector2 forceVector;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void ApplyForce(Direction direction)
    {
        forceVector = new Vector2(0, 0);    // redundant

        switch (direction)
        {
            case Direction.None:
                break;

            case Direction.Up:
                if (forceDirection == Direction.Vertical || forceDirection == Direction.Up || forceDirection == Direction.All)
                {
                    forceVector = new Vector2(0, 1);
                    rb.AddForce(forceVector * upForce);
                }
                break;

            case Direction.Down:
                if (forceDirection == Direction.Vertical || forceDirection == Direction.Down || forceDirection == Direction.All)
                {
                    forceVector = new Vector2(0, -1);
                    rb.AddForce(forceVector * downForce);
                }
                break;

            case Direction.Left:
                if (forceDirection == Direction.Horizontal || forceDirection == Direction.Left || forceDirection == Direction.All)
                {
                    forceVector = new Vector2(-1, 0);
                    rb.AddForce(forceVector * horizontalForce * Time.deltaTime);
                }
                break;

            case Direction.Right:
                if (forceDirection == Direction.Horizontal || forceDirection == Direction.Right || forceDirection == Direction.All)
                {
                    forceVector = new Vector2(1, 0);
                    rb.AddForce(forceVector * horizontalForce * Time.deltaTime);
                }
                break;

            default:
                break;
        }
    }

    void UpdateForceAmount(float _forceAmount)
    {
        // ~ TODO: have the ability to pass in force, so you can override forceAmount
        // ApplyForce, will need to ahve a section function to pass in
    }

    /*
    public void ApplyForce()
    {
        Debug.Log("Add Force: " + gameObject.name);
        rb.AddForce(forceVector * forceAmount);
    }
    */
}

public enum Direction
{
    None,
    All,
    Vertical,
    Horizontal,
    Up,
    Down,
    Left,
    Right
}
