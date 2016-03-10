using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;


public class CardDatabase : EditorWindow
{
    //Link to other DB
    private UnitDataList unitDataList;
    private AttackDataList atkDataList;
    private AttackListWindow atkListWindow;
    private UnitListWindow unitListWindow;
    private SchoolMaskWindow maskWindow;

    private CardDataList cardDataList;

    /**************************************/
    //For button layout
    /**************************************/
    List<bool> buttonToggled;
    public int toggledIndex = 0;
    Vector2 scrollPos = new Vector2();
    Vector2 statScroll = new Vector2();
    CardType filterCard = CardType.UNIT;
    /*****************************************************/

    /******************************************/
    //Card type dependant display
    /*****************************************/
    private UnitData currentUnit;
    private AttackData currentAtk;

    UnitTypes filterUnit = UnitTypes.HUMANOID;

    [MenuItem("Window/Card Data Editor %#e")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(CardDatabase));
        
    }

    void OnEnable()
    {
        //Link to the correct DB on enable. CardDataPath is necessarry for the other links
        string objectPath = EditorPrefs.GetString("CardDataPath");
        cardDataList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(CardDataList)) as CardDataList;

        string atkPath = EditorPrefs.GetString("AttackDatabasePath");
        atkDataList = AssetDatabase.LoadAssetAtPath(atkPath, typeof(AttackDataList)) as AttackDataList;

        string unitPath = EditorPrefs.GetString("UnitDataPath");
        unitDataList = AssetDatabase.LoadAssetAtPath(unitPath, typeof(UnitDataList)) as UnitDataList;

    }

    void OnDisable()
    {
        if (unitListWindow != null)
        {
            unitListWindow.Close();
            unitListWindow = null;
        }
            
        if (atkListWindow != null)
        {
            atkListWindow.Close();
            atkListWindow = null;
        }

        if(maskWindow != null)
        {
            maskWindow.Close();
            maskWindow = null;
        }
            
    }

    void OnGUI()
    {
        /***********************************/
        //Header
        /***********************************/
        GUILayout.Label("Card Data Editor", EditorStyles.boldLabel);
        if (cardDataList == null)
        {
            EditorGUILayout.LabelField("No card database available, error.");
        }

        /******************************************/
        //Sanity Checks and setups
        /******************************************/
        GUILayout.Space(20);
        if (cardDataList.cardList == null)
        {
            Debug.Log("New List was made");
            cardDataList.cardList = new List<CardData>();
            toggledIndex = 0;
        }

        if (buttonToggled == null || buttonToggled.Count < cardDataList.cardList.Count)
        {
            buttonToggled = new List<bool>();
            toggledIndex = 0;
            for (int i = 0; i < cardDataList.cardList.Count; i++)
                buttonToggled.Add(false);
        }

        if(toggledIndex > cardDataList.cardList.Count)
        {
            Debug.Log("Index was out of bounds");
            toggledIndex = cardDataList.cardList.Count - 1;
        }

        /******************************************/

        /******************************************/
        //Display things
        /******************************************/
        if (cardDataList != null)
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
                AddCard();
                cardDataList.cardList[toggledIndex].cardType = filterCard;
            }
            if (GUILayout.Button("Delete Item", GUILayout.ExpandWidth(false)))
            {
                DeleteCard(toggledIndex);
            }

            filterCard = (CardType)EditorGUILayout.EnumPopup(filterCard);

            if (GUILayout.Button("Elements", GUILayout.ExpandWidth(false)))
            {
                EditorWindow.GetWindow(typeof(SchoolMaskWindow));
            }

            if (GUILayout.Button("Sort", GUILayout.ExpandWidth(false)))
            {
                sortByName();
            }

            GUILayout.EndHorizontal(); 

            /****************************************/
            //How the GENEREIC list will look -Probly gonna implement different for each card type
            /***************************************/

            if (cardDataList.cardList.Count > 0)
            {
                if (cardDataList.cardList[toggledIndex].cardType != filterCard)
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
                statScroll = GUILayout.BeginScrollView(statScroll, false, false, GUILayout.MinHeight(175));

                GUILayout.BeginHorizontal();
                cardDataList.cardList[toggledIndex].cardName = EditorGUILayout.TextField("Card Name", cardDataList.cardList[toggledIndex].cardName, GUILayout.Width(300));

                cardDataList.cardList[toggledIndex].cardType = (CardType)EditorGUILayout.EnumPopup(cardDataList.cardList[toggledIndex].cardType, GUILayout.Width(200));
                if (cardDataList.cardList[toggledIndex].cardType != filterCard)
                    filterCard = cardDataList.cardList[toggledIndex].cardType;
               
                GUILayout.EndHorizontal();

                EditorGUILayout.TextField("School", cardDataList.cardList[toggledIndex].school.ToString(), GUILayout.Width(300));
                cardDataList.cardList[toggledIndex].rechargeTime = EditorGUILayout.IntSlider("Recharge Time", cardDataList.cardList[toggledIndex].rechargeTime, 0, 5, GUILayout.ExpandWidth(false));


                GUILayout.Space(10);

                EditorGUILayout.LabelField("ManaCost", EditorStyles.boldLabel);
                cardDataList.cardList[toggledIndex].manaCost[0] = EditorGUILayout.IntSlider("Any", cardDataList.cardList[toggledIndex].manaCost[0], 0, 5, GUILayout.Width(300));
                cardDataList.cardList[toggledIndex].manaCost[1] = EditorGUILayout.IntSlider("Fire", cardDataList.cardList[toggledIndex].manaCost[1], 0, 5, GUILayout.Width(300));
                cardDataList.cardList[toggledIndex].manaCost[2] = EditorGUILayout.IntSlider("Water", cardDataList.cardList[toggledIndex].manaCost[2], 0, 5, GUILayout.Width(300));
                cardDataList.cardList[toggledIndex].manaCost[3] = EditorGUILayout.IntSlider("Wind", cardDataList.cardList[toggledIndex].manaCost[3], 0, 5, GUILayout.Width(300));
                cardDataList.cardList[toggledIndex].manaCost[4] = EditorGUILayout.IntSlider("Earth", cardDataList.cardList[toggledIndex].manaCost[4], 0, 5, GUILayout.Width(300));
                


                GUILayout.EndScrollView();
                //Card type specific showing
                switch (filterCard)
                {
                    case CardType.UNIT:
                        showUnitCardInfo();
                        setCardSchool();
                        break;
                    case CardType.CRYSTAL:
                        showCrystalCardInfo();
                        setCardSchoolCrystal();
                        break;
                    case CardType.EQUIPMENT:
                        showEquipCardInfo();
                        setCardSchool();
                        break;
                    case CardType.SPELL:
                        showSpellCardInfo();
                        setCardSchool();
                        break;
                }
                EditorGUILayout.EndVertical();
                GUILayout.EndHorizontal();
                if(GUILayout.Button("Create Card"))
                {
                    GameObject card = Instantiate(Resources.Load("Card") as GameObject);
                    //card.transform.SetParent(GameObject.Find("Hand").transform);
                    
                    switch (cardDataList.cardList[toggledIndex].cardType)
                    {
                        case CardType.UNIT:
                            Debug.Log("Unit");
                            card.AddComponent<UnitCard>();
                            card.GetComponent<UnitCard>().init((cardDataList.cardList[toggledIndex]));
                            break;
                        case CardType.SPELL:
                            Debug.Log("Spell");
                            card.AddComponent<SpellCard>();
                            card.GetComponent<SpellCard>().init((cardDataList.cardList[toggledIndex]));
                            break;
                        case CardType.EQUIPMENT:
                            Debug.Log("Equip");
                            card.AddComponent<EquipmentCard>();
                            card.GetComponent<EquipmentCard>().init((cardDataList.cardList[toggledIndex]));
                            break;
                        case CardType.CRYSTAL:
                            Debug.Log("Crystal");
                            card.AddComponent<CrystalCard>();
                            card.GetComponent<CrystalCard>().init((cardDataList.cardList[toggledIndex]));
                            break;
                    }
                    Text text = card.transform.GetChild(1).GetComponent<Text>();
                    text.text = card.name;

                }
                EditorGUILayout.LabelField("Schools: " + SchoolMaskWindow.getFilterNames(), EditorStyles.boldLabel);

            }
            else
            {
                GUILayout.Label("This Card List is Empty.");
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(cardDataList);
            }
        }
    }

    void AddCard()
    {
        CardData newItem = new CardData();

        cardDataList.cardList.Add(newItem);
        buttonToggled.Add(false);
        toggledIndex = cardDataList.cardList.Count - 1;
    }

    void DeleteCard(int index)
    {
        if (cardDataList.cardList.Count > 0)
        {
            cardDataList.cardList.RemoveAt(index);
            buttonToggled.RemoveAt(index);
        }
        if (index >= cardDataList.cardList.Count)
        {
            toggledIndex = cardDataList.cardList.Count - 1;
        }
    }

    void showScrollButtons()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MinWidth(180));
        if (buttonToggled == null)
        {
            buttonToggled = new List<bool>();
            for (int i = 0; i < cardDataList.cardList.Count; i++)
            {
                buttonToggled.Add(false);
            }
        }

        for (int i = 0; i < cardDataList.cardList.Count; i++)
        {
            //Button
            if (cardDataList.cardList[i].cardType == filterCard && SchoolMaskWindow.isCardShown(cardDataList.cardList[i]))
            {
                colorToggle(i);
                if (GUILayout.Button(cardDataList.cardList[i].cardName, GUILayout.Width(150)))
                {
                    buttonToggled[i] = true;
                    if (buttonToggled[i] && i != toggledIndex)
                    {
                        if (toggledIndex < buttonToggled.Count && toggledIndex >=0)
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
        while ((i + 1) < cardDataList.cardList.Count && !wasFound)
        {
            i++;
            if (cardDataList.cardList[i].cardType == filterCard)
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
            if (cardDataList.cardList[i].cardType == filterCard)
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

    /************************************/
    //TYPE DEPENDANT SHOWING
    /************************************/

    /*****UNITS*********************************/

    void showUnitCardInfo()
    {
        //Filter the desired creature type
        List<UnitData> unitsInList = new List<UnitData>();
        List<String> unitNames = new List<String>();

        foreach(UnitData x in unitDataList.unitList.ToList<UnitData>())
        {
            if(x.uType == filterUnit)
            {
                unitsInList.Add(x);
                unitNames.Add(x.uName);
            }
        }

        
        if(GUILayout.Button("Unit List"))
        {
            unitListWindow = (UnitListWindow)EditorWindow.GetWindow(typeof(UnitListWindow));
        }

        currentUnit = unitDataList.findByID(cardDataList.cardList[toggledIndex].linkedTo);

        GUILayout.Space(10);
        //Associate the correct UnitType, possibly load into current object field
        if (currentUnit != null)
        {
            if (currentUnit.isHero)
                EditorGUILayout.LabelField("Hero Unit");
            EditorGUILayout.LabelField(currentUnit.uName, EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Stats");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("HP");
            EditorGUILayout.TextField(currentUnit.maxHp.ToString());
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Speed ");
            EditorGUILayout.TextField(currentUnit.maxSpeed.ToString());
            EditorGUILayout.EndHorizontal();

            if (currentUnit.isHero)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Impulse ");
                EditorGUILayout.TextField(currentUnit.impulse.ToString());
                //EditorGUILayout.EndHorizontal();

                //.BeginHorizontal();
                EditorGUILayout.LabelField("Memory ");
                EditorGUILayout.TextField(currentUnit.memory.ToString());
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.LabelField("Known Attacks");
            showKnownAttacks(currentUnit);

        }
        else
            EditorGUILayout.LabelField("No Unit Linked", EditorStyles.boldLabel);

    }

    void showKnownAttacks(UnitData unit)
    {
        if (atkDataList != null && atkDataList.attackList != null && atkDataList.attackList.Count > 0)
        {
            foreach (string x in unit.attacksData.ToList<String>())
            {
                AttackData knownAtk = atkDataList.findByID(x);
                if (knownAtk != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(knownAtk.aName);
                    EditorGUILayout.TextField("Pattern: " + knownAtk.pattern, GUILayout.ExpandWidth(false));
                    EditorGUILayout.TextField("Damage: " + knownAtk.expectedMin + " - " + knownAtk.expectedMax, GUILayout.ExpandWidth(false));
                    EditorGUILayout.TextField("Range: " + knownAtk.range, GUILayout.ExpandWidth(false));
                    EditorGUILayout.EndHorizontal();
                }
            }    
        }
    }


    public void assignElement(String uniqueID)
    {
        cardDataList.cardList[toggledIndex].linkedTo = uniqueID;
    }


    /*****CRYSTALS******************************/
    void showCrystalCardInfo()
    {
        EditorGUILayout.Space();
        cardDataList.cardList[toggledIndex].durability = EditorGUILayout.IntField("Durability", cardDataList.cardList[toggledIndex].durability);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mana Yield", EditorStyles.boldLabel);
        cardDataList.cardList[toggledIndex].manaYield[0] = EditorGUILayout.IntSlider("Any", cardDataList.cardList[toggledIndex].manaYield[0], 0, 5, GUILayout.Width(300));
        cardDataList.cardList[toggledIndex].manaYield[1] = EditorGUILayout.IntSlider("Fire", cardDataList.cardList[toggledIndex].manaYield[1], 0, 5, GUILayout.Width(300));
        cardDataList.cardList[toggledIndex].manaYield[2] = EditorGUILayout.IntSlider("Water", cardDataList.cardList[toggledIndex].manaYield[2], 0, 5, GUILayout.Width(300));
        cardDataList.cardList[toggledIndex].manaYield[3] = EditorGUILayout.IntSlider("Wind", cardDataList.cardList[toggledIndex].manaYield[3], 0, 5, GUILayout.Width(300));
        cardDataList.cardList[toggledIndex].manaYield[4] = EditorGUILayout.IntSlider("Earth", cardDataList.cardList[toggledIndex].manaYield[4], 0, 5, GUILayout.Width(300));

    }
    

    /*****EQUIPMENT****************************/
    void showEquipCardInfo()
    {
        EditorGUILayout.Space();
        cardDataList.cardList[toggledIndex].durability = EditorGUILayout.IntField("Durability", cardDataList.cardList[toggledIndex].durability);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Unit Type & Equipment Slot", EditorStyles.boldLabel);
        cardDataList.cardList[toggledIndex].type = (UnitTypes)EditorGUILayout.EnumPopup(cardDataList.cardList[toggledIndex].type, GUILayout.Width(200));
        cardDataList.cardList[toggledIndex].eType = (EquipType)EditorGUILayout.EnumPopup(cardDataList.cardList[toggledIndex].eType, GUILayout.Width(200));
    }


    /******SPELL******************************/
    void showSpellCardInfo()
    {
        if (GUILayout.Button("Attack List"))
        {
            AttackListWindow.CardEdit(true);
            atkListWindow = (AttackListWindow)EditorWindow.GetWindow(typeof(AttackListWindow));
        }

        currentAtk = atkDataList.findByID(cardDataList.cardList[toggledIndex].linkedTo);

        if (currentAtk != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(currentAtk.aName);
            EditorGUILayout.TextField("Pattern: " + currentAtk.pattern, GUILayout.ExpandWidth(false));
            EditorGUILayout.TextField("Damage: " + currentAtk.expectedMin + " - " + currentAtk.expectedMax, GUILayout.ExpandWidth(false));
            EditorGUILayout.TextField("Range: " + currentAtk.range, GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();
        }
        else
            EditorGUILayout.LabelField("No Attack selected", EditorStyles.boldLabel);
    }

    /*******Schools*****************/
    void setCardSchool()
    {
        int value = toTypeValue(cardDataList.cardList[toggledIndex].manaCost);
        
        switch(value)
        {
            case 2:
                cardDataList.cardList[toggledIndex].school = Elements.FIRE;
                break;
            case 4:
                cardDataList.cardList[toggledIndex].school = Elements.WATER;
                break;
            case 8:
                cardDataList.cardList[toggledIndex].school = Elements.WIND;
                break;
            case 16:
                cardDataList.cardList[toggledIndex].school = Elements.EARTH;
                break;
            case 10:
                cardDataList.cardList[toggledIndex].school = Elements.LIGHTNING;
                break;
            case 12:
                cardDataList.cardList[toggledIndex].school = Elements.ICE;
                break;
            case 18:
                cardDataList.cardList[toggledIndex].school = Elements.METAL;
                break;
            case 20:
                cardDataList.cardList[toggledIndex].school = Elements.POISON;
                break;
            default:
                cardDataList.cardList[toggledIndex].school = Elements.NORMAL;
                break;
        }
    }

    void setCardSchoolCrystal()
    {
        int value = toTypeValue(cardDataList.cardList[toggledIndex].manaYield);

        switch (value)
        {
            case 2:
                cardDataList.cardList[toggledIndex].school = Elements.FIRE;
                break;
            case 4:
                cardDataList.cardList[toggledIndex].school = Elements.WATER;
                break;
            case 8:
                cardDataList.cardList[toggledIndex].school = Elements.WIND;
                break;
            case 16:
                cardDataList.cardList[toggledIndex].school = Elements.EARTH;
                break;
            case 10:
                cardDataList.cardList[toggledIndex].school = Elements.LIGHTNING;
                break;
            case 12:
                cardDataList.cardList[toggledIndex].school = Elements.ICE;
                break;
            case 18:
                cardDataList.cardList[toggledIndex].school = Elements.METAL;
                break;
            case 20:
                cardDataList.cardList[toggledIndex].school = Elements.POISON;
                break;
            default:
                cardDataList.cardList[toggledIndex].school = Elements.NORMAL;
                break;
        }
    }

    int toTypeValue(int[] manaCosts)
    {
        int sum = 0;

        if (manaCosts[1] != 0)
            sum += 2;
        if (manaCosts[2] != 0)
            sum += 4;
        if (manaCosts[3] != 0)
            sum += 8;
        if (manaCosts[4] != 0)
            sum += 16;

        return sum;
    }

    /**************************/
    //Sort functions
    /****************************/
    void sortByName()
    {
        String oldID = cardDataList.cardList[toggledIndex].uniqueId;
        cardDataList.cardList.Sort(delegate (CardData x, CardData y)
        {
            if (x.cardName == null && y.cardName == null) return 0;
            else if (x.cardName == null) return -1;
            else if (y.cardName == null) return 1;
            else return x.cardName.CompareTo(y.cardName);
        });

        toggledIndex = findIndexByID(oldID);
    }

    int findIndexByID(String id)
    {
        for (int i = 0; i < cardDataList.cardList.Count; i++)
        {
            if (cardDataList.cardList[i].uniqueId == id)
                return i;
        }
        return 0;
    }

}

