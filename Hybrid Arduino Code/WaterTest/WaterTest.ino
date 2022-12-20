#define waterPin A0

float waterValue;

void setup() {
  Serial.begin(9600);
  pinMode(waterPin, OUTPUT);
}

void loop() {
  // Water
  waterValue = analogRead(waterPin);

  Serial.println(waterValue);
}
