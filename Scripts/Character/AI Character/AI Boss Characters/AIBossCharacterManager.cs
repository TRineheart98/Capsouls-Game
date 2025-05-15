using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class AIBossCharacterManager : AICharacterManager
{
    // Give this AI a unique ID
    public int bossID = 0;

    [Header("Music")]
    [SerializeField] AudioClip bossIntroClip;
    [SerializeField] AudioClip bossBattleLoopClip;

    [Header("Status")]
    public NetworkVariable<bool> bossFightIsActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> hasBeenAwakened = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> hasBeenDefeated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] List<FogWallInteractable> fogWalls;
    [SerializeField] string sleepAnimation;
    [SerializeField] string awakenAnimation;
    GameObject bossHealthBarInstance;

    [Header("States")]
    [SerializeField] BossSleepState sleepState;
    //When this AI spawned, check our save file (Dictionary)
    //If the save file does not contain a boss monster with this ID add it
    //If it is present, check if the boss has been defeated
    //If the boss has been defeated, disable this gameobject
    //If the boss has not been defeated, allow this object to continue to be active

    protected override void Awake()
    {
        base.Awake();

        hasBeenDefeated.Value = false;

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        bossFightIsActive.OnValueChanged += OnBossFightIsActiveChanged;
        OnBossFightIsActiveChanged(false, bossFightIsActive.Value);

        if (IsOwner)
        {
            sleepState = Instantiate(sleepState);

            currentState = sleepState;
        }

        if (IsServer)
        {
            characterNetworkManager.maxHealth.Value = 2000;
            characterNetworkManager.currentHealth.Value = 2000;

            aiCharacterNetworkManager.currentHealth.OnValueChanged += aiCharacterNetworkManager.CheckHP;

            //If our save data does not contain information on this boss, we add it now
            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);
            }
            //Otherwise, load the data that already exists on this boss
            else
            {
                hasBeenDefeated.Value = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];
                hasBeenAwakened.Value = WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID];
            }
            //Locate fog walls
            StartCoroutine(GetFogWallsFromWorldObjectManager());

            //If the boss has been awakened, enable the fog walls
            if (hasBeenAwakened.Value)
            {
                for (int i = 0; i < fogWalls.Count; i++)
                {
                    fogWalls[i].isActive.Value = true;
                }
            }

            //If the boss has been defeated, disable the fog walls
            if (hasBeenDefeated.Value)
            {
                for (int i = 0; i < fogWalls.Count; i++)
                {
                    fogWalls[i].isActive.Value = false;
                }

                aiCharacterNetworkManager.isActive.Value = false;
            }
        }

        if (!hasBeenAwakened.Value)
        {
            characterAnimatorManager.PlayTargetActionAnimation(sleepAnimation, true);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        bossFightIsActive.OnValueChanged -= OnBossFightIsActiveChanged;
    }

    private IEnumerator GetFogWallsFromWorldObjectManager()
    {
        while (WorldObjectManager.instance.fogWalls.Count == 0)
            yield return new WaitForEndOfFrame();

        //Locate fog walls
        fogWalls = new List<FogWallInteractable>();

        foreach (var fogWall in WorldObjectManager.instance.fogWalls)
        {
            if (fogWall.fogWallID == bossID)
            {
                fogWalls.Add(fogWall);
            }
        }
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        PlayerUIManager.instance.playerUIPopUpManager.SendBossDefeatedPopUp("GREAT ENEMY FELLED");
        PlayerUIManager.instance.playerUIPopUpManager.SendBossRewardPopUp();

        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;
            bossFightIsActive.Value = false;

            foreach (var fogWall in fogWalls)
            {
                fogWall.isActive.Value = false;
            }

            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
            }

            hasBeenDefeated.Value = true;

            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
            }
            else
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
            }

            //  Reward the player with Green Great Sword
            //  Reward the player with Green Great Sword (only once, and don't auto-equip)
            PlayerManager killerPlayer = FindFirstObjectByType<PlayerManager>();

            if (killerPlayer != null)
            {
                WeaponItem greenGreatSword = WorldItemDatabase.Instance.GetWeaponByID(2);

                // Check if the sword is already in the inventory
                bool alreadyHasSword = false;
                for (int i = 0; i < killerPlayer.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
                {
                    var weapon = killerPlayer.playerInventoryManager.weaponsInRightHandSlots[i];
                    if (weapon != null && weapon.itemID == greenGreatSword.itemID)
                    {
                        alreadyHasSword = true;
                        break;
                    }
                }

                if (!alreadyHasSword)
                {
                    for (int i = 0; i < killerPlayer.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
                    {
                        var slot = killerPlayer.playerInventoryManager.weaponsInRightHandSlots[i];
                        int currentIndex = killerPlayer.playerInventoryManager.rightHandWeaponIndex;

                        // Only insert if the slot is unarmed and NOT currently selected
                        if ((slot == null || slot.itemID == WorldItemDatabase.Instance.unarmedWeapon.itemID) && i != currentIndex)
                        {
                            killerPlayer.playerInventoryManager.weaponsInRightHandSlots[i] = greenGreatSword;
                            Debug.Log("Green Great Sword added to inventory slot " + i);
                            break;
                        }
                    }

                    if (!WorldSaveGameManager.instance.currentCharacterData.collectedItemIDs.Contains(greenGreatSword.itemID))
                    {
                        WorldSaveGameManager.instance.currentCharacterData.collectedItemIDs.Add(greenGreatSword.itemID);
                    }
                }

            }
        }

        yield return new WaitForSeconds(1);

        WorldSaveGameManager.instance.SaveGame();

        // Award players with runes

        // Disable character
    }

    public void WakeBoss()
    {
        if (IsOwner)
        {
            if (!hasBeenAwakened.Value)
            {
                characterAnimatorManager.PlayTargetActionAnimation(awakenAnimation, true);
            }

            bossFightIsActive.Value = true;
            hasBeenAwakened.Value = true;
            currentState = idle;

            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            }
            //Otherwise, load the data that already exists on this boss
            else
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            }

            for (int i = 0; i < fogWalls.Count; i++)
            {
                fogWalls[i].isActive.Value = true;
            }
        }

    }

    private void OnBossFightIsActiveChanged(bool oldStatus, bool newStatus)
    {
        if (bossFightIsActive.Value)
        {
            WorldSoundFXManager.instance.PlayBossTrack(bossIntroClip, bossBattleLoopClip);

            // Destroy existing bar first (safety check)
            if (bossHealthBarInstance != null)
            {
                Destroy(bossHealthBarInstance);
                bossHealthBarInstance = null;
            }

            // Create new bar
            bossHealthBarInstance = Instantiate(
                PlayerUIManager.instance.playerUIHudManager.bossHealthBarObject,
                PlayerUIManager.instance.playerUIHudManager.bossHealthBarParent
            );

            UI_Boss_HP_Bar bossHPBar = bossHealthBarInstance.GetComponentInChildren<UI_Boss_HP_Bar>();
            bossHPBar.EnableBossHPBar(this);
        }
        else
        {
            WorldSoundFXManager.instance.StopBossMusic();
        }
    }

    public void ClearBossUI()
    {
        WorldSoundFXManager.instance.StopBossMusic();

        if (bossHealthBarInstance != null)
        {
            UI_Boss_HP_Bar hpBar = bossHealthBarInstance.GetComponentInChildren<UI_Boss_HP_Bar>();
            if (hpBar != null)
            {
                hpBar.RemoveHPBar(0); // Match existing delay/destruction behavior
            }
            else
            {
                Destroy(bossHealthBarInstance); // fallback
            }

            bossHealthBarInstance = null;
        }
    }

}
