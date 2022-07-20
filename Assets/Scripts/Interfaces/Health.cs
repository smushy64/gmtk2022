using System;
using UnityEngine;

/// <summary>
/// <br>Class for all entities that can be damaged/healed.</br>
/// <br>Derives from MonoBehavior</br>
/// </summary>
public abstract class Health : MonoBehaviour
{
    public float HP { get; protected set; } = 100f;
    public float MaxHP { get; protected set; } = 100f;
    public bool IsAlive { get; protected set; } = true;

    public virtual void Damage(float delta) {
        HP = Mathf.Max(HP - delta, 0f);
        OnDamage?.Invoke();
        if( HP <= 0f ) {
            Death();
        }
    }
    public virtual void Heal(float delta) {
        HP = Mathf.Min(HP + delta, MaxHP);
        OnHeal?.Invoke();
    }

    protected virtual void Death() {
        OnDeath?.Invoke();
        IsAlive = false;
    }

    protected virtual void ResetHealth() {
        IsAlive = true;
        HP = MaxHP;
    }

    public Action OnDamage;
    public Action OnHeal;
    public Action OnDeath;
}
