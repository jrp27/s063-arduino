#include <ESP8266WiFi.h>
#include <ESP8266WiFiAP.h>
#include <ESP8266WiFiGeneric.h>
#include <ESP8266WiFiMulti.h>
#include <ESP8266WiFiScan.h>
#include <ESP8266WiFiSTA.h>
#include <ESP8266WiFiType.h>
#include <WiFiClient.h>
#include <WiFiClientSecure.h>
#include <WiFiServer.h>
#include <WiFiUdp.h>

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
    delay(50);
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

  // Initialize the D5,D6,D7 pins as an output 
  pinMode(D5, OUTPUT);    
  pinMode(D6, OUTPUT);
  pinMode(D7, OUTPUT);

}

bool flag_standing = false;
bool flag_sitting = false;
bool flag_typing = false;
bool flag_writing = false;
bool flag_usingPhone = false;
bool flag_napping = false;
int ledState = LOW;
unsigned long previousMillis = 0;
const long interval1 = 100;
const long interval2 = 200;
const long interval3 = 300;
const long interval4 = 400;
const long interval5 = 500;
const long interval6 = 600;
const long interval7 = 700;
const long interval8 = 800;
const long interval9 = 900;
const long interval10 = 1000;

void loop() {
  // Handle REST calls
  unsigned long currentMillis = millis(); 

 
 // Serial.print(flag_standing);
 // Serial.println("C: ");
 // Serial.println(currentMillis);
 // Serial.println("P: ");
 // Serial.println(previousMillis);

  if (flag_standing) {
      digitalWrite(D7, HIGH);
  }

  if (flag_sitting) {
    if (currentMillis - previousMillis >= 1500) {
      previousMillis = currentMillis;

      if (ledState == LOW) {
        ledState = HIGH;
      }
      else{
        ledState = LOW;
      }
      digitalWrite(D7, ledState);
    }  
  }

  if (flag_typing) {
    if (currentMillis - previousMillis >= 20) {
      previousMillis = currentMillis;

      if (ledState == LOW) {
        ledState = HIGH;
      }
      else{
        ledState = LOW;
      }
      digitalWrite(D7, ledState);
    }  
  }

  if (flag_writing) {
    if (currentMillis - previousMillis >= 20) {
      previousMillis = currentMillis;

      if (ledState == LOW) {
        ledState = HIGH;
      }
      else{
        ledState = LOW;
      }
      digitalWrite(D7, ledState);
    }  
 
  }
  
  if (flag_usingPhone) {
    if (currentMillis - previousMillis >= 100) {
      previousMillis = currentMillis;

      if (ledState == LOW) {
        ledState = HIGH;
      }
      else{
        ledState = LOW;
      }
      digitalWrite(D7, ledState);
    }   
 
  }

  if (flag_napping) {
    digitalWrite(D7, LOW);  
 
  }

  WiFiClient client = server.available();
  
  if(!client){
   return;
  }

  while(!client.available()){
    delay(1);
  }

  rest.handle(client);
  



}

// Standing
int standing(String command){
  Serial.println("standing");
  // ADD STANDING CODE HERE
  
  flag_standing = true;
  flag_sitting = false;
  flag_typing = false;
  flag_writing = false;
  flag_usingPhone = false;
  flag_napping = false;
  
  return 1;
}

// Sitting
int sitting(String command){
  Serial.println("sitting");
  // ADD SITTING CODE HERE

  flag_standing = false;
  flag_sitting = true;
  flag_typing = false;
  flag_writing = false;
  flag_usingPhone = false;
  flag_napping = false;
  
  return 1;
}

// Typing
int typing(String command) {
  Serial.println("typing");
  // ADD TYPING CODE HERE

  flag_standing = false;
  flag_sitting = false;
  flag_typing = true;
  flag_writing = false;
  flag_usingPhone = false;
  flag_napping = false;
  
  return 1;
    
}

// Writing
int writing(String command) {
  Serial.println("writing");
  

  flag_standing = false;
  flag_sitting = false;
  flag_typing = false;
  flag_writing = true;
  flag_usingPhone = false;
  flag_napping = false;
  
  return 1;
}

// Using Phone
int usingPhone(String command) {
  Serial.println("using phone");
  // ADD USING PHONE CODE HERE
  
  flag_standing = false;
  flag_sitting = false;
  flag_typing = false;
  flag_writing = false;
  flag_usingPhone = true;
  flag_napping = false; 
  
  return 1;
}


// Napping
int napping(String command) {
  Serial.println("napping");
  
  flag_standing = false;
  flag_sitting = false;
  flag_typing = false;
  flag_writing = false;
  flag_usingPhone = false;
  flag_napping = true; 

  return 1;
}


