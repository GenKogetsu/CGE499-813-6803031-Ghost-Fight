public class AtkPotionVisitor : IItemVisitor
{
    public void Visit(PlayerController player) => player.BoostAttack();
}
