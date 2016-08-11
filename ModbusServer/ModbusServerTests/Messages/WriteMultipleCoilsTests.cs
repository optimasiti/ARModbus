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
    public class WriteMultipleCoilsTests
    {
        [TestMethod()]
        public void ModbusServerWriteMultipleCoilsBuildTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.WriteMultipleCoils;
            byte[] fakeApplicationHeader = new byte[2] { 0, 0 };
            ushort startAddress = 5000;
            ushort quantity = 10;

            byte[] stream = new byte[10]{fakeApplicationHeader[0],
                                        fakeApplicationHeader[1],
                                        (byte)modbusCommand,
                                        (byte)(startAddress>>8 & 0xFF),
                                        (byte)(startAddress & 0xFF),
                                        (byte)(quantity>>8 & 0xFF),
                                        (byte)(quantity & 0xFF),
                                        2,
                                        Convert.ToByte("00101011",2),
                                        Convert.ToByte( "00000001",2)};

            WriteMultipleCoils writeMultipleCoils = WriteMultipleCoils.Build(stream, 2, stream.Length);

            Assert.AreEqual(modbusCommand, writeMultipleCoils.Command);
            Assert.AreEqual(startAddress, writeMultipleCoils.StartAddress);
            Assert.AreEqual(quantity, writeMultipleCoils.Quantity);

            bool[] expectedValues = new bool[10] { true, true, false, true, false, true, false, false, true, false };

            bool[] checkWriteData = writeMultipleCoils.GetWriteBitsData();
            
            for( int i = 0; i < quantity; i++ )
            {
                Assert.AreEqual(expectedValues[i], checkWriteData[i]);
            }
        }

        [TestMethod()]
        public void ModbusServerWriteMultipleCoilsToStreamTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.WriteMultipleCoils;
            byte[] fakeApplicationHeader = new byte[2] { 0, 0 };
            ushort startAddress = 5000;
            ushort quantity = 10;

            byte[] stream = new byte[10] {fakeApplicationHeader[0],
                                        fakeApplicationHeader[1],
                                        (byte)modbusCommand,
                                        (byte)(startAddress>>8 & 0xFF),
                                        (byte)(startAddress & 0xFF),
                                        (byte)(quantity>>8 & 0xFF),
                                        (byte)(quantity & 0xFF),
                                        2,
                                        Convert.ToByte("00101011",2),
                                        Convert.ToByte( "00000001",2)};

            WriteMultipleCoils writeMultipleCoils = WriteMultipleCoils.Build(stream, 2, stream.Length);

            byte[] sendStream = writeMultipleCoils.ToStream();

            bool equals = sendStream.SequenceEqual(new byte[5] {
                                        (byte)modbusCommand,
                                        (byte)(startAddress>>8 & 0xFF),
                                        (byte)(startAddress & 0xFF),
                                        (byte)(quantity>>8 & 0xFF),
                                        (byte)(quantity & 0xFF)});      

            Assert.AreEqual(true, equals);

        }

        [TestMethod()]
        public void ModbusServerWriteMultipleCoilsWithErrorToStreamTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.WriteMultipleCoils;
            byte[] fakeApplicationHeader = new byte[2] { 0, 0 };
            ushort startAddress = 5000;
            ushort quantity = 10;

            byte[] stream = new byte[10] {fakeApplicationHeader[0],
                                        fakeApplicationHeader[1],
                                        (byte)modbusCommand,
                                        (byte)(startAddress>>8 & 0xFF),
                                        (byte)(startAddress & 0xFF),
                                        (byte)(quantity>>8 & 0xFF),
                                        (byte)(quantity & 0xFF),
                                        2,
                                        Convert.ToByte("00101011",2),
                                        Convert.ToByte( "00000001",2)};

            WriteMultipleCoils writeMultipleCoils = WriteMultipleCoils.Build(stream, 2, stream.Length);
            writeMultipleCoils.SetErrorCode(0x1);

            byte[] sendStream = writeMultipleCoils.ToStream();

            bool equals = sendStream.SequenceEqual(new byte[2] { (byte)((byte)modbusCommand + 0x80), 0x01 });

            Assert.AreEqual(true, equals);
        }

    }
}