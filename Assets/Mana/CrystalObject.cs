using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class CrystalObject : MonoBehaviour
{
    public CrystalObject() { }

    CrystalCard linkedCard;
    public CrystalCard LinkedCard { get { return linkedCard; } set { linkedCard = value; } }

    Hero owner;
    Image img;
    Text buttText;
    Transform crysArea;
    public Hero Owner { get { return owner; } }

    bool used = false;
    public bool Used { get { return used; } set { used = value; } }

    public void init(CrystalCard card)
    {
        buttText = GetComponentInChildren<Text>();
        cName = card.cardName;
        school = card.school;
        durability = card.durability;

        manaYield = new int[5];
        for (int i = 0; i < manaYield.Length; i++)
        { 
            manaYield[i] = card.manaYield[i];
        }

        //Set visual effect
        img = gameObject.GetComponent<Image>();

        switch (school)
        {
            case Elements.EARTH:
                img.color = new Color(0.8f, 0.8f, 0.1f);
                break;
            case Elements.FIRE:
                img.color = Color.red;
                break;
            case Elements.WIND:
                img.color = Color.green;
                break;
            case Elements.WATER:
                img.color = Color.blue;
                break;
            case Elements.ICE:
                img.color = Color.cyan;
                break;
            case Elements.POISON:
                img.color = Color.magenta;
                break;
            case Elements.METAL:
                img.color = Color.grey;
                break;
            case Elements.LIGHTNING:
                img.color = Color.yellow;
                break;
            default:
                img.color = Color.white;
                break;
        }

        updateText();
        linkedCard = card;

        //Dock to crystalArea in its sorted place
        crysArea = GameObject.Find("ManaArea").transform;
        transform.SetParent(crysArea);
        setUsedAlpha(true);
        sortShard();

        //Put card in in-use pile
        //card.transform.SetParent(GameObject.Find("PlayedCard").transform);
        //card.gameObject.SetActive(false);
    }

    public void use()
    {
        if (Used)
            return;
        else
        {
            UnitHandler.currentHero.harvestMana(this);
            updateText();
        }     
    }

    public void setUsedAlpha(bool state)
    {
        Color c = img.color;
        if (state)
            c.a = 0.5f;
        else
            c.a = 1f;
        img.color = c;
    }

    private void updateText()
    {
        buttText.text = durability.ToString();
    }

    private void sortShard()
    {
        for (int i = 0; i < crysArea.childCount; i++)
        {
            if ((int)crysArea.GetChild(i).GetComponent<CrystalObject>().school < (int)this.school)
            {
                this.transform.SetSiblingIndex(i);
                return;
            }
        }
        this.transform.SetAsLastSibling();
    }

    private String cName;
    public Elements school;
    private int durability;
    public int Durability { get { return durability; } set { durability = value; } }

    public int[] manaYield;

}

