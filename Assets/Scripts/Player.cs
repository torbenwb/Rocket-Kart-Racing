using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    bool controlEnabled = true;
    [SerializeField] int playerIndex = 0;
    [SerializeField] Car car;

    public UnityEvent<Player, float> BoostMeterUpdate;

    public GameObject GetCarGameObject(){
        return car.gameObject;
    }

    public void EnableCarControl(){
        controlEnabled = true;
    }

    public void DisableCarControl(){
        controlEnabled = false;
        car.Drive(0f);
        car.Turn(0f);
        car.Boost(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!controlEnabled || !car) return;

        car.Drive(Input.GetAxisRaw($"p{playerIndex} Drive"));
        car.Turn(Input.GetAxisRaw($"p{playerIndex} Turn"));
        car.Boost(Input.GetButton($"p{playerIndex} Boost"));

        car.Roll(Input.GetAxisRaw($"p{playerIndex} Roll"));
        car.Yaw(Input.GetAxisRaw($"p{playerIndex} Yaw"));
        car.Pitch(Input.GetAxisRaw($"p{playerIndex} Pitch"));

        if (Input.GetButtonDown($"p{playerIndex} Jump")) car.Jump();
        if (Input.GetButtonDown($"p{playerIndex} Reset")) Race.ResetRacerToLastCheckpoint(GetCarGameObject());

        BoostMeterUpdate.Invoke(this, car.boost / car.maxBoost);
    }
}
