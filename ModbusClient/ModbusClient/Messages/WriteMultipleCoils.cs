using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModbusClient.Messages
{
    public class WriteMultipleCoils : ModbusMessage
    {
        readonly private ushort m_StartAddress;
        private readonly bool[] m_WriteBitsData;

        public ushort StartAddress { get { return m_StartAddress; } }

        public WriteMultipleCoils(ushort startAddress, bool[] values) : 
            base(ModbusCommand.WriteMultipleCoils)
        {
            if (values == null)
                throw new ArgumentNullException( "values" );
            
            m_WriteBitsData = new bool[values.Length];
            values.CopyTo(m_WriteBitsData, 0);
            m_StartAddress = startAddress;
        }

        public bool[] GetWriteBitsData()
        {
            bool[] copy = new bool[m_WriteBitsData.Length];
            m_WriteBitsData.CopyTo(copy, 0);
            return copy;
        }

        public override byte[] ToStream()
        {
            int quantity = m_WriteBitsData.Length;
            int byteCount = quantity / 8;
            if (quantity % 8 != 0)
                ++byteCount;

            byte[] stream = new byte[6 + byteCount ];

            stream[0] = (byte)Command;

            stream[1] = (byte)(m_StartAddress >> 8);
            stream[2] = (byte)(m_StartAddress & 0xFF);

            stream[3] = (byte)(quantity >> 8);
            stream[4] = (byte)(quantity & 0xFF);

            stream[5] = (byte)byteCount;

            for( int nByte = 0; nByte < byteCount; nByte++ )
            {
                byte value = 0;
                byte mask = 1;
                for (int i = 0; i < 8; i++)
                {
                    if (nByte * 8 + i >= quantity)
                        break;

                    if (m_WriteBitsData[nByte * 8 + i])
                        value |= mask;

                    mask <<= 1;
                }
                stream[6 + nByte] = value;
            }

            return stream;
        }


        public override bool DecodeResponse(byte[] stream, int startIndex)
        {
            if( stream == null || startIndex >= int.MaxValue - 4 || HasErrorCode(stream, startIndex))
                return false;

            if (stream.Length - startIndex != 5)
                throw new FormatException();

            if (((ushort)stream[startIndex + 1] << 8 | (ushort)stream[startIndex + 2]) != m_StartAddress)
                throw new FormatException();

            if (((ushort)stream[startIndex + 3] << 8 | (ushort)stream[startIndex + 4]) !=m_WriteBitsData.Length)
                throw new FormatException();

            return true;
        }


    }
}
