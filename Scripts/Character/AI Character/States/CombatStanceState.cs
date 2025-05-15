using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I/States/Combat Stance")]
public class CombatStanceState : AIState
{
    //1.Select an attack for the attack state, depending on distance and angle of the target in relation to the character
    [Header("Attacks")]
    public List<AICharacterAttackAction> aiCharacterAttacks;    //A list of all possible attacks this character can do
    private List<AICharacterAttackAction> potentialAttacks;     //All attacks possible in this situation (based on angle and distance etc..)
    private AICharacterAttackAction chosenAttack;
    private AICharacterAttackAction previousAttack;
    protected bool hasAttack = false;

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false;    //If the character can perform a combo attack, after the initial attack
    [SerializeField] protected int chanceToPerformCombo = 25;   //The chance (in percent) of the character to perform a combo on the next attack
    protected bool hasRolledForComboChange = false;      //If we already rolled for the chance during this state

    [Header("Engagement Distance")]
    [SerializeField] public float maximumEngagementDistance = 10; //Distance we have to be away from the target, before we enter the pursue target state

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return this;

        if (!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;

        //If you want the AI character to face and turn towards its target when its outside its FOV include this
        if (aiCharacter.aiCharacterCombatManager.enablePivot)
        {
            if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }
        }

        //Rotate to face our target
        aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

        //If our target is no longer present, switch back to idle
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idle);

        //If we do not have an attack, get one
        if (!hasAttack)
        {
            GetNewAttack(aiCharacter);
        }
        else
        {
            aiCharacter.attack.currentAttack = chosenAttack;
            //Roll for combo chance
            return SwitchState(aiCharacter, aiCharacter.attack);
        }

        //If we are outside of the combat engagement distance, switch to pursue target
        if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
            return SwitchState(aiCharacter, aiCharacter.pursueTarget);

        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aiCharacter)
    {
        potentialAttacks = new List<AICharacterAttackAction>();

        //1. Sort through all possible attacks                 
        foreach (var potentialAttack in aiCharacterAttacks)
        {
         //2.Remove attacks that cant be used in this situation (Based on angle and distance)

            //If we are too close, contiue on to the next attack
            if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                continue;
            //If we are too far, continue on to the next attack
            if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                continue;
            //If not within minimum field of view, continue on to the next attack
            if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;
            //If not within maximum field of view, continue on to the next attack
            if (potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;

         //3.Place remaining attacks into a list
            potentialAttacks.Add(potentialAttack);
        }
        //A way out of our loop just incase we dont find an attack
        if (potentialAttacks.Count <= 0)
            return;

        var totalWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            totalWeight += attack.attackWeight;
        }

        var randomWeightValue = Random.Range(0, totalWeight + 1);
        var processedWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            processedWeight += attack.attackWeight;

            if (randomWeightValue <= processedWeight)
            {
                //This is the attack
                chosenAttack = attack;
                previousAttack = chosenAttack;
                hasAttack = true;
                return;
            }
        }

        //4.Select this attack and pass it to the attack state
    }

    protected virtual bool RollForOutcomeChance(int outcomeChance)
    {
        bool outcomeWillBePerformed = false;

        int randomPercentage = Random.Range(0, 100);

        if (randomPercentage < outcomeChance)
            outcomeWillBePerformed = true;

        return outcomeWillBePerformed;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);
        hasAttack = false;
        hasRolledForComboChange = false;
    }


    //2.Process any combat logic here whilst, waiting to attack (blocking, strafing, dodging)
    //3.If target moves out of combat range, switch to pursue target
    //4.If target is no longer present, switch to idle state

}

