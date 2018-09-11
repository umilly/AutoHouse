/*
  Serial Event example

  When new serial data arrives, this sketch adds it to a String.
  When a newline is received, the loop prints the string and clears it.

  A good test for this is to try it with a GPS receiver that sends out
  NMEA 0183 sentences.

  NOTE: The serialEvent() feature is not available on the Leonardo, Micro, or
  other ATmega32U4 based boards.

  created 9 May 2011
  by Tom Igoe

  This example code is in the public domain.

  http://www.arduino.cc/en/Tutorial/SerialEvent
*/

String inputString = "";         // a String to hold incoming data
boolean stringComplete = false;  // whether the string is complete
int loopMs=10000;
int range=100;
int lastVal=0;
void setup() {
  // initialize serial:
  Serial.begin(9600);
  pinMode(5, OUTPUT);
  // reserve 200 bytes for the inputString:
  inputString.reserve(200);
  
}

void loop() {
  int shift=127-(int)(range/2);
  int m=millis();
  m=abs(m);  
  int curPos= m%loopMs;
  int direction=(int)(m%(loopMs*2))/loopMs;    
  int val=(int)((range/(float)loopMs)*(float)curPos);
  /*
  Serial.println("val:"+String(val));  
  Serial.println("loopMs:"+String(loopMs));  
  Serial.println("curPos:"+String(curPos));  
  Serial.println("direction:"+String(direction));  
  Serial.println("____________________");
  */
  if(direction==1)
  {
    val=range-val;    
  }
  if(val!=lastVal)
  {
    lastVal=val;
    analogWrite(5,shift+val);
  }
  // print the string when a newline arrives:
  if (stringComplete) {
    Serial.println(inputString);
    int val=inputString.toInt();
    analogWrite(5,val);
    // clear the string:
    inputString = "";
    stringComplete = false;
  }
}

/*
  SerialEvent occurs whenever a new data comes in the hardware serial RX. This
  routine is run between each time loop() runs, so using delay inside loop can
  delay response. Multiple bytes of data may be available.
*/
void serialEvent() {
  while (Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read();
    // add it to the inputString:
    inputString += inChar;
    // if the incoming character is a newline, set a flag so the main loop can
    // do something about it:
    if (inChar == '\n') {
      stringComplete = true;
      
    }
  }
}
