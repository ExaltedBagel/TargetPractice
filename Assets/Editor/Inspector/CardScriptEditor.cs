using UnityEngine;
using System.Collections;
using UnityEditor;
/*
[CustomEditor(typeof(Card))]
public class CardScriptEditor : Editor {

   public override void OnInspectorGUI()
   {
       Card myCardScript = (Card)target;

       myCardScript.cardName =  EditorGUILayout.TextField("Name", myCardScript.cardName);
       myCardScript.school = (Elements)EditorGUILayout.EnumPopup("School", myCardScript.school);
       myCardScript.rechargeTime = EditorGUILayout.IntField("Recharge", myCardScript.rechargeTime);

       EditorGUILayout.Space();
       EditorGUILayout.LabelField("Mana Cost");
       myCardScript.manaCost = new int[5];
       myCardScript.manaCost[0] = EditorGUILayout.IntField("Any", myCardScript.manaCost[0]);
       myCardScript.manaCost[1] = EditorGUILayout.IntField("Fire", myCardScript.manaCost[1]);
       myCardScript.manaCost[2] = EditorGUILayout.IntField("Water", myCardScript.manaCost[2]);
       myCardScript.manaCost[3] = EditorGUILayout.IntField("Wind", myCardScript.manaCost[3]);
       myCardScript.manaCost[4] = EditorGUILayout.IntField("Earth", myCardScript.manaCost[4]);
   }
}

[CustomEditor(typeof(UnitCard))]
public class UnitCardScriptEditor : CardScriptEditor
{
   public override void OnInspectorGUI()
   {
       base.OnInspectorGUI();
       EditorGUILayout.Space();

       UnitCard myUnitCardScript = (UnitCard)target;
       //myUnitCardScript.unit = (Unit)EditorGUILayout.ObjectField("Unit", myUnitCardScript.unit, typeof(Unit), true);
       //if(myUnitCardScript.unit != null)
       //    myUnitCardScript.unit.uType = (UnitTypes)EditorGUILayout.EnumPopup("School", myUnitCardScript.unit.uType);
   }
}

[CustomEditor(typeof(EquipmentCard))]
public class EquipementCardScriptEditor : CardScriptEditor
{
   public override void OnInspectorGUI()
   {
       base.OnInspectorGUI();
       EditorGUILayout.Space();

       EquipmentCard myEquipementCardScript = (EquipmentCard)target;
       myEquipementCardScript.eType = (EquipType)EditorGUILayout.EnumPopup("Equip Slot", myEquipementCardScript.eType);
       myEquipementCardScript.type = (UnitTypes)EditorGUILayout.EnumPopup("Unit Type", myEquipementCardScript.type);
       myEquipementCardScript.durability = EditorGUILayout.IntField("Durability", myEquipementCardScript.durability);
   }
}


[CustomEditor(typeof(SpellCard))]
public class SpellCardScriptEditor : CardScriptEditor
{
   GameObject atk;

   public override void OnInspectorGUI()
   {
       base.OnInspectorGUI();
       SpellCard mySpellCardScript = (SpellCard)target;
       EditorGUILayout.Space();
       EditorGUILayout.LabelField("Spell");

       atk = (GameObject)EditorGUILayout.ObjectField("Attack", atk, typeof(GameObject), true);

       //mySpellCardScript.attack = atk.GetComponent<Attack>();
   }
}

[CustomEditor(typeof(CrystalCard))]
public class CrystalCardScriptEditor : CardScriptEditor
{
   public override void OnInspectorGUI()
   {
       base.OnInspectorGUI();
       EditorGUILayout.Space();

       CrystalCard myCrystalCardScript = (CrystalCard)target;
       //myCrystalCardScript.charges = EditorGUILayout.IntField("Charges", myCrystalCardScript.charges);
   }

}

    */
