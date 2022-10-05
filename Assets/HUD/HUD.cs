using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [Header("References")]
    public GameObject healthP1;
    public GameObject healthP2;
    public GameObject staminaP1;
    public GameObject staminaP2;
    public GameObject healthIconPrefabP1;
    public GameObject healthIconPrefabP2;
    public GameObject staminaIconPrefabP1;
    public GameObject staminaIconPrefabP2;

    // State control
    private List<GameObject> healthIconsP1;
    private List<GameObject> healthIconsP2;
    private List<GameObject> staminaIconsP1;
    private List<GameObject> staminaIconsP2;

    // References
    private Player player1;
    private Player player2;

    void Start()
    {
        healthIconsP1 = new List<GameObject>();
        healthIconsP2 = new List<GameObject>();
        staminaIconsP1 = new List<GameObject>();
        staminaIconsP2 = new List<GameObject>();
        var players = FindObjectsOfType<Player>();
        player1 = players[0].isPlayer1 ? players[0] : players[1];
        player2 = players[1].isPlayer1 ? players[0] : players[1];
        UpdatePlayerLives(player1, player1.Lives);
        UpdatePlayerLives(player2, player2.Lives);
        UpdatePlayerStamina(player1, player1.Stamina);
        UpdatePlayerStamina(player2, player2.Stamina);
        Player.livesChangedEvent += UpdatePlayerLives;
        Player.staminaChangedEvent += UpdatePlayerStamina;
    }

    void OnDisable()
    {
        Player.livesChangedEvent -= UpdatePlayerLives;
        Player.staminaChangedEvent -= UpdatePlayerStamina;
    }

    private void UpdatePlayerLives(Player player, int livesDiff)
    {
        var healthIcons = player == player1 ? healthIconsP1 : healthIconsP2;

        if (livesDiff < 0)
        {
            for (int i = 0; i > livesDiff; i--)
            {
                var item = healthIcons[healthIcons.Count - 1];
                healthIcons.Remove(item);
                Destroy(item);
            }
        }
        else if (livesDiff > 0)
        {
            var health = player == player1 ? healthP1 : healthP2;
            var healthIconPrefab = player == player1 ? healthIconPrefabP1 : healthIconPrefabP2;
            for (int i = 0; i < livesDiff; i++)
            {
                var item = Instantiate(healthIconPrefab, health.transform);
                healthIcons.Add(item);
            }
        }
    }

    private void UpdatePlayerStamina(Player player, int staminaDiff)
    {
        var staminaIcons = player == player1 ? staminaIconsP1 : staminaIconsP2;

        if (staminaDiff < 0)
        {
            for (int i = 0; i > staminaDiff; i--)
            {
                var item = staminaIcons[staminaIcons.Count - 1];
                staminaIcons.Remove(item);
                Destroy(item);
            }
        }
        else if (staminaDiff > 0)
        {
            var stamina = player == player1 ? staminaP1 : staminaP2;
            var staminaIconPrefab = player == player1 ? staminaIconPrefabP1 : staminaIconPrefabP2;
            for (int i = 0; i < staminaDiff; i++)
            {
                var item = Instantiate(staminaIconPrefab, stamina.transform);
                staminaIcons.Add(item);
            }
        }
    }
}
