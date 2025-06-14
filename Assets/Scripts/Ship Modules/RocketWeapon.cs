using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketWeapon : _ShipModule
{

    public Transform firePoint;
    public GameObject bulletPrefab;

    int ammo = 0;

    public override void ModuleActivated() {
        
        GameObject nu = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, transform);
    }

    public override void ModuleCellsMatched(int amount) {
        ammo += amount;
        if (autoActivate) ModuleActivated();
    }

}
