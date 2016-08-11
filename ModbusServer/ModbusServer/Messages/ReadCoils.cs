using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModbusServer.Messages
{
    public class ReadCoils : ModbusMessage
    {
        private const ModbusCommand MODBUS_COMMAND = ModbusCommand.ReadCoils;
        
        private bool[] m_ReadBitsData;
        private ushort m_StartAddress;
        private ushort m_Quantity;

        public override ushort StartAddress { get { return m_StartAddress; } }
        public override ushort Quantity { get { return m_Quantity;} }


        private ReadCoils(byte[] stream, int startIndex, int streamSize) : 
                            base( MODBUS_COMMAND, stream, startIndex, streamSize )
        {
        }

        public static ReadCoils Build(byte[] stream, int startIndex, int streamSize)
        {
            ReadCoils message = new ReadCoils(stream, startIndex, streamSize);
            message.Decode();

            return message;
        }

        public override void SetReadBitsData(bool[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            m_ReadBitsData = new bool[values.Length];
            values.CopyTo(m_ReadBitsData, 0);
        }

        public override byte[] ToStream()
        {
            if (m_ErrorCode != 0)
                return ErrorResponseToStream();

            int count = m_Quantity / 8;
            if (m_Quantity % 8 != 0)
                ++count;

            byte[] stream = new byte[2 + count];

            stream[0] = (byte)MODBUS_COMMAND;
            stream[1] = (byte)(count);

            byte value;
            byte mask;

            for (int bytePos = 0; bytePos < count; ++bytePos)
            {
                value = 0;

                for( int bitPos = 0; bitPos < 8; ++bitPos )
                {
                    if (bytePos * 8 + bitPos >= m_Quantity)
                        break;

                    mask = (byte)(m_ReadBitsData[bytePos * 8 + bitPos] ? 1 : 0);
                    mask <<= bitPos;

                    value |= mask;
                }
                stream[2+bytePos] = value;
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

            if (Command != MODBUS_COMMAND)
                throw new FormatException();
        }

    }
}
