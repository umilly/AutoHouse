#include <Wire.h>
#include "Adafruit_MCP23008.h"
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
Adafruit_MCP23008 mcp;
const char* ssid = "AI_Zabava";
const char* password = "9052203377";
IPAddress ipServidor(192, 168, 1, 1); //Адрес шлюза (в моем случае роутера)
IPAddress ipCliente(192, 168, 1, 140); //Адрес нашего устройства
IPAddress Subnet(255, 255, 255, 0); //Подсеть
ESP8266WebServer server(80);
String webString=""; 
const char* host = "192.168.1.100";

//RELE SETTINGS
const int ReleCount=6;
String devices[ReleCount]={"_rele_clim_d_","_rele_clim_d_","_rele_clim_d_","_rele_clim_d_","_rele_clim_d_","_rele_clim_d_"};
int pins[ReleCount]={0,1,2,3,4,5};
int values[ReleCount]={0,0,0,0,0,0};
int heartBeat=20000;
long ms=0;
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
    url += "ip=192.168.1.140";// айпи контроллера    
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

void CheckHearBeat()
{
  bool needSend=false;
  long curMs=millis();
  if(curMs<ms)
  {
    ms=curMs;
    needSend=true;
  }
  if(curMs<ms+heartBeat)
  {
    needSend=true;
  } 
  server.send(200, "text/plain", webString);
  delay(100);
  if(needSend)
  {     
    ms=millis();
    sendSensorsInfo();
  }
}
 
void loop(void)
{
  CheckHearBeat();
  for(int i=0;i<ReleCount;i++)
  {
    mcp.digitalWrite(pins[i],values[i]);
  }
  
  server.handleClient();
} 
 

