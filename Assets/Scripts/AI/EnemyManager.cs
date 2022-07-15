using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<EnemyNavMesh> EnemiesAttacking = new List<EnemyNavMesh>();
    [SerializeField] private int CountOfEnemiesAttacking = 0;

    private void Start()
    {
        EnemyActions.AddEnemyAttacking += AddenemyAttacking;
        EnemyActions.RemoveEnemyAttacking += RemoveenemyAttacking;
    }
    private void OnDisable()
    {
        EnemyActions.AddEnemyAttacking -= AddenemyAttacking;
        EnemyActions.RemoveEnemyAttacking -= RemoveenemyAttacking;
    }
    public void AddenemyAttacking(EnemyNavMesh enemy)
    {
        EnemiesAttacking.Add(enemy);
        CountOfEnemiesAttacking = EnemiesAttacking.Count;
    }
    public void RemoveenemyAttacking(EnemyNavMesh enemy)
    {
        EnemiesAttacking.Remove(enemy);
        CountOfEnemiesAttacking = EnemiesAttacking.Count;
    }
}
