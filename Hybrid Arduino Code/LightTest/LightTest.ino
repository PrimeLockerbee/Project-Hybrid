#define lightPin A0

float waterValue;

void setup() {
  Serial.begin(9600);
  pinMode(lightPin, OUTPUT);
}

void loop() {
  // Water
  waterValue = analogRead(lightPin);

  Serial.println(waterValue);
}
