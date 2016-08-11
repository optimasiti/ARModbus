using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusServer.Messages;
using System.IO;

namespace ModbusServer.Messages
{
    public class ModbusTcpMessage 
    {
        private const int MBAP_HEADER_SIZE = 7;

        private readonly ModbusMessage m_ProtocolDataUnit;
        private readonly ushort m_TransactionId;
        private readonly byte m_UnitIdentifier;

        public ModbusMessage ProtocolDataUnit {get { return m_ProtocolDataUnit; } }
        public ushort TransactionId { get { return m_TransactionId; } }
        public byte UnitIdentifier { get { return m_UnitIdentifier;  } }
        
        private ModbusTcpMessage( ModbusMessage protocolDataUnit, ushort transactionId, byte unitIdentifier  )
        {
            m_ProtocolDataUnit = protocolDataUnit;
            m_TransactionId = transactionId;
            m_UnitIdentifier = unitIdentifier;
        }

        public static ModbusTcpMessage Build(byte[] stream, int streamSize)
        {
            if (stream == null || streamSize < MBAP_HEADER_SIZE + 1)
                throw new FormatException(); 

            if (stream[2] != 0 || stream[3] != 0)
                throw new FormatException(); 

            ushort transactionId = (ushort)(((ushort)stream[0]) << 8 | (ushort)stream[1]);
            byte unitIdentifier = stream[6];

            try
            {
                switch ((ModbusMessage.ModbusCommand)stream[MBAP_HEADER_SIZE])
                {
                    case ModbusMessage.ModbusCommand.ReadHoldingRegisters:
                        return new ModbusTcpMessage(
                                ReadHoldingRegisters.Build(stream, MBAP_HEADER_SIZE, streamSize),
                                transactionId, unitIdentifier);

                    case ModbusMessage.ModbusCommand.ReadCoils:
                        return new ModbusTcpMessage(
                                ReadCoils.Build(stream, MBAP_HEADER_SIZE, streamSize),
                                transactionId, unitIdentifier);

                    case ModbusMessage.ModbusCommand.WriteMultipleRegisters:
                        return new ModbusTcpMessage(
                                WriteMultipleRegisters.Build(stream, MBAP_HEADER_SIZE, streamSize),
                                transactionId, unitIdentifier);

                    case ModbusMessage.ModbusCommand.WriteMultipleCoils:
                        return new ModbusTcpMessage(
                                WriteMultipleCoils.Build(stream, MBAP_HEADER_SIZE, streamSize),
                                transactionId, unitIdentifier);
                }
            }
            catch
            {
                throw new FormatException();
            }

            throw new FormatException();
        }

        public byte[] ToStream()
        {
            byte[] pduRawMessage = m_ProtocolDataUnit.ToStream();
            byte[] rawMessage = new byte[MBAP_HEADER_SIZE];

            rawMessage[0] = (byte)(m_TransactionId >> 8);
            rawMessage[1] = (byte)(m_TransactionId & 0xFF);
            rawMessage[2] = 0;
            rawMessage[3] = 0;
            rawMessage[4] = (byte)((pduRawMessage.Length + 1) >> 8);
            rawMessage[5] = (byte)((pduRawMessage.Length + 1) & 0xFF);
            rawMessage[6] = m_UnitIdentifier;

            return rawMessage.Concat(pduRawMessage).ToArray();
        }
    }
}
