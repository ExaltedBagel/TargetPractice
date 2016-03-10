using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public abstract class UnitBase : MonoBehaviour, IEquatable<UnitBase>
{
    public UnitBase() { }
    protected UnitCard linkedCard;
    public UnitCard LinkedCard { get { return linkedCard; } set { linkedCard = value; } }

    //List de banque d'attaques
    private static AttackDataList attackDataList;

    [SerializeField]
    public GameObject unit;

    [SerializeField]
    public int team;

    //See Action States
    public bool hasMoved = false;
    public bool hasActed = false;
    public bool isSelected = false; //To implement to fix the selection bug

    public GameObject atkCircle;
    public GameObject selectionCircle;

    /*************************************/
    //Stats
    /************************************/
    [SerializeField]
    public string uName;
    [SerializeField]
    public UnitTypes uType;
    [SerializeField]
    public int hp;
    [SerializeField]
    public float speed;

    /************************************/
    //Offensive stats - Base = 1.0f
    /************************************/
    [SerializeField]
    public float[] typeAtk = new float[(int)DamageType.TOTAL];

    /************************************/
    //Defence stats
    /************************************/
    [SerializeField]
    public float[] typeDef = new float[(int)DamageType.TOTAL];

    /*************************************/
    //Movement --- Might be migrated to a handler since this might be static in the end
    /************************************/
    public bool moving;
    public float remDist;
    private float movSpeed;
    private float movFrac;

    /*************************************/
    //Combat
    /************************************/
    [SerializeField]
    public List<Attack> attacks;

    //Assures-toi de bien enlever le unit de la liste des units lorsque détruit
    void OnDestroy()
    {
        TurnHandler.removeFromBucket(this, this.team);
        UnitHandler.removeFromUnitList(this);
        Debug.Log("Target: " + this.uName + " removed from game! List size: " + UnitHandler.units.Count);
        Destroy(this.unit);
    }

    void Awake()
    {
        attackDataList = GameObject.Find("References").GetComponent<DBReferences>().atkDataList;
    }
    //CALL THIS FUNCTION TO CREATE A UNIT FROM DATA
    //MEANT FOR THE CARDS TO CALL
    public virtual void init(UnitData uData)
    {
        //Get all the stats from unitData
        uName = uData.uName;
        uType = uData.uType;
        hp = uData.maxHp;
        speed = uData.maxSpeed;

        for (int i = 0; i < typeAtk.Length; i++)
        {
            typeAtk[i] = uData.typeAtk[i];
            typeDef[i] = uData.typeDef[i];
        }
        attacks = new List<Attack>();
        //Add all attacks
        foreach (String x in uData.attacksData.ToList())
        {
            addAttack(attackDataList.findByID(x));

        }

        //Set all components
        unit = gameObject;
        GameObject instance = (Resources.Load("Unit/AttackCircle") as GameObject);
        atkCircle = (GameObject)Instantiate(instance, this.transform.position, Quaternion.identity);
        atkCircle.transform.SetParent(this.unit.transform);
        atkCircle.transform.position -= new Vector3(0, 1.3f, 0);
        atkCircle.name = "AttackCircle";

        instance = (Resources.Load("Unit/SelectCircle") as GameObject);
        selectionCircle = (GameObject)Instantiate(instance, this.transform.position, Quaternion.identity);
        selectionCircle.transform.SetParent(this.unit.transform);
        selectionCircle.transform.position -= new Vector3(0, 0.5f, 0);
        selectionCircle.name = "SelectCircle";

        unit.name = uName;
    }


    void addAttack(AttackData atkData)
    {
        if (atkData == null)
        {
            Debug.Log("Attack Was Errored");
            return;
        }
            
        Attack atk;
        switch (atkData.pattern)
        {
            case AttackPattern.SINGLE:
                atk = new AttackSingleTarget(atkData);
                if (atk != null)
                    attacks.Add(atk);
                break;
            case AttackPattern.BLAST:
                atk = new AttackBlastCircle(atkData);
                if (atk != null)
                    attacks.Add(atk);
                break;
                /*
            case AttackPattern.LINE:
                atk = new AttackLine(atkData)
                if (atk != null)
                    attacks.Add(atk);
                break;
                */
        }
    }

    /*************************************/
    //Functions
    /************************************/
    public void printName()
    {
        Debug.Log(uName);
    }

    public int getId()
    {
        return unit.GetInstanceID();
    }

    /*************************************/
    //Atk Function
    /************************************/
    public void toggleAtkCircle(bool state)
    {
        Debug.Log("Sanity check " + atkCircle == null);
        if(atkCircle != null)
        {
            if (state)
                atkCircle.SetActive(true);
            else
                atkCircle.SetActive(false);
        }
        
    }

    //Toggles atk circle on with certain radius
    public void toggleAtkCircle(float radius)
    {
        atkCircle.SetActive(true);
        atkCircle.transform.localScale = new Vector3(radius, 1, radius);
    }

    public Collider[] overlapArea(LayerMask targetTeam)
    {
        Collider[] ennemies = Physics.OverlapSphere(atkCircle.transform.position, atkCircle.transform.localScale.x / 2, targetTeam);
        foreach (Collider ennemy in ennemies)
            Debug.Log(ennemy.gameObject.name);

        return ennemies;
    }

    public void resetAtkCircle()
    {
        if (!Equals(atkCircle, null))
            atkCircle.transform.localScale = new Vector3(attacks[0].range, 0.1f, attacks[0].range);
    }

    public bool tryRays(GameObject target, LayerMask team)
    {
        bool result = false;
        Vector3 baseVector = target.transform.position - unit.transform.position;
        float angle = 0f;
        //Sweep the target
        for (; angle < 30 && !result; angle += 1f)
        {
            RaycastHit hit;
            Ray ray = new Ray(unit.transform.position, Quaternion.AngleAxis(angle, Vector3.up) * baseVector);
            Debug.DrawRay(unit.transform.position, Quaternion.AngleAxis(angle, Vector3.up) * baseVector, Color.green, 3.0f);

            if (Physics.Raycast(ray, out hit, team))
            {
                if (hit.collider.transform.root.gameObject.GetInstanceID() == target.transform.root.gameObject.GetInstanceID())
                    result = true;
            }

            ray = new Ray(unit.transform.position, Quaternion.AngleAxis(-angle, Vector3.up) * baseVector);
            Debug.DrawRay(unit.transform.position, Quaternion.AngleAxis(-angle, Vector3.up) * baseVector, Color.red, 3.0f);

            if (Physics.Raycast(ray, out hit, team))
            {
                if (hit.collider.gameObject.name == target.name)
                    result = true;
            }

        }
        return result;
    }

    //REWORK TO BE CONSISTENT WITH APPLY ATTACK
    public void attackUnit(UnitBase target, Attack attack)
    {
        //To fix
        int damage = CMath.calculateDamage(this, target, attack);
        target.hp -= damage;
        Debug.Log(target.uName + " was dealt " + damage + " : " + target.hp + " hp left!");
        hasActed = true;
        if (target.hp <= 0)
        {
            Debug.Log("Destroying Unit now: ");
            Destroy(target);
            target = null;
        }

    }

    //AnotherWay to see it
    public void applyAttack(UnitBase attacker, Attack attack)
    {
        //To fix
        int damage = CMath.calculateDamage(attacker, this, attack);
        this.hp -= damage;
        Debug.Log(this.uName + " was dealt " + damage + " : " + this.hp + " hp left!");
        hasActed = true;
        if (this.hp <= 0)
        {
            Debug.Log("Destroying Unit now: ");
            Destroy(this.gameObject);
        }
    }

    /*************************************/
    //Move Functions
    /************************************/
    public void moveUnit()
    {
        if ((!hasActed || !hasMoved))
        {
            moving = true;
        }
        else
        {
            Debug.Log("Unit is out of actions");
        }

    }

    public bool Equals(UnitBase other)
    {
        if (Equals(this, null) && Equals(other, null))
            return true;
        else if (Equals(this, null) | Equals(other, null))
            return false;
        else if (unit.gameObject.GetInstanceID() == other.unit.gameObject.GetInstanceID())
            return true;
        else
            return false;
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as UnitBase);
    }
    public override int GetHashCode()
    {
        return unit.gameObject.GetInstanceID(); 
    }
}
