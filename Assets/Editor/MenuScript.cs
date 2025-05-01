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
            var prefabPath = $"{CurrentFolder()}/New Unit Prefab.prefab";
            var unitTypePath = $"{CurrentFolder()}/New Unit Type.asset";

            AssetDatabase.CopyAsset("Assets/Resources/UnitTypes/Base.prefab", prefabPath);
            var prefab = AssetDatabase.LoadAssetAtPath<Unit>(prefabPath);

            var unitType = ScriptableObject.CreateInstance<UnitType>();
            unitType.Prefab = prefab;
            prefab.Type = unitType;
            AssetDatabase.CreateAsset(unitType, unitTypePath);

            var theFile = AssetDatabase.LoadAssetAtPath<UnitType>(unitTypePath);
            EditorUtility.CopySerialized(unitType, theFile);
            theFile.name = "New Unit Type";
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