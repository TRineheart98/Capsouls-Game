using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Idle")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.characterCombatManager.currentTarget != null)
        {
            //Return the pursue target state (Change the state to pursue the target)
            return SwitchState(aiCharacter, aiCharacter.pursueTarget);
        }
        else
        {
            //Return this state, to continually search for a target (Keep this state, until a target is found)
            aiCharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);
            return this;
        }
    }


}
