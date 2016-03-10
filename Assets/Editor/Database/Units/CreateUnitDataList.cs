using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateUnitDataList
{
    [MenuItem("Assets/Create/Unit Data List")]
    public static UnitDataList Create()
    {
        UnitDataList asset = ScriptableObject.CreateInstance<UnitDataList>();

        AssetDatabase.CreateAsset(asset, "Assets/UnitList.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}

