using UnityEngine;

/// <summary>
/// Pellet that aims for blocks on the grid
/// </summary>
public class Bullet : MonoBehaviour
{
    public Rigidbody Rb { get; set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Block"))
        {
            Die();
        }
    }

    /// <summary>
    /// Handles bullet death
    /// </summary>
    private void Die()
    {
        Destroy(gameObject);
    }
}   
