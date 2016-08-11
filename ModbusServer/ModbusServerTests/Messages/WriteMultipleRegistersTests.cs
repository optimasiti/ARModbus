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
    public class WriteMultipleRegistersTests
    {
        [TestMethod()]
        public void ModbusServerWriteMultipleRegistersBuildTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.WriteMultipleRegisters;
            byte[] fakeApplicationHeader = new byte[2] { 0, 0 };
            ushort startAddress = 5000;
            ushort quantity = 2;
            ushort[] values = new ushort[2] { 1010, 1020 };

            byte[] stream = new byte[12] {  fakeApplicationHeader[0],
                                        fakeApplicationHeader[1],
                                        (byte)modbusCommand,
                                        (byte)(startAddress>>8 & 0xFF),
                                        (byte)(startAddress & 0xFF),
                                        (byte)(quantity>>8 & 0xFF),
                                        (byte)(quantity & 0xFF),
                                        (byte)(quantity*2),
                                        (byte)(values[0]>>8 &0xFF),
                                        (byte)(values[0] & 0xFF), 
                                        (byte)(values[1]>>8 & 0xFF), 
                                        (byte)(values[1] & 0xFF)};

            WriteMultipleRegisters writeMultipleRegisters = WriteMultipleRegisters.Build(stream, 2, stream.Length);

            Assert.AreEqual(modbusCommand, writeMultipleRegisters.Command);
            Assert.AreEqual(startAddress, writeMultipleRegisters.StartAddress);
            Assert.AreEqual(quantity, writeMultipleRegisters.Quantity);

            ushort[] checkWriteData = writeMultipleRegisters.GetWriteWordsData();
            Assert.AreEqual(values[0], checkWriteData[0]);
            Assert.AreEqual(values[1], checkWriteData[1]);
        }

        [TestMethod()]
        public void ModbusServerWriteMultipleRegistersToStreamTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.WriteMultipleRegisters;
            byte[] fakeApplicationHeader = new byte[2] { 0, 0 };
            ushort startAddress = 5000;
            ushort quantity = 2;
            ushort[] values = new ushort[2] { 1010, 1020 };

            byte[] stream = new byte[12] { fakeApplicationHeader[0],
                                        fakeApplicationHeader[1],
                                        (byte)modbusCommand,
                                        (byte)(startAddress>>8 & 0xFF),
                                        (byte)(startAddress & 0xFF),
                                        (byte)(quantity>>8 & 0xFF),
                                        (byte)(quantity & 0xFF),
                                        (byte)(quantity*2),
                                        (byte)(values[0]>>8 &0xFF),
                                        (byte)(values[0] & 0xFF),
                                        (byte)(values[1]>>8 & 0xFF),
                                        (byte)(values[1] & 0xFF)};

            WriteMultipleRegisters writeMultipleRegisters = WriteMultipleRegisters.Build(stream, 2, stream.Length);

            byte[] sendStream = writeMultipleRegisters.ToStream();

            bool equals = sendStream.SequenceEqual(new byte[5] {
                                        (byte)modbusCommand,
                                        (byte)(startAddress>>8 & 0xFF),
                                        (byte)(startAddress & 0xFF),
                                        (byte)(quantity>>8 & 0xFF),
                                        (byte)(quantity & 0xFF)}); 

            Assert.AreEqual(true, equals);

        }

        [TestMethod()]
        public void ModbusServerWriteMultipleRegistersWithErrorToStreamTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.WriteMultipleRegisters;
            byte[] fakeApplicationHeader = new byte[2] { 0, 0 };
            ushort startAddress = 5000;
            ushort quantity = 2;
            ushort[] values = new ushort[2] { 1010, 1020 };

            byte[] stream = new byte[12] { fakeApplicationHeader[0],
                                        fakeApplicationHeader[1],
                                        (byte)modbusCommand,
                                        (byte)(startAddress>>8 & 0xFF),
                                        (byte)(startAddress & 0xFF),
                                        (byte)(quantity>>8 & 0xFF),
                                        (byte)(quantity & 0xFF),
                                        (byte)(quantity*2),
                                        (byte)(values[0]>>8 &0xFF),
                                        (byte)(values[0] & 0xFF),
                                        (byte)(values[1]>>8 & 0xFF),
                                        (byte)(values[1] & 0xFF)};

            WriteMultipleRegisters writeMultipleRegisters = WriteMultipleRegisters.Build(stream, 2, stream.Length);
            writeMultipleRegisters.SetErrorCode(0x01);

            byte[] sendStream = writeMultipleRegisters.ToStream();

            bool equals = sendStream.SequenceEqual(new byte[2] { (byte)((byte)modbusCommand + 0x80), 0x01 });

            Assert.AreEqual(true, equals);
        }

    }
}