from RPLCD.i2c import CharLCD
import time
lcd = CharLCD('PCF8574', 0x27)

import paho.mqtt.client as mqtt

lcd.clear()
# The callback for when the client receives a CONNACK response from the server.
def on_connect(client, userdata, flags, rc):
    print("Connected with result code "+str(rc))

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    client.subscribe("#")


# The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, msg):
    if msg.topic == "rtl_433/raspberrypi/devices/Nexus-TH/1/255/time":
        lcd.cursor_pos = (0,0)
        lcd.write_string(msg.payload)
    if msg.topic == "rtl_433/raspberrypi/devices/Nexus-TH/1/255/temperature_C":
        lcd.cursor_pos = (1,0)
        lcd.write_string(msg.payload)

client = mqtt.Client()
client.on_connect = on_connect
client.on_message = on_message

client.connect("localhost", 1883, 60)

# Blocking call that processes network traffic, dispatches callbacks and
# handles reconnecting.
# Other loop*() functions are available that give a threaded interface and a
# manual interface.
client.loop_forever()

# lcd.clear()
# lcd.write_string('Kitchen')
# lcd.cursor_pos = (1,0)
# lcd.write_string('-15C 1105hPa 50%')
# time.sleep(5)

# lcd.clear()
# lcd.write_string('Bedroom')
# lcd.cursor_pos = (1,0)
# lcd.write_string('-19C 1100hPa 90%')

# time.sleep(1)