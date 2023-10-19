using UnityEngine;

public class Player : MonoBehaviour
{
    public Car car;
    public RocketBoost rocketBoost;
    public AirControl airControl;
    // Whether or not the player can control the car / abilities
    bool controlEnabled;

    public void SetControlEnabled(bool enabled)
    {
        controlEnabled = enabled;
    }

    private void Update()
    {
        if (!car) return;
        if (!controlEnabled) return;

        car.Drive(Input.GetAxisRaw("Vertical"));
        car.Turn(Input.GetAxisRaw("Horizontal"));
        car.Brake(Input.GetKey(KeyCode.LeftShift) ? 1f : 0f);

        if (rocketBoost)
        {
            rocketBoost.ToggleBoost(Input.GetKey(KeyCode.LeftShift));
        }

        if (airControl)
        {
            if (Input.GetKeyDown(KeyCode.Space)) airControl.Jump();
            airControl.Pitch(Input.GetAxisRaw("Vertical"));
            airControl.Yaw(Input.GetAxisRaw("Horizontal"));
            airControl.Roll(Input.GetAxisRaw("Roll"));
        }
    }
}
