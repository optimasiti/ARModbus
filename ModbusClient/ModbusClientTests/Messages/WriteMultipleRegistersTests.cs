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
    public class WriteMultipleRegistersTests
    {
        [TestMethod()]
        public void ModbusClientWriteMultipleRegistersWriteMultipleRegistersTest()
        {
            ushort startAddress = 100;
            ushort[] values = new ushort[2] { 1000, 2000 };

            WriteMultipleRegisters message = new WriteMultipleRegisters(startAddress, values);

            Assert.AreEqual(ModbusMessage.ModbusCommand.WriteMultipleRegisters, message.Command);
            Assert.AreEqual(startAddress, message.StartAddress);
            Assert.AreEqual(true, message.GetWriteWordsData().SequenceEqual(values));
        }


        [TestMethod()]
        public void ModbusClientWriteMultipleRegistersToStreamTest()
        {
            ushort startAddress = 100;
            ushort[] values = new ushort[3] { 1010, 1020, 1030 };

            WriteMultipleRegisters message = new WriteMultipleRegisters(startAddress, values);

            byte[] stream = message.ToStream();
            ushort checkAddress = (ushort)((ushort)stream[1] << 8 | (ushort)stream[2]);
            ushort checkQuantity = (ushort)((ushort)stream[3] << 8 | (ushort)stream[4]);
            ushort[] checkValues = new ushort[3];

            for (int i = 0; i < 3; i++)
            {
                checkValues[i] = (ushort)((ushort)stream[6 + i * 2] << 8 | (ushort)stream[6 + i * 2 + 1]);
                Assert.AreEqual(values[i], checkValues[i]);
            }

            Assert.AreEqual((byte)ModbusMessage.ModbusCommand.WriteMultipleRegisters, stream[0]);
            Assert.AreEqual(startAddress, checkAddress);
            Assert.AreEqual(values.Length, checkQuantity);
            Assert.AreEqual(values.Length * 2, stream[5]);
            Assert.AreEqual(12, stream.Length);

        }

        [TestMethod()]
        public void ModbusClientWriteMultipleRegistersDecodeResponseTest()
        {
            ushort startAddress = 5;
            ushort[] values = new ushort[2] { 1000, 2000 };


            WriteMultipleRegisters message = new WriteMultipleRegisters(startAddress, values);

            byte[] stream = new byte[12] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x06, 0x01, 0x10, 0x00, 0x05, 0x00, 0x02 };

            bool decoded = message.DecodeResponse(stream, 7);

            Assert.AreEqual(true, decoded);
        }
        
        [TestMethod()]
        public void ModbusClientWriteMultipleCoilsDecodeResponseExceptionsTest()
        {
            ushort startAddress = 5;

            WriteMultipleRegisters message = new WriteMultipleRegisters(startAddress, new ushort[10]);

            byte[] stream = new byte[12] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x06, 0x01, 0x10, 0x00, 0x05, 0x00, 0x02 };
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
                message = new WriteMultipleRegisters((ushort)(startAddress + 10), new ushort[2]);
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

            WriteMultipleRegisters message = new WriteMultipleRegisters(startAddress, new ushort[2]);

            byte[] stream = new byte[9] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x06, 0x01, 0x90, 0x01 };

            bool decoded = message.DecodeResponse(stream, 7);

            Assert.AreEqual(1, message.ModbusErrorCode);
        }
    }
}