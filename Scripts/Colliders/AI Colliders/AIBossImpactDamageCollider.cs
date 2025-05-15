using System.Collections.Generic;
using UnityEngine;

public class AIBossImpactDamageCollider : DamageCollider
{
    [SerializeField] AIBossAshveilCharacterManager ashveilCharacterManager;

    protected override void Awake()
    {
        base.Awake();

        ashveilCharacterManager = GetComponentInParent<AIBossAshveilCharacterManager>();
    }
    public void ImpactAttack()
    {
        GameObject impactVFX = Instantiate(ashveilCharacterManager.ashveilCombatManager.ashveilImpactVFX, transform.position, transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(transform.position, ashveilCharacterManager.ashveilCombatManager.ashveilImpactAOERadius, WorldUtilityManager.Instance.GetCharacterLayers());
        List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        foreach (var collider in colliders)
        {
            CharacterManager character = collider.GetComponentInParent<CharacterManager>();

            if (character != null)
            {
                if (charactersDamaged.Contains(character))
                    continue;

                if (character == ashveilCharacterManager)
                    continue;

                charactersDamaged.Add(character);

                if (character.IsOwner)
                {
                    TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                    damageEffect.physicalDamage = ashveilCharacterManager.ashveilCombatManager.ashveilImpactDamage;
                    damageEffect.poiseDamage = ashveilCharacterManager.ashveilCombatManager.ashveilImpactDamage;

                    character.characterEffectsManager.ProcessInstantEffect(damageEffect);

                }
            }
        }
    }

}
