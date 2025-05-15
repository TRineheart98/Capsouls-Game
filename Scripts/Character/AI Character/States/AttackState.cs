using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Attack")]
public class AttackState : AIState
{
    [Header("Current Attack")]
    [HideInInspector] public AICharacterAttackAction currentAttack;
    [HideInInspector] public bool willPerformCombo = false;

    [Header("State Flags")]
    protected bool hasPerformedAttack = false;
    protected bool hasPerformedCombo = false;

    [Header("Pivot After Attack")]
    [SerializeField] protected bool pivotAfterAttack = false;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idle);

        if (aiCharacter.aiCharacterCombatManager.currentTarget.isDead.Value)
            return SwitchState(aiCharacter, aiCharacter.idle);

        //Rotate towards the target whilst attacking
        aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhilstAttacking(aiCharacter);

        //Set movement values to 0
        aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);

        //Perform a combo if available
        if (willPerformCombo && !hasPerformedCombo)
        {
            if (currentAttack.comboAction != null)
            {
                //hasPerformedCombo = true;
                //currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
            }
        }

        if (aiCharacter.isPerformingAction)
            return this;

        if (!hasPerformedAttack)
        {
            //If we have not performed an action, wait before performing another
           if (aiCharacter.aiCharacterCombatManager.actionRecoveryTimer > 0)
                return this;

            PerformAttack(aiCharacter);

            //Return to the top, so if we have combo we process that when we are able
            return this;
        }

        if (pivotAfterAttack)
            aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);

        return SwitchState(aiCharacter, aiCharacter.combatStance);

    }

    protected void PerformAttack(AICharacterManager aiCharacter)
    {
        hasPerformedAttack = true;
        currentAttack.AttemptToPerformAction(aiCharacter);
        aiCharacter.aiCharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasPerformedAttack = false;
        hasPerformedCombo = false;
    }
}
