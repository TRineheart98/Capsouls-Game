using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AIBossAshveilCombatManager : AICharacterCombatManager
{
    AIBossAshveilCharacterManager ashveilManager;

    [Header("Damage Colliders")]
    [SerializeField] AIBossAshveilWeaponDamageCollider twoHandWeaponCollider;
    [SerializeField] AIBossImpactDamageCollider impactDamageCollider;
    public float ashveilImpactAOERadius = 1.5f;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] float attack01DamageModifier = 1.0f;
    public float ashveilImpactDamage = 25;

    [Header("VFX")]
    public GameObject ashveilImpactVFX;

    protected override void Awake()
    {
        base.Awake();
        ashveilManager = GetComponent<AIBossAshveilCharacterManager>();
    }

    public void SetAttack01Damage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
        twoHandWeaponCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }

    public void SetAttack02Damage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
    }

    public void SetAttack03Damage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
        twoHandWeaponCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }

    public void EnableTwoHandWeaponDamageCollider()
    {
        twoHandWeaponCollider.EnableDamageCollider();
        ashveilManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(ashveilManager.ashveilSoundFXManager.swordWooshes));
    }

    public void DisableTwoHandWeaponDamageCollider()
    {
        twoHandWeaponCollider.DisableDamageCollider();
    }

    public void ActivateAshveilSwordImpact()
    {
        impactDamageCollider.ImpactAttack();
    }
}
