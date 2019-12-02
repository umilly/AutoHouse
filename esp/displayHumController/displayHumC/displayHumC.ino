//libs
#include <Wire.h> 
#include "LiquidCrystal_I2C.h"
#include "SI7021.h"
#include "Encoder.h"
#include "Adafruit_MCP23017.h"
#include "Rotary.h"
#include "RotaryEncOverMCP.h"

//Global
SI7021 sensor;
LiquidCrystal_I2C lcd(0x27,16,2);
unsigned long _HTU21_StartTime = 0UL;
float curHum=0,curTemp=0;
float setHum=40;

Adafruit_MCP23017 mcp;

//Array of pointers of all MCPs if there is more than one
Adafruit_MCP23017* allMCPs[] = { &mcp };
constexpr int numMCPs = (int)(sizeof(allMCPs) / sizeof(*allMCPs));

/* function prototypes */
void RotaryEncoderChanged(bool clockwise, int id);

/* Array of all rotary encoders and their pins */
RotaryEncOverMCP rotaryEncoders[] = {
        // outputA,B on GPA7,GPA6, register with callback and ID=1
        RotaryEncOverMCP(&mcp, 1, 0, &RotaryEncoderChanged, 1)
};
constexpr int numEncoders = (int)(sizeof(rotaryEncoders) / sizeof(*rotaryEncoders));
int count=0;

void PringLCD()
{
  //lcd.home();
  lcd.setCursor(0,0);   
  lcd.print(String(count++)+ ":vlaznost'="+ String(curHum));         
  lcd.setCursor(0,1);   
  lcd.print("zadano="+ String(setHum));  
}
void RotaryEncoderChanged(bool clockwise, int id) {
    Serial.println("Encoder " + String(id) + ": "
            + (clockwise ? String("clockwise") : String("counter-clock-wise")));
    if(clockwise)
    {
      setHum+=0.5;
    }
    else
    {
      setHum-=0.5;
    }                
}

void pollAll() {
    //We could also call ".poll()" on each encoder,
    //however that will cause an I2C transfer
    //for every encoder.
    //It's faster to only go through each MCP object,
    //read it, and then feed it into the encoder as input.
    for(int j = 0; j < numMCPs; j++) {
        uint16_t gpioAB = allMCPs[j]->readGPIOAB();
        for (int i=0; i < numEncoders; i++) {
            //only feed this in the encoder if this
            //is coming from the correct MCP
            if(rotaryEncoders[i].getMCP() == allMCPs[j])
                rotaryEncoders[i].feedInput(gpioAB);
        }
    }
}

void setup() {
  // put your setup code here, to run once:
  Wire.pins(4, 5);
  sensor.begin(SDA,SCL);
  lcd.init();                
  lcd.backlight();
  lcd.print("Roma mudak"); 
  delay(1000);
  //lcd.clear(); 
  
  delay(10);
   mcp.begin();      // use default address 0

    //Initialize input encoders (pin mode, interrupt)
    for(int i=0; i < numEncoders; i++) {
        rotaryEncoders[i].init();
    }
}

void loop() {
  delay(100);
  pollAll();
  if( (_HTU21_StartTime ==0) || ( _isTimer(_HTU21_StartTime, 5000 ))) 
  {
    _HTU21_StartTime = millis();   
    gettemperature();
  }    
  //PringLCD();
}


void gettemperature() 
{
  curHum = sensor.getHumidityPercent();
  curTemp = sensor.getCelsiusHundredths();
  curTemp = curTemp / 100;
}



bool _isTimer(unsigned long startTime, unsigned long period )
{
  unsigned long currentTime;
  currentTime = millis();
  if (currentTime>= startTime) {return (currentTime>=(startTime + period));} else {return (currentTime >=(4294967295-startTime+period));}
}
