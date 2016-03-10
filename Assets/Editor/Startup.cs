using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class Startup
{
    public static AttackDataList attackDataList;
    public static UnitDataList unitDataList;
    public static CardDataList cardDataList;

    static Startup()
    {
        //Get all the database paths setup for the editor
        string atkPath = "Assets/AttackList.asset";
        string unitPath = "Assets/UnitList.asset";
        string cardPath = "Assets/CardList.asset";

        //Attacks
        EditorPrefs.SetString("AttackDatabasePath", atkPath);
        attackDataList = AssetDatabase.LoadAssetAtPath(atkPath, typeof(AttackDataList)) as AttackDataList;
        if(attackDataList == null)
        {
            Debug.Log("No attack database, creating empty one.");
            AttackDataList asset = ScriptableObject.CreateInstance<AttackDataList>();
            AssetDatabase.CreateAsset(asset, atkPath);
            AssetDatabase.SaveAssets();
        }

        //Creatures
        EditorPrefs.SetString("UnitDataPath", unitPath);
        unitDataList = AssetDatabase.LoadAssetAtPath(unitPath, typeof(UnitDataList)) as UnitDataList;
        if (unitDataList == null)
        {
            Debug.Log("No unit database, creating empty one.");
            UnitDataList asset = ScriptableObject.CreateInstance<UnitDataList>();
            AssetDatabase.CreateAsset(asset, unitPath);
            AssetDatabase.SaveAssets();
        }

        //Cards
        EditorPrefs.SetString("CardDataPath", cardPath);
        cardDataList = AssetDatabase.LoadAssetAtPath(cardPath, typeof(CardDataList)) as CardDataList;
        if(cardDataList == null)
        {
            Debug.Log("No card database, creating empty one.");
            CardDataList asset = ScriptableObject.CreateInstance<CardDataList>();
            AssetDatabase.CreateAsset(asset, cardPath);
            AssetDatabase.SaveAssets();
        }
    }
}

