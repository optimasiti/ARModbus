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
    public class WriteMultipleCoilsTests
    {
        
        [TestMethod()]
        public void WriteMultipleCoilsWriteMultipleCoilsTest()
        {
            ushort startAddress = 100;
            bool[] values = new bool[10] { true, true, false, true, false, true, false, false, true, false };

            WriteMultipleCoils message = new WriteMultipleCoils(startAddress, values);

            Assert.AreEqual(ModbusMessage.ModbusCommand.WriteMultipleCoils, message.Command);
            Assert.AreEqual(startAddress, message.StartAddress);
            Assert.AreEqual(true, message.GetWriteBitsData().SequenceEqual(values));
    }

    [TestMethod()]
        public void ModbusClientWriteMultipleCoilsToStreamTest()
        {
            ushort startAddress = 100;
            bool[] values = new bool[10] { true, true, false, true, false, true, false, false, true, false };

            WriteMultipleCoils message = new WriteMultipleCoils(startAddress, values);

            byte[] stream = message.ToStream();
            ushort checkAddress = (ushort)((ushort)stream[1] << 8 | (ushort)stream[2]);
            ushort checkQuantity = (ushort)((ushort)stream[3] << 8 | (ushort)stream[4]);

            Assert.AreEqual(2, stream[5]);
            Assert.AreEqual(Convert.ToUInt16("00101011", 2), stream[6]);
            Assert.AreEqual(Convert.ToUInt16("00000001", 2), stream[7]);

            Assert.AreEqual((byte)ModbusMessage.ModbusCommand.WriteMultipleCoils, stream[0]);
            Assert.AreEqual(startAddress, checkAddress);
            Assert.AreEqual(values.Length, checkQuantity);
            Assert.AreEqual(8, stream.Length);

        }

        [TestMethod()]
        public void ModbusClientWriteMultipleCoilsDecodeResponseTest()
        {
            ushort startAddress = 5;
            bool[] values = new bool[10] { true, false, true, false, true, false, true, false, true, false }; 


            WriteMultipleCoils message = new WriteMultipleCoils(startAddress, values);

            byte[] stream = new byte[12] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x06, 0x01, 0x0F, 0x00, 0x05, 0x00, 0x0A };

            bool decoded = message.DecodeResponse(stream, 7);

            Assert.AreEqual(true, decoded);
        }

        [TestMethod()]
        public void ModbusClientWriteMultipleCoilsDecodeResponseExceptionsTest()
        {
            ushort startAddress = 5;
            
            WriteMultipleCoils message = new WriteMultipleCoils(startAddress, new bool[2]);

            byte[] stream = new byte[12] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x06, 0x01, 0x0F, 0x00, 0x05, 0x00, 0x0A };
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
                message = new WriteMultipleCoils((ushort)(startAddress + 10), new bool[10]);
                message.DecodeResponse(stream, 7);
            }
            catch (FormatException)
            {
                raised = true;
            }
            Assert.AreEqual(true, raised, "Error in START_ADDRESS not detected.");

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
        public void ModbusClientWriteMultipleCoilsDecodeResponseModbusErrorCodeTest()
        {
            ushort startAddress = 100;

            WriteMultipleCoils message = new WriteMultipleCoils(startAddress, new bool[2]);

            byte[] stream = new byte[9] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x06, 0x01, 0x8F, 0x01 };

            bool decoded = message.DecodeResponse(stream, 7);

            Assert.AreEqual(1, message.ModbusErrorCode);

        }
        

    }
}