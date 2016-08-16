# ARModbus
Implementation in C# of a subset of the TCP/IP Modbus Protocol.

## Supported functions
* Read Coils
* Read Holding Registers
* Write Multiple Coils
* Write Multiple Registers

## Modbus Client
Add a reference to ModbusClient.dll

**Example**
```
ModbusTcpClient client = new ModbusTcpClient();
client.Connect("localhost", /*port*/ 502);
ushort[] values = client.ReadHoldingRegisters( 1 /*unit identifier*/, 100 /*start address*/, 10 /*quantity*/ );
```

Enclose with try/catch ReadHoldingRegisters call to capture a possible IOException.

## Modbus Server
Add a reference to ModbusServer.dll

Modbus Server decodes a stream with a request from a Modbus Client and makes a response ready to send. The management of the connections
is out of its scope.

**Example**
```
// byte[] stream : has data read from a connection.

ModbusTcpMessage message = ModbusTcpMessage.Build(stream, stream.Length);

if( message.Command == ModbusMessage.ModbusCommand.ReadHoldingRegisters )
{
  //message.ProtocolDataUnit.StartAddress has start address
  //message.ProtocolDataUnit.Quantity has quantity of holding registers to read
  
  //ushort[message.ProtocolDataUnit.Quantity] values = Read data from some place
  
  message.ProtocolDataUnit.SetReadWordsData( values );
  
  //message.ToStream() has stream response to send
}
```
