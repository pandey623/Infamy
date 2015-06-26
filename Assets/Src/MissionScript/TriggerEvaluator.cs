using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerEvalator {
    public void Run() {
        
    }

    public void Reset() {
        
    }

    public void Pause() {
        
    }

    public void Resume() {
        
    }
}

public enum TriggerStatus {
    Pending, Completed, Paused
}

public abstract class Trigger {
    protected Action action;
    protected Condition condition;
    protected TriggerUpdateScheduler scheduler;
    protected TriggerStatus status;

    public abstract TriggerStatus Run();
}

public class WhenTrigger {

}

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

public class TriggerUpdateScheduler {
    
    private Timer timer;
    private float updateInterval;
    private List<Trigger> triggers;
 
    private static TriggerUpdateScheduler defaultScheduler;
    public static TriggerUpdateScheduler GetDefaultScheduler() {
        if (defaultScheduler == null) {
            defaultScheduler = new TriggerUpdateScheduler();
        }
        return defaultScheduler;
    }

    public TriggerUpdateScheduler(float updateInterval = -1) {
        this.timer = new Timer();
        this.updateInterval = updateInterval;
        this.triggers = new List<Trigger>();
        TriggerUpdateRuntime.AddScheduler(this);
    }

    public void Update() {
        if (updateInterval < 0 || timer.ReadyWithReset(updateInterval)) {
            for (int i = 0; i < triggers.Count; i++) {
                if (triggers[i].Run() != TriggerStatus.Pending) {
                    triggers.RemoveAt(--i);
                }
            }
        }
    }
}