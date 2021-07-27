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

public class _ShipModule : MonoBehaviour {

    public CellType poweredBy;

    private void Start() {
        ShipController.InstallModule(this);
    }
    private void OnDestroy() {
        ShipController.UnInstallModule(this);
    }
    public virtual void OnMatchCell(int amount) {

    }
}