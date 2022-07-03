#include <Wire.h>
#include "Adafruit_MCP23X17.h"
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
Adafruit_MCP23X17 mcp;
const char* ssid = "AI_Zabava";
const char* password = "9052203377";
IPAddress ipServidor(192, 168, 1, 1); //Адрес шлюза (в моем случае роутера)
IPAddress ipCliente(192, 168, 1, 111); //Адрес нашего устройства
IPAddress Subnet(255, 255, 255, 0); //Подсеть
ESP8266WebServer server(80);
String webString="OK"; 
const char* host = "192.168.1.100";
const char* contrip = "192.168.1.111";
unsigned long heartbeat = 0UL;

//RELE SETTINGS
const int ReleCount=12;
String devices[ReleCount]={"ch1","ch2","ch3","ch4","ch5","ch6","ch7","ch8","ch9","ch10","ch11","ch12"};
int pins[ReleCount]={0,1,2,3,4,5,8,9,10,11,12,13};
int values[ReleCount]={0,0,0,0,0,0,0,0,0,0,0,0};

//END RELE SETTINGS


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

  for(int i=0;i<ReleCount;i++)
  {
    mcp.digitalWrite(pins[i],values[i]);
  }
  
  server.send(200, "text/plain", webString);
  //delay(100);
  if(needSend)
  {     
    sendSensorsInfo();
  }
} 
void sendSensorsInfo()
{
    WiFiClient client;
    const int httpPort = 5555;
    if(!client.connected())
    {
      Serial.println(". starting new connection. ");
      
    if (!client.connect(host, httpPort)) 
    {
      Serial.println("connection failed");
      return;
    }
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
  mcp.begin_I2C();
  
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
  
  server.on("/switchRele", switchRele); 
  server.begin();
  Serial.println("HTTP server started");
}


 
void loop(void)
{
  if(_isTimer(heartbeat, 40000)) 
  {
   heartbeat = millis(); 
   sendSensorsInfo();
  }
  
 // for(int i=0;i<ReleCount;i++)
 // {
 //   mcp.digitalWrite(pins[i],values[i]);
 // }
  
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
 
