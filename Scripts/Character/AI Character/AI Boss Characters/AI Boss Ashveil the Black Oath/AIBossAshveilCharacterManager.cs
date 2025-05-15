using UnityEngine;

public class AIBossAshveilCharacterManager : AIBossCharacterManager
{
    [HideInInspector] public AIBossAshveilSoundFXManager ashveilSoundFXManager;
    [HideInInspector] public AIBossAshveilCombatManager ashveilCombatManager;

    protected override void Awake()
    {
        base.Awake();

        ashveilSoundFXManager = GetComponent<AIBossAshveilSoundFXManager>();
        ashveilCombatManager = GetComponent<AIBossAshveilCombatManager>();
    }
}
