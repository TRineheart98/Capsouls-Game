using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WorldObjectManager : MonoBehaviour
{
    public static WorldObjectManager instance;

    [Header("Objects")]
    [SerializeField] List<NetworkObjectSpawner> networkObjectSpawners;
    [SerializeField] List<GameObject> spawnedInObjects;

    [Header("Fog Walls")]
    public List<FogWallInteractable> fogWalls;

    //Create an object script that holds the logic for fog walls
    //Spawn in those fogwalls as network objects during start of game (Must have a spawner)
    //Create a general object spawner
    //When the fog walls are spawned, add them to the world fog wall list
    //Grab the correct fogwall from the list on the boss manager when the boss is being initialized

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

    public void SpawnObject(NetworkObjectSpawner networkObjectSpawner)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            networkObjectSpawners.Add(networkObjectSpawner);
            networkObjectSpawner.AttempToSpawnCharacter();
        }
    }

    public void AddFogWallToList(FogWallInteractable fogWall)
    {
        if (!fogWalls.Contains(fogWall))
        {
            fogWalls.Add(fogWall);
        }
    }

    public void RemoveFogWallToList(FogWallInteractable fogWall)
    {
        if (fogWalls.Contains(fogWall))
        {
            fogWalls.Remove(fogWall);
        }
    }
}
