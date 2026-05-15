public class HealthPotionVisitor : IItemVisitor
{
    public void Visit(PlayerController player) => player.Heal(30);
}
