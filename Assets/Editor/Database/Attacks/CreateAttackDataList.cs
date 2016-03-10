using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateAttackDataList
{
    [MenuItem("Assets/Create/Attack Data List")]
    public static AttackDataList Create()
    {
        AttackDataList asset = ScriptableObject.CreateInstance<AttackDataList>();

        AssetDatabase.CreateAsset(asset, "Assets/AttackList.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}

