using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModuleType {
    None,
    DarkMatter,
    Wild,
    LaserCannon,
    RocketLauncher,
    ShieldGenerator,
    RepairDroids,
    EngineDrive
}

public class ShipModule : MonoBehaviour
{

    public ModuleType type = ModuleType.None;

    public virtual void OnMatchCell(int amount) {

    }
}


public class ShipModuleEngine : ShipModule {

    new public ModuleType type = ModuleType.EngineDrive;

    public override void OnMatchCell(int amount) {
        // TODO: add behavior here...
    }

}