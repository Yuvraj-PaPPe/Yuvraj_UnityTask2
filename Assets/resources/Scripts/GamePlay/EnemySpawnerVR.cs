using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnerVR : MonoBehaviour
{
    [Header("Enemy Troop Prefabs")]
    public GameObject archerPrefab;
    public GameObject knightPrefab;
    public GameObject tankPrefab;

    [Header("Spawn Points")]
    public Transform archerSpawnPoint;
    public Transform knightSpawnPoint;
    public Transform tankSpawnPoint;

    [Header("Cooldown Timers")]
    public float archerCooldown = 5f;
    public float knightCooldown = 7f;
    public float tankCooldown = 10f;

    private float archerTimer = 0f;
    private float knightTimer = 0f;
    private float tankTimer = 0f;

    [Header("Troop Control")]
    public int maxEnemiesAlive = 5;    // limit
    private int currentEnemies = 0;

    void Update()
    {
        // Count current enemies
        currentEnemies = CountEnemies();

        // Stop spawning if too many
        if (currentEnemies >= maxEnemiesAlive) return;

        archerTimer += Time.deltaTime;
        knightTimer += Time.deltaTime;
        tankTimer += Time.deltaTime;

        if (archerTimer >= archerCooldown)
        {
            SpawnEnemy(archerPrefab, archerSpawnPoint);
            archerTimer = 0f;
        }

        if (knightTimer >= knightCooldown)
        {
            SpawnEnemy(knightPrefab, knightSpawnPoint);
            knightTimer = 0f;
        }

        if (tankTimer >= tankCooldown)
        {
            SpawnEnemy(tankPrefab, tankSpawnPoint);
            tankTimer = 0f;
        }
    }

    void SpawnEnemy(GameObject prefab, Transform spawnPoint)
    {
        GameObject troop = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(troop.transform.position, out hit, 5f, NavMesh.AllAreas))
            troop.transform.position = hit.position;

        // assign troop to enemy side
        troop.GetComponent<TeamComponent>().team = Team.Enemy;
    }

    int CountEnemies()
    {
        Troop[] troops = Object.FindObjectsByType<Troop>(FindObjectsSortMode.None);
        int count = 0;

        foreach (var t in troops)
        {
            TeamComponent tc = t.GetComponent<TeamComponent>();
            if (tc != null && tc.team == Team.Enemy)
                count++;
        }

        return count;
    }

}
