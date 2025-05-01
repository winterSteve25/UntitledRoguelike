using System;
using System.IO;
using System.Reflection;
using Combat;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class MenuScript
    {
        [MenuItem("Assets/Create/Combat/New Unit Prefab")]
        private static void NewUnit()
        {
            var currentFolder = CurrentFolder();
            var prefabPath = $"{currentFolder}/New Unit Prefab.prefab";
            var unitTypePath = $"{currentFolder}/New Unit Type.asset";

            AssetDatabase.CopyAsset("Assets/Resources/UnitTypes/Base.prefab", prefabPath);

            var unitType = ScriptableObject.CreateInstance<UnitType>();
            AssetDatabase.CreateAsset(unitType, unitTypePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var loadedUnitType = AssetDatabase.LoadAssetAtPath<UnitType>(unitTypePath);
            var loadedPrefab = AssetDatabase.LoadAssetAtPath<Unit>(prefabPath);

            loadedUnitType.Prefab = loadedPrefab;
            loadedPrefab.Type = loadedUnitType;

            EditorUtility.SetDirty(loadedUnitType);
            EditorUtility.SetDirty(loadedPrefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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