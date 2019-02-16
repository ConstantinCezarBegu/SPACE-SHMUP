using System.Collections;	
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    protected BoundsCheck bndCheck;
    public float showDamageDuration = 0.1f; // # seconds to show damage // a
    public float powerUpDropChance = 1f;
    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;// All the Materials of this & its children
    public bool showingDamage = false;
    public float damageDoneTime; // Time to stop showing damage
    public bool notifiedOfDestruction = false; // Will be used later

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        // Get materials and colors for this GameObject and its children
        materials = Utils.GetAllMaterials(gameObject); // b
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    public Vector3 pos {	
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }
    void Update()
    {
        Move();
        if (bndCheck != null && bndCheck.offDown)
        {
            Destroy(gameObject);
        }
        if (showingDamage && Time.time > damageDoneTime)
        { // c
            UnShowDamage();
        }

    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }


    void OnCollisionEnter(Collision coll)
    { // a
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectilePlayer": // b
                Projectile p = otherGO.GetComponent<Projectile>();
                // If this Enemy is off screen, don't damage it.
                if (!bndCheck.isOnScreen)
                { // c
                    Destroy(otherGO);
                    break;
                }
                // Hurt this Enemy
                ShowDamage();
                // Get the damage amount from the Main WEAP_DICT.
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)
                {
                    // Tell the Main singleton that this ship was destroyed // b
                    if (!notifiedOfDestruction)
                    {
                        Main.S.shipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    // Destroy this Enemy
                    Destroy(this.gameObject);
                }
                Destroy(otherGO); // e
                break;
            default:
                print("Enemy hit by non-ProjectilePlayer: " + otherGO.name); // f
                break;
        }
    }

    void ShowDamage()
    { // e
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    void UnShowDamage()
    { // f
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }

}