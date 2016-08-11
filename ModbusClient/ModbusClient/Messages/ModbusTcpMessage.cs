using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusClient.Messages
{
    public class ModbusTcpMessage
    {
        private const int MBAP_HEADER_SIZE = 7;

        private readonly ModbusMessage m_ProtocolDataUnit;
        private readonly ushort m_TransactionId;
        private readonly byte m_UnitIdentifier;

        public ModbusMessage ProtocolDataUnit { get { return m_ProtocolDataUnit; } }
        public ushort TransactionId { get { return m_TransactionId; } }
        public byte UnitIdentifier { get { return m_UnitIdentifier; } }

        public ModbusTcpMessage(ModbusMessage protocolDataUnit, ushort transactionId, byte unitIdentifier)
        {
            m_ProtocolDataUnit = protocolDataUnit;
            m_TransactionId = transactionId;
            m_UnitIdentifier = unitIdentifier;
        }

        public byte[] ToStream()
        {
            byte[] pduStream = m_ProtocolDataUnit.ToStream();
            byte[] stream = new byte[MBAP_HEADER_SIZE];

            stream[0] = (byte)(m_TransactionId >> 8);
            stream[1] = (byte)(m_TransactionId & 0xFF);
            stream[2] = 0;
            stream[3] = 0;
            stream[4] = (byte)((pduStream.Length + 1) >> 8);
            stream[5] = (byte)((pduStream.Length + 1) & 0xFF);
            stream[6] = m_UnitIdentifier;

            return stream.Concat(pduStream).ToArray();
        }

        public bool DecodeResponse(byte[] stream)
        {
            if (!CheckAPHeader(stream))
            {
                throw new FormatException();
            }
                
            return m_ProtocolDataUnit.DecodeResponse(stream, MBAP_HEADER_SIZE);
        }

        private bool CheckAPHeader(byte[] stream)
        {
            if (stream.Length < MBAP_HEADER_SIZE)
                return false;

            ushort transactionId = (ushort)(((ushort)stream[0]) << 8 | (ushort)stream[1]);
            ushort protocolCode = (ushort)(((ushort)stream[2]) << 8 | (ushort)stream[3]);
            byte unitIdentifier = stream[6];

            return transactionId == m_TransactionId &&
                    protocolCode == 0 &&
                    unitIdentifier == m_UnitIdentifier;
        }

    }

}
