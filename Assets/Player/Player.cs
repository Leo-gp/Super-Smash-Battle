using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum AimDirection { Forward, Up, Down }

    [Header("Attributes")]
    public float speed;
    public float jumpForce;
    public float turningSpeed;
    public float shotSpeed;
    public float shotRate;

    [Header("Settings")]
    public bool isPlayer1;
    public Transform bulletSpawnPos;
    public Vector2 shotForwardDirection;
    public Vector2 shotUpDirection;
    public Vector2 shotDownDirection;
    public Vector2 bulletRepelForce;
    public float facingRightYAngle;
    public float facingLeftYAngle;
    public float groundedDistance;

    [Header("References")]
    public LayerMask groundLayer;
    public Bullet bulletPrefab;
    public PhysicMaterial noFrictionMat;
    public AudioClip shootSound;
    public AudioClip jumpSound;
    public AudioClip dieSound;

    // Non-continuous inputs control (KeyDown and KeyUp)
    private bool jumpKeyPressed;

    // State control
    private bool falling;
    private bool facingRight;
    private bool turningDirection;
    private AimDirection aimDirection;
    private bool stunned;
    private bool jumped;
    private bool doubleJumped;
    private float lastShotTime;
    private bool aerialHit;
    private bool groundHit;
    private bool doubleHit;
    private int _lives;
    public int Lives
    {
        get
        {
            return _lives;
        }
        set
        {
            int livesDiff = value - _lives;
            _lives = value;
            livesChangedEvent?.Invoke(this, livesDiff);
        }
    }
    private int _stamina;
    public int Stamina
    {
        get
        {
            return _stamina;
        }
        set
        {
            int staminaDiff = value - _stamina;
            _stamina = value;
            staminaChangedEvent?.Invoke(this, staminaDiff);
        }
    }

    // Events
    public static event Action<Player, int> livesChangedEvent;
    public static event Action<Player, int> staminaChangedEvent;

    // References
    private Rigidbody rb;
    private CapsuleCollider col;
    private AudioSource audioSrc;
    private GameManager gm;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        audioSrc = GetComponent<AudioSource>();
        gm = FindObjectOfType<GameManager>();
        if (isPlayer1)
            facingRight = true;
        else
            facingRight = false;
    }

    void Start()
    {
        Respawn();
    }

    void Update()
    {
        if (!jumpKeyPressed)
        {
            if (isPlayer1)
                jumpKeyPressed = Input.GetButtonDown("JumpP1");
            else
                jumpKeyPressed = Input.GetButtonDown("JumpP2");
        }

        DebugGroundCheck();

        if ((isPlayer1 && Input.GetButtonDown("ShootP1")) || (!isPlayer1 && Input.GetButtonDown("ShootP2")))
        {
            if (CanShoot())
                Shoot();
        }

        if (!Grounded() && !falling)
        {
            StartFall();
        }
        else if (Grounded() && falling)
        {
            jumped = false;
            doubleJumped = false;
            stunned = false;
            groundHit = false;
            aerialHit = false;
            doubleHit = false;
            EndFall();
        }
    }

    void FixedUpdate()
    {
        if (!stunned)
        {
            float horizontalMove;
            if (isPlayer1)
                horizontalMove = Input.GetAxis("HorizontalP1");
            else
                horizontalMove = Input.GetAxis("HorizontalP2");

            rb.velocity = new Vector3(horizontalMove * speed, rb.velocity.y, 0);

            if (horizontalMove < 0 && facingRight || horizontalMove > 0 && !facingRight)
            {
                if (!turningDirection)
                    StartCoroutine(TurnDirection());
            }

            if (jumpKeyPressed && CanJump())
            {
                Jump();
            }
        }
        else
        {
            if (jumpKeyPressed && CanRecover())
            {
                Recover();
            }
        }

        jumpKeyPressed = false;
    }

    private AimDirection GetAimDirection()
    {
        if ((isPlayer1 && Input.GetAxisRaw("VerticalP1") == 0) || (!isPlayer1 && Input.GetAxisRaw("VerticalP2") == 0))
        {
            return AimDirection.Forward;
        }
        else if ((isPlayer1 && Input.GetAxisRaw("VerticalP1") > 0) || (!isPlayer1 && Input.GetAxisRaw("VerticalP2") > 0))
            return AimDirection.Up;
        else
            return AimDirection.Down;
    }

    private IEnumerator TurnDirection()
    {
        turningDirection = true;
        float t = 0f;
        if (facingRight)
        {
            facingRight = false;
            Vector3 startingAngles = transform.eulerAngles;
            Vector3 facingLeftAngles = new Vector3(startingAngles.x, facingLeftYAngle, startingAngles.z);
            while (t < 1f)
            {
                t += Time.deltaTime * turningSpeed;
                transform.eulerAngles = Vector3.Lerp(startingAngles, facingLeftAngles, t);
                yield return null;
            }
        }
        else
        {
            facingRight = true;
            Vector3 startingAngles = transform.eulerAngles;
            Vector3 facingRightAngles = new Vector3(startingAngles.x, facingRightYAngle, startingAngles.z);
            while (t < 1f)
            {
                t += Time.deltaTime * turningSpeed;
                transform.eulerAngles = Vector3.Lerp(startingAngles, facingRightAngles, t);
                yield return null;
            }
        }
        turningDirection = false;
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        if (!Grounded() || jumped)
            doubleJumped = true;
        jumped = true;
        audioSrc.PlayOneShot(jumpSound);
    }

    private bool CanJump()
    {
        if (!Grounded() && jumped && doubleJumped)
            return false;
        return true;
    }

    private void Recover()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce / 2f, rb.velocity.z);
        stunned = false;
        jumped = true;
        doubleJumped = true;
        Stamina -= aerialHit ? 2 : 1;
        audioSrc.PlayOneShot(jumpSound);
    }

    private bool CanRecover()
    {
        bool enoughStamina = Stamina - (aerialHit ? 2 : 1) >= 0 ? true : false;
        if (!enoughStamina || doubleHit)
            return false;
        return true;
    }

    public bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundedDistance, groundLayer);
    }

    private void DebugGroundCheck()
    {
        Vector3 dir = new Vector3(0, -groundedDistance, 0);
        Debug.DrawRay(transform.position, dir, Color.red);
    }

    private void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
        Physics.IgnoreCollision(col, bullet.GetComponent<Collider>());
        bullet.RepelForce = bulletRepelForce;
        Vector2 shotDirection = Vector2.zero;
        switch (GetAimDirection())
        {
            case AimDirection.Forward:
                shotDirection = shotForwardDirection;
                break;
            case AimDirection.Up:
                shotDirection = shotUpDirection;
                break;
            case AimDirection.Down:
                shotDirection = shotDownDirection;
                break;
        }
        if (!facingRight)
        {
            shotDirection = new Vector2(-shotDirection.x, shotDirection.y);
            bullet.GoingRight = false;
        }
        else
        {
            bullet.GoingRight = true;
        }
        bullet.GetComponent<Rigidbody>().AddForce(shotDirection.normalized * shotSpeed);
        lastShotTime = Time.time;
        audioSrc.PlayOneShot(shootSound);
    }

    private bool CanShoot()
    {
        return (Time.time - lastShotTime > 1f / shotRate) && !stunned;
    }

    public void Hit(Bullet bullet)
    {
        if (groundHit || aerialHit)
            doubleHit = true;

        if (Grounded())
            groundHit = true;
        else
            aerialHit = true;

        if (bullet.GoingRight)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(bullet.RepelForce, ForceMode.VelocityChange);
        }
        else
        {
            Vector2 repel = new Vector2(-bullet.RepelForce.x, bullet.RepelForce.y);
            rb.velocity = Vector3.zero;
            rb.AddForce(repel, ForceMode.VelocityChange);
        }

        stunned = true;
    }

    private void StartFall()
    {
        falling = true;
        rb.drag = 0;
        col.material = noFrictionMat;
    }

    private void EndFall()
    {
        falling = false;
        rb.drag = 5;
        col.material = null;
    }

    public IEnumerator Die()
    {
        Lives--;
        if (Lives > 0)
        {
            audioSrc.PlayOneShot(dieSound, 0.6f);
            yield return new WaitForSeconds(gm.respawnTime);
            Respawn();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Respawn()
    {
        rb.velocity = Vector3.zero;
        transform.position = SpawnManager.instance.GetRandomSpawnPoint(this);
        stunned = false;
        jumped = false;
        Stamina = gm.startingStamina;
    }
}