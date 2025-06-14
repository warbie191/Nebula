using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGenerator : _ShipModule {

    
    public override void ModuleActivated() {
        
    }

    public override void ModuleCellsMatched(int amount) {
        ShipController.shieldAmount += amount;
    }
}
