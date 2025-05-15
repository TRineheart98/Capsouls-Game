using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSaveData
{
    [Header("Character Name")]
    public string characterName = "Character";

    [Header("Time Played")]
    public float secondsPlayed;

    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;

    [Header("Resources")]
    public int currentHealth;
    public float currentStamina;

    [Header("Stats")]
    public int vitality;
    public int endurance;

    [Header("Bosses")]
    public SerializableDictionary<int, bool>bossesAwakened;     //The int is the boss ID, The bool is the awakened status
    public SerializableDictionary<int, bool> bossesDefeated;    //The int is the boss ID, The bool is the defeated status

    [Header("Items Collected")]
    public List<int> collectedItemIDs = new();

    public CharacterSaveData()
    {
        bossesAwakened = new SerializableDictionary<int, bool>();
        bossesDefeated = new SerializableDictionary<int, bool>();
    }
}
