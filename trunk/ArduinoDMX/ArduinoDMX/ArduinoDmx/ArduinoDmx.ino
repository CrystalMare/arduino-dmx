#include <DmxSimple.h>

int dmxMessage [3] = { 0, 0, 0};

const unsigned int MAX_CHANNEL = 7;

void setup() {
  initializeSerial(9600);
  DmxSimple.maxChannel(MAX_CHANNEL);
}

void loop() {
  if (Serial.available() < 5) return;
  if (!handleSerial(dmxMessage)) return;

  switch (dmxMessage[0]) {
  case 2: //SET
    DmxSimple.write(dmxMessage[1], dmxMessage[2]);
    break;
  case 3: //CLEAR
    for (int i = 1; i <= MAX_CHANNEL; i++) {
      DmxSimple.write(i, 0);
    }
  }
}


