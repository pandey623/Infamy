using UnityEngine;

public enum FlightControlMode {
    Auto, Stick
}

public class FlightControls {
    public float yaw;
    public float pitch;
    public float roll;
    public float throttle;
    public float slip;
    public float rollOverride;
    public Vector3 destination; 
    public FlightControlMode mode;

    public FlightControls() { }

    public void Clamp() {
        yaw = Mathf.Clamp(yaw, -1, 1);
        pitch = Mathf.Clamp(pitch, -1, 1);
        roll = Mathf.Clamp(roll, -1, 1);
        throttle = Mathf.Clamp(throttle, -1, 1);
        slip = Mathf.Clamp(slip, 0, 1);
    }

    public void SetStickInputs(float yaw, float pitch, float roll) {
        mode = FlightControlMode.Stick;
        this.yaw = yaw;
        this.pitch = pitch;
        this.roll = roll;
    }

    public void SetThrottle(float throttle) {
        this.throttle = throttle;
    }
     
    public void GoTo(Vector3 destination, float throttle = 1f, float rollOverride = 0) {
        mode = FlightControlMode.Auto;
        this.throttle = throttle;
        this.rollOverride = rollOverride;
        this.destination = destination;
    }

    public void TurnTo(Vector3 lookAt, float rollOverride = 0) {
        this.rollOverride = rollOverride;
        this.destination = lookAt;
    }
}