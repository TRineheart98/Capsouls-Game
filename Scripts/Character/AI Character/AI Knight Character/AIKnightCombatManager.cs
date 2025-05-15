using UnityEngine;

public class AIKnightCombatManager : AICharacterCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] AIKnightWeaponDamageCollider twoHandWeaponCollider;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] float attack01DamageModifier = 1.0f;
    [SerializeField] float attack02DamageModifier = 2.0f;

    public void SetAttack01Damage()
    {
        twoHandWeaponCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }

    public void SetAttack02Damage()
    {
        twoHandWeaponCollider.physicalDamage = baseDamage * attack02DamageModifier;
    }

    public void EnableTwoHandWeaponDamageCollider()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
        twoHandWeaponCollider.EnableDamageCollider();
    }

    public void DisableTwoHandWeaponDamageCollider()
    {
        twoHandWeaponCollider.DisableDamageCollider();
    }
}
