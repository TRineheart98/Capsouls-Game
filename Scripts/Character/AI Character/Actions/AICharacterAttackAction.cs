using UnityEngine;

[CreateAssetMenu(menuName = "A.I/Actions/Attack")]
public class AICharacterAttackAction : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] private string attackAnimation;

    [Header("Combo Action")]
    public AICharacterAttackAction comboAction; //The combo action of this attack action

    [Header("Action Values")]
    [SerializeField] AttackType attackType; //Attack Type
    public int attackWeight = 50;

    //Attack can be repeated
    public float actionRecoveryTime = 0; //The time before the character can make another attack after performing this one
    public float minimumAttackAngle = -35;
    public float maximumAttackAngle = 35;
    public float minimumAttackDistance = 0;
    public float maximumAttackDistance = 2;
    public void AttemptToPerformAction(AICharacterManager aiCharacter)
    {
        aiCharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(attackType, attackAnimation, true, true);
    }
}
