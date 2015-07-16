using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

//todo: Nail down slip factor and find a nice way to make it dynamic

//Note: rigidbody.angularVelocity is a Vector3. Its direction is the axis of rotation and its magnitude the speed of rotation (counter-clockwise) in radians/second. 
//So if we want to know the rotation speed, we need to get the magnitude of the vector: rigidbody.angularVelocity.magnitude 
//Which will give the ABSOLUTE speed in radians/second (i.e. a value of 2pi corresponds to 1 revolution per second). In order to convert it to revolution per minute, multiply by 60/2pi = 30/pi. rpm = (30 / pi) * magnitude

//a cool effect for formations is to juggle their slots as they are moving 
//a cool effect for individuals getting space might be to reduce aerodynamic effect (slip factor) and have them orient
//more towards their target while travelling in direction of velocity

public class SpaceCraftController : MonoBehaviour {
    [SerializeField]
    private float m_MaxEnginePower = 40f;        // The maximum output of the engine.
    [SerializeField]
    private float m_AerodynamicEffect = 0.5f;   // How much aerodynamics affect the speed of the aeroplane.
    [SerializeField]
    private float m_ThrottleChangeSpeed = 0.3f;  // The speed with which the throttle changes.

    public float Throttle { get; private set; }                     // The amount of throttle being used.
    public float ForwardSpeed { get; private set; }                 // How fast the aeroplane is traveling in it's forward direction.
    public float EnginePower { get; private set; }                  // How much power the engine is being given.
    public float MaxEnginePower { get { return m_MaxEnginePower; } }    // The maximum output of the engine.
    public float RollAngle { get; private set; }
    public float PitchAngle { get; private set; }
    public float RollInput { get; private set; }
    public float PitchInput { get; private set; }
    public float YawInput { get; private set; }
    public float ThrottleInput { get; private set; }

    public float maxPitchSpeed = 90f;
    public float maxRollSpeed = 90f;
    public float maxYawSpeed = 90f;
    public bool useAeroFactor = true;
    public FlightControls controls;

    private float m_AeroFactor;
    private new Rigidbody rigidbody;

    [Header("Debug")]
    public bool drawVelocity = true;

    void FixedUpdate() {
        controls.Clamp();
        UpdateThrottle();
        TurnTowardsPoint(controls.destination);
    }

    public float PitchRateDegrees {
        get {
            return rigidbody.angularDrag / maxPitchSpeed;
        }
    }

    public float YawRateDegrees {
        get {
            return rigidbody.angularDrag / maxYawSpeed;
        }
    }

    public float RollRateDegrees {
        get {
            return rigidbody.angularDrag / maxRollSpeed;
        }
    }

    //todo account of angular velocity change to reach target?
    //1.5 * Forward speed gives us little more leeway
    public float TurnRadius {
        get {
            float pitchRadius = maxPitchSpeed * Mathf.Deg2Rad;
            float yawRadius = maxYawSpeed * Mathf.Deg2Rad;
            if (pitchRadius > yawRadius) {
                return ForwardSpeed * 1.5f / (yawRadius);
            } else {
                return ForwardSpeed * 1.5f / (pitchRadius);
            }
        }
    }

    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    //todo move to entity
    public float speed = 0;
    public float maxSpeed = 10f;

    public void UpdateThrottle() {
        float accelerationRate = 0.25f; //todo move to entity //percent of max speed / second.read from ship specs
        float targetSpeed = maxSpeed * controls.throttle;
        float speedStep = accelerationRate * maxSpeed * Time.fixedDeltaTime;
        if (speed > targetSpeed) {
            speed = Mathf.Clamp(speed - speedStep, 0, speed);
        } else if (speed < targetSpeed) {
            speed = Mathf.Clamp(speed + speedStep, 0, maxSpeed);
        }
        rigidbody.velocity = transform.forward * speed;
    }

    public void Move(float rollInput, float pitchInput, float yawInput, float throttleInput) {
        RollInput = rollInput;
        PitchInput = pitchInput;
        YawInput = yawInput;
        ThrottleInput = throttleInput;

        ClampInputs();


        CalculateRollAndPitchAngles();

        ControlThrottle();

        CaluclateAerodynamicEffect();

        if (drawVelocity) {
            Debug.DrawRay(transform.position, GetComponent<Rigidbody>().velocity.normalized * ForwardSpeed, Color.white);
        }
    }

    private void ClampInputs() {
        RollInput = Mathf.Clamp(RollInput, -1, 1);
        PitchInput = Mathf.Clamp(PitchInput, -1, 1);
        YawInput = Mathf.Clamp(YawInput, -1, 1);
        ThrottleInput = Mathf.Clamp(ThrottleInput, -1, 1);
    }

    public float GetRollAngle() {
        var flatForward = transform.forward;
        flatForward.y = 0;
        if (flatForward.sqrMagnitude > 0) {
            flatForward.Normalize();
            // calculate current pitch angle
            var localFlatForward = transform.InverseTransformDirection(flatForward);
            PitchAngle = Mathf.Atan2(localFlatForward.y, localFlatForward.z);
            // calculate current roll angle
            var flatRight = Vector3.Cross(Vector3.up, flatForward);
            var localFlatRight = transform.InverseTransformDirection(flatRight);
            RollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);
        }
        return RollAngle;
    }

    private void CalculateRollAndPitchAngles() {
        // Calculate roll & pitch angles
        // Calculate the flat forward direction (with no y component).
        var flatForward = transform.forward;
        flatForward.y = 0;
        if (flatForward.sqrMagnitude > 0) {
            flatForward.Normalize();
            // calculate current pitch angle
            var localFlatForward = transform.InverseTransformDirection(flatForward);
            PitchAngle = Mathf.Atan2(localFlatForward.y, localFlatForward.z);
            // calculate current roll angle
            var flatRight = Vector3.Cross(Vector3.up, flatForward);
            var localFlatRight = transform.InverseTransformDirection(flatRight);
            RollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);
        }
    }

    private void ControlThrottle() {
        Throttle = Mathf.Clamp01(Throttle + ThrottleInput * Time.deltaTime * m_ThrottleChangeSpeed);
        EnginePower = Throttle * m_MaxEnginePower;
        rigidbody.AddForce(EnginePower * transform.forward, ForceMode.Acceleration);
        var velocity = rigidbody.velocity;
        var magnitude = velocity.magnitude;
        if (magnitude > 0 && magnitude > m_MaxEnginePower) {
            velocity *= (m_MaxEnginePower / magnitude);
            rigidbody.velocity = velocity;
        }
        // Forward speed is the speed in the forward direction (not the same as its velocity)
        var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        ForwardSpeed = Mathf.Max(0, localVelocity.z);
    }

    //it seems like reducing this value introduces more slip but a value that is too low
    private void CaluclateAerodynamicEffect() {
        // "Aerodynamic" calculations. This is a very simple approximation of the effect that a plane
        // will naturally try to align itself in the direction that it's facing when moving at speed.
        // Without this, the plane would behave a bit like the asteroids spaceship!
        if (rigidbody.velocity.sqrMagnitude > 0) {
            // compare the direction we're pointing with the direction we're moving
            //areoFactor is between 1 and -1
            m_AeroFactor = Vector3.Dot(transform.forward, rigidbody.velocity.normalized);
            // multipled by itself results in a desirable rolloff curve of the effect
            m_AeroFactor *= m_AeroFactor;
            // Finally we calculate a new velocity by bending the current velocity direction towards
            // the the direction the plane is facing, by an amount based on this aeroFactor
            var newVelocity = Vector3.Lerp(rigidbody.velocity, transform.forward * ForwardSpeed,
                                           m_AeroFactor * ForwardSpeed *
                                           m_AerodynamicEffect * Time.deltaTime);
            rigidbody.velocity = newVelocity;

            // also rotate the plane towards the direction of movement
            //rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation,
            //                                      Quaternion.LookRotation(rigidbody.velocity, transform.up),
            //                                      m_AerodynamicEffect * Time.deltaTime);
        }
    }

    public void TurnTowardsPoint(Vector3 point, float roll = 0f) {
        Vector3 localTarget = transform.InverseTransformPoint(point);
        float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
        float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);

        Vector3 av = rigidbody.angularVelocity;
        Vector3 localAV = transform.InverseTransformDirection(rigidbody.angularVelocity);
        float maxYaw = maxYawSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime;
        float maxRoll = maxRollSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime;
        float maxPitch = maxRollSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime;
        //input values are directions RELATIVE to CURRENT ORIENTATION, not accounting for angular velocity
        //which means to actually go in the desired direction, we need to take that input and adjust for AV

        //this treats inputs as 'desired angular velocities' 
        //note that we do not turn as tightly as we can in this mode because input nodes are mapped
        //to input values, so only outer nodes will result in a full strength turn
        float desiredX = targetAnglePitch;
        float desiredY = targetAngleYaw;
        float desiredZ = roll * Mathf.Deg2Rad + GetRollAngle();

        if (desiredX >= localAV.x) {
            localAV.x = Mathf.Clamp(localAV.x + maxPitch, localAV.x, desiredX);
        } else {
            localAV.x = Mathf.Clamp(localAV.x - maxPitch, desiredX, localAV.x);
        }

        if (desiredY >= localAV.y) {
            localAV.y = Mathf.Clamp(localAV.y + maxYaw, localAV.y, desiredY);
        } else {
            localAV.y = Mathf.Clamp(localAV.y - maxYaw, desiredY, localAV.y);
        }

        if (desiredZ >= localAV.z) {
            localAV.z = Mathf.Clamp(localAV.z + maxRoll, localAV.z, desiredZ);
        } else {
            localAV.z = Mathf.Clamp(localAV.z - maxRoll, desiredZ, localAV.z);
        }
        localAV.x = Mathf.Clamp(localAV.x, -maxPitchSpeed * Mathf.Deg2Rad, maxPitchSpeed * Mathf.Deg2Rad);
        localAV.y = Mathf.Clamp(localAV.y, -maxYawSpeed * Mathf.Deg2Rad, maxYawSpeed * Mathf.Deg2Rad);
        localAV.z = Mathf.Clamp(localAV.z, -maxRollSpeed * Mathf.Deg2Rad, maxRollSpeed * Mathf.Deg2Rad);
        rigidbody.angularVelocity = transform.TransformDirection(localAV);
    }

    //this accounts for overshoot -- probably make this configurable, larger ships will not want to
    //overshoot and different ships / skill levels might have different overshoot abilities
    //it might also be situational, could be an interesting mission parameter.
    //  torque *= Time.fixedDeltaTime;
    //might be able to do a vector blend on torque so that it blends with current angular velocity
    //this would help reduce / enhance oversteering

    //if (Vector3.Dot(transform.forward, (target.position - transform.position).normalized) > 0.98f) {
    //    if (Vector3.Distance(transform.position, target.position) < ForwardSpeed * 1.5f) {
    //        rigidbody.angularVelocity = torque;
    //    } else {
    //        rigidbody.AddTorque(torque, ForceMode.Acceleration);
    //    }
    //} else {
    //    rigidbody.AddTorque(torque, ForceMode.Acceleration);
    //}

}