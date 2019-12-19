# Challenge 19: IoT Hub and Durable Entities

![Challenge 19: IoT Hub and Durable Entities](https://res.cloudinary.com/jen-looper/image/upload/v1576003473/images/challenge-19_lhfnyf.jpg)

## Solution

Resources used:
* Azure Functions
* IoT Hub
* Logic App

![Design](screenshots/design.png)

The simulated device sends a message to the IoT Hub, which then via an Event Subscription, triggers a Logic App. Then depending on the action, the start of stop action is triggered via the Azure Function.

The following commands are used for the simulator:
* `python SimulatedAirCompressor.py start`
* `python SimulatedAirCompressor.py stop`

![Event Subscription](screenshots/event-subscription.png)
![Logic App](screenshots/logic-app.png)
![Query](screenshots/query-api.png)

## The Challenge

Let's head over to the South of France where Julie and her colleagues have decided to prepare a special gift for all patients where she is working: custom-shaped inflatable balloons for the holidays! In order to make them, she'll need to inflate a LOT of balloons with the help of an air compressor and one 12 litres air tank.

Julie has some IoT devices that tell her when the air compressor is starting and stopping to fill the tanks. Can we write her a service that will compute the amount of air available and let her know if she has enough air for her balloons?

Let's assume that our compressor is filling a 12 litres air tank, at the rate of 25 bar (around 360psi) per minute. The max pressure of the tank is 200 bar. You'll need to deduce the actual pressure of the tank and the number of balloons Julie can inflate at any point in time, knowing each balloon consumes 0,6 bar of the air tank pressure.

A great way to model this would be to use the Azure IoT Hub. You can create some Azure Functions Durable Entities to model a "tank" or "compressor" entity, as well as another Azure Function

If you like physics: we have assumed the compressor is working at a rate of 18 m3/hour, and that a balloon is 4 litres inflated at 2 bar. With Boyle-Mariotte law (or Van der Walls equations), you should be able to do more precise calculations.
