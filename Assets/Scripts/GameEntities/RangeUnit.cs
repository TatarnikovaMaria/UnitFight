using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeUnit : BaseUnit
{
    public GameObject bulletPrefab;
    public Transform gunShotPosition;

    private float bulletSpeed = 10f;

    public override void Init(UnitSettings settings)
    {
        base.Init(settings);
        this.bulletSpeed = settings.bulletSpeed;
    }

    protected override void Attack()
    {
        GameObject bullet = Instantiate(bulletPrefab, gunShotPosition.position, gunShotPosition.rotation, transform.parent);
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if(bulletComp != null)
        {
            bulletComp.Shoot(targetEnemy != null ? targetEnemy.transform : null, bulletSpeed, attackDamage);
        }
    }
}
