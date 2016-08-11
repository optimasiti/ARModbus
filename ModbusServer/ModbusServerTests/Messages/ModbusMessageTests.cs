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
    public class ModbusMessageTests
    {
        [TestMethod()]
        public void ModbusServerModbusMessageModbusMessageTest()
        {
            ModbusMessage.ModbusCommand modbusCommand = ModbusMessage.ModbusCommand.ReadHoldingRegisters;

            ModbusMessage modbusMessage = new ModbusMessage(modbusCommand, new byte[2] { 0, 0 }, 0, 2);

            Assert.AreEqual(modbusCommand, modbusMessage.Command);
        }
    }
}