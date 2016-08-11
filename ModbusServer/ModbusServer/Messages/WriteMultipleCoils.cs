using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModbusServer.Messages
{
    public class WriteMultipleCoils : ModbusMessage
    {
        private const ModbusCommand MODBUS_COMMAND = ModbusCommand.WriteMultipleCoils;

        private bool[] m_WriteBitsData;
        private ushort m_StartAddress;
        private ushort m_Quantity;

        public override ushort StartAddress { get { return m_StartAddress; } }
        public override ushort Quantity { get { return m_Quantity; } }

        private WriteMultipleCoils(byte[] rawMessage, int startIndex, int size) : 
            base( MODBUS_COMMAND, rawMessage, startIndex, size)
        {
        }

        public static WriteMultipleCoils Build(byte[] rawMessage, int startIndex, int size)
        {
            WriteMultipleCoils message = new WriteMultipleCoils(rawMessage, startIndex, size);
            message.Decode();

            return message;
        }

        public override bool[] GetWriteBitsData()
        {
            bool[] copy = new bool[m_WriteBitsData.Length];
            m_WriteBitsData.CopyTo(copy, 0);
            return copy;
        }

        public override byte[] ToStream()
        {
            if (m_ErrorCode != 0)
                return ErrorResponseToStream();

            byte[] stream = new byte[5];

            stream[0] = (byte)MODBUS_COMMAND;
            stream[1] = (byte)(m_StartAddress >> 8);
            stream[2] = (byte)(m_StartAddress & 0xFF);
            stream[3] = (byte)(m_Quantity >> 8);
            stream[4] = (byte)(m_Quantity & 0xFF);

            return stream;
        }
        
        private void Decode()
        {
            if (m_Stream.Length < 6)
                throw new FormatException();

            if ((byte)Command != m_Stream[0])
                throw new FormatException();

            m_StartAddress = (ushort)(((ushort)m_Stream[1]) << 8 | (ushort)m_Stream[2]);
            m_Quantity = (ushort)(((ushort)m_Stream[3]) << 8 | (ushort)m_Stream[4]);

            int bytesCount = m_Stream[5];
            int expectedBytesCount = m_Quantity / 8;
            if (m_Quantity % 8 != 0)
                ++expectedBytesCount;

            if (expectedBytesCount != bytesCount)
                throw new FormatException();

            if (m_Stream.Length != (6 + bytesCount))
                throw new FormatException();

            m_WriteBitsData = new bool[m_Quantity];

            for( int byteCount = 0; byteCount < bytesCount; byteCount++ )
            {
                int mask = 1;
                for( int i = 0; i < 8; i++ )
                {
                    if (byteCount * 8 + i >= m_Quantity)
                        break;

                    m_WriteBitsData[byteCount * 8 + i] = ((m_Stream[6 + byteCount] & mask) != 0);
                    mask <<= 1;
                }
            }
        }

    }
}
