import random
import sys
import time

# Using the Python Device SDK for IoT Hub:
#   https://github.com/Azure/azure-iot-sdk-python
# The sample connects to a device-specific MQTT endpoint on your IoT Hub.
from azure.iot.device import IoTHubDeviceClient, Message

# The device connection string to authenticate the device with your IoT hub.
# Using the Azure CLI:
# az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyNodeDevice --output table
CONNECTION_STRING = "{device-connection-string}"

# Define the JSON message to send to IoT Hub.
MSG_TXT = '{{"action": "{action}"}}'

def iothub_client_init():
    # Create an IoT Hub client
    client = IoTHubDeviceClient.create_from_connection_string(CONNECTION_STRING)
    return client

def iothub_client_telemetry_sample_run():

    try:
        client = iothub_client_init()

        print ( "IoT Hub device sending message" )

        # Build the message.
        action = sys.argv[1]
        msg_txt_formatted = MSG_TXT.format(action=action)
        message = Message(msg_txt_formatted)

        # Send the message.
        print( "Sending message: {}".format(msg_txt_formatted) )
        client.send_message(message)
        print ( "Message successfully sent" )

    except KeyboardInterrupt:
        print ( "IoTHubClient sample stopped" )

if __name__ == '__main__':
    print ( "IoT Hub - Simulated device" )
    iothub_client_telemetry_sample_run()