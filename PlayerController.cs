using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Entity entity;
    private EngineSystem engines;
    private FlightControls flightControls;
    private WeaponSystem weaponSystem;

    private float lastW;
    public float yawDamp = 0.5f;
    public float pitchDamp = 1;
    public float rollDamp = 2;
    public float startThrottle = 1f;

    public void Start() {
        entity = GetComponent<Entity>();
        engines = GetComponent<EngineSystem>();
        flightControls = engines.FlightControls;
        weaponSystem = GetComponent<WeaponSystem>();
        lastW = startThrottle;
    }

    public void Update() {
        float x = Input.GetAxis("JoystickX") * yawDamp;
        float y = Input.GetAxis("JoystickY"); 
        float z = Input.GetAxis("JoystickZ") * rollDamp;
        float w = Input.GetAxisRaw("JoystickThrottle");
        bool fire = Input.GetButton("Fire1");

        if (w != lastW) {
            lastW = w;
        } 

        flightControls.SetThrottle( (lastW + 1) * 0.5f);
        flightControls.SetStickInputs(x, y, -z);
        if (fire) {
            weaponSystem.weaponGroups[0].Fire();
        }
    }
}

//missiles
//fire groups
//generic ui
//afterburner
//radar
//targeting system / nav
