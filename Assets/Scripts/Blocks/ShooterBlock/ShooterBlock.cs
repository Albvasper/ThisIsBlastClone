using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Block that once places on the dock it will shoot at blocks of the same color.
/// </summary>
public class ShooterBlock : Block
{
    public bool OnDock;
    public int Ammo { get; private set; } = 0;

    [Range(0f, 1f) ][SerializeField] private float firingRate = 0.3f;
    [Range(0f, 100f) ][SerializeField] private float firingForce = 50f;

    [Header("Components")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private TextMeshProUGUI ammoCounter;
    [SerializeField] private LevelManager levelManager;

    protected override void Awake()
    {
        base.Awake();
        OnDock = false;
    }

    private void Start()
    {
        Initialize(Color);
        foreach (Block block in GridManager.Instance.grid)
        {
            if (block.Color == Color)
                Ammo++;
        }
        ammoCounter.text = Ammo.ToString();
    }

    /// <summary>
    /// Starts shooting coroutine
    /// </summary>
    public void ReadyToShoot()
    {
        StartCoroutine(SearchAndDestroyBlock());
    }

    /// <summary>
    /// Iterates through the bottom row of the grid. If it finds a block that is the same color as the shooter, it
    /// shoots at it and destroys that block. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator SearchAndDestroyBlock()
    {
        while(GridManager.Instance.grid.GetLength(0) > 0)
        {
            // Iterate through the bottom row
            for (int i = 0; i < GridManager.Instance.gridX; i++)
            {
                // Compare colors
                if (GridManager.Instance.grid[i, 0].Color == Color)
                {   
                    ShootAt(GridManager.Instance.grid[i, 0]);
                    yield return new WaitForSeconds(firingRate);
                }
            }
        }
    }
    
    public void Upgrade(int extraAmmoCount)
    {
        Ammo += extraAmmoCount;
    }

    public void StopShooting()
    {
        StopAllCoroutines();
    }
    
    public void Die(int direction)
    {
        StartCoroutine(DeathAnimation(direction));
    }
    
    // Moves the block outside of the camera view and then destroys it.
    protected virtual IEnumerator DeathAnimation(int direction)
    {
        float duration = 2.5f;
        float elapsed = 0f;
        float velocity = 0.1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position += new Vector3 (velocity, 0, 0) * direction;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void ShootAt(Block targetBlock)
    {
        if (targetBlock != null)
        {
            Ammo--;
            ammoCounter.text = Ammo.ToString();
            // Spawn bullet
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet bulletComponent =  bullet.GetComponent<Bullet>();
            // Aim at block and shoot
            Vector3 direction = targetBlock.transform.position - transform.position;
            bulletComponent.Rb.AddForce(direction.normalized * firingForce, ForceMode.Impulse);
            // Register progress on level
            targetBlock.Die();
            levelManager.MakeProgress();
        }
    }
}
