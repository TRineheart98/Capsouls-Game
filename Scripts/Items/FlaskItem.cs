using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Flask")]
public class FlaskItem : QuickSlotItem
{
    [Header("Empty Item")]
    [SerializeField] GameObject emptyFlaskItem;

    public override void AttemptToUseItem(PlayerManager player)
    {
        if (!CanIUseThisItem(player))
            return;

        player.playerAnimatorManager.PlayTargetActionAnimation(useItemAnimation, true, true);
    }
}
