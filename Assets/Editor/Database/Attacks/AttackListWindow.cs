using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class AttackListWindow : EditorWindow
{
    CardDatabase cardDataBase;
    UnitDatabase unitDataBase;
    AttackDataList atkDataList;
    AttackPattern filterPattern;

    static bool isCardEdit;
    /**************************************/
    //For button layout in rows of 3
    /**************************************/
    List<bool> buttonToggled;
    int toggledIndex = 0;
    int inRow;
    int MAXINROW = 1;
    Vector2 scrollPos = new Vector2();
    /*****************************************************/

    void OnEnable()
    {   
        string objectPath = EditorPrefs.GetString("AttackDatabasePath");
        atkDataList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(AttackDataList)) as AttackDataList;
        if(isCardEdit)
            cardDataBase = (CardDatabase)EditorWindow.GetWindow(typeof(CardDatabase));
        else
            unitDataBase = (UnitDatabase)EditorWindow.GetWindow(typeof(UnitDatabase));
    }

    void OnGUI()
    {
        if (atkDataList == null)
        {
            string objectPath = EditorPrefs.GetString("AttackDatabasePath");
            atkDataList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(AttackDataList)) as AttackDataList;
            
        }

        if(isCardEdit && cardDataBase == null)
        {
            cardDataBase = (CardDatabase)EditorWindow.GetWindow(typeof(CardDatabase));
            unitDataBase = null;
        }
        else if(!isCardEdit && unitDataBase == null)
        {
            unitDataBase = (UnitDatabase)EditorWindow.GetWindow(typeof(UnitDatabase));
            cardDataBase = null;
        }

        if(atkDataList.attackList.Count == 0)
            EditorGUILayout.LabelField("Attack List Data Not Available", EditorStyles.boldLabel);
        else
        {
            if (buttonToggled == null || buttonToggled.Count < atkDataList.attackList.Count)
            {
                buttonToggled = new List<bool>();
                toggledIndex = 0;
                for (int i = 0; i < atkDataList.attackList.Count; i++)
                    buttonToggled.Add(false);
            }

            //Filter list or something
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Attack List Data", EditorStyles.boldLabel);

            filterPattern = (AttackPattern)EditorGUILayout.EnumPopup(filterPattern);
            if (GUILayout.Button("Sort", GUILayout.ExpandWidth(false)))
            {
                sortByName();
            }
            GUILayout.EndHorizontal();

            //Show the list
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(180));

            showScrollButtons();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            showAttackSummary();

            if(isCardEdit)
            {
                if (GUILayout.Button("Link this attack"))
                {
                    cardDataBase.assignElement(atkDataList.attackList[toggledIndex].uniqueId);
                    cardDataBase.Repaint();
                }
            }
            else
            {
                if (GUILayout.Button("Add this attack"))
                {
                    unitDataBase.AddAttack(unitDataBase.toggledIndex, atkDataList.attackList[toggledIndex].uniqueId);
                    unitDataBase.Repaint();
                }
            }      
            EditorGUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }

    void showScrollButtons()
    {
        inRow = 0;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.ExpandWidth(false));

        if (buttonToggled == null || buttonToggled.Count != atkDataList.attackList.Count)
        {
            buttonToggled = new List<bool>();
            for (int i = 0; i < atkDataList.attackList.Count; i++)
            {
                buttonToggled.Add(false);
            }
        }

        //We will display buttons in rows of 3
        EditorGUILayout.BeginHorizontal();

        for (int i = 0; i < atkDataList.attackList.Count; i++)
        {
            //Button
            if (atkDataList.attackList[i].pattern == filterPattern)
            {
                colorToggle(i);
                if (GUILayout.Button(atkDataList.attackList[i].aName, GUILayout.ExpandWidth(false), GUILayout.Width(150)))
                {
                    buttonToggled[i] = true;
                    if (buttonToggled[i] && i != toggledIndex)
                    {
                        if (toggledIndex < buttonToggled.Count)
                            buttonToggled[toggledIndex] = false;
                        toggledIndex = i;
                    }
                }

                colorToggle(i);

                buttonRowMaker();
            }

        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
        GUI.color = Color.white;

    }


    void showAttackSummary()
    {
        //---------Current selection info-----------
        GUILayout.BeginHorizontal();
        atkDataList.attackList[toggledIndex].aName = EditorGUILayout.TextField("Attack Name", atkDataList.attackList[toggledIndex].aName as string);
        //if (atkDataList.attackList[toggledIndex].pattern != filterPattern)
        //    filterPattern = atkDataList.attackList[toggledIndex].pattern;
        atkDataList.attackList[toggledIndex].pattern = (AttackPattern)EditorGUILayout.EnumPopup("Attack Pattern", atkDataList.attackList[toggledIndex].pattern);
        
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        atkDataList.attackList[toggledIndex].range = EditorGUILayout.Slider("Range", atkDataList.attackList[toggledIndex].range, 4f, 30f);

        GUILayout.Space(10);
        atkDataList.attackList[toggledIndex].dmg = EditorGUILayout.IntSlider("Damage/Success", atkDataList.attackList[toggledIndex].dmg, 1, 5);
        atkDataList.attackList[toggledIndex].nDice = EditorGUILayout.IntSlider("Number of rolls", atkDataList.attackList[toggledIndex].nDice, 1, 10);
        atkDataList.attackList[toggledIndex].roll = EditorGUILayout.IntSlider("%Success/Roll", atkDataList.attackList[toggledIndex].roll, 1, 100);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Attack Stats Summary");
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        string expected = "Expected: " + (atkDataList.attackList[toggledIndex].dmg * atkDataList.attackList[toggledIndex].nDice * ((float)atkDataList.attackList[toggledIndex].roll / 100)).ToString();
        EditorGUILayout.LabelField(expected);

        atkDataList.attackList[toggledIndex].expectedMin = calculateMin(atkDataList.attackList[toggledIndex].nDice, atkDataList.attackList[toggledIndex].roll) * atkDataList.attackList[toggledIndex].dmg;
        atkDataList.attackList[toggledIndex].expectedMax = calculateMax(atkDataList.attackList[toggledIndex].nDice, atkDataList.attackList[toggledIndex].roll) * atkDataList.attackList[toggledIndex].dmg;

        string expectedRange = "Damage Range : " + atkDataList.attackList[toggledIndex].expectedMin.ToString() + " - " + atkDataList.attackList[toggledIndex].expectedMax.ToString();
        EditorGUILayout.LabelField(expectedRange);
        GUILayout.EndHorizontal();
    }


    void colorToggle(int index)
    {
        if (toggledIndex == index)
        {
            GUI.color = Color.cyan;
        }
        else
        {
            GUI.color = Color.white;
        }
    }

    void buttonRowMaker()
    {
        inRow++;
        if (inRow == MAXINROW)
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            inRow = 0;
        }
    }

    void sortByName()
    {
        String oldID = atkDataList.attackList[toggledIndex].uniqueId;
        atkDataList.attackList.Sort(delegate (AttackData x, AttackData y)
        {
            if (x.aName == null && y.aName == null) return 0;
            else if (x.aName == null) return -1;
            else if (y.aName == null) return 1;
            else return x.aName.CompareTo(y.aName);
        });

        toggledIndex = findIndexByID(oldID);
    }

    int findIndexByID(String id)
    {
        for (int i = 0; i < atkDataList.attackList.Count; i++)
        {
            if (atkDataList.attackList[i].uniqueId == id)
                return i;
        }
        return 0;
    }

    int calculateMin(int n, int pInt)
    {
        float totalProb = 0f;
        int i = 0;
        float p = ((float)pInt) / 100f;
        while (totalProb < 0.05f)
        {
            //Combinaison
            totalProb += ((float)factorial(n) / (factorial(i) * factorial(n - i))) * (Mathf.Pow(p, i)) * (Mathf.Pow(1 - p, n - i));
            i++;
        }

        return i - 1;
    }

    int calculateMax(int n, int pInt)
    {
        float totalProb = 0f;
        int i = n;
        float p = ((float)pInt) / 100f;
        while (totalProb < 0.05f)
        {
            //Combinaison
            totalProb += ((float)factorial(n) / (factorial(i) * factorial(n - i))) * (Mathf.Pow(p, i)) * (Mathf.Pow(1 - p, n - i));
            i--;
        }

        return i + 1;
    }

    int factorial(int n)
    {
        int result = 1;
        for (int i = 1; i < n; i++)
        {
            result = result * i;
        }
        return result;
    }

    public static void CardEdit(bool state)
    {
        isCardEdit = state;
    }
}
