public class SpeedPotionVisitor : IItemVisitor
{
    public void Visit(PlayerController player) => player.ActivateSpeedBoost(5f);
}
