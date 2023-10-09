using UnityEngine;

public class Player : MonoBehaviour
{
    public Car car;

    private void Update()
    {
        if (!car) return;

        car.Drive(Input.GetAxisRaw("Vertical"));
        car.Turn(Input.GetAxisRaw("Horizontal"));
        car.Brake(Input.GetKey(KeyCode.LeftShift) ? 1f : 0f);
    }
}
