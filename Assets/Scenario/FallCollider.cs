using UnityEngine;

public class FallCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            StartCoroutine(player.Die());
        }
    }
}
