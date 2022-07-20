using UnityEngine;

public class MonsterAnimator : MonoBehaviour
{

    Animator animator;

    private void Awake() {
        animator = GetComponentInChildren<Animator>();
    }

    static int
        isMovingID = Animator.StringToHash("IsMoving"),
        speedID    = Animator.StringToHash("Speed"),
        onDeathID  = Animator.StringToHash("OnDeath"),
        onHurtID   = Animator.StringToHash("OnHurt"),
        onAttackID = Animator.StringToHash("OnAttack"),
        attackID   = Animator.StringToHash("Attack")
    ;

    bool isMoving = false;

    public void StartMoving() {
        isMoving = true;
        animator.SetBool(isMovingID, isMoving);
    }
    public void StopMoving() {
        isMoving = false;
        animator.SetBool(isMovingID, isMoving);
    }

    /// <summary>
    /// Set variable that controls transition between walk, chase and fast chase
    /// </summary>
    /// <param name="speed">This is clamped before being sent to animator</param>
    public void SetMovementSpeed(float speed) => animator.SetFloat(speedID, Mathf.Clamp01(speed));

    public void Attack() {
        animator.SetInteger(attackID, Random.Range(0, 2));
        animator.SetTrigger(onAttackID);
    }

    public void TakeDamage() {
        animator.SetTrigger(onHurtID);
    }

    public void Death() {
        animator.SetTrigger(onDeathID);
    }

}
