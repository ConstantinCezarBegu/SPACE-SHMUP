using System.Linq;
using UnityEngine;

/// <summary>
/// Part is another serializable data storage class just like WeaponDefinition
/// </summary>
[System.Serializable]
public class Part
{
    // These three fields need to be defined in the Inspector pane
    public string name; // The name of this part
    public float health; // The amount of health this part has

    public string[] protectedBy; // The other parts that protect this

    // These two fields are set automatically in Start().
    // Caching like this makes it faster and easier to find these later
    [HideInInspector] // Makes field on the next line not appear in the Inspector
    public GameObject go; // The GameObject of this part

    [HideInInspector] public Material mat; // The Material to show damage
}

/// <summary>
/// Enemy_4 will start offscreen and then pick a random point on screen to
/// move to. Once it has arrived, it will pick another random point and
/// continue until the player has shot it down.
/// </summary>
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")] public Part[] parts; // The array of ship Parts

    private Vector3 _p0, _p1; // The two points to interpolate
    private float _timeStart; // Birth time for this Enemy_4
    private float _duration = 4; // Duration of movement

    private void Start()
    {
        // There is already an initial position chosen by Main.SpawnEnemy()
        // so add it to points as the initial p0 & p1
        _p0 = _p1 = pos;
        InitMovement();

        // Cache GameObject & Material of each Part in parts
        foreach (var prt in parts)
        {
            var t = transform.Find(prt.name);
            if (t == null) continue;
            prt.go = t.gameObject;
            prt.mat = prt.go.GetComponent<Renderer>().material;
        }
    }

    private void InitMovement()
    {
        _p0 = _p1; // Set p0 to the old p1
        // Assign a new on-screen location to p1
        var widMinRad = bndCheck.camWidth - bndCheck.radius;
        var hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        _p1.x = Random.Range(-widMinRad, widMinRad);
        _p1.y = Random.Range(-hgtMinRad, hgtMinRad);
        // Reset the time
        _timeStart = Time.time;
    }

    public override void Move()
    {
        // This completely overrides Enemy.Move() with a linear interpolation
        var u = (Time.time - _timeStart) / _duration;
        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2); // Apply Ease Out easing to u
        pos = (1 - u) * _p0 + u * _p1; // Simple linear interpolation
    }

    // These two functions find a Part in parts based on name or GameObject
    private Part FindPart(string n)
    {
        return (from prt in parts where prt.name == n select (prt)).FirstOrDefault();
    }

    private Part FindPart(GameObject go)
    {
        return (from prt in parts where prt.go == go select (prt)).FirstOrDefault();
    }

    // These functions return true if the Part has been destroyed
    private bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }

    private bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }

    private bool Destroyed(Part prt)
    {
        if (prt == null)
        {
            // If no real ph was passed in
            return (true); // Return true (meaning yes, it was destroyed)
        }

        // Returns the result of the comparison: prt.health <= 0
        // If prt.health is 0 or less, returns true (yes, it was destroyed)
        return (prt.health <= 0);
    }

    // This changes the color of just one Part to red instead of the whole ship.
    private void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    // This will override the OnCollisionEnter that is part of Enemy.cs.
    private void OnCollisionEnter(Collision coll)
    {
        var other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectilePlayer":
                var p = other.GetComponent<Projectile>();
                // If this Enemy is off screen, don't damage it.
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                // Hurt this Enemy
                var goHit = coll.contacts[0].thisCollider.gameObject;
                var prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    // If prtHit wasn't found…
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }

                // Check whether this part is still protected
                if (prtHit.protectedBy != null)
                {
                    if (prtHit.protectedBy.Any(s => !Destroyed(s)))
                    {
                        Destroy(other); // Destroy the ProjectilePlayer
                        return; // return before damaging Enemy_4
                    }
                }

                // It's not protected, so make it take damage
                // Get the damage amount from the Projectile.type and Main.W_DEFS $$$$$$$$$$$$$$$$
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                // Show damage on the part
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    // Instead of destroying this enemy, disable the damaged part
                    prtHit.go.SetActive(false);
                }

                // Check to see if the whole ship is destroyed
                var allDestroyed = parts.All(Destroyed); // Assume it is destroyed

                if (allDestroyed)
                {
                    // If it IS completely destroyed...
                    // ...tell the Main singleton that this ship was destroyed
                    Main.S.shipDestroyed(this);
                    // Destroy this Enemy
                    Destroy(gameObject);
                }

                Destroy(other); // Destroy the ProjectilePlayer
                break;
        }
    }
}