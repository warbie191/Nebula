using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : _ShipModule
{

    float energyLevel = 0;

    public override void ModuleActivated() {
        
    }

    public override void ModuleCellsMatched(int amount) {
        energyLevel += amount;
        if (autoActivate) ModuleActivated();
    }
}
