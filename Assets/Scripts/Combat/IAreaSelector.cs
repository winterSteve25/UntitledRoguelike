using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Combat
{
    public interface IAreaSelector
    {
        UniTask<Vector2Int> SelectArea(Vector2Int center, Vector2Int destinationSize, int radius,
            Predicate<Vector2Int> isValid, SpotSelectionMode mode = SpotSelectionMode.Omnidirectional);
    }
}