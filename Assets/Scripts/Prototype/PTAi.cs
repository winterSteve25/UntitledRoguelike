using System.Linq;
using Combat;
using Levels;
using UnityEngine;

namespace Prototype
{
    public class PtAi : LevelAI
    {
        private Unit _target;

        [SerializeField] private Unit slime;

        protected override void DecideMove()
        {
            if (_target == null)
            {
                _target = FindTarget();
            }

            if (_target == null) return;
            
            var distanceToTarget = Vector3.Distance(_target.transform.position, transform.position);
        }

        private Unit FindTarget()
        {
            var friendlyUnits = CombatMan.ActiveUnits.Where(x => x.Friendly);
            var enumerable = friendlyUnits.ToList();
            if (!enumerable.Any()) return null;
            
            return enumerable.First();
        }
    }
}