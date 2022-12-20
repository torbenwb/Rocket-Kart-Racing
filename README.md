# MC_Rocket_Kart_Racing

In this project my goal was to create a racing game using a physics driven car model while incorporating the player abilities from Rocket League specifically the abilities to jump / air jump, control the rotation of the car in the air, and to rocket boost in the car's forward direction. With the car and abilities, I created three different game modes:

* **Time Trials**: A simple race track where the player competes against their best time. The race track was designed to require the use of the abilities described above.
* **Tricks**: An optional mode where the player drives around a giant skate park and increases their score by performing tricks in the air.
* **Practice Pitch**: A simplified version of the practice pitch in Rocket League where players can practice scoring goals without AI or other players.

## Physics Driven Car

![](https://github.com/torbenwb/MC_Rocket_Kart_Racing/blob/main/ReadMe_Images/Chapter_1.gif)

The first step in building any racing / car based action game is the car. In this project the car is controlled by a very simple public interface which allows the player to `Drive()`, `Turn()` and `Brake()`, however internally the motion and rotation of the car is controlled through a series of forces:

* **Suspension Force**: Calculates an upwards spring force based on how close the car is to the ground.
* **Longitudinal Force**: Forward / backwards force based on a combination of user input to drive forward and backwards and a friction coefficient to stop / slow the car's movement when on the ground.
* **Lateral Force**: Right/Left force provides force oppositional to lateral velocity based on friction coefficient.
* **Turning Force**: Converts user input and forward velocity into torque to turn the car at a certain rate while driving forward / backward.

### Suspension Force 

Suspension force is calculated per wheel touching the ground. For each wheel, the `Car` script performs a downwards raycast at the wheel's position. The distance between the desired end of the raycast and the hit point determines the `offset`. The greater the `offset` the more spring force applied to push the car back up.

```cs
private void ApplySuspensionForce()
  {
      bool tempGrounded = false;

      foreach(Transform wheel in wheels)
      {
          Vector3 origin = wheel.position;
          Vector3 direction = -wheel.up;
          RaycastHit hit;
          float offset = 0f;

          if (Physics.Raycast(origin,direction,out hit, wheelRadius)){
              tempGrounded = true;

              Vector3 end = origin + (direction * wheelRadius);
              offset = (end - hit.point).magnitude;

              float pointVelocity = Vector3.Dot(wheel.up, rigidbody.GetPointVelocity(wheel.position));
              float suspensionForce = (springStrength * offset) + (-pointVelocity * springDamping);
              rigidbody.AddForceAtPosition(wheel.up * suspensionForce, wheel.position);

              wheel.GetChild(0).transform.localPosition = Vector3.up * offset;
          }
      }

      grounded = tempGrounded;
  }
```

### Longitudinal Force

The longitudinal force is calculated by determining whether or not the player is trying to move forward / backward. If so, then friction is ignored and instead we apply force porportional to the player's input axis and the ratio of remaining speed. This ratio is defined by how fast the car is currently moving in its forwad direction. If the car is already moving at max speed, then no force will be applied.

When the player is not applying input, the force applied will be proportional to the current velocity of the car multiplied by the longitudinal friction coefficient.

```cs
private void ApplyLongitudinalForce()
{
    Vector3 force = Vector3.zero;
    float forwardVelocity = Vector3.Dot(transform.forward, rigidbody.velocity);
    float maxSpeedRatio = (1 - (Mathf.Abs(forwardVelocity) / maxSpeed));

    if (Mathf.Abs(driveAxis) > 0){
       force = transform.forward * driveAxis * maxSpeed * maxSpeedRatio;
    }
    else{
        force = transform.forward * -forwardVelocity * longitudinalFriction;
    }

    rigidbody.AddForce(force);
}
```

### Lateral Force

The lateral force is calculated similarly to the longitudinal force except without user input, as such oppositional force is constantly applied.

```cs
private void ApplyLateralForce()
{
    float rightVelocity = Vector3.Dot(transform.right, rigidbody.velocity);
    rigidbody.AddForce(transform.right * -rightVelocity * lateralFriction);
}
```

### Turning Force

The final force to apply is the turning force (torque) which is proportional to the car's current forward velocity. In a more accurate physics simulation the torque would be calculated by applying forces at the position of each wheel but since I'm aiming for a more arcadey feel I decided to opt for a simpler approximation.

```cs
private void ApplyTurningForce()
{
    float forwardVelocity = Vector3.Dot(transform.forward, rigidbody.velocity);
    float rotationalVelocity = Vector3.Dot(transform.up, rigidbody.angularVelocity);

    float torque = forwardVelocity * turnAxis * (Mathf.Deg2Rad * steeringAngle);
    torque += -rotationalVelocity * turnDamping;

    rigidbody.AddTorque(transform.up * torque);
}
```

## Player Abilities

![](https://github.com/torbenwb/MC_Rocket_Kart_Racing/blob/main/ReadMe_Images/Chapter_2.gif)

After implementing the car controller, it was time to move on to the Rocket League player abilities. To avoid making changes to the `Car` script, these abilities are implemented in the `AirControl` and `RocketBoost` scripts.

I reverse engineered these mechanics as follows:

### Jump / Air Jump

In Rocket League, the player has access to a jump when on the ground as well as a single, less powerful jump while in the air. This air jump resets when the player touches the ground. Jump force is applied as an impulse force in the car's upwards direction so if the car is rotated in any way that will affect the direction in which force is applied.

```cs
private void ApplyJumpForce(){
    if (car.GetGrounded()){
        yawRotation = pitchRotation = rollRotation = 0f;
        rigidbody.AddForce(transform.up * jumpStrength, ForceMode.Impulse);
        jumpFX.Play();
    }
    else{
        if (airJumps > 0){
            rigidbody.AddForce(transform.up * jumpStrength * airMod, ForceMode.Impulse);
            jumpFX.Play();
            airJumps--;
        }
    }
}
```

### Rotation Control

To control the rotation of the car in the air, we have to determine how quickly the car is already rotating in each direction. If the player is attempting to rotate further in that direction, torque is applied proportional the corresponding rotation rate, if the player is not attempting to rotate in that direction then oppositional damping force is applied instead.

```cs
private void ApplyAirRotationForce(){
    float pitchVelocity = Vector3.Dot(transform.right, rigidbody.angularVelocity);
    float yawVelocity = Vector3.Dot(transform.up, rigidbody.angularVelocity);
    float rollVelocity = Vector3.Dot(transform.forward, rigidbody.angularVelocity);

    float yawTorque = (Mathf.Abs(yawAxis) > 0f) ? yawAxis * yawRate : -yawVelocity * rotationDamping;
    float rollTorque = (Mathf.Abs(rollAxis) > 0f) ? rollAxis * rollRate : -rollVelocity * rotationDamping;
    float pitchTorque = (Mathf.Abs(pitchAxis) > 0f) ? pitchAxis * pitchRate : -pitchVelocity * rotationDamping;

    rigidbody.AddTorque(transform.up * yawTorque);
    rigidbody.AddTorque(transform.forward * rollTorque);
    rigidbody.AddTorque(transform.right * pitchTorque);


    yawRotation = (Mathf.Abs(yawAxis) > 0f) ? yawAxis * yawRate : 0f;
    rollRotation = (Mathf.Abs(rollAxis) > 0f) ? rollAxis * rollRate : 0f;
    pitchRotation = (Mathf.Abs(pitchAxis) > 0f) ? pitchAxis * pitchRate : 0f;
}
```

## Time Trials 

![](https://github.com/torbenwb/MC_Rocket_Kart_Racing/blob/main/ReadMe_Images/Chapter_3.gif)

## Polish and UI

![](https://github.com/torbenwb/MC_Rocket_Kart_Racing/blob/main/ReadMe_Images/Chapter_4.gif)
