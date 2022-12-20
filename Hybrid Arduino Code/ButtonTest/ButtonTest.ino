#define button 4

float waterValue;

void setup() {
  Serial.begin(9600);
}

void loop() {
  // Water
  waterValue = digitalRead(button);

  Serial.println(waterValue);
}
