using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Movement : MonoBehaviour
{

    [Header("Player Type")]
    //public Players playerType;

    [Header("Body")]
    Body body;

    [Header("Legs - ARCHIVED?")]                // ~ TODO: Be more explicit about which part of left leg or use body
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

    [Header("Input - Mobile")]
    public bool mobileCanJump;
    public bool mobileCanDuck;
    public VariableJoystick variableJoystick;

    [Header("Input")]
    float moveX = 0f;
    float moveY = 0f;
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
    public Movement targetPlayerMovement;

    [Header("Head - for facing direct")]
    public Transform headTransform;
    public Transform headTransformOriginal;


    // Start is called before the first frame update
    void Start()
    {
        // Direction
        moveX = 0f;
        moveY = 0f;

        // Used for Facing Direction
        headTransform = headTransformOriginal;

        facingRight = true;

        // Setup Body
        body = GetComponent<Body>();

        if(GameManager.Inst != null)
        {
            // ~Default to "optionsMobileControls" for all players, but not NPC's ~ TODO: Commented out because of double-jumping for NPCs; Fix this
            //body.optionMobileControls = GameManager.Inst.isPlayerTypePlayer(body.playerType);
            body.useJumpTimer = !GameManager.Inst.isPlayerTypePlayer(body.playerType);
        }

        if (body.optionMobileControls == true)
        {
            mobileCanDuck = true;
            mobileCanJump = true;
        }
        else
        {
            mobileCanDuck = false;
            mobileCanJump = false;
        }

        // Setup Lets ~ Can this be simplified? Can it be in each body part? Tasked to Body.cs
        leftLegRB = leftLeg.GetComponent<Rigidbody2D>();
        rightLegRB = rightLeg.GetComponent<Rigidbody2D>();
        leftLegHinge = leftLegRB.GetComponent<HingeJoint2D>();
        rightLegHinge = rightLegRB.GetComponent<HingeJoint2D>();

        // Assign Animator
        anim = GetComponent<Animator>();

        ResetEnemyTimer();

        // Start Enemy with 0 jump timer
        jumpCooldown = 0;
        ResetJumpCooldown();



        //Enemy AI Setup - Move Later                                                   /// JUST CHANGTED THIS COLD CAUSE BOYGS
        if (body.playerType == Players.AI)
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
        else if(body.playerType == Players.P1)
        {
            if(GameManager.Inst != null)
                variableJoystick = GameManager.Inst.variableJoystickP1;
        }
        else if (body.playerType == Players.P2)
        {
            if (GameManager.Inst != null)
                variableJoystick = GameManager.Inst.variableJoystickP2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Disable Movement if Main Menu
        if (GameManager.Inst == null)
            return;

        //if (GameManager.Inst.isRoundOver && playerType == Players.AI)
        //body.DisableAllVelocity();

        // Stop Input/AI Input if you are NOT the winner player (i.e. Player, or last NPC Striker)
        if(GameManager.Inst != null)
        {
            if (GameManager.Inst.isRoundOver && GameManager.Inst.playerWinner != body.playerType)
                return;
        }

        if (body.alive == false)
            return;


        // Check what Direction Players are Facing
        CheckFacingDirection();

        if (body.playerType == Players.AI)
        {
            CheckEnemyActions();
            return;
        }


        // Player P1 and Player P1 Checks //

        // Handle Players
        HandleInputs();

        // Check Movement
        CheckMovement();

        // Checks for Input, but also handles Jump Counter countdown
        CheckJump();

        // Checks for Duck
        CheckDuck();
    }

    void HandleInputs()
    {
        if (body.playerType == Players.P1)
        {
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");
        }

        if (body.playerType == Players.P2)
        {
            moveX = Input.GetAxis("Horizontal1");
            moveY = Input.GetAxis("Vertical1");
        }

        if (body.playerType == Players.P3)
        {
            moveX = Input.GetAxis("Horizontal1");
            moveY = Input.GetAxis("Vertical1");
        }

        if (body.playerType == Players.P4)
        {
            moveX = Input.GetAxis("Horizontal1");
            moveY = Input.GetAxis("Vertical1");
        }

        // Add Mobiles Directions if available (overwrite the keys by default)
        if (variableJoystick != null)
        {
            if (variableJoystick.Horizontal > variableJoystick.DeadZone || variableJoystick.Horizontal < -variableJoystick.DeadZone)
                moveX = variableJoystick.Horizontal;

            if (variableJoystick.Vertical > variableJoystick.DeadZone || variableJoystick.Vertical < -variableJoystick.DeadZone)
                moveY = variableJoystick.Vertical;
        }

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
            //Debug.Log("targetPlayerMovement.headTransform.position " + targetPlayerMovement.headTransform.position);
            //Debug.Log("this.headTransform.position " + this.headTransform.position);

            Vector2 direction = targetPlayerMovement.headTransform.position - this.headTransform.position;

            //Debug.Log("direction " + direction);

            // ~ TODO: Set this up in inspector via AI
            int chanceToMoveInOppositeDirectionOfPlayer = Random.Range(0, 100);

            // What additional Actions to Take after moving
            int bonusMoveInt = Random.Range(0, 100);

            if (direction.x < 0 && chanceToMoveInOppositeDirectionOfPlayer > 10)
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
        if (moveX != 0)
        {
            if(moveX > 0)
                ActionMoveRight();
            else
                ActionMoveLeft();
        }
        else
        {
            anim.Play("idle");
        }
    }

    void CheckJump()
    {
        jumpCooldown -= Time.deltaTime;

        if (jumpCooldown > 0 && body.useJumpTimer)
            return;

        if (moveY <= 0)
            return;

        if (body.optionMobileControls && !mobileCanJump)
            return;

        ActionMoveJump();
    }

    void CheckDuck()
    {

        if (moveY >= 0)
            return;

        if (body.optionMobileControls && !mobileCanDuck)
            return;

        ActionMoveDuck();

    }
    

    public void ActionMoveJump()
    {
        body.AddDirectionalForceToRelevantBodyParts(Direction.Up);

        if (body.optionMobileControls)
            StartCoroutine(MobileJumpCooldown(.5f));
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

        if(body.optionMobileControls)
            StartCoroutine(MobileDuckCooldown(.5f));

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
        if (body.useJumpTimer)
        {
            jumpCooldown = jumpCooldownMax;
        }
        else
        {
            jumpCooldown = 0;
        }
        
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
    IEnumerator MobileJumpCooldown(float seconds)
    {
        mobileCanJump = false;
        yield return new WaitForSeconds(seconds);
        mobileCanJump = true;
    }

    IEnumerator MobileDuckCooldown(float seconds)
    {
        mobileCanDuck = false;
        yield return new WaitForSeconds(seconds);
        mobileCanDuck = true;
    }


    bool bodyPartExists(Rigidbody2D bodyPart)
    {
        if (bodyPart == null)
            return false;

        return true;
    }
    #endregion
}
