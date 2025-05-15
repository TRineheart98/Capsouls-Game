using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    //Process instant effects (Take damage, heal)

    //Process timed effects (Poison, build ups)

    //Process static effects (Adding/Removing buffs)

    CharacterManager character;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplatterVFX;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        //If we have manually placed a blood splatter VFX on this model, play its version
        if (bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
        //Else, we use the generic (Default verison) we have elsewhere
        else
        {
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
    }
}
