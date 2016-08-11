using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModbusServer.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModbusServer.Messages.Tests
{
    [TestClass()]
    public class ModbusTcpMessageTests
    {
        [TestMethod()]
        public void ModbusServerModbusTcpMessageBuildTest()
        {
            ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.ReadHoldingRegisters;
            ushort transactionId = 1;
            byte protocolId = 0;
            ushort length = 6;
            byte unitIdentifier = 45;
            ushort startAddress = 5000;
            ushort quantity = 2;
            
            byte[] stream = new byte[12] {  (byte)(transactionId>>8 & 0xFF),
                                            (byte)(transactionId & 0xFF),
                                            (byte)(protocolId>>8 & 0xFF),
                                            (byte)(protocolId & 0xFF),
                                            (byte)(length>>8 & 0xFF),
                                            (byte)(length & 0xFF),
                                            unitIdentifier, 
                                            (byte)modbusCommand,
                                            (byte)(startAddress>>8 & 0xFF),
                                            (byte)(startAddress & 0xFF),
                                            (byte)(quantity>>8 & 0xFF),
                                            (byte)(quantity & 0xFF)};

            ModbusTcpMessage message = ModbusTcpMessage.Build(stream, stream.Length);
        }

        [TestMethod()]
        public void ModbusServerModbusTcpMessageToStreamReadCoilsTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.ReadCoils;
            ushort transactionId = 1;
            byte protocolId = 0;
            ushort length = 6;
            byte unitIdentifier = 45;
            ushort startAddress = 5000;
            ushort quantity = 2;

            byte[] stream = new byte[12] {  (byte)(transactionId>>8 & 0xFF),
                                            (byte)(transactionId & 0xFF),
                                            (byte)(protocolId>>8 & 0xFF),
                                            (byte)(protocolId & 0xFF),
                                            (byte)(length>>8 & 0xFF),
                                            (byte)(length & 0xFF),
                                            unitIdentifier,
                                            (byte)modbusCommand,
                                            (byte)(startAddress>>8 & 0xFF),
                                            (byte)(startAddress & 0xFF),
                                            (byte)(quantity>>8 & 0xFF),
                                            (byte)(quantity & 0xFF)};

            ModbusTcpMessage message = ModbusTcpMessage.Build(stream, stream.Length);
            message.ProtocolDataUnit.SetReadBitsData( new bool[2] { true, false } );

            byte[] expectedMessage = new byte[10] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x2D, 0x01, 0x01,
                    0x01 };

            Assert.AreEqual(true, message.ToStream().SequenceEqual(expectedMessage));
        }

        [TestMethod()]
        public void ModbusServerModbusTcpMessageToStreamReadHoldingRegistersTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.ReadHoldingRegisters;
            ushort transactionId = 1;
            byte protocolId = 0;
            ushort length = 6;
            byte unitIdentifier = 45;
            ushort startAddress = 5000;
            ushort quantity = 2;

            byte[] stream = new byte[12] {  (byte)(transactionId>>8 & 0xFF),
                                            (byte)(transactionId & 0xFF),
                                            (byte)(protocolId>>8 & 0xFF),
                                            (byte)(protocolId & 0xFF),
                                            (byte)(length>>8 & 0xFF),
                                            (byte)(length & 0xFF),
                                            unitIdentifier,
                                            (byte)modbusCommand,
                                            (byte)(startAddress>>8 & 0xFF),
                                            (byte)(startAddress & 0xFF),
                                            (byte)(quantity>>8 & 0xFF),
                                            (byte)(quantity & 0xFF)};

            ModbusTcpMessage message = ModbusTcpMessage.Build(stream, stream.Length);
            message.ProtocolDataUnit.SetReadWordsData( new ushort[2] { 1200, 1305 } );


            byte[] expectedMessage = new byte[13] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x07, 0x2D, 0x03, 0x04,
                    0x04, 0xB0, 0x05, 0x19 };

            Assert.AreEqual(true, message.ToStream().SequenceEqual(expectedMessage));

        }

        [TestMethod()]
        public void ModbusServerModbusTcpMessageToStreamReadWriteMultipleRegistersTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.WriteMultipleRegisters;
            ushort transactionId = 1;
            byte protocolId = 0;
            ushort length = 6;
            byte unitIdentifier = 1;
            ushort startAddress = 5000;
            ushort quantity = 2;
            ushort[] values = new ushort[2] { 2020, 2030 };

            byte[] stream = new byte[17] {(byte)(transactionId>>8 & 0xFF),
                                            (byte)(transactionId & 0xFF),
                                            (byte)(protocolId>>8 & 0xFF),
                                            (byte)(protocolId & 0xFF),
                                            (byte)(length>>8 & 0xFF),
                                            (byte)(length & 0xFF),
                                            unitIdentifier,
                                            (byte)modbusCommand,
                                            (byte)(startAddress>>8 & 0xFF),
                                            (byte)(startAddress & 0xFF),
                                            (byte)(quantity>>8 & 0xFF),
                                            (byte)(quantity & 0xFF),
                                            (byte)(quantity*2),
                                            (byte)(values[0]>>8 &0xFF),
                                            (byte)(values[0] & 0xFF),
                                            (byte)(values[1]>>8 &0xFF),
                                            (byte)(values[1] & 0xFF)};

            ModbusTcpMessage message = ModbusTcpMessage.Build(stream, stream.Length);

            byte[] expectedMessage = new byte[12] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x01, 0x10, 0x13,
                    0x88, 0x00, 0x02 };

            Assert.AreEqual(true, message.ToStream().SequenceEqual(expectedMessage));
        }
        
        [TestMethod()]
        public void ModbusServerModbusTcpMessageToStreamWriteCoilsTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.WriteMultipleCoils;
            ushort transactionId = 1;
            byte protocolId = 0;
            ushort length = 6;
            byte unitIdentifier = 1;
            ushort startAddress = 5000;
            ushort quantity = 10;

            byte[] stream = new byte[15] { (byte)(transactionId>>8 & 0xFF),
                                            (byte)(transactionId & 0xFF),
                                            (byte)(protocolId>>8 & 0xFF),
                                            (byte)(protocolId & 0xFF),
                                            (byte)(length>>8 & 0xFF),
                                            (byte)(length & 0xFF),
                                            unitIdentifier,
                                            (byte)modbusCommand,
                                            (byte)(startAddress>>8 & 0xFF),
                                            (byte)(startAddress & 0xFF),
                                            (byte)(quantity>>8 & 0xFF),
                                            (byte)(quantity & 0xFF),
                                            2,
                                            Convert.ToByte("00101011",2),
                                            Convert.ToByte("00000001",2)};
            
            ModbusTcpMessage message = ModbusTcpMessage.Build(stream, stream.Length);

            byte[] expectedMessage = new byte[12] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x01, 0x0f, 0x13,
                    0x88, 0x00, 0x0A };

            Assert.AreEqual(true, message.ToStream().SequenceEqual(expectedMessage));

        }

    }
}