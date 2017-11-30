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

  // Sets the API routes

  // gestures
  rest.function("standing", standing);
  rest.function("sitting", sitting);
  rest.function("typing", typing);
  rest.function("writing", writing);
  rest.function("usingPhone", usingPhone);
  rest.function("napping", napping);

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

// Standing
int standing(String command){
  Serial.print("standing");
  // ADD STANDING CODE HERE
  return 1;
}

// Sitting
int sitting(String command){
  Serial.print("sitting");
  // ADD SITTING CODE HERE
  return 1;
}

// Typing
int typing(String command) {
  Serial.print("typing");
  // ADD TYPING CODE HERE
  return 1;
}

// Writing
int writing(String command) {
  Serial.print("writing");
  // ADD WRITING CODE HERE
  return 1;
}

// Using Phone
int usingPhone(String command) {
  Serial.print("using phone");
  // ADD USING PHONE CODE HERE
  return 1;
}

// Napping
int napping(String command) {
  Serial.print("napping");
  // ADD NAPPING CODE HERE
  return 1;
}



