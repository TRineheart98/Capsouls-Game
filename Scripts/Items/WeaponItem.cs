using UnityEngine;

public class WeaponItem : Item
{
    //Animator controller override (Change attack animations based on weapon you are currently using

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Requirements")]
    public int strengthREQ = 0;
    public int dexREQ = 0;
    public int intREQ = 0;
    public int faithREQ = 0;

    [Header("Weapon Base Damage")]
    public float physicalDamage = 0;             //In the future we could break this down more, giving it Standard, Strike, Slash, and Pierce
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Weapon Poise")]
    public float poiseDamage = 10;

    [Header("Attack Modifiers")]
    public float light_Attack_01_Modifier = 1;
    public float heavy_Attack_01_Modifier = 1.5f;
    public float charge_Attack_01_Modifier = 2.0f;


    [Header("Stamina Cost Modifiers")]
    public int baseStaminaCost = 10;
    public float lightAttackStaminaCostMultiplier = 1.0f;
    public float heavyAttackStaminaCostMultiplier = 2.0f;
    public float chargedAttackStaminaCostMultiplier = 3.0f;

    [Header("Actions")]
    public WeaponItemAction oh_RB_Action;   //One hand right bumper action
    public WeaponItemAction oh_RT_Action;   //One hand right trigger action

    [Header("Whooshes")]
    public AudioClip[] wooshes;
}
