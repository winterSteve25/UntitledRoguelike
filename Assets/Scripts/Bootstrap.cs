using PrimeTween;
using UnityEngine;

public static class Bootstrap
{
    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
    }
}