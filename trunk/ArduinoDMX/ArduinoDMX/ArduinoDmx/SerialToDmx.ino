//Handles serial messages
boolean handleSerial(int * message) {
  for (int i = 0; i < 3; i++) message[i] = 0; //Reset message array
  
  if (Serial.available() == 0) return false;

  //Instruction
  byte data = Serial.read();
  switch(data) {
  //53 = S (SET)
  case 53: 
    message[0] = 2;
    break;
  default:
    clearSerial();
    return false;
  }

  //Channel
  byte ch1 = Serial.read();
  byte ch2 = Serial.read();
  int channel = ch1 + (ch2 << 8);
  //Fixture
  byte fixture = Serial.read();
  
  //Stop
  if (Serial.read() != 23) {
    Serial.print("ERR");
    Serial.println();
    clearSerial();
    return false;
  }

  message[1] = channel;
  message[2] = fixture;
  
  clearSerial();
  return true;
}

//Initialize serial and clear initial buffer
void initializeSerial(int baudrate) {
  Serial.begin(baudrate);
  clearSerial();
}

//Clears buffer
void clearSerial() {
  while (Serial.available() > 0) Serial.read(); 
}



