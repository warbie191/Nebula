using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {

    public static ShipController main; // null

    public List<ShipModule> installedModules = new List<ShipModule>();

    void Start() {
        main = this;
    }

    public void GemsMatched(ModuleType moduleType, int amount) {
        
        print(amount + " of " + moduleType + " matched");
        
        foreach(ShipModule module in installedModules) {
            if(module.type == moduleType) {
                module.OnMatchCell(amount);
            }
        }
    }
    
}
