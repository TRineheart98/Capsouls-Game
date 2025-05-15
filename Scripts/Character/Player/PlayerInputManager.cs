using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;



public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    public PlayerManager player;
    // Think about goals in steps
    // 1. Find a way to read the value of a joy stick
    // 2.Move character based on those values

    PlayerControls playerControls;

    [Header("Player Camera Input")]
    [SerializeField] Vector2 camera_Input;
    public float cameraVertical_Input;
    public float cameraHorizontal_Input;

    [Header("Lock On Input")]
    [SerializeField] bool lockOn_Input;
    [SerializeField] bool lockOn_Left_Input;
    [SerializeField] bool lockOn_Right_Input;
    private Coroutine lockOnCoroutine;

    [Header("Player Movement Input")]
    [SerializeField] Vector2 movementInput;
    public float vertical_Input;
    public float horizontal_Input;
    public float moveAmount;

    [Header("Player Action Input")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;
    [SerializeField] bool use_Item_Input = false;

    [Header("Bumper Inputs")]
    [SerializeField] bool RB_Input = false;

    [Header("Trigger Inputs")]
    [SerializeField] bool RT_Input = false;
    [SerializeField] bool Hold_RT_Input = false;

    [Header("D-Pad Inputs")]
    [SerializeField] bool switch_Left_Weapon_Input = false;
    [SerializeField] bool switch_Right_Weapon_Input = false;

    [Header("Pause Input")]
    [SerializeField] bool pause_Input = false;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        //When the scene changes, run this logic.
        SceneManager.activeSceneChanged += OnSceneChange;

        instance.enabled = false;

        if (playerControls != null)
        {
            playerControls.Disable();
        }

    }
    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        //If we are loading into our world scene, enable our player input controls.
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;

            if (playerControls != null)
            {
                playerControls.Enable();
            }

        }
        //Otherwise we must be at the main menu, disable our player controls.
        // This is so our player cant move around if we enter things like a character creation menu etc.
        else
        {
            instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }
    }

    private void OnEnable() //Making sure PlayerControls is enabled and we are calling on it.
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.UI.Enable();

            //Sticks 
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>(); //Getting input from PlayerControls Player Movement input actions
            playerControls.PlayerCamera.Movement.performed += i => camera_Input = i.ReadValue<Vector2>(); //Getting input from PlayerControls Camera Movement input actions

            //Actions
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.X.performed += i => use_Item_Input = true;

            //Bumpers
            playerControls.PlayerActions.RB.performed += i => RB_Input = true;

            //Triggers
            playerControls.PlayerActions.RT.performed += i => RT_Input = true;
            playerControls.PlayerActions.HoldRT.performed += i => Hold_RT_Input = true;
            playerControls.PlayerActions.HoldRT.canceled += i => Hold_RT_Input = false;

            //D_Pads
            playerControls.PlayerActions.SwitchLeftWeapon.performed += i => switch_Left_Weapon_Input = true;
            playerControls.PlayerActions.SwitchRightWeapon.performed += i => switch_Right_Weapon_Input = true;

            // Lock on input
            playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;
            playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => lockOn_Left_Input = true;
            playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => lockOn_Right_Input = true;

            //Since sprinting is a hold button action we are going to program it as a performed and canceled.
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

            //Pause
            playerControls.UI.Pause.performed += i => pause_Input  = true;
        }
        playerControls.Enable();
    }

    private void OnDestroy()
    {
        // If we destroy this object, unsubscribe from this event.
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    //If we minimize the game, controls will not read.
    private void OnApplicationFocus(bool focus)
    {
        if(enabled)
        {
            if(focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }

    private void Update()
    {
        HandleAllInputs();
    }

    private void HandleAllInputs()
    {
        HandleUseItemInput();
        HandleLockOnInput();
        HandleLockOnSwitchTargetInput();
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();
        HandleRBInput();
        HandleRTInput();
        HandleChargeRTInput();
        HandleSwitchRightWeaponInput();
        HandleSwitchLeftWeaponInput();
        HandlePauseInput();
    }

    //Use item
    private void HandleUseItemInput()
    {
        if (use_Item_Input)
        {
            use_Item_Input = false;

            if (player.playerInventoryManager.currentQuickSlotItem != null)
            {
                player.playerInventoryManager.currentQuickSlotItem.AttemptToUseItem(player);
            }
        }
    }

    //LOCK ON FUNCTIONS
    private void HandleLockOnInput()
    {
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerCombatManager.currentTarget == null)
                return;

            if (player.playerCombatManager.currentTarget.isDead.Value)
            {
                player.playerNetworkManager.isLockedOn.Value = false;
            }

            //Attempt to find a new target

            //To insure our coroutine doesnt run multiple times over
            if (lockOnCoroutine != null)
                StopCoroutine(lockOnCoroutine);
            lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
        }

        if (lockOn_Input && player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;
            PlayerCamera.instance.ClearLockOnTargets();
            player.playerNetworkManager.isLockedOn.Value = false;
            //Disable Lock On
            return;
        }

        if (lockOn_Input && !player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;

            //If we are using a range weapon return (we dont want to lock on while aiming)

            //Enable Lock On
            PlayerCamera.instance.HandleLocatingLockOnTargets();

            if (PlayerCamera.instance.nearestLockOnTarget != null)
            {
                // Set the target as our current target
                player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }
        }
    }

    private void HandleLockOnSwitchTargetInput()
    {
        if (lockOn_Left_Input)
        {
            lockOn_Left_Input = false;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.leftLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }

        if (lockOn_Right_Input)
        {
            lockOn_Right_Input = false;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                }
            }
        }
    }

    //MOVEMENT FUNCTIONS
    private void HandlePlayerMovementInput()
    {
        vertical_Input = movementInput.y; //Getting the y value of our movementInput and assigning it to this variable
        horizontal_Input = movementInput.x;  //Getting the x value of our movementInput and assigning it to this variable

        //Return the absolute number (Meaning number without the negative sign, so its always positive)
        moveAmount = Mathf.Clamp01(Mathf.Abs(vertical_Input) + Mathf.Abs(horizontal_Input));

        //Clamping values so they are 0, 0.5, or 1
        if(moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f; //Walking
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1; //Running
        }

        //Why do we pass 0 on the horizontal? Because we only want non-strafing movement
        // We use the horizontal when we are strafing or locked on

        if(player == null)
        {
            return;
        }

        if (moveAmount != 0)
        {
            player.playerNetworkManager.isMoving.Value = true;
        }
        else
        {
            player.playerNetworkManager.isMoving.Value = false;
        }

        //If not locked on, pass the move amount
        if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
        }
        //If locked on, pass horizontal and vertical values
        else
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontal_Input, vertical_Input, player.playerNetworkManager.isSprinting.Value);
        }


    }

    private void HandleCameraMovementInput()
    {
        cameraVertical_Input = camera_Input.y;    //Getting the y value of our cameraInput and assigning it to this variable
        cameraHorizontal_Input = camera_Input.x;  //Getting the x value of our cameraInput and assigning it to this variable


    }

    //ACTION FUNCTIONS
    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;
            // Future Note: Return if menu or ui window is open

            //Perform the dodge
            player.playerLocomotionManager.AttemptToPerformDodge();
        }
    }

    private void HandleSprintInput()
    {
        if(sprintInput)
        {
            player.playerLocomotionManager.HandleSprinting();
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;

            //If we have ui open return

            //Attempt to perform the jump
            player.playerLocomotionManager.AttemptToPerformJump();
        }
    }

    private void HandleRBInput()
    {
        if (RB_Input)
        {
            RB_Input = false;

            player.playerNetworkManager.SetCharacterActionHand(true);

            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_RB_Action, player.playerInventoryManager.currentRightHandWeapon);
        }
    }

    private void HandleRTInput()
    {
        if (RT_Input)
        {
            RT_Input = false;

            player.playerNetworkManager.SetCharacterActionHand(true);

            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_RT_Action, player.playerInventoryManager.currentRightHandWeapon);
        }
    }

    private void HandleChargeRTInput()
    {
        //We only check if we are in an action that requires it (Attacking)
        if (player.isPerformingAction)
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerNetworkManager.isChargingAttack.Value = Hold_RT_Input;
            }
        }
    }

    private void HandleSwitchRightWeaponInput()
    {
        if (switch_Right_Weapon_Input)
        {
            switch_Right_Weapon_Input = false;
            player.playerEquipmentManager.SwitchRightWeapon();
        }
    }

    private void HandleSwitchLeftWeaponInput()
    {
        if (switch_Left_Weapon_Input)
        {
            switch_Left_Weapon_Input = false;
            player.playerEquipmentManager.SwitchLeftWeapon();
        }
    }

    private void HandlePauseInput()
    {
        if (pause_Input)
        {
            pause_Input = false;

            PlayerUIPauseMenuManager.instance.TogglePause();

        }
    }

}
