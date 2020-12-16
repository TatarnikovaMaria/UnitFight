using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float lifeTime = 10f;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void Shoot(Transform target, float bulletSpeed, int bulletDamage)
    {
        StartCoroutine(Move(target, bulletSpeed, bulletDamage));
    }

    IEnumerator Move(Transform target, float bulletSpeed, int bulletDamage)
    {
        while (target == null || Vector3.Distance(_transform.position, target.position) > LvlManager.instance.WorldCellSize)
        {
            _transform.position += _transform.forward * bulletSpeed * Time.deltaTime;
            yield return null;
        }

        if (target != null)
        {
            IDamagable damagable = target.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.GetDamage(bulletDamage);
            }
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
