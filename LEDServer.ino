#include <ESP8266WiFi.h>
#include <aREST.h>

// Creates aREST instance
aREST rest = aREST();

// WiFi params
const char* ssid = "MIT";
const char* password = "";

// The port to listen for incoming TCP conections
#define LISTEN_PORT 80

// Create an instance of the server
WiFiServer server(LISTEN_PORT);

void setup() {
  Serial.begin(115200);
  delay(10);

  // Sets up restful API stuff

  //rest.set_id("123456");
  //rest.set_name("esp8266");

  // Sets the API routes

  // LEDs on/off
  rest.function("led/on", ledOn);
  rest.function("led/off", ledOff);

  // Connect to WiFi network
  Serial.println();
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  // Connect to WiFi
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi connected");

  // Start the server
  server.begin();
  Serial.println("Server startered");

  // Print IP Address
  Serial.print("Communicate with server at: ");
  Serial.print(WiFi.localIP());
  Serial.println("/");

}

void loop() {
  // Handle REST calls
  WiFiClient client = server.available();
  
  if (!client) {
    return;
  }
  while(!client.available()){
    delay(1);
  }
  rest.handle(client);
}

// Turn on LED
int ledOn(String command){

  // TALK TO LED
  Serial.println("turning on");
  return 1;
}

// Turn off LED
int ledOff(String command){

  // TALK TO LED
  Serial.println("turning off");
  return 1;
}


