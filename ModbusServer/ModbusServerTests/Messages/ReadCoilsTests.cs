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
    public class ReadCoilsTests
    {

        [TestMethod()]
        public void ModbusServerReadCoilsBuildTest()
        {
            ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.ReadCoils;
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

            ReadCoils readCoils = ReadCoils.Build(stream, 2, stream.Length);

            Assert.AreEqual(modbusCommand, readCoils.Command);
            Assert.AreEqual(startAddress, readCoils.StartAddress);
            Assert.AreEqual(quantity, readCoils.Quantity);
            readCoils.SetReadBitsData( new bool[2] { true, false} );
        }

        [TestMethod()]
        public void ModbusServerReadCoilsToStreamTest()
        {
            ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.ReadCoils;
            byte[] fakeApplicationHeader = new byte[2] { 0, 0 };
            ushort startAddress = 5000;
            ushort quantity = 2;

            byte[] readStream = new byte[7]{   fakeApplicationHeader[0],
                                                fakeApplicationHeader[1],
                                                (byte)modbusCommand,
                                                (byte)(startAddress>>8 & 0xFF),
                                                (byte)(startAddress & 0xFF),
                                                (byte)(quantity>>8 & 0xFF),
                                                (byte)(quantity & 0xFF)};

            ReadCoils readCoils = ReadCoils.Build(readStream, 2, readStream.Length);
            readCoils.SetReadBitsData( new bool[2] { false, true } );

            byte[] sendStream = readCoils.ToStream();

            bool equals = sendStream.SequenceEqual(new byte[3] { 0x01, 0x01, 0x02 });

            Assert.AreEqual(true, equals);
        }

        [TestMethod()]
        public void ModbusServerReadCoilsWithErrorToStreamTest()
        {
            ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.ReadCoils;
            byte[] fakeApplicationHeader = new byte[2] { 0, 0 };
            ushort startAddress = 5000;
            ushort quantity = 2;

            byte[] readStream = new byte[7]{   fakeApplicationHeader[0],
                                                fakeApplicationHeader[1],
                                                (byte)modbusCommand,
                                                (byte)(startAddress>>8 & 0xFF),
                                                (byte)(startAddress & 0xFF),
                                                (byte)(quantity>>8 & 0xFF),
                                                (byte)(quantity & 0xFF)};

            ReadCoils readCoils = ReadCoils.Build(readStream, 2, readStream.Length);
            readCoils.SetErrorCode(0x1);

            byte[] sendStream = readCoils.ToStream();

            bool equals = sendStream.SequenceEqual(new byte[2] { (byte)((byte)modbusCommand  + 0x80), 0x01 });

            Assert.AreEqual(true, equals);
        }

    }
}