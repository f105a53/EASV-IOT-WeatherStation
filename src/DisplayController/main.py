from RPLCD.i2c import CharLCD
import time
lcd = CharLCD('PCF8574', 0x27)

lcd.clear()
lcd.write_string('Kitchen')
lcd.cursor_pos = (1,0)
lcd.write_string('-15C 1105hPa 50%')
time.sleep(5)

lcd.clear()
lcd.write_string('Bedroom')
lcd.cursor_pos = (1,0)
lcd.write_string('-19C 1100hPa 90%')

time.sleep(1)