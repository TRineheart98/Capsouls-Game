using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class WorldGameSessionManager : MonoBehaviour
{
    public static WorldGameSessionManager instance;

    [Header("Active Player In Session")]
    public List<PlayerManager> players = new List<PlayerManager>();

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

    public void AddPlayerToActivePlayersList(PlayerManager player)
    {
        //Check the list, if it does not already contain the player, add them
        if(!players.Contains(player))
        {
            players.Add(player);
        }

        //Check the list for null slots, and remove the null slots
        for (int i = players.Count - 1; i > -1; i--)
        {
            if (players[i] == null)
            {
                players.RemoveAt(i);
            }
        }
    }

    public void RemovePlayerFromActivePlayersList(PlayerManager player)
    {
        //Check the list, if it does contain the player, remove them
        if (players.Contains(player))
        {
            players.Remove(player);
        }

        //Check the list for null slots, and remove the null slots
        for (int i = players.Count - 1; i > -1; i--)
        {
            if (players[i] == null)
            {
                players.RemoveAt(i);
            }
        }
    }
}
