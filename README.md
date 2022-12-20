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

## Player Abilities

![](https://github.com/torbenwb/MC_Rocket_Kart_Racing/blob/main/ReadMe_Images/Chapter_2.gif)

## Time Trials 

![](https://github.com/torbenwb/MC_Rocket_Kart_Racing/blob/main/ReadMe_Images/Chapter_3.gif)

## Polish and UI

![](https://github.com/torbenwb/MC_Rocket_Kart_Racing/blob/main/ReadMe_Images/Chapter_4.gif)
