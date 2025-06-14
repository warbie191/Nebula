using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType {
    None,
    Wild,
    LaserCannon,
    RocketLauncher,
    ShieldGenerator,
    RepairDroids,
    EngineDrive
}

public abstract class _ShipModule : MonoBehaviour {

    public CellType poweredBy;
    public bool autoActivate = true;

    private void Start() {
        ShipController.InstallModule(this);
    }
    private void OnDestroy() {
        ShipController.UnInstallModule(this);
    }
    public abstract void ModuleCellsMatched(int amount);
    public abstract void ModuleActivated();
}