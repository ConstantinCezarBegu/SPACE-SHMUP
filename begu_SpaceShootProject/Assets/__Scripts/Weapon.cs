using System.Collections;
using System.Collections.Generic;
using UnityEngine;	
public enum WeaponType
{
    none,
    blaster,
    spread,
    phaser,
    missile,
    laser,
    shield
}


[System.Serializable]	
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;	
    public Color color = Color.white;
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float continuousDamage = 0;
    public float delayBetweenShots = 0;
    public float velocity = 20;	
}





public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;
    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; // Time last shot was fired
    private Renderer collarRend;
    void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();
        // Call SetType() for the default _type of WeaponType.none
        SetType(_type); // a
                        // Dynamically create an anchor for all Projectiles
        if (PROJECTILE_ANCHOR == null)
        { // b
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        // Find the fireDelegate of the root GameObject
        GameObject rootGO = transform.root.gameObject; // c
        if (rootGO.GetComponent<Player>() != null)
        { // d
            rootGO.GetComponent<Player>().fireDelegate += Fire;
        }
    }
    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }
    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        { // e
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type); // f
        collarRend.material.color = def.color;
        lastShotTime = 0; // You can fire immediately after _type is set. // g
    }
    public void Fire()
    {
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return; // h
                                                   // If it hasn't been enough time between shots, return
        if (Time.time - lastShotTime < def.delayBetweenShots)
        { // i
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity; // j
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        { // k
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
            case WeaponType.spread: // l
                p = MakeProjectile(); // Make middle Projectile
                p.rigid.velocity = vel;
                p = MakeProjectile(); // Make right Projectile
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Make left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;
        }
    }
    public Projectile MakeProjectile()
    { // m
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Player")
        { // n
            go.tag = "ProjectilePlayer";
            go.layer = LayerMask.NameToLayer("ProjectilePlayer");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true); // o
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time; // p
        return (p);
    }
}
