using UnityEngine;

public class CharacterLocomotionManager : MonoBehaviour 
{
    CharacterManager character;

    [Header("Flags")]
    public bool isRolling = false;

    [Header("Ground Check & Jumping")]
    [SerializeField] protected float gravityForce = -40;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius = 0.3f;
    [SerializeField] protected Vector3 yVelocity;   //The force at which our character is pulled up or down (Jumping or Falling)
    [SerializeField] protected float groundedYVelocity = -20;    //The force at which they are sticking to the ground
    [SerializeField] protected float fallStartYVelocity = -5;   //The force at which our character begins to fall when they become ungrounded
    protected bool fallingVelocityHasBeenSet = false;
    protected float inAirTimer = 0;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        HandleGroundCheck();
    }

    protected void HandleGroundCheck()
    {
        character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);

        if (character.isGrounded )
        {
            //If we are not attempting to jump or move upward
            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            //If we are not jumping, our falling velocity has not been set
            if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity;
            }

            inAirTimer += Time.deltaTime;
            character.animator.SetFloat("InAirTimer", inAirTimer);
            yVelocity.y += gravityForce * Time.deltaTime;
        }
        //So we always have a force pulling us down
        character.characterController.Move(yVelocity * Time.deltaTime);
    }

    //Draws our ground sphere in the scene view
    //protected void OnDrawGizmosSelected()
    //{
       // Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
    //}

    public void EnableCanRotate()
    {
        character.canRotate = true;
    }

    public void DisableCanRotate()
    {
        character.canRotate = false;
    }
}
