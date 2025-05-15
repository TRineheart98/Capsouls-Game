using UnityEngine;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;
    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        //Calculated at runtime to determine vitality and endurance stats
        CalculateHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value);
        CalculateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
    }
}
