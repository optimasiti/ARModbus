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
    public class ReadHoldingRegistersTests
    {
        [TestMethod()]
        public void ModbusClientReadHoldingRegistersReadHoldingRegistersTest()
        {
            ushort startAddress = 100;
            byte quantity = 10;

            ReadHoldingRegisters message = new ReadHoldingRegisters(startAddress, quantity);

            Assert.AreEqual(ModbusMessage.ModbusCommand.ReadHoldingRegisters, message.Command);
            Assert.AreEqual(startAddress, message.StartAddress);
            Assert.AreEqual(quantity, message.Quantity);
        }

        [TestMethod()]
        public void ModbusClientReadHoldingRegistersToStreamTest()
        {
            ushort startAddress = 100;
            byte quantity = 10;

            ReadHoldingRegisters message = new ReadHoldingRegisters(startAddress, quantity);

            byte[] stream = message.ToStream();
            ushort checkAddress = (ushort)((ushort)stream[1] << 8 | (ushort)stream[2]);
            ushort checkQuantity = (ushort)((ushort)stream[3] << 8 | (ushort)stream[4]);

            Assert.AreEqual((byte)ModbusMessage.ModbusCommand.ReadHoldingRegisters, stream[0]);
            Assert.AreEqual(startAddress, checkAddress);
            Assert.AreEqual(quantity, checkQuantity);
            Assert.AreEqual(5, stream.Length);
        }

        [TestMethod()]
        public void ModbusClientReadHoldingRegistersDecodeResponseTest()
        {
            ushort startAddress = 100;
            ushort quantity = 2;
            ushort[] expectedReadValues = new ushort[2] { 1200, 1305 };

            ReadHoldingRegisters message = new ReadHoldingRegisters(startAddress, quantity);

            byte[] stream = new byte[13] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x07, 0x01, 0x03, 0x04, 0x04, 0xB0, 0x05, 0x19 };

            bool decoded = message.DecodeResponse(stream, 7);

            Assert.AreEqual(true, decoded);
            Assert.AreEqual(true, expectedReadValues.SequenceEqual(message.GetReadWordsData()));
        }

        [TestMethod()]
        public void ModbusClientReadHoldingRegistersDecodeResponseExceptionsTest()
        {
            ushort startAddress = 100;
            byte quantity = 5;

            ReadHoldingRegisters message = new ReadHoldingRegisters(startAddress, quantity);

            byte[] stream = new byte[13] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x07, 0x01, 0x03, 0x04, 0x04, 0xB0, 0x05, 0x19 };
            bool raised = false;
            try
            {
                message.DecodeResponse(stream, 7);
            }
            catch (FormatException)
            {
                raised = true;
            }

            Assert.AreEqual(true, raised, "Error in QUANTITY not detected.");

            raised = false;
            try
            {
                message.DecodeResponse(stream, stream.Length);
            }
            catch (FormatException)
            {
                raised = true;
            }
            Assert.AreEqual(true, raised, "Message too short not detected.");

        }

        [TestMethod()]
        public void ModbusClientReadHoldingRegistersDecodeResponseModbusErrorCodeTest()
        {
            ushort startAddress = 100;
            byte quantity = 10;

            ReadHoldingRegisters message = new ReadHoldingRegisters(startAddress, quantity);

            byte[] stream = new byte[9] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x05, 0x01, 0x83, 0x02 };

            bool decoded = message.DecodeResponse(stream, 7);

            Assert.AreEqual(2, message.ModbusErrorCode);
        }
    }
}