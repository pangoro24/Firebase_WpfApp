import os
from time import sleep
from datetime import datetime
import json
import RPi.GPIO as GPIO
import Adafruit_DHT
from functools import partial
from firebase import firebase

firebase = firebase.FirebaseApplication('https://rpi-dht-fb.firebaseio.com/', None)
#result = firebase.get('/dht', None)
#print (result)

GPIO.setmode(GPIO.BCM)
GPIO.cleanup()
GPIO.setwarnings(False)

def update_firebase():
    sensor = Adafruit_DHT.DHT22
    pin = 4
    humidity, temperature = Adafruit_DHT.read_retry(sensor, pin)
    if humidity is not None and temperature is not None:
        print('Temp={0:0.2f}*C  Humidity={1:0.2f}%'.format(temperature, humidity))
        ts = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        data = {"temperature": temperature, "humidity": humidity, "timestamp": ts}
        firebase.post('/sensors/dht', data)
    else:
        print('Failed to get reading. Try again!')
        sleep(10)
    

while True:
    update_firebase()
    sleep(10) #sleep for 10 seconds and push again
