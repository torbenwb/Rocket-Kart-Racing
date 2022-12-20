# MC_Rocket_Kart_Racing

## Physics Driven Car

![](https://github.com/torbenwb/MC_Rocket_Kart_Racing/blob/main/ReadMe_Images/Chapter_1.gif)

The first step in building any racing / car based action game is the car. In this project the car is controlled by a very simple public interface which allows the player to `Drive()`, `Turn()` and `Brake()`, however internally the motion and rotation of the car is controlled through a series of forces:

* Suspension Force: Calculates an upwards spring force based on how close the car is to the ground.
* Longitudinal Force: Forward / backwards force based on a combination of user input to drive forward and backwards and a friction coefficient to stop / slow the car's movement when on the ground.
* Lateral Force: Right/Left force provides force oppositional to lateral velocity based on friction coefficient.
* Turning Force: Converts user input and forward velocity into torque to turn the car at a certain rate while driving forward / backward.
