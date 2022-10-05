using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int startingLives;
    public int startingStamina;
    public float respawnTime;
    public float respawnDistanceFromPlayer;

    [Header("References")]
    public GameObject gameOverScreen;
    public TextMeshProUGUI gameOverMsg;
    public AudioClip gameOverSound;
    public AudioClip dieGameOverSound;

    // State Control
    private bool player1Won;

    // References
    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }
    private AudioSource audioSrc;

    void Awake()
    {
        Time.timeScale = 1f;
        audioSrc = GetComponent<AudioSource>();
        var players = FindObjectsOfType<Player>();
        Player1 = players[0].isPlayer1 ? players[0] : players[1];
        Player2 = players[1].isPlayer1 ? players[0] : players[1];
        Player.livesChangedEvent += OnPlayerLivesChanged;
        Player1.Lives = startingLives;
        Player2.Lives = startingLives;
        Player1.Stamina = startingStamina;
        Player2.Stamina = startingStamina;
        gameOverScreen.SetActive(false);
    }

    void OnDisable()
    {
        Player.livesChangedEvent -= OnPlayerLivesChanged;
    }

    public void OnPlayerLivesChanged(Player player, int livesDiff)
    {
        if (player.Lives <= 0)
        {
            player1Won = player == Player2 ? true : false;
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        Time.timeScale = 0f;
        Player1.enabled = false;
        Player2.enabled = false;
        audioSrc.Stop();
        audioSrc.PlayOneShot(dieGameOverSound, 1f);
        yield return new WaitForSecondsRealtime(dieGameOverSound.length + 1f);
        gameOverScreen.SetActive(true);
        gameOverMsg.text = "PLAYER " + (player1Won ? "1" : "2") + " VENCEU!";
        audioSrc.PlayOneShot(gameOverSound);
    }
}
