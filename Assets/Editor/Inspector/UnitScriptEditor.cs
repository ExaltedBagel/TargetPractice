using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;


//[CustomEditor(typeof(Unit))]
//public class UnitScriptEditor : Editor
//{
//    protected bool atkFactShow = false;
//    protected bool defFactShow = false;

//    protected bool isHero = false;

//    public override void OnInspectorGUI()
//    {
//        Unit myUnit = (Unit)target;

//        myUnit.atkCircle = (GameObject)EditorGUILayout.ObjectField("Attack Circle", myUnit.atkCircle, typeof(GameObject), true);
//        myUnit.selectionCircle = (GameObject)EditorGUILayout.ObjectField("Select Circle", myUnit.selectionCircle, typeof(GameObject), true);
//        myUnit.attackDataList = (AttackDataList)EditorGUILayout.ObjectField("Attack List Database", myUnit.attackDataList, typeof(AttackDataList), true);

//        myUnit.uName = EditorGUILayout.TextField("Name", myUnit.uName);
//        myUnit.uType = (UnitTypes)EditorGUILayout.EnumPopup("Unit Type", myUnit.uType);
//        EditorGUILayout.LabelField("Team:");
//        myUnit.team = EditorGUILayout.IntSlider(myUnit.team, 1, 2);
//        EditorGUILayout.Space();

//        //Stats
//        EditorGUILayout.LabelField("Stats");
//        myUnit.hp = EditorGUILayout.IntField("HP", myUnit.hp);
//        myUnit.speed = EditorGUILayout.FloatField("Speed", myUnit.speed);

//        if(isHero)
//        {
//            Hero myHero = (Hero)target;
//            EditorGUILayout.Space();
//            EditorGUILayout.LabelField("Hero Stats");
//            myHero.impulse = EditorGUILayout.IntField("Impulse", myHero.impulse);
//            myHero.memory = EditorGUILayout.IntField("Memory", myHero.memory);
//        }

//        /*****************************************/
//        //Atk Factors
//        /*****************************************/
//        EditorGUILayout.Space();
//        atkFactShow = EditorGUILayout.Foldout(atkFactShow, "Atk Factors");
//        if(atkFactShow)
//        {
//            for (int i = 0; i < (int)DamageType.TOTAL; i++)
//            {
//                myUnit.typeAtk[i] = EditorGUILayout.Slider(Enum.GetName(typeof(DamageType), i), myUnit.typeAtk[i], 0.5f, 2.0f);
//            }
//            if (GUILayout.Button("Reset"))
//                for (int i = 0; i < (int)DamageType.TOTAL; i++)
//                {
//                    myUnit.typeAtk[i] = 1.0f;
//                }
//        }  

//        /****************************************/
//        //Defence factors
//        /****************************************/
//        EditorGUILayout.Space();
//        defFactShow = EditorGUILayout.Foldout(defFactShow, "Defence Factors");
//        if (defFactShow)
//        {
//            for (int i = 0; i < (int)DamageType.TOTAL; i++)
//            {
//                myUnit.typeDef[i] = EditorGUILayout.Slider(Enum.GetName(typeof(DamageType), i), myUnit.typeDef[i], 0.5f, 2.0f);
//            }
//            if(GUILayout.Button("Reset"))
//                for (int i = 0; i < (int)DamageType.TOTAL; i++)
//                {
//                    myUnit.typeDef[i] = 1.0f;
//                }
//        }

//        /****************************************/
//        //Attack listing
//        /****************************************/
      
//        //View attacks
//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Attack List");

//        foreach (Attack x in myUnit.attacks.ToList())
//        {
//            EditorGUILayout.BeginHorizontal();
//            EditorGUILayout.PrefixLabel(x.aName);
//            if (GUILayout.Button("Remove", GUILayout.Width(120f)))
//                myUnit.attacks.Remove(x);
//            EditorGUILayout.EndHorizontal();
//        }
//    }
    
//}

//[CustomEditor(typeof(Hero))]
//public class HeroScriptEditor : UnitScriptEditor
//{
    
//    public override void OnInspectorGUI()
//    {
//        isHero = true;
//        base.OnInspectorGUI();
//    }
//}

