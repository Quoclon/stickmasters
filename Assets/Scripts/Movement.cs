using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

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
    public float enemyTargeLockTimerMax;
    public float enemyTargetLockTimer;


    [Header("Head - for facing direct")]
    public Transform headTransform;
    public Transform headTransformOriginal;


    [Header("Old Input System (Multiplayer Keyboard")]
    public bool isUsingKeyboard;
    PlayerInput playerInput;



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
            TargetClosestPlayer();
        }
        else if(body.playerType == Players.P1)
        {
            if(GameManager.Inst != null)
            {
                variableJoystick = GameManager.Inst.variableJoystickP1;
                variableJoystick.SetMode(GameManager.Inst.joystickType);    // Later have this be per player and adjustable in options?
            }
        }
        else if (body.playerType == Players.P2)
        {
            if (GameManager.Inst != null)
            {
                variableJoystick = GameManager.Inst.variableJoystickP2;
                variableJoystick.SetMode(JoystickType.Fixed);
            }
        }

        // Setup Old Input Manager || Manually Assign Keyboard (based on some passed in menu variables)
        playerInput = GetComponent<PlayerInput>();
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            Debug.Log("i: " + i);
            Debug.Log(playerInput.playerIndex);
            if (playerInput.playerIndex == i)
            {
                //playerInput.SwitchCurrentControlScheme(Gamepad.all[i]);
            }
        }
        
        foreach (var input in PlayerInput.all)
        {
            /*
            Debug.Log("input.name: " + input.name);
            Debug.Log("input.playerIndex: " + input.playerIndex);
            Debug.Log("input.currentControlScheme: " + input.currentControlScheme);
            Debug.Log("input.currentActionMap: " + input.currentActionMap);
            */
            if(input.currentControlScheme == null)
            {
                //Debug.Log("No Control Sceheme Available or Assigned - isUsingKeyboard now set to True");
                isUsingKeyboard = true;
            }
            //input.SwitchCurrentControlScheme("Keyboard", Keyboard.current);
            //Debug.Log("input.currentControlScheme: " + input.currentControlScheme);




        }
    }


    void TargetWeakestPlayer()
    {
        if (GameManager.Inst == null)
            return;

        float weakestPlayerHealth = 100000f;
        Body weakestPlayerBody = body;          // Risk of causing issues targeting self - shoudl resolve below, could be issues if only AI left

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            Body playerBody = player.GetComponent<Body>();

            if (GameManager.Inst.isPlayerTypePlayer(playerBody.playerType)) // ~ PROBLEM IF YOU WANTED AI TO BATTLE EACH OTHER; Team Based later?
            {
                //Debug.Log(playerBody.playerType);
                if (playerBody.alive)
                {
                    //Debug.Log("Enemy: " + body.playerType + " PlayerBody: " + playerBody.playerType + " Health: " + playerBody.health);
                    if (playerBody.health < weakestPlayerHealth)
                    {
                        weakestPlayerBody = playerBody;
                        weakestPlayerHealth = playerBody.health;
                        //Debug.Log("weakest PlayerBody: " + weakestPlayerBody.playerType);
                    }
                }

            }
        }

        //Debug.Log("Final Weakest Player Body: " + weakestPlayerBody.playerType);
        targetPlayerMovement = weakestPlayerBody.GetComponent<Movement>();
        enemyTargetLockTimer = enemyTargeLockTimerMax;
    }

    void TargetClosestPlayer()
    {
        if (GameManager.Inst == null)
            return;

        float closestPlayerDistance = 10000f;
        Body cloestPlayerBody = body; // ~ THIS COULD CAUSE BUGS

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); 
        //Debug.Log("Enemy Body Checking: " + body.playerType);
        foreach (var player in players)
        {
            Body playerBody = player.GetComponent<Body>();             
            //if (playerBody.playerType == Players.P1)
            if (GameManager.Inst.isPlayerTypePlayer(playerBody.playerType)) // ~ PROBLEM IF YOU WANTED AI TO BATTLE EACH OTHER; Team Based later?
            {
                //Debug.Log(playerBody.playerType);
                if (playerBody.alive)
                {
                    float distance = Mathf.Abs(Vector3.Distance(body.chest.transform.position, playerBody.chest.transform.position));
                    //Debug.Log("Enemy: " + body.playerType + " PlayerBody: " + playerBody.playerType + " Distance: " + distance);
                    if (distance < closestPlayerDistance)
                    {
                        cloestPlayerBody = playerBody;
                        closestPlayerDistance = distance;
                        //Debug.Log("current cloestPlayerBody: " + cloestPlayerBody.playerType);
                    }
                }

            }
        }

        //Debug.Log("Final Closet Player Body: " + cloestPlayerBody.playerType);
        targetPlayerMovement = cloestPlayerBody.GetComponent<Movement>();
        enemyTargetLockTimer = enemyTargeLockTimerMax;
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

        // Handle Players
        HandleInputs();

        // Check Movement
        CheckMovement();

        // Checks for Input, but also handles Jump Counter countdown
        CheckJump();

        // Checks for Duck
        CheckDuck();
    }

    // PlayerInput has Maps/Actions (i.e. Movement) that get called via 'On'Movement
    // This happens whenever there is input as defined in 'Movement' of Input Asset
    public void OnMovement(InputValue value)
    {
        // Reset Input before next frame of Input
        moveX = 0;
        moveY = 0;

        // Get the Vecto2 values from the value pasesd in (i.e WASD, Left Analog Stick)     
        moveX = Mathf.RoundToInt(value.Get<Vector2>().x);
        moveY = Mathf.RoundToInt(value.Get<Vector2>().y);
    }

    void HandleInputs()
    {
        //if (!GameManager.Inst.isMobileWebGL)
            //return;

        // Also check for Player 1?
        if (!isUsingKeyboard)
            return;

        // Reset Input before next frame of Input
        moveX = 0;
        moveY = 0;

        #region Old Input System - Keyboard        
        
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
            moveX = Input.GetAxis("Horizontal2");
            moveY = Input.GetAxis("Vertical2");
        }

        if (body.playerType == Players.P4)
        {
            moveX = Input.GetAxis("Horizontal3");
            moveY = Input.GetAxis("Vertical3");
        }
        #endregion

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
        enemyTargetLockTimer -= Time.deltaTime;


        if (enemyActionTimer >= 0)
            return;

        int randomInt = Random.Range(0, 100);

        if(targetPlayerMovement != null)
        {
            if (!targetPlayerMovement.body.alive || enemyTargetLockTimer <= 0)
            {
                // Equal chance to target the clost player or the weakest player
                int randomTarget = Random.Range(0, 2);
                //Debug.Log("Random Targeting - Int: " + randomTarget);
                if (randomTarget == 0)
                    TargetClosestPlayer();
                else
                    TargetWeakestPlayer();
            }

            if (randomInt > 0)
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
        }


        ResetEnemyTimer();
    }


    void CheckMovement()
    {
        //Debug.Log("CheckMovement: X " + moveX);
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
