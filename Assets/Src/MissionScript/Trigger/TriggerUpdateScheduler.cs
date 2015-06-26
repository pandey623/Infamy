using System.Collections.Generic;;

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

    public void AddTrigger(Trigger trigger) {
        triggers.Add(trigger);
    }
}