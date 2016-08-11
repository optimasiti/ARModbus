using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModbusServer.Messages
{
    public class WriteMultipleRegisters : ModbusMessage
    {
        private const ModbusCommand MODBUS_COMMAND = ModbusCommand.WriteMultipleRegisters;

        private ushort[] m_WriteWordsData;
        private ushort m_StartAddress;
        private ushort m_Quantity;


        public override ushort StartAddress { get { return m_StartAddress; } }
        public override ushort Quantity { get { return m_Quantity; } }


        private WriteMultipleRegisters(byte[] rawMessage, int startIndex, int size) : 
            base( MODBUS_COMMAND, rawMessage, startIndex, size)
        {
        }

        public static WriteMultipleRegisters Build(byte[] rawMessage, int startIndex, int size)
        {
            WriteMultipleRegisters message = new WriteMultipleRegisters(rawMessage, startIndex, size);
            message.Decode();

            return message;
        }

        public override ushort[] GetWriteWordsData()
        {
            ushort[] copy = new ushort[m_WriteWordsData.Length];
            m_WriteWordsData.CopyTo(copy, 0);
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

            int valuesSize = m_Stream[5];

            if (valuesSize != m_Quantity * 2)
                throw new FormatException();

            if (m_Stream.Length != (6 + valuesSize))
                throw new FormatException();

            m_WriteWordsData = new ushort[m_Quantity];

            for( int i = 0; i < m_Quantity; i++ )
                m_WriteWordsData[i] = (ushort)(((ushort)m_Stream[6+i*2]) << 8 | (ushort)m_Stream[6+i*2+1]);
        }

    }
}
