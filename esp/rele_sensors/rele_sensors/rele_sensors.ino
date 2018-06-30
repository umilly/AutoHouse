#include <Wire.h>
//#include "Adafruit_MCP23008.h"
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
//Adafruit_MCP23008 mcp;
const char* ssid = "AI_Zabava";
const char* password = "9052203377";
IPAddress ipServidor(192, 168, 1, 1); //Адрес шлюза (в моем случае роутера)
IPAddress ipCliente(192, 168, 1, 140); //Адрес нашего устройства
IPAddress Subnet(255, 255, 255, 0); //Подсеть
ESP8266WebServer server(80);
String webString=""; 

//SENSOR SETTINGS
const int SensorCount=2;
String SensorNames[SensorCount]={"l2","l3"};
int SensorPins[SensorCount]={0,1};
int SensorValues[SensorCount]={1,1};
int SensorTimers[SensorCount]{0,0};
int SensorCoolDown=300;

//RELE SETTINGS
const int ReleCount=3;
String devices[ReleCount]={"_rele_clim_d_","_rele_clim_d_","_rele_clim_d_"};
int pins[ReleCount]={2,3,4};
int values[ReleCount]={0,0,0};
//END RELE SETTINGS
const char* host = "192.168.1.100";

void handle_root() 
{
  webString="";
  for(int i=0; i<ReleCount;i++)
  { 
    webString+=String(i+1)+devices[i]+String(values[i])+"<br>";
  }
  server.send(200, "text/plain", webString);
  delay(100);
}
void switchRele()
{
  int rele=-1;
  int value=-1;
  for(int i=0;i<server.args();i++)
  {
    if(server.argName(i)=="id")
      rele=server.arg(i).toInt();
    if(server.argName(i)=="val0")
      value=server.arg(i).toInt();
  }
  if(rele>=0&&value>=0)
  {
    values[rele]=value;
  }
} 
void setup(void)
{
  //mcp.begin();
  for(int i=0;i<SensorCount;i++)
  {
    //mcp.pinMode(SensorPinss[i], INPUT);
  }
  for(int i=0;i<ReleCount;i++)
  {
    //mcp.pinMode(pins[i], OUTPUT);
  }
  
  Serial.begin(115200); 
  
  // Connect to WiFi network
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  WiFi.config(ipCliente, ipServidor, Subnet);
  Serial.print("\n\r \n\rWorking to connect");
  
  // Wait for connection
  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("DHT Weather Reading Server");
  Serial.print("Connected to ");
  Serial.println(ssid);
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
  
  server.on("/", handle_root); 
  server.on("/switchRele", switchRele); 
  server.begin();
  Serial.println("HTTP server started");
}

void CheckSensors()
{
  int currentTime=millis();
  for(int i=0;i<SensorCount;i++)
  {
     if(currentTime-SensorTimers[i]<SensorCoolDown)
        continue;
     if(CheckSensorChanged(i));
        SensorTimers[i]=currentTime;
  }
}
bool CheckSensorChanged(int i)
{
  int pin = SensorPins[i];
  String sensorName=SensorNames[i];
  int sensorValue=SensorValues[i];
  //if (mcp.digitalRead (pin) == HIGH) 
  {
    Serial.print("connecting to ");
    Serial.println(host);
    
    // Use WiFiClient class to create TCP connections
    WiFiClient client;
    const int httpPort = 5555;
    if (!client.connect(host, httpPort)) 
    {
      Serial.println("connection failed");
      return false;
    }
    String url = "/SetParam/";
    url += sensorName;
    url += "?";
    url += sensorValue;
    client.print(String("GET ") + url + " HTTP/1.1\r\n" +
    "Host: " + host+":"+httpPort + "\r\n" + 
    "Connection: close\r\n\r\n");  
    return true;
  }
  return false;
}
void loop(void)
{  
  CheckSensors();
  for(int i=0;i<ReleCount;i++)
  {
    //mcp.digitalWrite(pins[i],values[i]);
  }
  
  server.handleClient();
} 
