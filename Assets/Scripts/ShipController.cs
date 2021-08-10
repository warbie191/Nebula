using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController {

    
    // need to track "unlocked" modules... those available at beginning of mission?
    // need to track held modules... those in cargo, but not installed
    // need to track installed modules

    private static List<_ShipModule> cargoModules = new List<_ShipModule>();

    private static List<_ShipModule> installedModules = new List<_ShipModule>();

    public static float shieldAmount = 0;

    public static void InstallModule(_ShipModule module) {
        if(!installedModules.Contains(module)) installedModules.Add(module);
    }
    public static void UnInstallModule(_ShipModule module) {
        installedModules.Remove(module);
    }

    public static void GemsMatched(CellType moduleType, int amount) {
        
        //Debug.Log(amount + " of " + moduleType + " matched");
        
        foreach(_ShipModule module in installedModules) {

            if(module.poweredBy == moduleType) {
                module.ModuleCellsMatched(amount);
            }
        }
    }
    
}
