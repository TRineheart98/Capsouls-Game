using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]

public class TakeDamageEffect : InstantCharacterEffect 
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage; //If the damage is caused by another character attack it will be stored here

    [Header("Damage")]
    public float physicalDamage = 0;             //In the future we could break this down more, giving it Standard, Strike, Slash, and Pierce
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Final Damage")]
    private int finalDamageDealt = 0;              //The total damage the character will take after all calculations have been made

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false;          //If characters pose is broken, play a stunned animation

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound FX")]
    public bool willPlayDamageSFX = true;
    public AudioClip elementalDamageSoundFX;    //Ex: If you use a flaming sword it would have a flame sound on top of the willPlayDamageSFX

    [Header("Direction Damage Taken From")]
    public float angleHitFrom;                  //Used to determine what damage animation to play, used to determine what animation to play, such as getting in the back or side
    public Vector3 contactPoint;                //Used to determine where blood effecst instantiate


    public override void ProcessEffect(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value)
            return;

        base.ProcessEffect(character);

        //If the character is dead, do not process additional damage effects
        if (character.isDead.Value)
        {
            return;
        }

        //Is our character vulnerable or not

        //Calculate Damage
        CalculateDamage(character);

        //Check which direction the damage came from

        //Play a damage animation
        PlayDirectionalBasedDamageAnimation(character);

        //Check for buildups(bleed,posion)

        //Play damage sound fx
        PlayDamageSFX(character);

        //Play damage vfx (blood)
        PlayDamageVFX(character);

        //If character is AI, check for new target if character causing damage is present
    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner)
            return;

        if (characterCausingDamage != null)
        {
            //Check for damage midifiers and modify base damage (Physical damage buff, elemental damage buff ect)
        }

        //Check character for flat defenses and subtract them from damage

        //Check character for armor absorption and subtract the percentage from the damage

        //Add all damage types together, and apply the final damage
        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage +  lightningDamage + holyDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        Debug.Log($"Final Damage Dealt: {finalDamageDealt}");
        character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

        //Calculate Poise damage to determine if the character will be stunned
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        // If we have fire damage, lets play fire particles
        // If we have lightning damage, lightning particles

        //If we have just a physical weapon, play blood effect
        character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
    }

    private void PlayDamageSFX(CharacterManager character)
    {
        AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

        character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
        character.characterSoundFXManager.PlayDamageGruntSoundFX();
        //If fire damage is greater than 0, play burn sfx
        //If lightning damage is greater than 0, play zap sfx
    }

    private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
    {
        if (!character.IsOwner)
            return;

        if (character.isDead.Value)
            return;

        //TODO: Calculate if pose is broken

        poiseIsBroken = true;

        if (angleHitFrom >= 145 && angleHitFrom <= 180)
        {
            //Play front animation
            damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
        }
        else if (angleHitFrom <= -145 && angleHitFrom >= -180)
        {
            //Play front animation
            damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
        }
        else if (angleHitFrom >= -45 && angleHitFrom <= 45)
        {
            //Play back animation
            damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Medium_Damage);
        }
        else if (angleHitFrom >= -144 && angleHitFrom <= -45)
        {
            //Play left animation
            damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Medium_Damage);
        }
        else if (angleHitFrom >= 45 && angleHitFrom <= 144)
        {
            //Play right animation
            damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Medium_Damage);
        }

        //If poise is broken, play a staggering damage animation
        if (poiseIsBroken)
        {
            character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
            character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
        }
    }

}
