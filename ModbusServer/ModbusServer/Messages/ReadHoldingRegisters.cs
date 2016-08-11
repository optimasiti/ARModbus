using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModbusServer.Messages
{
    public class ReadHoldingRegisters : ModbusMessage
    {
        private const ModbusCommand MODBUS_COMMAND = ModbusCommand.ReadHoldingRegisters;

        private ushort[] m_ReadWordsData;
        private ushort m_Quantity;
        private ushort m_StartAddress;

        public override ushort Quantity { get { return m_Quantity; } }
        public override ushort StartAddress { get { return m_StartAddress; } }

        private ReadHoldingRegisters( byte[] rawMessage, int startIndex, int size ) : 
            base( MODBUS_COMMAND, rawMessage, startIndex, size)
        {
        }

        public static ReadHoldingRegisters Build( byte[] rawMessage, int startIndex, int size )
        {
            ReadHoldingRegisters message = new ReadHoldingRegisters(rawMessage, startIndex, size);
            message.Decode();

            return message;
        }

        public override void SetReadWordsData(ushort[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            m_ReadWordsData = new ushort[values.Length];
            values.CopyTo(m_ReadWordsData, 0);
        }


        public override byte[] ToStream()
        {
            if (m_ErrorCode != 0)
                return ErrorResponseToStream();

            byte[] stream = new byte[2+m_Quantity*2];

            stream[0] = (byte)MODBUS_COMMAND;
            stream[1] = (byte)(m_Quantity * 2);

            for( int i = 0; i< m_Quantity; i++ )
            {
                stream[i*2 + 2] = (byte)(m_ReadWordsData[i] >> 8);
                stream[i * 2 + 1 + 2] = (byte)(m_ReadWordsData[i] & 0xFF);
            }

            return stream;
        }

        private void Decode()
        {
            if (m_Stream.Length != 5)
                throw new FormatException();

            if ((byte)Command != m_Stream[0])
                throw new FormatException();

            m_StartAddress = (ushort)(((ushort)m_Stream[1]) << 8 | (ushort)m_Stream[2]);
            m_Quantity = (ushort)(((ushort)m_Stream[3]) << 8 | (ushort)m_Stream[4]);
        }
    }
}
