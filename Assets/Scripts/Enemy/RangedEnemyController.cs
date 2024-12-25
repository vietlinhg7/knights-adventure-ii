using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RangedEnemyController : EnemyController
{
    [SerializeField] private GameObject rangedAttackPrefab;

    protected override void Attack()
    {
        base.Attack();
        GameObject attackObject = Instantiate(rangedAttackPrefab);
    }
}
