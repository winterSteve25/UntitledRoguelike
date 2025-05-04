using Combat;
using UnityEngine;

namespace Content.WhirlPool
{
    public class WhirlPoolGadget : Gadget
    {
        public static readonly Vector2Int Size = new(3, 3);

        public override Vector2Int GridSize => Size;
    }
}