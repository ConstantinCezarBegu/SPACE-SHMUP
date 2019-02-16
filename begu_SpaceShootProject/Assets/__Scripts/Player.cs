using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    static public Player S;

    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;
    private GameObject lastTriggerGo = null;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;


    void Start() {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("Player.Awake() - Attempted to assign second Player.S!");
        }

        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);

    }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go == lastTriggerGo) {
            return;
        }

        lastTriggerGo = go;

        if (go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else if (go.tag == "PowerUp")
        {
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);
        }

    }

    public float shieldLevel
    {
        get {
            return (_shieldLevel);     
        }

        set
        { _shieldLevel = Mathf.Min(value, 4);
            if (value < 0) {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }

    }

    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");      
        float yAxis = Input.GetAxis("Vertical");     
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
          //  TempFire();
        }

        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        { 
            fireDelegate(); 
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield: // a
                shieldLevel++;
                break;
            default: // b
                if (pu.type == weapons[0].type)
                { // If it is the same type // c
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null)
                    {
                        // Set it to pu.type
                        w.SetType(pu.type);
                    }
                }
                else
                { // If this is a different weapon type // d
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }
    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }




}