using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Movement : MonoBehaviour
{

    [Header("Player Type")]
    public Players playerType;

    [Header("Body")]
    Body body;

    [Header("Legs")]                // ~ TODO: Be more explicit about which part of left leg or use body
    public GameObject leftLeg;
    public GameObject rightLeg;
    Rigidbody2D leftLegRB;
    Rigidbody2D rightLegRB;
    HingeJoint2D leftLegHinge;
    HingeJoint2D rightLegHinge;

    [Header("Animation")]
    public bool offsetHorizontalMovement;
    Animator anim;
    bool facingRight;

    [Header("Input")]
    bool isMovingHorizontal;
    bool isButtonDownLeft;
    bool isButtonDownRight;

    [Header("Forces")]
    [SerializeField] float speed = 2f;
    //[SerializeField] float jumpHeight = 2f;
    //[SerializeField] float duckAmt = 20f;
    [SerializeField] float legWait = .5f;

    [Header("Jump Cooldown")]
    [SerializeField] float jumpCooldownMax;
    [SerializeField] float jumpCooldown;

    [Header("Enemy")]
    public float enemyActionTimerMax;
    public float enemyActionTimer;
    Movement targetPlayerMovement;

    [Header("Head - for facing direct")]
    public Transform headTransform;
    public Transform headTransformOriginal;


    // Start is called before the first frame update
    void Start()
    {
        // Used for Facing Direction
        headTransform = headTransformOriginal;

        facingRight = true;

        // Setup Body
        body = GetComponent<Body>();

        // Setup Lets ~ Can this be simplified? Can it be in each body part? Tasked to Body.cs
        leftLegRB = leftLeg.GetComponent<Rigidbody2D>();
        rightLegRB = rightLeg.GetComponent<Rigidbody2D>();
        leftLegHinge = leftLegRB.GetComponent<HingeJoint2D>();
        rightLegHinge = rightLegRB.GetComponent<HingeJoint2D>();

        // Assign Animator
        anim = GetComponent<Animator>();

        // Start Enemy with 0 jump timer
        ResetEnemyTimer();
        //ResetJumpCooldown();
        jumpCooldown = 0;

        //Enemy AI Setup - Move Later                                                   /// JUST CHANGTED THIS COLD CAUSE BOYGS
        if(body.playerType == Players.AI)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                Body playerBody = player.GetComponent<Body>();              /// JUST CHANGTED THIS COLD CAUSE BOYGS
                if (playerBody.playerType == Players.P1)
                {
                    targetPlayerMovement = playerBody.GetComponent<Movement>();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Inst.isGameOver && GameManager.Inst.playerWinner != playerType)
            return;

        if (body.alive == false)
            return;

        // Check what Direction Players are Facing
        CheckFacingDirection();

        // Check Acions
        if (body.playerType == Players.P1)
        {
            // Player1 Actions
            if (Input.GetAxis("Horizontal") != 0)
                CheckMovement();
            else
                anim.Play("idle");

            CheckJump();
            CheckDuck();
        }
        else
        {   
            // AI Actions
            CheckEnemyActions();  
        }
  
    }

    public void PressButtonLeft()
    {
        Debug.Log("isButtonDownLeft: " + isButtonDownLeft);
        isButtonDownLeft = true;
        Debug.Log("isButtonDownLeft: " + isButtonDownLeft);
        CheckMovement();
        Debug.Log("isButtonDownLeft: " + isButtonDownLeft);
        isButtonDownLeft = false;
        Debug.Log("isButtonDownLeft: " + isButtonDownLeft);

    }

    public void PressButtonRight()
    {
        isButtonDownRight = true;
        CheckMovement();
        isButtonDownRight = false;
    }


    void CheckFacingDirection()
    {

        if (facingRight && headTransform != null)
        {
            headTransform.localScale = new Vector3(1, headTransformOriginal.localScale.y, headTransformOriginal.localScale.z);
        }

        if (!facingRight && headTransform != null)
        {
            headTransform.localScale = new Vector3(-1, headTransformOriginal.localScale.y, headTransformOriginal.localScale.z);
        }
    }

    void CheckEnemyActions()
    {
        enemyActionTimer -= Time.deltaTime;
        jumpCooldown -= Time.deltaTime;

        if (enemyActionTimer >= 0)
            return;

        int randomInt = Random.Range(0, 100);

        if(targetPlayerMovement != null && randomInt > 0)
        {
            Vector2 direction = targetPlayerMovement.headTransform.position - this.headTransform.position;

            int bonusMoveInt = Random.Range(0, 100);
            if (direction.x < 0)
            {
                //Debug.Log("Moving Left");
                ActionMoveLeft();
                ActionMoveLeft();
                ActionMoveLeft();
                ActionMoveLeft();
                ActionMoveLeft();
                ActionMoveLeft();
                
                if (bonusMoveInt > 80)
                {
                    ActionMoveDuck();
                }

                else if (bonusMoveInt > 70 && jumpCooldown <= 0)
                {
                    ActionMoveJump();
                }
            }

            // These are backwards... maybe due to facing
            else
            {
                //Debug.Log("Moving Right");
                ActionMoveRight();
                ActionMoveRight();
                ActionMoveRight();
                ActionMoveRight();
                ActionMoveRight();
                ActionMoveRight();
         
                if (bonusMoveInt > 80)
                {
                    ActionMoveDuck();
                }

                else if (bonusMoveInt > 70 && jumpCooldown <= 0)
                {
                    ActionMoveJump();
                }
            }
        }

        ResetEnemyTimer();
    }


    void CheckMovement()
    {
        // ~ TODO: Have a bool for Movement Based on EITHER (Axis Raw OR Button Push)
        if (Input.GetAxis("Horizontal") > 0)
            ActionMoveRight();
        else
            ActionMoveLeft();

        // Mobile Controls?
        if (isButtonDownLeft)
            ActionMoveLeft();

        if (isButtonDownRight)
            ActionMoveRight();
    }

    void CheckJump()
    {
        jumpCooldown -= Time.deltaTime;

        if (jumpCooldown > 0)
            return;

        // Player Jumping
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {    
            ActionMoveJump();
        }
    }

    void CheckDuck()
    {
        // Player Jumping
        if (Input.GetKeyDown(KeyCode.S))
        {
            ActionMoveDuck();
        }
    }
    

    public void ActionMoveJump()
    {
        body.AddDirectionalForceToRelevantBodyParts(Direction.Up);
        /*
        if (bodyPartExists(leftLegRB))
            if (leftLegHinge.enabled)
                leftLegRB.AddForce(Vector2.up * (jumpHeight * 1000));

        if (bodyPartExists(rightLegRB))
            if (rightLegHinge.enabled)
                rightLegRB.AddForce(Vector2.up * (jumpHeight * 1000));
       */
        ResetJumpCooldown();
    }

    public void ActionMoveDuck()
    {
        body.AddDirectionalForceToRelevantBodyParts(Direction.Down);

        /*
        if (bodyPartExists(leftLegRB))
            if(leftLegHinge.enabled)
                leftLegRB.AddForce(Vector2.down * (duckAmt * 1000));

        if (bodyPartExists(rightLegRB))
            if (rightLegHinge.enabled)
                rightLegRB.AddForce(Vector2.down * (duckAmt * 1000));
        */

    }

    public void ActionMoveRight()
    {
        facingRight = true;
        anim.Play("WalkRight");
        //StartCoroutine(MoveRight(legWait));
        //legWait = .25f;

        // ~ A little nicer animations, but requires identifying Left/Right legs in Body
        if(offsetHorizontalMovement)
            body.AddDirectionalForceToRelevantBodyParts(Direction.Right, legWait);
        else
        body.AddDirectionalForceToRelevantBodyParts(Direction.Right);      
    }

    public void ActionMoveLeft()
    {
        facingRight = false;
        anim.Play("WalkLeft");
        //StartCoroutine(MoveLeft(legWait));
        //legWait = .25f;

        // ~ A little nicer animations, but requires identifying Left/Right legs in Body
        if (offsetHorizontalMovement)
            body.AddDirectionalForceToRelevantBodyParts(Direction.Left, legWait);
        else
            body.AddDirectionalForceToRelevantBodyParts(Direction.Left);
    }


    void ResetEnemyTimer()
    {
        // Default - In case I forget to add an ActionTimer on the gameObject
        if (enemyActionTimerMax <= 0)
            enemyActionTimerMax = 0.25f;

        enemyActionTimer = enemyActionTimerMax;
    }

    void ResetJumpCooldown()
    {
        jumpCooldown = jumpCooldownMax;
    }

    #region Archive
    IEnumerator MoveRight(float seconds)
    {
        //~ TODO: move this to body, pass in part to move, the body part can have it's own force knowledge
        // could be like body.TorqueObject, body.MoveObject(body.leftLeg, force)
        if (bodyPartExists(leftLegRB))
            if (leftLegHinge.enabled)
                leftLegRB.AddForce(Vector2.right * (speed * 1000) * Time.deltaTime);

        yield return new WaitForSeconds(seconds);

        if (bodyPartExists(rightLegRB))
            if (rightLegHinge.enabled)
                rightLegRB.AddForce(Vector2.right * (speed * 1000) * Time.deltaTime);
    }
    IEnumerator MoveLeft(float seconds)
    {
        if (bodyPartExists(rightLegRB))
            if (rightLegHinge.enabled)
                rightLegRB.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);

        yield return new WaitForSeconds(seconds);

        if (bodyPartExists(leftLegRB))
            if (leftLegHinge.enabled)
                leftLegRB.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);
    }

    bool bodyPartExists(Rigidbody2D bodyPart)
    {
        if (bodyPart == null)
            return false;

        return true;
    }
    #endregion
}
