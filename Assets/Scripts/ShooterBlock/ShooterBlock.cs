using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Block that once places on the dock it will shoot at blocks of the same color.
/// </summary>
public class ShooterBlock : Block
{
    [Header("Properties")]
    public bool OnDock;
    [Range(0f, 1f) ][SerializeField] private float firingRate = 0.3f;
    [Range(0f, 100f) ][SerializeField] private float firingForce = 50f;

    [Header("Components")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private TextMeshProUGUI ammoCounter;

    private int ammo = 0;

    protected override void Awake()
    {
        base.Awake();
        OnDock = false;
    }

    private void OnEnable()
    {
        // Count how many blocks of the same color are in the grid and add the same number of bullets
    }

    private void Start()
    {
        Initialize(Color);
        foreach (Block block in GridManager.Instance.grid)
        {
            if (block.Color == Color)
                ammo++;
        }
        ammoCounter.text = ammo.ToString();
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

    private void ShootAt(Block targetBlock)
    {
        if (targetBlock != null)
        {
            ammo--;
            ammoCounter.text = ammo.ToString();
            // Spawn bullet
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet bulletComponent =  bullet.GetComponent<Bullet>();
            // Aim at block
            Vector3 direction = targetBlock.transform.position - transform.position;
            bulletComponent.Rb.AddForce(direction.normalized * firingForce, ForceMode.Impulse);
            targetBlock.Kill();
        }
    }
}
