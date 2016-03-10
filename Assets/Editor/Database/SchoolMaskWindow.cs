using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class SchoolMaskWindow : EditorWindow
{
    //One bool for every element in the game.
    [SerializeField]
    private static bool[] schoolMask; // = { false, false, false, false, false, false, false, false, false }
    bool toggleAll;
    static CardDatabase cardWindow;

    static SchoolMaskWindow()
    {
        Debug.Log("Static Constructor");
        schoolMask = new bool[9];
        for (int i = 0; i < schoolMask.Length; i++)
            schoolMask[i] = true;
        cardWindow = (CardDatabase)EditorWindow.GetWindow(typeof(CardDatabase));
    }

    void OnEnable()
    {
        //cardWindow = (CardDatabase)EditorWindow.GetWindow(typeof(CardDatabase));
        //if (schoolMask == null)
        //{
        //    schoolMask = new bool[9];
        //}  

    }

    void OnGUI()
    {
        //if(cardWindow == null)
        //    cardWindow = (CardDatabase)EditorWindow.GetWindow(typeof(CardDatabase));
        EditorGUILayout.LabelField("School Filter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        schoolMask[(int)Elements.FIRE]= EditorGUILayout.Toggle("Fire", schoolMask[(int)Elements.FIRE]);
        schoolMask[(int)Elements.WATER] = EditorGUILayout.Toggle("Water", schoolMask[(int)Elements.WATER]);
        schoolMask[(int)Elements.WIND] = EditorGUILayout.Toggle("Wind", schoolMask[(int)Elements.WIND]);
        schoolMask[(int)Elements.EARTH] = EditorGUILayout.Toggle("Earth", schoolMask[(int)Elements.EARTH]);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        schoolMask[(int)Elements.ICE] = EditorGUILayout.Toggle("Ice", schoolMask[(int)Elements.ICE]);
        schoolMask[(int)Elements.METAL] = EditorGUILayout.Toggle("Metal", schoolMask[(int)Elements.METAL]);
        schoolMask[(int)Elements.POISON] = EditorGUILayout.Toggle("Poison", schoolMask[(int)Elements.POISON]);
        schoolMask[(int)Elements.LIGHTNING] = EditorGUILayout.Toggle("Lightning", schoolMask[(int)Elements.LIGHTNING]);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        schoolMask[(int)Elements.NORMAL] = EditorGUILayout.Toggle("Normal", schoolMask[(int)Elements.NORMAL]);

        if (GUI.changed)
        {
            cardWindow.Repaint();
            //EditorUtility.SetDirty(schoolMask);
        }
    }


    public static bool isCardShown(CardData card)
    {
       // if(schoolMask == null)
        //    schoolMask = new bool[9];
        return schoolMask[(int)card.school];
    }

    public static string getFilterNames()
    {
        //if (schoolMask == null)
        //    schoolMask = new bool[9];
        String filterNames = "";

        for (int i = 0; i < (int)Elements.TOTAL; i++)
        {
            if(schoolMask[i])
            {
                filterNames += Enum.GetName(typeof(Elements), i) + " ";
            }

        }
        return filterNames;
    }
}

