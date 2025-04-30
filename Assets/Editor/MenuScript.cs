using System;
using System.Reflection;
using UnityEditor;

namespace Editor
{
    public static class MenuScript
    {
        [MenuItem("Assets/Create/Combat/New Unit Prefab")]
        private static void NewUnit()
        {
            AssetDatabase.CopyAsset("Assets/Resources/UnitTypes/Base.prefab", $"{CurrentFolder()}/New Prefab.prefab");
        }

        private static string CurrentFolder()
        {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath =
                projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            return obj.ToString();
        }
    }
}