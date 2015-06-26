using System;
using System.Collections.Generic;

public abstract class Trigger {
    protected Action action;
    protected Condition condition;
    protected TriggerUpdateScheduler scheduler;
    protected TriggerStatus status;

    public abstract TriggerStatus Run();

    public Trigger(TriggerUpdateScheduler scheduler) {
        if (scheduler == null) {
            this.scheduler = TriggerUpdateScheduler.GetDefaultScheduler();
        } else {
            this.scheduler = scheduler;
        }
        this.scheduler.AddTrigger(this);
    }
}