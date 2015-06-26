using UnityEngine;
using System.Collections.Generic;

public class TriggerUpdateRuntime : MonoBehaviour {
    private static List<TriggerUpdateScheduler> schedulers;

    public static void AddScheduler(TriggerUpdateScheduler scheduler) {
        if (schedulers == null) {
            schedulers = new List<TriggerUpdateScheduler>();
        }
        schedulers.Add(scheduler);
    }

    public void Awake() {
        //ensure single instance    
    }

    public void Update() {
        for (int i = 0; i < TriggerUpdateRuntime.schedulers.Count; i++) {
            TriggerUpdateRuntime.schedulers[i].Update();
        }
    }
}
