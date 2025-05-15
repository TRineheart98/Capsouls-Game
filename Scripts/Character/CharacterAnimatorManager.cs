using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    int horizontal;
    int vertical;

    [Header("Damage Animations")]
    public string lastDamageAnimationPlayed;
    //Forward damage reaction animations
    [SerializeField] string hit_Forward_Medium_01 = "Hit_Forward_Medium_01";
    [SerializeField] string hit_Forward_Medium_02 = "Hit_Forward_Medium_02";
    //Backward damage reaction animations
    [SerializeField] string hit_Backward_Medium_01 = "Hit_Backward_Medium_01";
    [SerializeField] string hit_Backward_Medium_02 = "Hit_Backward_Medium_02";
    //Left damage reaction animations
    [SerializeField] string hit_Left_Medium_01 = "Hit_Left_Medium_01";
    [SerializeField] string hit_Left_Medium_02 = "Hit_Left_Medium_02";
    //Right damage reaction animations
    [SerializeField] string hit_Right_Medium_01 = "Hit_Right_Medium_01";
    [SerializeField] string hit_Right_Medium_02 = "Hit_Right_Medium_02";

    public List<string> forward_Medium_Damage = new List<string>();
    public List<string> backward_Medium_Damage = new List<string>();
    public List<string> left_Medium_Damage = new List<string>();
    public List<string> right_Medium_Damage = new List<string>();

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    protected virtual void Start()
    {
        forward_Medium_Damage.Add(hit_Forward_Medium_01);
        forward_Medium_Damage.Add(hit_Forward_Medium_02);

        backward_Medium_Damage.Add(hit_Backward_Medium_01);
        backward_Medium_Damage.Add(hit_Backward_Medium_02);

        left_Medium_Damage.Add(hit_Left_Medium_01);
        left_Medium_Damage.Add(hit_Left_Medium_02);

        right_Medium_Damage.Add(hit_Right_Medium_01);
        right_Medium_Damage.Add(hit_Right_Medium_02);
    }    

    public string GetRandomAnimationFromList(List<string> animationList)
    {
        List<string> finalList = new List<string>();

        foreach (var item in animationList)
        {
            finalList.Add(item);
        }

        //Check if we have played a recent animation
        finalList.Remove(lastDamageAnimationPlayed);

        //Check the list for null entries and remove them
        for (int i = finalList.Count - 1; i > -1; i--)
        {
            if (finalList[i] == null)
            {
                finalList.RemoveAt(i);
            }
        }

        int randomValue = Random.Range(0, finalList.Count);

        return finalList[randomValue];
    }

    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float snappedHorizontal;
        float snappedVertical;

        //This if chain will round the horizontal movement to -1, -0.5, 0, 0.5 or 1
        if (horizontalMovement > 0 && horizontalMovement <= 0.5f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.5f && horizontalMovement <= 1)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement >= -0.5f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.5f && horizontalMovement >= -1)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }

        //This if chain will round the vertical movement to -1, -0.5, 0, 0.5 or 1 
        if (verticalMovement > 0 && verticalMovement <= 0.5f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.5f && verticalMovement <= 1)
        {
            snappedVertical = 1;
        }
        else if (verticalMovement < 0 && verticalMovement >= -0.5f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.5f && verticalMovement >= -1)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }

        character.animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    public virtual void PlayTargetActionAnimation(
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootMotion = false,
        bool canRotate = false,
        bool canMove = false)
    {
        //Debug.Log("PLAYING ANIMATION: " +  targetAnimation);  //For debugging animations
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;

        //Tell the server/host we played an animation, and to play that animation for everbody else present within the game world
        character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

    public virtual void PlayTargetAttackActionAnimation(AttackType attackType,
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootMotion = false,
        bool canRotate = false,
        bool canMove = false)
    {
        character.characterCombatManager.currentAttackType = attackType;
        character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;

        //Tell the server/host we played an animation, and to play that animation for everbody else present within the game world
        character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

    public virtual void EnableCanDoCombo()
    {

    }

    public virtual void DisableCanDoCombo()
    {

    }
}
