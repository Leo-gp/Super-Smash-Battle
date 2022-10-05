using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem explosionEffect;
    public AudioClip shootExplosionSound;

    // Attributes
    public Vector2 RepelForce { get; set; }
    public bool GoingRight { get; set; }

    // References
    private AudioSource audioSrc;

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
        Destroy(gameObject, 10f);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().Hit(this);
        }
        var effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect.gameObject, effect.main.duration);
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        audioSrc.PlayOneShot(shootExplosionSound);
        Destroy(gameObject, shootExplosionSound.length);
    }
}
