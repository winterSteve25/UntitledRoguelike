using Combat;
using Cysharp.Threading.Tasks;
using Levels;
using Unity.Netcode;
using UnityEngine;

namespace Content.Explode
{
    public class ExplodeAbility : NetworkBehaviour, IAbility
    {
        public string Name => "Explode";
        public int Cost => 1;
        public bool Blocking => false;
        public string Description => "Explode this unit at the start of the next friendly turn dealing 2 damage";
        public Sprite Icon => Resources.Load<Sprite>("Sprites/AbilityIcons/Explode");

        [SerializeField] private int radius;
        [SerializeField] private int damage;
        [SerializeField] private Sprite aboutToExplode;
        [SerializeField] private Sprite aboutToMask;
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private SpriteRenderer mask;

        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            unit.Interactable = false;
            ChangeTextureRpc();
            await IAbility.UntilNextFriendlyTurn(combatManager);
            Explode(combatManager, unit);
            return true;
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ChangeTextureRpc()
        {
            visual.sprite = aboutToExplode;
            mask.sprite = aboutToMask;
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SpawnExplosionEffectRpc(Vector2Int pos, NetworkObjectReference unit)
        {
            if (!unit.TryGet(out var obj)) return;
            
            var exp = Instantiate(Resources.Load<GameObject>("Sprites/VFX/Explosion"));
            exp.transform.localScale *= radius * 2 + 1;

            if (!CombatManager.Current.AmIFriendly)
            {
                exp.transform.position = (Level.Current.CellToWorld(pos) + obj.transform.position) * 0.5f;
            }
            else
            {
                exp.transform.position = Level.Current.CellToWorld(pos) + new Vector3(0.25f * exp.transform.localScale.x, 0.25f * exp.transform.localScale.y, 0);
            }
        }

        public void Explode(CombatManager combatManager, Unit unit)
        {
            var pos = unit.GridPositionSync;
            SpawnExplosionEffectRpc(pos, unit.NetworkObject);
            
            pos.x -= radius;
            pos.y -= radius;
            
            foreach (Unit u in combatManager.GetUnitsInArea(pos, new Vector2Int(radius * 2 + unit.Type.Size.x, radius * 2 + unit.Type.Size.y)))
            {
                if (u == unit) continue;
                u.AddHpRpc(-damage, DamageSource.Explosion);
            }
            
            combatManager.DespawnUnit(unit, true);
        }
    }
}