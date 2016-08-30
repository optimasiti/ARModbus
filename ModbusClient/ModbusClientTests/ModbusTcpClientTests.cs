using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModbusClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace ModbusClient.Tests
{
    [TestClass()]
    public class ModbusTcpClientTests
    {
        [TestMethod()]
        public void ModbusClientModbusTcpClientTest() 
        {
            string hostName = "localhost";
            int port = 501;

            ModbusTcpClient client = new ModbusTcpClient(hostName, port);

            FakeModbusServer server = new FakeModbusServer(port);
            server.StartServer();

            bool connected = client.Connect();
            server.StopServer();

            Assert.AreEqual(true, connected);
        }

        [TestMethod()]
        public void ModbusClientModbusTcpClientConnectTest()
        {
            ModbusTcpClient client = new ModbusTcpClient();

            string hostName = "localhost";
            int port = 502;

            FakeModbusServer server = new FakeModbusServer(port);
            server.StartServer();

            bool connected = client.Connect(hostName, port);
            server.StopServer();

            Assert.AreEqual(true, connected);
        }

        [TestMethod()]
        public void ModbusClientModbusTcpClientReadCoilsTest()
        {
            ModbusTcpClient client = new ModbusTcpClient();
            ModbusTcpClient.ResetTransactionId();

            string hostName = "localhost";
            int port = 503;
            byte unitIdentifier = 1;
            ushort address = 5000;
            ushort quantity = 10;

            FakeModbusServer fakeServer = new FakeModbusServer(port);
            fakeServer.StartServer();

            client.Connect(hostName, port);

            bool[] readBitsData = client.ReadCoils(unitIdentifier, address, quantity);

            for (int i = 0; i < quantity; i++)
                Assert.AreEqual(i % 2 != 0, readBitsData[i]);

            fakeServer.StopServer();
        }

        [TestMethod()]
        public void ModbusClientModbusTcpClientReadHoldingRegistersTest()
        {
            ModbusTcpClient client = new ModbusTcpClient();
            ModbusTcpClient.ResetTransactionId();

            string hostName = "localhost";
            int port = 504;
            byte unitIdentifier = 1;
            ushort address = 5;
            ushort quantity = 2;

            FakeModbusServer fakeServer = new FakeModbusServer(port);
            fakeServer.StartServer();

            client.Connect(hostName, port);
            ushort[] readWordsData = client.ReadHoldingRegisters(unitIdentifier, address, quantity);

            Assert.AreEqual(1200, readWordsData[0]);
            Assert.AreEqual(1305, readWordsData[1]);

            fakeServer.StopServer();
        }

        [TestMethod()]
        public void ModbusClientModbusTcpClientWriteMultipleRegistersTest()
        {
            ModbusTcpClient client = new ModbusTcpClient();
            ModbusTcpClient.ResetTransactionId();

            string hostName = "localhost";
            int port = 512;
            byte unitIdentifier = 1;
            ushort address = 5;
            ushort[] values = new ushort[2] { 1010, 1020 };


            FakeModbusServer fakeServer = new FakeModbusServer(port);
            fakeServer.StartServer();

            client.Connect(hostName, port);
            client.WriteMultipleRegisters(unitIdentifier, address, values);

            fakeServer.StopServer();
        }

        [TestMethod()]
        public void ModbusClientModbusTcpClientWriteMultipleCoilsTest()
        {
            ModbusTcpClient client = new ModbusTcpClient();
            ModbusTcpClient.ResetTransactionId();

            string hostName = "localhost";
            int port = 513;
            byte unitIdentifier = 1;
            ushort address = 5;
            bool[] values = new bool[10] { true, true, false, true, false, true, false, false, true, false };


            FakeModbusServer fakeServer = new FakeModbusServer(port);
            fakeServer.StartServer();

            client.Connect(hostName, port);
            client.WriteMultipleCoils(unitIdentifier, address, values);

            fakeServer.StopServer();
        }

        [TestMethod()]
        public void ModbusClientModbusTcpClientWithoutResponseTest()
        {
            ModbusTcpClient client = new ModbusTcpClient();
            ModbusTcpClient.ResetTransactionId();

            string hostName = "localhost";
            int port = 514;
            byte unitIdentifier = 2;
            ushort address = 5000;
            ushort quantity = 10;

            FakeModbusServer fakeServer = new FakeModbusServer(port);
            fakeServer.StartServer();

            client.Connect(hostName, port);

            bool raised = false;
            try
            {
                bool[] readBitsData = client.ReadCoils(unitIdentifier, address, quantity);
            }
            catch (IOException)
            {
                raised = true;
            }

            fakeServer.StopServer();

            Assert.AreEqual(true, raised);
        }

        [TestMethod()]
        public void ModbusClientModbusTcpClientNoConnectTest()
        {
            ModbusTcpClient client = new ModbusTcpClient();
            ModbusTcpClient.ResetTransactionId();

            string hostName = "localhost";
            int port = 514;

            bool connected = client.Connect(hostName, port);

            Assert.AreEqual(false, connected);
        }

        [TestMethod()] 
        public void ModbusClientModbusTcpClientAutoReconnectTest()
        {
            string hostName = "localhost";
            int port = 504;

            ModbusTcpClient client = new ModbusTcpClient(hostName, port);
            ModbusTcpClient.ResetTransactionId();

            byte unitIdentifier = 1;
            ushort address = 5;
            ushort quantity = 2;

            FakeModbusServer fakeServer = new FakeModbusServer(port);
            fakeServer.StartServer();

            ushort[] readWordsData = client.ReadHoldingRegisters(unitIdentifier, address, quantity);

            Assert.AreEqual(1200, readWordsData[0]);
            Assert.AreEqual(1305, readWordsData[1]);

            fakeServer.StopServer();
        }

        [TestMethod()] 
        public void ModbusClient_ModbusTcpClient_TwoReadsTest()
        {
            string hostName = "localhost";
            int port = 504;

            ModbusTcpClient client = new ModbusTcpClient(hostName, port);
            ModbusTcpClient.ResetTransactionId();

            byte unitIdentifier = 1;
            ushort address = 5;
            ushort quantity = 2;

            FakeModbusServer fakeServer = new FakeModbusServer(port);
            fakeServer.StartServer();

            ushort[] readWordsData = client.ReadHoldingRegisters(unitIdentifier, address, quantity);

            Assert.AreEqual(1200, readWordsData[0]);
            Assert.AreEqual(1305, readWordsData[1]);

            ModbusTcpClient.ResetTransactionId();

            address = 5000;
            quantity = 10;

            bool[] readBitsData = client.ReadCoils(unitIdentifier, address, quantity);

            for (int i = 0; i < quantity; i++)
                Assert.AreEqual(i % 2 != 0, readBitsData[i]);
                
            fakeServer.StopServer();
        }


    }
}