﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class UnitDatabase : EditorWindow
{
    protected bool showFactors = false;
    protected bool isHero = false;

    private UnitDataList unitPDataList;
    public UnitDataList unitDataList { get { return unitPDataList; } }

    private AttackListWindow atkWindow;
    private AttackDataList atkDataList;
    private AttackData currentAttack;
    private int selectionIndex;

    /**************************************/
    //For button layout in rows of 3
    /**************************************/
    List<bool> buttonToggled;
    public int toggledIndex = 0;
    Vector2 scrollPos = new Vector2();
    Vector2 statScroll = new Vector2();
    Vector2 atkScroll = new Vector2();
    UnitTypes filterType = UnitTypes.HUMANOID;
    /*****************************************************/

    [MenuItem("Window/Unit Data Editor %#e")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(UnitDatabase));
    }

    void OnEnable()
    {
        string objectPath = EditorPrefs.GetString("UnitDataPath");
        unitPDataList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnitDataList)) as UnitDataList;
           
        string atkPath = EditorPrefs.GetString("AttackDatabasePath");
        atkDataList = AssetDatabase.LoadAssetAtPath(atkPath, typeof(AttackDataList)) as AttackDataList;

    }

    void OnDisable()
    {
        if(atkWindow != null)
        {
            atkWindow.Close();
            atkWindow = null;
        }
            
    }

    void OnGUI()
    {
        /***********************************/
        //Header
        /***********************************/

        GUILayout.Label("Unit Data Editor", EditorStyles.boldLabel);
        if (unitDataList == null)
        {
            EditorGUILayout.LabelField("No Attack database available, error.");
        }

        /******************************************/
        //Sanity Checks and setups
        /******************************************/

        GUILayout.Space(20);

        if (unitDataList.unitList == null)
        {
            Debug.Log("New List was made");
            unitDataList.unitList = new List<UnitData>();
            toggledIndex = 0;
        }

        if (buttonToggled == null || buttonToggled.Count < unitDataList.unitList.Count)
        {
            buttonToggled = new List<bool>();
            toggledIndex = 0;
            for (int i = 0; i < unitDataList.unitList.Count; i++)
                buttonToggled.Add(false);
        }

        if (toggledIndex > unitDataList.unitList.Count)
        {
            Debug.Log("Index was out of bounds");
            toggledIndex = unitDataList.unitList.Count - 1;
        }

        /************************************************************/

        /******************************************/
        //Display things
        /******************************************/
        if (unitDataList != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                findPrev();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                findNext();
            }

            GUILayout.Space(60);

            if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false)))
            {
                AddUnit();
                unitDataList.unitList[toggledIndex].uType = filterType;

            }
            if (GUILayout.Button("Delete Item", GUILayout.ExpandWidth(false)))
            {
                DeleteUnit(toggledIndex);
            }

            filterType = (UnitTypes)EditorGUILayout.EnumPopup(filterType);

            if (GUILayout.Button("Sort", GUILayout.ExpandWidth(false)))
            {
                sortByName();
            }

            GUILayout.EndHorizontal();
            if (unitDataList.unitList == null)
            {
                Debug.Log("Mistakes were made - You should never reach this");
            }  

            /****************************************/
            //How the list will look
            /***************************************/
            
            if (unitDataList.unitList.Count > 0)
            {
                if (unitDataList.unitList[toggledIndex].uType != filterType)
                {
                    toggledIndex = 0;
                    findNext();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                //----------Show all the buttons-----------
                showScrollButtons();
                //-----------------------------------------
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);


                EditorGUILayout.BeginVertical();
                //---------Current selection info-----------
                statScroll = GUILayout.BeginScrollView(statScroll,false, false);

                GUILayout.BeginHorizontal();
                unitDataList.unitList[toggledIndex].uName = EditorGUILayout.TextField("Unit Name", unitDataList.unitList[toggledIndex].uName);
                unitDataList.unitList[toggledIndex].uType = (UnitTypes)EditorGUILayout.EnumPopup("Unit Type", unitDataList.unitList[toggledIndex].uType);
                if (unitDataList.unitList[toggledIndex].uType != filterType)
                    filterType = unitDataList.unitList[toggledIndex].uType;
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                unitDataList.unitList[toggledIndex].isHero = EditorGUILayout.Toggle("Hero", unitDataList.unitList[toggledIndex].isHero, GUILayout.ExpandWidth(false));
                GUILayout.Space(10);

                EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
                unitDataList.unitList[toggledIndex].maxHp = EditorGUILayout.IntField("HP", unitDataList.unitList[toggledIndex].maxHp, GUILayout.ExpandWidth(false));
                unitDataList.unitList[toggledIndex].maxSpeed = EditorGUILayout.Slider("Speed", unitDataList.unitList[toggledIndex].maxSpeed, 5, 20, GUILayout.ExpandWidth(false));
                
                if (unitDataList.unitList[toggledIndex].isHero)
                {
                    unitDataList.unitList[toggledIndex].impulse = EditorGUILayout.IntSlider("Impulse", unitDataList.unitList[toggledIndex].impulse, 3, 6, GUILayout.ExpandWidth(false));
                    unitDataList.unitList[toggledIndex].memory = EditorGUILayout.IntSlider("Memory", unitDataList.unitList[toggledIndex].memory, 0, 5, GUILayout.ExpandWidth(false));
                }
                
                /*****************************************/
                //Atk/Def Factors
                /*****************************************/
                EditorGUILayout.Space();
                
                showFactors = EditorGUILayout.Foldout(showFactors, "Type Factors");
                
                if (showFactors)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Attack Factors");
                    EditorGUILayout.LabelField("Defence Factors");
                    GUILayout.EndHorizontal();
                    for (int i = 0; i < (int)DamageType.TOTAL; i++)
                    {
                        GUILayout.BeginHorizontal();
                        unitDataList.unitList[toggledIndex].typeAtk[i] = EditorGUILayout.Slider(Enum.GetName(typeof(DamageType), i), unitDataList.unitList[toggledIndex].typeAtk[i], 0.5f, 2.0f);
                        unitDataList.unitList[toggledIndex].typeDef[i] = EditorGUILayout.Slider(Enum.GetName(typeof(DamageType), i), unitDataList.unitList[toggledIndex].typeDef[i], 0.5f, 2.0f);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Reset Attack Factors"))
                        for (int i = 0; i < (int)DamageType.TOTAL; i++)
                        {
                            unitDataList.unitList[toggledIndex].typeAtk[i] = 1.0f;
                        }
                    if (GUILayout.Button("Reset Defence Factors"))
                        for (int i = 0; i < (int)DamageType.TOTAL; i++)
                        {
                            unitDataList.unitList[toggledIndex].typeDef[i] = 1.0f;
                        }
                    GUILayout.EndHorizontal();
                    
                }

                GUILayout.EndScrollView();


                /*************************************/
                //UnitAttack WindowSpawn
                /************************************/
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Unit Attacks", EditorStyles.boldLabel);

                atkScroll = EditorGUILayout.BeginScrollView(atkScroll, GUILayout.Height(120));
                foreach(string x in unitDataList.unitList[toggledIndex].attacksData.ToList<String>())
                {
                    AttackData knownAtk = atkDataList.findByID(x);
                    if(knownAtk != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        //EditorGUILayout.PrefixLabel(knownAtk.aName);
                        showAttackSummary(knownAtk);
                        if (GUILayout.Button("Remove", GUILayout.Width(120f)))
                            RemoveAttack((toggledIndex), x);
                        EditorGUILayout.EndHorizontal();
                    }       
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Attack List"))
                {
                    AttackListWindow.CardEdit(false);
                    atkWindow = (AttackListWindow)EditorWindow.GetWindow(typeof(AttackListWindow));
                }


                if (GUILayout.Button("Clear Residual Data"))
                    refreshUnitsAttackLists();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                GUILayout.EndHorizontal();

            }
            else
            {
                GUILayout.Label("This Unit List is Empty.");
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(unitDataList);
            }
        }
    }

    void AddUnit()
    {
        UnitData newItem = new UnitData();

        unitDataList.unitList.Add(newItem);
        if (unitDataList.unitList.Count == 1)
        {
            buttonToggled = new List<bool>();
            toggledIndex = 0;
        }

        buttonToggled.Add(false);
        toggledIndex = unitDataList.unitList.Count - 1;
    }

    void DeleteUnit(int index)
    {
        if (unitDataList.unitList.Count > 0 )
        {
            unitDataList.unitList.RemoveAt(index);
            buttonToggled.RemoveAt(index);
        }
        if (index >= unitDataList.unitList.Count)
        {
            toggledIndex= unitDataList.unitList.Count - 1;
        }

    }

    public void AddAttack(int unitIndex, String atk)
    {
        unitDataList.unitList[unitIndex].attacksData.Add(String.Copy(atk));
    }

    void RemoveAttack(int unitIndex, string atk)
    {
        unitDataList.unitList[unitIndex].attacksData.Remove(atk);
    }

    void refreshUnitsAttackLists()
    {
        //Search all units
        for(int i = 0; i < unitDataList.unitList.Count; i++)
        {
            //Display unit's known attacks - Scrapping the nulls
            foreach (string x in unitDataList.unitList[i].attacksData.ToList())
            {
                //Search the DB for matching unique IDs
                AttackData knownAtk = atkDataList.findByID(x);
                if (knownAtk == null)
                {
                    RemoveAttack(i, x);
                }               
            } 
        }
    }


    void showScrollButtons()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
        if (buttonToggled == null)
        {
            buttonToggled = new List<bool>();
            for (int i = 0; i < unitDataList.unitList.Count; i++)
            {
                buttonToggled.Add(false);
            }
        }

        for (int i = 0; i < unitDataList.unitList.Count; i++)
        {
            //Button
            if (unitDataList.unitList[i].uType == filterType)
            {
                colorToggle(i);
                if (GUILayout.Button(unitDataList.unitList[i].uName, GUILayout.Width(150)))
                {
                    buttonToggled[i] = true;
                    if (buttonToggled[i] && i != toggledIndex)
                    {
                        if (toggledIndex < buttonToggled.Count)
                        {
                            buttonToggled[toggledIndex] = false;
                        }
                        toggledIndex = i;
                    }
                }
                colorToggle(i);
            }

        }

        EditorGUILayout.EndScrollView();
        GUI.color = Color.white;
    }

    void showAttackSummary(AttackData currentAtk)
    {
        if (currentAtk != null)
        {
            
            EditorGUILayout.PrefixLabel(currentAtk.aName, EditorStyles.boldLabel);
            EditorGUILayout.TextField("Pattern: " + currentAtk.pattern, GUILayout.ExpandWidth(false));
            EditorGUILayout.TextField("Damage: " + currentAtk.expectedMin + " - " + currentAtk.expectedMax, GUILayout.ExpandWidth(false));
            EditorGUILayout.TextField("Range: " + currentAtk.range, GUILayout.ExpandWidth(false));
            
        }
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

    void findNext()
    {
        int i = toggledIndex;
        bool wasFound = false;
        //Start looking at next button
        while ((i + 1) < unitDataList.unitList.Count && !wasFound)
        {
            i++;
            if (unitDataList.unitList[i].uType == filterType)
            {
                wasFound = true;
            }
        }

        if (wasFound)
        {
            buttonToggled[i] = true;
            if (buttonToggled[i] && i != toggledIndex)
            {
                if (toggledIndex < buttonToggled.Count)
                    buttonToggled[toggledIndex] = false;
                toggledIndex = i;
            }
        }
    }

    void findPrev()
    {
        int i = toggledIndex;
        bool wasFound = false;
        //Start looking at next button
        while ((i - 1) >= 0 && !wasFound)
        {
            i--;
            if (unitDataList.unitList[i].uType == filterType)
            {
                wasFound = true;
            }
        }

        if (wasFound)
        {
            buttonToggled[i] = true;
            if (buttonToggled[i] && i != toggledIndex)
            {
                if (toggledIndex < buttonToggled.Count)
                    buttonToggled[toggledIndex] = false;
                toggledIndex = i;
            }
        }
    }

    /**************************/
    //Sort functions
    /****************************/
    void sortByName()
    {
        String oldID = unitDataList.unitList[toggledIndex].uniqueId;
        unitDataList.unitList.Sort(delegate (UnitData x, UnitData y)
        {
            if (x.uName == null && y.uName == null) return 0;
            else if (x.uName == null) return -1;
            else if (y.uName == null) return 1;
            else return x.uName.CompareTo(y.uName);
        });

        toggledIndex= findIndexByID(oldID);
    }

    int findIndexByID(String id)
    {
        for (int i = 0; i < unitDataList.unitList.Count; i++)
        {
            if (unitDataList.unitList[i].uniqueId == id)
                return i;
        }
        return 0;
    }

}

