#include <DmxSimple.h>


int dmxMessage [3] = {-1, 0, 0};

void setup() {
  initializeSerial(9600);
}

void loop() {
  if (Serial.available() < 5) return;
  if (!handleSerial(dmxMessage)) return;
  
  DmxSimple.write(dmxMessage[1], dmxMessage[2]);
  
}
