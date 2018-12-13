#include <Wire.h>
#include "Adafruit_MCP23008.h"
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
Adafruit_MCP23008 mcp;
const char* ssid = "AI_Zabava";
const char* password = "9052203377";
IPAddress ipServidor(192, 168, 1, 1);        //Адрес шлюза (в моем случае роутера)
IPAddress ipCliente(192, 168, 1, 154);       //Адрес нашего устройства
IPAddress Subnet(255, 255, 255, 0);          //Подсеть
ESP8266WebServer server(80);
String webString=""; 
const char* host = "192.168.1.100";
const char* contrip = "192.168.1.154";
unsigned long heartbeat = 0UL;

//SENSOR SETTINGS
const int SensorCount=3;                      //количество кнопок
String SensorNames[SensorCount]={"36","35","34"};  //номер параметра
int SensorPins[SensorCount]={0,1,2};            //пины на контроллере
int SensorValues[SensorCount]={1,1,1};          //значения
unsigned long SensorTimers[SensorCount]={0,0,0};
int SensorCoolDown=3000;

//RELE SETTINGS
const int ReleCount=4;
String devices[ReleCount]={"ch1","ch2","ch3","ch4"};
int pins[ReleCount]={3,4,5,6};
int values[ReleCount]={0,0,0,0};
//END RELE SETTINGS

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
  bool needSend=false;
  for(int i=0;i<server.args();i++)
  {
    if(server.argName(i)=="id")
      rele=server.arg(i).toInt();
    if(server.argName(i)=="val0")
      value=server.arg(i).toInt();
  }
  if(rele>=0&&value>=0&&values[rele]!=value)
  {
    values[rele]=value;
    needSend=true;
  }
  server.send(200, "text/plain", webString);
  delay(100);
  if(needSend)
  {     
    sendSensorsInfo();
  }
} 

void sendSensorsInfo()
{
    WiFiClient client;
    const int httpPort = 5555;
    if (!client.connect(host, httpPort)) 
    {
      Serial.println("connection failed");
      return;
    }
    String url = "/SetSensorsValues?"; //команда
    url += "ip=";// айпи контроллера    
    url += contrip;
    for(int i=0;i<ReleCount;i++)
    {
        url +="&"+devices[i]+"="+values[i];
    }     
    client.print(String("GET ") + url + " HTTP/1.1\r\n" +
    "Host: " + host+":"+httpPort + "\r\n" + 
    "Connection: close\r\n\r\n");  

}
void setup(void)
{
  mcp.begin();
  for(int i=0;i<SensorCount;i++)
  {
    mcp.pinMode(SensorPins[i], INPUT);
  }
  for(int i=0;i<ReleCount;i++)
  {
    mcp.pinMode(pins[i], OUTPUT);
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
  Serial.println("Volko Server");
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
  for(int i=0;i<SensorCount;i++)
  {
     
     if(!_isTimer(SensorTimers[i],SensorCoolDown))
        continue;
     if(CheckSensorChanged(i));
     {
       Serial.print("sensor      : "+i);       
       Serial.print("current time: "+millis());  
       Serial.print("sensor  time: "+SensorTimers[i]);  
       SensorTimers[i]=millis();
     }
  }
}
bool CheckSensorChanged(int i)
{
  int pin = SensorPins[i];
  String sensorName=SensorNames[i];
  int sensorValue=SensorValues[i];
  if (mcp.digitalRead (pin) == HIGH) 
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
  if(_isTimer(heartbeat, 30000)) 
  {
   heartbeat = millis(); 
   sendSensorsInfo();
  }
  
  CheckSensors();
  
  for(int i=0;i<ReleCount;i++)
  {
    mcp.digitalWrite(pins[i],values[i]);
  }
  
  server.handleClient();
} 
bool _isTimer(unsigned long startTime, unsigned long period )
{
    unsigned long currentTime;
    currentTime = millis();
    if (currentTime>= startTime) 
    {
       return (currentTime>=(startTime + period));
    }
    else 
    {
       return (currentTime >=(4294967295-startTime+period));
    }
}