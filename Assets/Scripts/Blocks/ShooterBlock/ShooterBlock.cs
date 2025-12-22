using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Block that once places on the dock it will shoot at piggy banks or blocks of the same color.
/// </summary>
public class ShooterBlock : Block
{
    public bool readyToDeploy; 
    public bool supriseShooter;
    public bool OnDock;
    public int Ammo { get; private set; } = 0;

    [Range(0f, 1f) ][SerializeField] private float firingRate = 0.3f;
    [Range(0f, 100f) ][SerializeField] private float firingForce = 50f;

    [Header("Components")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private TextMeshProUGUI ammoCounter;

    private int currentTargetX = 0;
    private int currentTargetZ = 0;
    private float lastFireTime;
    private const float IdleThreshold = 2f;

    protected override void Awake()
    {
        base.Awake();
        OnDock = false;
        readyToDeploy = false;
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
    
    /// <summary>
    /// Return if shooter stopped firing for a couple of seconds.
    /// </summary>
    /// <returns></returns>
    public bool HasStoppedShooting()
    {
        return Time.time - lastFireTime >= IdleThreshold;
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

    // Search for target: Look for piggy banks, if there are none look for block that are the same color as the shooter
    private IEnumerator SearchForBlocks()
    {
        while (Ammo > 0)
        {
            Block target = GetBlock();
            
            if (target == null || !IsValidTarget(target))
            {
                AimForNextBlock();
                yield return new WaitForSeconds(firingRate);
                continue;
            }

            ShootAt(target);

            if (!IsPiggyBank(target))
            {
                AimForNextBlock();
            }

            yield return new WaitForSeconds(firingRate);
        }
    }

    // Gets the current block target giving priority to piggy banks.
    private Block GetBlock()
    {
        Block piggyBank = ScanForPiggyBank();
        if (piggyBank != null)
            return piggyBank;

        return GridManager.Instance.grid[currentTargetX, 0, currentTargetZ];
    }

    // Searchs for piggy banks and returns them if there are any
    private Block ScanForPiggyBank()
    {
        for (int x = 0; x < GridManager.Instance.gridX; x++)
        {
            for (int z = 0; z < GridManager.Instance.gridZ; z++)
            {
                Block block = GridManager.Instance.grid[x, 0, z];
                if (block != null && block.Color == BlockColor.PiggyBank)
                {
                    currentTargetX = x;
                    currentTargetZ = z;
                    return block;
                }
            }
        }
        return null;
    }

    // Returns if the current block is a piggy bank or a normal block
    private bool IsValidTarget(Block block)
    {
        return IsPiggyBank(block) || IsSameColor(block);
    }

    // Returns if the current block is the same color as the shooter
    private bool IsSameColor(Block block)
    {
        return block.Color == Color;
    }
    // Returns if the current block is a piggy bank
    private bool IsPiggyBank(Block block)
    {
        return block.Color == BlockColor.PiggyBank;
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
        lastFireTime = Time.time;
        if (targetBlock != null)
        {
            if (targetBlock.Color != BlockColor.PiggyBank)
            {
                Ammo--;
                ammoCounter.text = Ammo.ToString();
            }
            // Spawn bullet
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet bulletComponent =  bullet.GetComponent<Bullet>();
            // Aim at block and shoot
            Vector3 direction = targetBlock.transform.position - transform.position;
            bulletComponent.Rb.AddForce(direction.normalized * firingForce, ForceMode.Impulse);
            // Deal damage to block
            targetBlock.TakeDamage();
        }
    }
}
