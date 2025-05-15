using UnityEngine;

public class AIBossAshveilSoundFXManager : CharacterSoundFXManager
{
    [Header("Sword Wooshes")]
    public AudioClip[] swordWooshes;

    [Header("Sword Impacts")]
    public AudioClip[] swordImpacts;

    [Header("Foot Impacts")]
    public AudioClip[] footImpacts;

    public virtual void PlaySwordImpactSoundFX()
    {
        if (swordImpacts.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(swordImpacts),2);
        }
    }
}
