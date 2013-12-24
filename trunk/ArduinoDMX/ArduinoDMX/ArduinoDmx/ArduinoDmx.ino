#include <DmxMaster.h>

int dmxMessage [3] = {-1, 0, 0};

void setup() {
  initializeSerial(9600);
  pinMode(3, OUTPUT);
}

void loop() {
  if (Serial.available() < 5) return;
  if (!handleSerial(dmxMessage)) return;
  if (dmxMessage[1] == 20) {
    digitalWrite(3, HIGH);
  } else {
    digitalWrite(3, LOW); 
  }
}
