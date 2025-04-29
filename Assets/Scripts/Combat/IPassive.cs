namespace Combat
{
    public interface IPassive
    {
        void OnSpawned(Unit unit);
        void OnDespawned(Unit unit);
    }
}