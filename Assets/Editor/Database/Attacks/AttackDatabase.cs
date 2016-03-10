using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AttackDatabase : EditorWindow
{
    public AttackDataList attackDataList;

    /**************************************/
    //For button layout
    /**************************************/
    List<bool> buttonToggled;
    int toggledIndex = 0;
    Vector2 scrollPos = new Vector2();
    AttackPattern filterPattern = AttackPattern.SINGLE;
    DamageType currentDmgType;
    /*****************************************************/

    [MenuItem("Window/Attack Data Editor %#e")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(AttackDatabase));
    }

    void OnEnable()
    {
        string objectPath = EditorPrefs.GetString("AttackDatabasePath");
        attackDataList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(AttackDataList)) as AttackDataList;
    }

    void OnGUI()
    {
        /***********************************/
        //Header
        /***********************************/
        GUILayout.Label("Attack Data Editor", EditorStyles.boldLabel);
        if (attackDataList == null)
        {
            EditorGUILayout.LabelField("No Attack database available, error.");
        }


        /******************************************/
        //Sanity Checks and setups
        /******************************************/

        GUILayout.Space(20);
        if (attackDataList.attackList == null)
        {
            Debug.Log("New List was made");
            attackDataList.attackList = new List<AttackData>();
            toggledIndex = 0;
        }

        if (buttonToggled == null || buttonToggled.Count < attackDataList.attackList.Count)
        {
            buttonToggled = new List<bool>();
            toggledIndex = 0;
            for (int i = 0; i < attackDataList.attackList.Count; i++)
                buttonToggled.Add(false);
        }

        if (toggledIndex > attackDataList.attackList.Count)
        {
            Debug.Log("Index was out of bounds");
            toggledIndex = attackDataList.attackList.Count - 1;
        }

        /************************************************************/

        /******************************************/
        //Display things
        /******************************************/
        if (attackDataList != null)
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
                AddAttack();
                attackDataList.attackList[toggledIndex].pattern = filterPattern;
            }
            if (GUILayout.Button("Delete Item", GUILayout.ExpandWidth(false)))
            {
                DeleteAttack(toggledIndex);
            }

            filterPattern = (AttackPattern)EditorGUILayout.EnumPopup(filterPattern);

            if (GUILayout.Button("Sort", GUILayout.ExpandWidth(false)))
            {
                sortByName();
            }

            GUILayout.EndHorizontal();
            if (attackDataList.attackList == null)
                Debug.Log("Mistakes were made - You should never reach this");

            /****************************************/
            //How the list will look
            /***************************************/

            if (attackDataList.attackList.Count > 0)
            {
                if(attackDataList.attackList[toggledIndex].pattern != filterPattern)
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
                GUILayout.BeginHorizontal();
                attackDataList.attackList[toggledIndex].aName = EditorGUILayout.TextField("Attack Name", attackDataList.attackList[toggledIndex].aName as string);
                attackDataList.attackList[toggledIndex].pattern = (AttackPattern)EditorGUILayout.EnumPopup("Attack Pattern", attackDataList.attackList[toggledIndex].pattern);
                if (attackDataList.attackList[toggledIndex].pattern != filterPattern)
                    filterPattern = attackDataList.attackList[toggledIndex].pattern;
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                attackDataList.attackList[toggledIndex].range = EditorGUILayout.Slider("Range", attackDataList.attackList[toggledIndex].range, 4f, 30f);
                if (attackDataList.attackList[toggledIndex].pattern != AttackPattern.SINGLE)
                    attackDataList.attackList[toggledIndex].radius = EditorGUILayout.Slider("Radius", attackDataList.attackList[toggledIndex].radius, 4f, 30f);
                GUILayout.Space(10);
                attackDataList.attackList[toggledIndex].dmg = EditorGUILayout.IntSlider("Damage/Success", attackDataList.attackList[toggledIndex].dmg, 1, 5);
                attackDataList.attackList[toggledIndex].nDice = EditorGUILayout.IntSlider("Number of rolls", attackDataList.attackList[toggledIndex].nDice, 1, 10);
                attackDataList.attackList[toggledIndex].roll = EditorGUILayout.IntSlider("%Success/Roll", attackDataList.attackList[toggledIndex].roll, 1, 100);

                //Display Damage types---------------------------
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Attack Damage Type", EditorStyles.boldLabel);
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                foreach (DamageType x in attackDataList.attackList[toggledIndex].dmgTypes.ToList())
                {
                    if (GUILayout.Button(x.ToString()))
                        attackDataList.attackList[toggledIndex].dmgTypes.Remove(x);
                }
                GUILayout.EndVertical();
                
                currentDmgType = (DamageType)EditorGUILayout.EnumPopup("Damage Type", currentDmgType);
                if (GUILayout.Button("Add"))
                    if (!attackDataList.attackList[toggledIndex].dmgTypes.Contains(currentDmgType))
                        attackDataList.attackList[toggledIndex].dmgTypes.Add(currentDmgType);
                GUILayout.EndHorizontal();
                //---------------------------------------------

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Attack Stats Summary", EditorStyles.boldLabel);
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                string expected = "Expected: " + (attackDataList.attackList[toggledIndex].dmg * attackDataList.attackList[toggledIndex].nDice * ((float)attackDataList.attackList[toggledIndex].roll/100)).ToString();
                EditorGUILayout.LabelField(expected);

                attackDataList.attackList[toggledIndex].expectedMin = calculateMin(attackDataList.attackList[toggledIndex].nDice, attackDataList.attackList[toggledIndex].roll) * attackDataList.attackList[toggledIndex].dmg;
                attackDataList.attackList[toggledIndex].expectedMax = calculateMax(attackDataList.attackList[toggledIndex].nDice, attackDataList.attackList[toggledIndex].roll) * attackDataList.attackList[toggledIndex].dmg;

                string expectedRange = "Damage Range : " + attackDataList.attackList[toggledIndex].expectedMin.ToString() + " - " + attackDataList.attackList[toggledIndex].expectedMax.ToString();
                EditorGUILayout.LabelField(expectedRange);
                GUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("This Attack List is Empty.");
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(attackDataList);
            }
        }
    }

    void AddAttack()
    {
        AttackData newItem = new AttackData();
        attackDataList.attackList.Add(newItem);
        if(attackDataList.attackList.Count == 1)
        {
            buttonToggled = new List<bool>();
            toggledIndex = 0;
        } 
        buttonToggled.Add(false);
        toggledIndex= attackDataList.attackList.Count - 1;
    }

    void DeleteAttack(int index)
    {
        if(attackDataList.attackList.Count > 0)
        {
            attackDataList.attackList.RemoveAt(index);
            buttonToggled.RemoveAt(index);
        }
        if(index >= attackDataList.attackList.Count)
        {
            toggledIndex = attackDataList.attackList.Count - 1;
        }
    }

    public static int calculateMin(int n, int pInt)
    {
        float totalProb = 0f;
        int i = 0;
        float p = ((float)pInt)/100f;
        while(totalProb < 0.05f)
        {
            //Combinaison
            totalProb += ((float)factorial(n) / (factorial(i) * factorial(n - i))) * (Mathf.Pow(p, i)) * (Mathf.Pow(1 - p, n - i));
            i++;
        }

        return i -1;
    }

    public static int calculateMax(int n, int pInt)
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

    static int factorial(int n)
    {
        int result = 1;
        for (int i = 1; i < n; i++)
        {
            result = result * i;
        }
        return result;
    }

    void showScrollButtons()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

        if(buttonToggled == null)
        {
            buttonToggled = new List<bool>();
            for (int i = 0; i < attackDataList.attackList.Count; i++)
            {
                buttonToggled.Add(false);
            }
        }

        for (int i = 0; i < attackDataList.attackList.Count; i++)
        {
            //Button
            if(attackDataList.attackList[i].pattern == filterPattern)
            {
                colorToggle(i);
                if (GUILayout.Button(attackDataList.attackList[i].aName, GUILayout.Width(150)))
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
                
            }
                    
        }

        EditorGUILayout.EndScrollView();
        GUI.color = Color.white;
        
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
        while ((i + 1) < attackDataList.attackList.Count && !wasFound)
        {
            i++;
            if (attackDataList.attackList[i].pattern == filterPattern)
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
            if (attackDataList.attackList[i].pattern == filterPattern)
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
        String oldID = attackDataList.attackList[toggledIndex].uniqueId;
        attackDataList.attackList.Sort(delegate (AttackData x, AttackData y)
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
        for(int i = 0; i < attackDataList.attackList.Count; i++)
        {
            if (attackDataList.attackList[i].uniqueId == id)
                return i;
        }
        return 0;
    }
}

