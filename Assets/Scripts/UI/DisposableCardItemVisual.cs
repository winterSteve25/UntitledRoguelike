using Combat.Deck;

namespace UI
{
    public class DisposableCardItemVisual : ItemVisual
    {
        protected override void TryUseOrCancel()
        {
            Inventory.RemoveItem(Slot.Pos);
        }
    }
}