

#include <ESP8266WiFi.h>
#include <Wire.h>
#include <SI7021.h>
SI7021 sensor;
unsigned long _HTU21_StartTime = 0UL;
float humd, temp;
const char* ssid = "AI_Zabava";
const char* password = "9052203377";
IPAddress ipServidor(192, 168, 1, 1); //Адрес шлюза (в моем случае роутера)
IPAddress ipCliente(192, 168, 1, 120); //Адрес нашего устройства
IPAddress Subnet(255, 255, 255, 0); //Подсеть
const char* host = "192.168.1.100";
const char* ParamId = "60";
const char* ParamId2 = "61";


void setup() {
  Serial.begin(115200);
  Wire.pins(4, 5);
  sensor.begin(SDA,SCL);
  delay(10);
  
  // Connect to WiFi network
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  WiFi.config(ipCliente, ipServidor, Subnet);
  Serial.print("\n\r \n\rWorking to connect");
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println(WiFi.localIP());
}


void loop() 
{
 
  delay(100);
  
  if( (_HTU21_StartTime ==0) || ( _isTimer(_HTU21_StartTime, 30000 ))) 
  {
  _HTU21_StartTime = millis(); 
  
  gettemperature();

  WiFiClient client;
  const int httpPort = 5555;
  if (!client.connect(host, httpPort)) 
  {
    Serial.println("connection failed");
    return;
  }
  
  String url = "/SetSensorsValues?"; //команда
  url += "ip=192.168.1.120";// айпи контроллера
  //дальше перечисление всех параметров и их значений. В нормальной прошивке я писал все параметры и их имена в массивах, 
  //если бы тут было также - я бы написал for для всех значений, а так пиши в ручуную каждый, как в примере ниже.
  url += "&ParamId="; // имя датчика1 прописанное в конфигурации  (ParamId)
  url += ParamId;     // значение показания датчика
  url += "&ParamId2="; // имя датчика2 прописанное в конфигурации  (ParamId2)
  url += ParamId2;   // значение показания датчика 
  
  client.print(String("GET ") + url + " HTTP/1.1\r\n" +
               "Host: " + host+":"+httpPort + "\r\n" + 
               "Connection: close\r\n\r\n");

  }

}

void gettemperature() 
  {
  humd = sensor.getHumidityPercent();
  temp = sensor.getCelsiusHundredths();
  temp = temp / 100;
  }



bool _isTimer(unsigned long startTime, unsigned long period )
  {
  unsigned long currentTime;
currentTime = millis();
if (currentTime>= startTime) {return (currentTime>=(startTime + period));} else {return (currentTime >=(4294967295-startTime+period));}
  }




 








  
  
