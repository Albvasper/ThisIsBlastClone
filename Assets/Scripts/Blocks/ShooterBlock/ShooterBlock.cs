using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Block that once places on the dock it will shoot at blocks of the same color.
/// </summary>
public class ShooterBlock : Block
{
    public bool supriseShooter;
    public bool OnDock;
    public int Ammo { get; private set; } = 0;

    [Range(0f, 1f) ][SerializeField] private float firingRate = 0.3f;
    [Range(0f, 100f) ][SerializeField] private float firingForce = 50f;

    [Header("Components")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private TextMeshProUGUI ammoCounter;
    [SerializeField] private LevelManager levelManager;

    private int currentTargetX = 0;
    private int currentTargetZ = 0;

    protected override void Awake()
    {
        base.Awake();
        OnDock = false;
    }

    private void Start()
    {
        if (supriseShooter)
            meshRenderer.material = GridManager.Instance.Surprise;
        else
            AssignMaterial(Color);
    }

    public void AssignAmmo(int ammo)
    {
        Ammo = ammo;
        ammoCounter.text = Ammo.ToString();
    }

    /// <summary>
    /// Starts shooting coroutine
    /// </summary>
    public void ReadyToShoot()
    {
        StopAllCoroutines();
        StartCoroutine(SearchForBlocks());
    }

    public void Upgrade(int extraAmmoCount)
    {
        Ammo += extraAmmoCount;
        ammoCounter.text = Ammo.ToString();
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

    private IEnumerator SearchForBlocks()
    {
        while (Ammo > 0)
        {
            Block target = GridManager.Instance.grid[currentTargetX, 0, currentTargetZ];
            if (target != null && target.Color == Color)
            {
                // Found target
                ShootAt(target);
            }
            AimForNextBlock();
            yield return new WaitForSeconds(firingRate);
        }
    }

    private void AimForNextBlock()
    {
        // Move on the Z axis first
        currentTargetZ++;

        if (currentTargetZ >= GridManager.Instance.gridZ)
        {
            currentTargetZ = 0;
            currentTargetX++;
        }

        // Loop back to start when X exceeds grid width
        if (currentTargetX >= GridManager.Instance.gridX)
        {
            currentTargetX = 0;
        }
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
            GridManager.Instance.RemoveBlock(targetBlock);
            targetBlock.Die();
            levelManager.MakeProgress();
        }
    }
}
