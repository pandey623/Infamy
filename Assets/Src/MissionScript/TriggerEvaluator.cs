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

    public abstract bool Run();
}

public class WhenTrigger {

}

public class TriggerUpdateRuntime : MonoBehaviour {
    private List<TriggerUpdateScheduler> schedulers;

    
}

public class TriggerUpdateScheduler {
    public static TriggerUpdateScheduler GetDefaultScheduler() {
        if
    }
}