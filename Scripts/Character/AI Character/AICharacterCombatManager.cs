using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{
    protected AICharacterManager aiCharacter;

    [Header("Action Recovery")]
    public float actionRecoveryTimer = 0;

    [Header("Pivot")]
    public bool enablePivot = true;

    [Header("Target Information")]
    public float distanceFromTarget;
    public float viewableAngle;
    public Vector3 targetsDirection;

    [Header("Detection")]
    [SerializeField] float detectionRadius = 15;
    public float minimumFOV = -35;
    public float maximumFOV = 35;

    [Header("Attack Rotation Speed")]
    public float attackRotationSpeed = 25;

    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
        lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
    }

    public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
    {
        if (currentTarget != null)
            return;

        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.Instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

            if (targetCharacter == null) continue;

            if (targetCharacter == aiCharacter) continue;

            if (targetCharacter.isDead.Value) continue;

            //Can I attack this target, if so, make them a target
            if (WorldUtilityManager.Instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
            {
                //If a potential target is found, it has to be infront of us
                Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float angleOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                if (angleOfPotentialTarget > minimumFOV && angleOfPotentialTarget < maximumFOV)
                {
                    // Final thing, we check for environmental blocks
                    if (Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position, WorldUtilityManager.Instance.GetEnviroLayers()))
                    {
                        Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                    }
                    else
                    {
                        targetsDirection = targetCharacter.transform.position - transform.position;
                        viewableAngle = WorldUtilityManager.Instance.GetAngleOfTarget(transform, targetsDirection);
                        aiCharacter.characterCombatManager.SetTarget(targetCharacter);

                        if (enablePivot)
                        {
                            PivotTowardsTarget(aiCharacter);
                        }
                    }
                }
            }
        }
    }

    public void PivotTowardsTarget(AICharacterManager aiCharacter)
    {

        CharacterManager currentTarget = aiCharacter.characterCombatManager.currentTarget;

        if (currentTarget == null)
            return;

        // Get direction and viewable angle
        Vector3 directionToTarget = currentTarget.transform.position - aiCharacter.transform.position;
        directionToTarget.y = 0; // Optional: Keep it horizontal
        float angle = Vector3.SignedAngle(aiCharacter.transform.forward, directionToTarget, Vector3.up);
        viewableAngle = angle;

        // Rotate manually for small angles
        if (angle >= 20 && angle <= 60)
        {
            aiCharacter.transform.rotation = Quaternion.Slerp(
                aiCharacter.transform.rotation,
                Quaternion.LookRotation(directionToTarget),
                Time.deltaTime * 5f);
        }
        else if (angle <= -20 && angle >= -60)
        {
            aiCharacter.transform.rotation = Quaternion.Slerp(
                aiCharacter.transform.rotation,
                Quaternion.LookRotation(directionToTarget),
                Time.deltaTime * 5f);
        }
        // Use animations for wider turns
        else if (angle >= 61 && angle <= 110)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
        }
        else if (angle <= -61 && angle >= -110)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
        }
        else if (angle >= 111 && angle <= 180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
        }
        else if (angle <= -111 && angle >= -180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
        }

    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }

    public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
    {
        if (currentTarget == null)
            return;

        //Check if we can rotate
        if (!aiCharacter.canRotate)
            return;

        //Check if we are performing an action
        if (!aiCharacter.isPerformingAction)
            return;

        //2.Rotate towards the target at a specified rotation tspeed during specified frames
        Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        if (targetDirection == Vector3.zero)
            targetDirection = aiCharacter.transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);


    }

    public void HandleActionRecovery(AICharacterManager aiCharacter)
    {
        if (actionRecoveryTimer > 0)
        {
            if (!aiCharacter.isPerformingAction)
            {
                actionRecoveryTimer -= Time.deltaTime;
            }
        }
    }
}
