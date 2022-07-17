using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EnemyActions
{
    public static Action<EnemyNavMesh> AddEnemyAttacking;
    public static Action<EnemyNavMesh> RemoveEnemyAttacking;
    public static Action<EnemyNavMesh> OnEnemyKilled;
}
