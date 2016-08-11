using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ModbusClient.Messages;

namespace ModbusClient.Tests
{
    /// <summary>
    /// Acts like a modbus server with pre-programmed response. For testing.
    /// </summary>
    public class FakeModbusServer
    {
        private const int ReadTimeOutMSecs = 1000;

        private TcpListener m_TcpListener;
        int m_Port;

        public FakeModbusServer( int port )
        {
            m_Port = port;
        }

        public void StartServer()
        {
            Thread thread = new Thread(new ThreadStart(ExecBody));
            thread.Start();
            while (!thread.IsAlive);
        }

        public void StopServer()
        {
            Thread.Sleep(ReadTimeOutMSecs + 100);
            m_TcpListener.Stop();
        }

        public void ExecBody()
        {
            m_TcpListener = new TcpListener(IPAddress.Any, m_Port);
            m_TcpListener.Start();

            try
            {
                TcpClient tcpClient = m_TcpListener.AcceptTcpClient();
                NetworkStream clientStream = tcpClient.GetStream();
                clientStream.ReadTimeout = ReadTimeOutMSecs;

                byte[] readStream = new byte[280];
                int bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(readStream, 0, readStream.Length);
                }
                catch
                {
                    return;
                }

                if (bytesRead > 0)
                {
                    TrateTestQuery(readStream, bytesRead, tcpClient);
                }
            }
            catch
            {
            }
        }

        private void TrateTestQuery(byte[] readStream, int streamSize, TcpClient tcpClient)
        {
            const int UnitIdentifierPos = 6;
            const int ModbusMessagePos = 7;
            const int AddressPos0 = 8;
            const int AddressPos1 = 9;

            if (streamSize == 12 &&
                readStream[UnitIdentifierPos] == 1 &&
                readStream[ModbusMessagePos] == (byte)ModbusMessage.ModbusCommand.ReadCoils &&
                readStream[AddressPos0] == 0x13 &&
                readStream[AddressPos1] == 0x88 )
            {
                TrateModbusTcpClientReadCoils(tcpClient);
            }
            else if( streamSize == 12 &&
                readStream[UnitIdentifierPos] == 2 &&
                readStream[ModbusMessagePos] == (byte)ModbusMessage.ModbusCommand.ReadCoils &&
                readStream[AddressPos0] == 0x13 &&
                readStream[AddressPos1] == 0x88)
            {
                //No response. Timeout.
            }
            else if ( streamSize == 12 &&
                readStream[UnitIdentifierPos] == 1 &&
                readStream[ModbusMessagePos] == (byte)ModbusMessage.ModbusCommand.ReadHoldingRegisters &&
                readStream[AddressPos0] == 0x00 &&
                readStream[AddressPos1] == 0x05 )
            {
                TrateModbusTcpClientReadHoldingRegisters(tcpClient);
            }
            else if( streamSize == 17 && 
                readStream[UnitIdentifierPos] == 1 &&
                readStream[ModbusMessagePos] == (byte)ModbusMessage.ModbusCommand.WriteMultipleRegisters &&
                readStream[AddressPos0] == 0x00 &&
                readStream[AddressPos1] == 0x05)
            {
                TrateModbusTcpClientWriteMultipleRegisters(tcpClient);
            }
            else if( streamSize == 15 &&
                readStream[UnitIdentifierPos] == 1 &&
                readStream[ModbusMessagePos] == (byte)ModbusMessage.ModbusCommand.WriteMultipleCoils &&
                readStream[AddressPos0] == 0x00 &&
                readStream[AddressPos1] == 0x05)
            {
                TrateModbusTcpClientWriteMultipleCoils(tcpClient);
            }
        }

        private void TrateModbusTcpClientReadCoils( TcpClient tcpClient )
        {
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(new byte[] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x05, 0x01, 0x01, 0x02, 0xAA, 0x02 }, 0, 11);
        }

        private void TrateModbusTcpClientReadHoldingRegisters(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(new byte[] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x07, 0x01, 0x03, 0x04, 0x04, 0xB0, 0x05, 0x19 }, 0, 13);
        }

        private void TrateModbusTcpClientWriteMultipleRegisters(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(new byte[] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x06, 0x01, 0x10, 0x00, 0x05, 0x00, 0x02 }, 0, 12);
        }

        private void TrateModbusTcpClientWriteMultipleCoils(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(new byte[] { 0x00, 0x01, 0x0, 0x00, 0x00, 0x06, 0x01, 0x0F, 0x00, 0x05, 0x00, 0x0A }, 0, 12);
        }
    }
}
