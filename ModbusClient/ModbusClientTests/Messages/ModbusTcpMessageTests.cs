using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModbusClient.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusClient.Messages.Tests
{
    [TestClass()]
    public class ModbusTcpMessageTests
    {
        [TestMethod()]
        public void ModbusClientModbusTcpMessageModbusTcpMessageTest()
        {
            ushort transactionId = 100;
            byte unitIdentifier = 45;
            ushort startAddress = 1000;
            ushort quantity = 10;

            ReadHoldingRegisters protocolDataUnit = new ReadHoldingRegisters(startAddress, quantity);
            ModbusTcpMessage tcpMessage = new ModbusTcpMessage(protocolDataUnit, transactionId, unitIdentifier);

            Assert.AreEqual(transactionId, tcpMessage.TransactionId);
            Assert.AreEqual(unitIdentifier, tcpMessage.UnitIdentifier);
            Assert.AreEqual(protocolDataUnit, tcpMessage.ProtocolDataUnit);
        }

        [TestMethod()]
        public void ModbusClientModbusTcpMessageToStreamTest()
        {
            ushort transactionId = 100;
            byte unitIdentifier = 45;
            ushort startAddress = 100;
            byte quantity = 10;

            ReadHoldingRegisters pdu = new ReadHoldingRegisters(startAddress, quantity);
            ModbusTcpMessage tcpMessage = new ModbusTcpMessage(pdu, transactionId, unitIdentifier);

            byte[] stream = tcpMessage.ToStream();

            Assert.AreEqual(true, stream.SequenceEqual(new byte[12] { 0x00, 0x64, 0x00, 0x00, 0x00,
                            0x06, 0x2D, 0x03, 0x00, 0x64, 0x00, 0x0A }));
        }

        [TestMethod()]
        public void ModbusClientModbusTcpMessageDecodeResponseTest()
        {
            ushort transactionId = 10;
            byte unitIdentifier = 11;
            ushort startAddress = 100;
            ushort quantity = 2;

            ReadHoldingRegisters pdu = new ReadHoldingRegisters(startAddress, quantity);
            ModbusTcpMessage tcpMessage = new ModbusTcpMessage(pdu, transactionId, unitIdentifier);

            byte[] stream = new byte[13] { 0x00, 0x0A, 0x0, 0x00, 0x00, 0x07, 0x0B, 0x03, 0x04, 0x04, 0xB0, 0x05, 0x19 };

            bool decoded = tcpMessage.DecodeResponse(stream);

            Assert.AreEqual(true, decoded);
        }

        [TestMethod()]
        public void ModbusClientModbusTcpMessageDecodeResponseExceptionsTest()
        {
            ushort TransactionId = 10; 
            byte UnitIdentifier = 11;
            ushort startAddress = 100;
            ushort quantity = 2;

            ReadHoldingRegisters pdu = new ReadHoldingRegisters(startAddress, quantity);
            ModbusTcpMessage tcpMessage = new ModbusTcpMessage(pdu, TransactionId, UnitIdentifier);

            bool raised = false;

            try
            {
                tcpMessage.DecodeResponse( new byte[3]);
            }
            catch
            {
                raised = true;
            }
            Assert.AreEqual(true, raised, "Error MBAP Header too short not detected.");
        }

    }
}