using UnityEngine;
using System.Collections;

/// <summary>
/// Block that has move HP than a normal block and shooters waste no bullets on.
/// </summary>
public class PiggyBank : Block
{

    protected override void Awake()
    {
        maxHealth = 10;
        base.Awake();
    }

    public override void TakeDamage()
    {
        TakingDamageAnimation();
        base.TakeDamage();
    }
    
    protected override IEnumerator DeathAnimation()
    {
        yield return null;
        // TODO: Death animation
        Destroy(gameObject);
    }

    private void TakingDamageAnimation()
    {
        const float SizeIncrement = 0.1f;
        transform.localScale = transform.localScale + new Vector3(SizeIncrement, SizeIncrement, SizeIncrement);
    }
}
