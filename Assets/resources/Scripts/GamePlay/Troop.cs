using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TeamComponent))]
public class Troop : MonoBehaviour
{
    [Header("Troop Data")]
    public TroopType troopType = TroopType.Melee;   // Melee, Ranged, Tank
    public float maxHealth = 50f;
    public float attackDamage = 10f;
    public float attackRange = 2.5f;               // used for melee + archer vs troops
    public float attackCooldown = 1f;

    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 15f;

    [Header("Targeting")]
    public float scanInterval = 0.5f;
    public float baseAttackDistance = 2.5f;        // distance to attack point for melee/archer
    public float tankFireRange = 10f;              // how close tank must get to shoot base

    [Header("Healthbar")]
    public GameObject healthBarPrefab;

    private float currentHealth;
    private float attackTimer;
    private float scanTimer;

    private NavMeshAgent agent;
    public TeamComponent teamComponent;   // public so other scripts can read
    private BaseHealth targetBase;
    private Troop currentEnemyTarget;
    private Transform attackPoint;
    private HealthBar healthBar;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.autoBraking = true;

        teamComponent = GetComponent<TeamComponent>();
        currentHealth = maxHealth;

        AssignTargetBase();
        SetDestinationToBase();
        SpawnHealthBar();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;
        scanTimer -= Time.deltaTime;

        // ---------------- TANK SPECIAL LOGIC ----------------
        // Tanks do NOT care about enemy troops. They only move toward and attack the base.
        if (troopType != TroopType.Tank)
        {
            // Normal troops: scan for nearby enemies
            if (scanTimer <= 0f)
            {
                scanTimer = scanInterval;
                FindClosestEnemyTroop();
            }

            // If we have an enemy troop in range → attack troop
            if (currentEnemyTarget != null)
            {
                float distance = Vector3.Distance(transform.position, currentEnemyTarget.transform.position);

                if (distance <= attackRange)
                {
                    agent.isStopped = true;
                    transform.LookAt(currentEnemyTarget.transform);
                    PerformAttackOnTroop();
                    return;
                }
            }
        }

        // Movement / base attack logic (shared)
        agent.isStopped = false;
        MoveTowardsBase();
    }

    // Decide which base & attack point to use
    private void AssignTargetBase()
    {
        if (teamComponent.team == Team.Player)
        {
            targetBase = GameObject.FindWithTag("EnemyBase").GetComponent<BaseHealth>();
            attackPoint = GameObject.FindWithTag("EnemyAttackPoint").transform;
        }
        else
        {
            targetBase = GameObject.FindWithTag("PlayerBase").GetComponent<BaseHealth>();
            attackPoint = GameObject.FindWithTag("PlayerAttackPoint").transform;
        }
    }

    private void SetDestinationToBase()
    {
        if (attackPoint != null)
            agent.SetDestination(attackPoint.position);
    }

    private void MoveTowardsBase()
    {
        if (attackPoint == null || targetBase == null) return;

        float distToPoint = Vector3.Distance(transform.position, attackPoint.position);

        // ----- Tank: fire at base from tankFireRange -----
        if (troopType == TroopType.Tank)
        {
            if (distToPoint <= tankFireRange)
            {
                agent.isStopped = true;
                transform.LookAt(targetBase.transform);
                PerformAttackOnBase();
            }
            else
            {
                if (!agent.isStopped && !agent.hasPath)
                    SetDestinationToBase();
            }

            return;
        }

        // ----- Normal troops (Melee, Archer) -----
        if (distToPoint <= baseAttackDistance)
        {
            agent.isStopped = true;
            transform.LookAt(targetBase.transform);
            PerformAttackOnBase();
        }
        else
        {
            if (!agent.isStopped && !agent.hasPath)
                SetDestinationToBase();
        }
    }

    // Find nearest enemy troop (for Melee & Archer only)
    private void FindClosestEnemyTroop()
    {
        Troop[] troops = Object.FindObjectsByType<Troop>(FindObjectsSortMode.None);
        float closestDist = Mathf.Infinity;
        Troop bestTarget = null;

        foreach (var troop in troops)
        {
            if (troop == null || troop == this) continue;
            if (troop.teamComponent == null || teamComponent == null) continue;
            if (troop.teamComponent.team == teamComponent.team) continue;

            float dist = Vector3.Distance(transform.position, troop.transform.position);

            // Only care about relatively nearby enemies
            if (dist < closestDist && dist <= attackRange + 3f)
            {
                closestDist = dist;
                bestTarget = troop;
            }
        }

        currentEnemyTarget = bestTarget;
    }

    // Attack logic vs troops (tanks skip this)
    private void PerformAttackOnTroop()
    {
        // Tanks never attack troops
        if (troopType == TroopType.Tank) return;

        if (currentEnemyTarget == null) return;
        if (attackTimer > 0f) return;

        attackTimer = attackCooldown;

        if (troopType == TroopType.Melee)
        {
            currentEnemyTarget.TakeDamage(attackDamage);
        }
        else // Ranged
        {
            FireProjectile(currentEnemyTarget.transform);
        }
    }

    // Attack base (all types, but range depends on type)
    private void PerformAttackOnBase()
    {
        if (attackTimer > 0f) return;
        attackTimer = attackCooldown;

        if (troopType == TroopType.Melee)
        {
            targetBase.TakeDamage(attackDamage);
        }
        else // Archer & Tank ranged attacks
        {
            FireProjectile(targetBase.transform);
        }
    }

    private void FireProjectile(Transform target)
    {
        if (projectilePrefab == null || projectileSpawnPoint == null) return;

        GameObject obj = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        Projectile p = obj.GetComponent<Projectile>();
        p.Initialize(target, attackDamage, projectileSpeed);
    }

    private void SpawnHealthBar()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("WorldCanvas");
        if (canvas == null)
        {
            Debug.LogError("WorldCanvas not found!");
            return;
        }

        if (healthBarPrefab == null)
        {
            Debug.LogError("HealthBar prefab missing on troop!");
            return;
        }

        GameObject hb = Instantiate(healthBarPrefab, canvas.transform);
        healthBar = hb.GetComponent<HealthBar>();

        healthBar.target = transform;
        healthBar.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took damage: {amount} | Remaining: {currentHealth}");

        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            if (healthBar != null)
                Destroy(healthBar.gameObject);

            Destroy(gameObject);
        }
    }
}
