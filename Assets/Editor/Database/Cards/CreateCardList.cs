using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateCardList
{
    [MenuItem("Assets/Create/Unit Data List")]
    public static CardDataList Create()
    {
        CardDataList asset = ScriptableObject.CreateInstance<CardDataList>();

        AssetDatabase.CreateAsset(asset, "Assets/CardList.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}

