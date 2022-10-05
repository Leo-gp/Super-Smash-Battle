using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Settings")]
    public Vector2[] spawnPoints;

    // Singleton
    public static SpawnManager instance;

    // References
    private GameManager gm;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        gm = FindObjectOfType<GameManager>();
    }

    public Vector3 GetRandomSpawnPoint(Player playerToSpawn)
    {
        var oppositePlayer = playerToSpawn.isPlayer1 ? gm.Player2 : gm.Player1;
        bool tooCloseFromOpposite = true;
        Vector3 spawnPoint = Vector3.zero;
        int iterations = 0;
        while (tooCloseFromOpposite && iterations < 1000)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            tooCloseFromOpposite = Vector3.Distance(spawnPoint, oppositePlayer.transform.position) < gm.respawnDistanceFromPlayer;
            iterations++;
        }
        if (iterations >= 1000)
            Debug.LogWarning("GameManager's 'respawnDistanceFromPlayer' value seems to be too high or the spawn points are too close.");
        return spawnPoint;
    }
}
