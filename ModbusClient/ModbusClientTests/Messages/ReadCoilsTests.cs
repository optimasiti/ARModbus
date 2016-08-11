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
    public class ReadCoilsTests
    {
        [TestMethod()]
        public void ModbusClientReadCoilsReadCoilsTest()
        {
            ushort startAddress = 100;
            byte quantity = 10;

            ReadCoils message = new ReadCoils(startAddress, quantity);

            Assert.AreEqual(ModbusMessage.ModbusCommand.ReadCoils, message.Command);
            Assert.AreEqual(startAddress, message.StartAddress);
            Assert.AreEqual(quantity, message.Quantity);
        }

        [TestMethod()]
        public void ModbusClientReadCoilsToStreamTest()
        {
            ushort startAddress = 100;
            byte quantity = 10;

            ReadCoils message = new ReadCoils(startAddress, quantity);

            byte[] stream = message.ToStream();
            ushort checkAddress = (ushort)((ushort)stream[1] << 8 | (ushort)stream[2]);
            ushort checkQuantity = (ushort)((ushort)stream[3] << 8 | (ushort)stream[4]);

            Assert.AreEqual((byte)ModbusMessage.ModbusCommand.ReadCoils, stream[0]);
            Assert.AreEqual(startAddress, checkAddress);
            Assert.AreEqual(quantity, checkQuantity);
            Assert.AreEqual(5, stream.Length);
        }

        [TestMethod()]
        public void ModbusClientReadCoilsDecodeResponseTest()
        {
            ushort startAddress = 100;
            byte quantity = 10;
            bool[] expectedReadValues = new bool[10] { false, true, false, true, false, true, false, true, false, true };

            ReadCoils message = new ReadCoils(startAddress, quantity);

            byte[] stream = new byte[11] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x05, 0x01, 0x01, 0x02, 0xAA, 0x02 };

            bool decoded = message.DecodeResponse(stream, 7 );

            Assert.AreEqual(true, decoded);
            Assert.AreEqual(true, expectedReadValues.SequenceEqual(message.GetReadBitsData()));

        }
        
        [TestMethod()]
        public void ModbusClientReadCoilsDecodeResponseExceptionsTest()
        {
            ushort startAddress = 100;
            byte quantity = 5;

            ReadCoils message = new ReadCoils(startAddress, quantity);

            byte[] stream = new byte[11] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x05, 0x01, 0x01, 0x02, 0xAA, 0x02 };
            bool raised = false;
            try
            {
                message.DecodeResponse(stream, 7);
            }
            catch( FormatException)
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
        public void ModbusClientReadCoilsDecodeResponseModbusErrorCodeTest()
        {
            ushort startAddress = 100;
            byte quantity = 10;

            ReadCoils message = new ReadCoils(startAddress, quantity);

            byte[] stream = new byte[9] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x05, 0x01, 0x81, 0x02 };

            bool decoded = message.DecodeResponse(stream, 7);

            Assert.AreEqual(2, message.ModbusErrorCode);
        }
    }
}