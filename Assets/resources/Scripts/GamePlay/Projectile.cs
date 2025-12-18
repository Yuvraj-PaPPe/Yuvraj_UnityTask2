using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public ProjectileType projectileType;  // Archer / Tank defined externally
    public float damage;
    public float speed = 10f;

    [Header("Hit Effect (optional)")]
    public GameObject hitEffect;

    private Transform target;

    public void Initialize(Transform target, float damageValue, float projectileSpeed)
    {
        this.target = target;             // IMPORTANT FIX
        damage = damageValue;
        speed = projectileSpeed;

        // Behavior differences based on projectile type
        if (projectileType == ProjectileType.Tank)
        {
            transform.localScale *= 1.5f;  // Bigger projectile
            damage *= 2f;                  // More damage
            speed *= 0.7f;                 // Slower travel
        }
        // Archer behavior stays default
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            if (target.TryGetComponent<Troop>(out Troop troop))
                troop.TakeDamage(damage);

            if (target.TryGetComponent<BaseHealth>(out BaseHealth baseHP))
                baseHP.TakeDamage(damage);

            if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
