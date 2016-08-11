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
    public class ReadHoldingRegisterTests
    {
        [TestMethod()]
        public void ModbusServerReadHoldingRegisterBuildTest()
        {
            ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.ReadHoldingRegisters;
            byte[] fakeApplicationHeader = new byte[2] { 0, 0 };
            ushort startAddress = 5000;
            ushort quantity = 2;

            byte[] stream = new byte[7]{fakeApplicationHeader[0],
                                        fakeApplicationHeader[1],
                                        (byte)modbusCommand,
                                        (byte)(startAddress>>8 & 0xFF),
                                        (byte)(startAddress & 0xFF),
                                        (byte)(quantity>>8 & 0xFF),
                                        (byte)(quantity & 0xFF) };

            ReadHoldingRegisters readHoldingRegister = ReadHoldingRegisters.Build(stream, 2, stream.Length);

            Assert.AreEqual(modbusCommand, readHoldingRegister.Command);
            Assert.AreEqual(startAddress, readHoldingRegister.StartAddress);
            Assert.AreEqual(quantity, readHoldingRegister.Quantity);
            readHoldingRegister.SetReadWordsData( new ushort[2] { 1234, 4523 } );

        }

        [TestMethod()]
        public void ModbusServerReadHoldingRegisterToStreamTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.ReadHoldingRegisters;
            byte[] fakeApplicationHeader = new byte[2] { 0, 0 };
            ushort startAddress = 5000;
            ushort quantity = 2;

            byte[] readStream = new byte[7] {  fakeApplicationHeader[0],
                                                fakeApplicationHeader[1],
                                                (byte)modbusCommand,
                                                (byte)(startAddress>>8 & 0xFF),
                                                (byte)(startAddress & 0xFF),
                                                (byte)(quantity>>8 & 0xFF),
                                                (byte)(quantity & 0xFF)};
        
            ReadHoldingRegisters readHoldingRegister = ReadHoldingRegisters.Build(readStream, 2, readStream.Length);
            readHoldingRegister.SetReadWordsData( new ushort[2] { 1200, 1305 } );

            byte[] sendStream = readHoldingRegister.ToStream();

            bool equals = sendStream.SequenceEqual(new byte[6] { 0x03, 0x04, 0x04, 0xB0, 0x05, 0x19 });

            Assert.AreEqual(true, equals);
        }

        [TestMethod()]
        public void ModbusServerReadHoldingRegisterWithErrorToStreamTest()
        {
            const ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.ReadHoldingRegisters;
            byte[] fakeApplicationHeader = new byte[2] { 0, 0 };
            ushort startAddress = 5000;
            ushort quantity = 2;

            byte[] readStream = new byte[7] {  fakeApplicationHeader[0],
                                                fakeApplicationHeader[1],
                                                (byte)modbusCommand,
                                                (byte)(startAddress>>8 & 0xFF),
                                                (byte)(startAddress & 0xFF),
                                                (byte)(quantity>>8 & 0xFF),
                                                (byte)(quantity & 0xFF)};

            ReadHoldingRegisters readHoldingRegister = ReadHoldingRegisters.Build(readStream, 2, readStream.Length);
            readHoldingRegister.SetErrorCode(0x1);

            byte[] sendStream = readHoldingRegister.ToStream();

            bool equals = sendStream.SequenceEqual(new byte[2] { (byte)((byte)modbusCommand + 0x80), 0x01 });

            Assert.AreEqual(true, equals);
        }

    }
}
