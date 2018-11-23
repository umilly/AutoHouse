#include <Wire.h>
#include "Adafruit_MCP23008.h"
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
Adafruit_MCP23008 mcp;
const char* ssid = "AI_Zabava";
const char* password = "9052203377";
IPAddress ipServidor(192, 168, 1, 1);        //Адрес шлюза (в моем случае роутера)
IPAddress ipCliente(192, 168, 1, 151);       //Адрес нашего устройства
IPAddress Subnet(255, 255, 255, 0);          //Подсеть
ESP8266WebServer server(80);
String webString=""; 

//SENSOR SETTINGS
const int SensorCount=3;                      //количество кнопок
String SensorNames[SensorCount]={"27","28","29"};  //номер параметра
int SensorPins[SensorCount]={0,1,2};            //пины на контроллере
int SensorValues[SensorCount]={1,1,1};          //значения
int SensorTimers[SensorCount]{0,0,0};
int SensorCoolDown=300;

//RELE SETTINGS
const int ReleCount=4;
String devices[ReleCount]={"_rele_clim_d_","_rele_clim_d_","_rele_clim_d_","_rele_clim_d_"};
int pins[ReleCount]={3,4,5,6};
int values[ReleCount]={0,0,0,0};
//END RELE SETTINGS
const char* host = "192.168.1.100";           //адрес сервера

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
  server.send(200, "text/plain", webString);
  delay(100);
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
  int currentTime=millis();
  bool needSend=false;
  for(int i=0;i<SensorCount;i++)
  {
     if(currentTime-SensorTimers[i]<SensorCoolDown)
        continue;
     if(CheckSensorChanged(i));
     {
        SensorTimers[i]=currentTime;
        needSend=true;
     }
  }
  if(needSend){
          String url = "/SetSensorsValues?"; //команда
  url += "ip=192.168.1.151";// айпи контроллера
  //дальше перечисление всех параметров и их значений. В нормальной прошивке я писал все параметры и их имена в массивах, 
  //если бы тут было также - я бы написал for для всех значений, а так пиши в ручуную каждый, как в примере ниже.
  for(int i=0;i<SensorCount;i++)
  {
    url += "&"+SensorNames[i]+"="+ SensorValues[i];
  }
  for(int i=0;i<ReleCount;i++)
  {
    
  }
  url += "&ParamId="; // имя датчика1 прописанное в конфигурации  (ParamId)
  url += ParamId;     // значение показания датчика
  url += "&ParamId2="; // имя датчика2 прописанное в конфигурации  (ParamId2)
  url += ParamId2;   // значение показания датчика 

    client.print(String("GET ") + url + " HTTP/1.1\r\n" +
    "Host: " + host+":"+httpPort + "\r\n" + 
    "Connection: close\r\n\r\n");  
    
    }
}
bool CheckSensorChanged(int i)
{
  int pin = SensorPins[i];
  String sensorName=SensorNames[i];
  //int sensorValue=SensorValues[i];
  bool isHigh=mcp.digitalRead (pin) == HIGH;
  SensorValues[i]= isHigh?1:0;
  if (isHigh) 
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
    return true;
  }
  return false;
}
void loop(void)
{  
  CheckSensors();
  
  for(int i=0;i<ReleCount;i++)
  {
    mcp.digitalWrite(pins[i],values[i]);
  }
  
  server.handleClient();
} 
