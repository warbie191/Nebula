using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketWeapon : _ShipModule
{

    public Transform firePoint;
    public GameObject bulletPrefab;

    int ammo = 0;


    public override void OnMatchCell(int amount) {
        ammo += amount;
        GameObject nu = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, transform);
    }
}
