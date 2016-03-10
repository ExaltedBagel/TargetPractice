using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class HarvestedCrystal : MonoBehaviour
{
    public Elements School { get { return school; } }
    private Elements school;
    private Image img;
    static Transform harvestArea = GameObject.Find("HarvestedArea").transform;

    public void init(Elements sch)
    {
        school = sch;
        img = gameObject.GetComponent<Image>();
        gameObject.transform.SetParent(harvestArea);

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
        sortShard();
    }

    public void makeNeutral()
    {
        UnitHandler.currentHero.convertMana(school);
        school = Elements.NORMAL;
        img.color = Color.white;
        gameObject.transform.SetAsLastSibling();
    }

    private void sortShard()
    {
        for(int i = 0; i < harvestArea.childCount; i++)
        {
            if((int)harvestArea.GetChild(i).GetComponent<HarvestedCrystal>().School < (int)this.School)
            {
                this.transform.SetSiblingIndex(i);
                return;
            }
        }
        this.transform.SetAsLastSibling();
    }

    public static void setUsedAlpha(int[] manaCost)
    {
        for (int i = 0; i < manaCost.Length; i++)
        {
            //Number of crystals to shade and corresponding elemental type
            int n = manaCost[i];
            Elements sch = (Elements)i;
            Debug.Log(sch.ToString());
            for (int j = harvestArea.childCount - 1; j >= 0 && n > 0; j--)
            {
                HarvestedCrystal crys = harvestArea.GetChild(j).GetComponent<HarvestedCrystal>();
                if (crys.School == sch)
                {
                    n--;
                    setUsed(crys, true);
                }
            }
        }
    }

    public static void resetUsedAlpha()
    {
        foreach (HarvestedCrystal x in harvestArea.GetComponentsInChildren<HarvestedCrystal>())
            setUsed(x, false);
    }

    private static void setUsed(HarvestedCrystal crys, bool state)
    {
        Color c = crys.img.color;
        if (state)
            c.a = 0.5f;
        else
            c.a = 1f;
        crys.img.color = c;
    }
   
}

