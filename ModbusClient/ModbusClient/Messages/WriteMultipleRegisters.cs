using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModbusClient.Messages
{
    public class WriteMultipleRegisters : ModbusMessage
    {
        readonly private ushort m_StartAddress;
        private readonly ushort[] m_WriteWordsData;

        public ushort StartAddress { get { return m_StartAddress; } }

        public WriteMultipleRegisters(ushort startAddress, ushort[] values) : 
            base(ModbusCommand.WriteMultipleRegisters)
        {
            if (values == null)
                throw new ArgumentException("ushort[] values");

            m_WriteWordsData = new ushort[values.Length];
            values.CopyTo(m_WriteWordsData, 0);
            m_StartAddress = startAddress;
        }

        public ushort[] GetWriteWordsData()
        {
            ushort[] copy = new ushort[m_WriteWordsData.Length];
            m_WriteWordsData.CopyTo(copy, 0);
            return copy;
        }

        public override byte[] ToStream()
        {
            int quantity = m_WriteWordsData.Length;
            byte[] stream = new byte[6+quantity*2];

            stream[0] = (byte)Command;

            stream[1] = (byte)(m_StartAddress >> 8);
            stream[2] = (byte)(m_StartAddress & 0xFF);

            stream[3] = (byte)(quantity >> 8);
            stream[4] = (byte)(quantity & 0xFF);

            stream[5] = (byte)(quantity * 2);

            for( int i = 0; i < quantity; i++ )
            {
                stream[6 + i * 2]       = (byte)((m_WriteWordsData[i] >> 8) & 0xFF);
                stream[6 + i * 2 + 1]   = (byte)(m_WriteWordsData[i] & 0xFF);
            }

            return stream;
        }

        public override bool DecodeResponse(byte[] stream, int startIndex)
        {
            if( stream == null || startIndex >= int.MaxValue - 4 || HasErrorCode(stream, startIndex))
                return false;

            if (stream.Length - startIndex != 5)
                throw new FormatException();

            if (((ushort)stream[startIndex+1] << 8 | (ushort)stream[startIndex+2]) != m_StartAddress )
                throw new FormatException();

            if (((ushort)stream[startIndex+3] << 8 | (ushort)stream[startIndex+4]) != m_WriteWordsData.Length)
                throw new FormatException();

            return true;
        }
        

    }
}
