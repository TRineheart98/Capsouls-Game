using UnityEngine;
using Unity.Netcode;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Characters")]
    [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
    [SerializeField] List<AICharacterManager> spawnedInCharacters;

    [Header("Bosses")]
    [SerializeField] List<AIBossCharacterManager> spawnedInBosses;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            aiCharacterSpawners.Add(aiCharacterSpawner);
            aiCharacterSpawner.AttempToSpawnCharacter();
        }
    }

    public void AddCharacterToSpawnCharactersList(AICharacterManager character)
    {
        if (spawnedInCharacters.Contains(character))
            return;

        spawnedInCharacters.Add(character);

        AIBossCharacterManager bossCharacter = character as AIBossCharacterManager;

        if (bossCharacter != null)
        {
            if (spawnedInBosses.Contains(bossCharacter))
                return;

            spawnedInBosses.Add(bossCharacter);
        }
    }

    public AIBossCharacterManager GetBossCharacterByID(int ID)
    {
        return spawnedInBosses.FirstOrDefault(boss => boss.bossID == ID);  
    }

    public void ResetAllCharacters()
    {
        DespawnAllCharacters();

        foreach (var spawner in aiCharacterSpawners)
        {
            spawner.AttempToSpawnCharacter();
        }
    }

    private void DespawnAllCharacters()
    {
        for (int i = spawnedInCharacters.Count - 1; i >= 0; i--)
        {
            var character = spawnedInCharacters[i];

            if (character == null || !character.gameObject) // Object already destroyed
            {
                spawnedInCharacters.RemoveAt(i);
                continue;
            }

            if (character.NetworkObject != null && character.NetworkObject.IsSpawned)
            {
                character.NetworkObject.Despawn(true);
            }

            spawnedInCharacters.RemoveAt(i);
        }

        // Also clear the boss list since they’re tied to characters
        spawnedInBosses.Clear();
    }

    private void DisableAllCharacters()
    {

    }

}
