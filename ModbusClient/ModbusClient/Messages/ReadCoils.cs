using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ModbusClient.Messages
{
    public class ReadCoils : ModbusMessage
    {
        readonly private ushort m_StartAddress;
        readonly private ushort m_Quantity;
        private bool[] m_ReadBitsData;

        public ushort StartAddress { get { return m_StartAddress; } }
        public ushort Quantity { get { return m_Quantity; } }

        public ReadCoils(ushort startAddress, ushort quantity) : base(ModbusCommand.ReadCoils)
        {
            m_StartAddress = startAddress;
            m_Quantity = quantity;
        }
        
        public override bool[] GetReadBitsData()
        {
            if (m_ReadBitsData == null)
                return null;

            bool[] copy = new bool[m_ReadBitsData.Length];
            m_ReadBitsData.CopyTo(copy, 0);
            return copy;
        }

        public override byte[] ToStream()
        {
            byte[] stream = new byte[5];

            stream[0] = (byte)Command;

            stream[1] = (byte)(m_StartAddress >> 8);
            stream[2] = (byte)(m_StartAddress & 0xFF);

            stream[3] = (byte)(m_Quantity >> 8);
            stream[4] = (byte)(m_Quantity & 0xFF);

            return stream;
        }

        public override bool DecodeResponse(byte[] stream, int startIndex)
        {
            if(stream == null || HasErrorCode(stream, startIndex))
                return false;

            int quantity = m_Quantity / 8;
            if(m_Quantity % 8 != 0 )
                ++quantity;

            if (startIndex >= int.MaxValue - quantity - 2 ||
                stream.Length != startIndex + quantity + 2 ||
                stream[startIndex] != (byte)Command ||
                stream[startIndex + 1] != quantity )
            {
                throw new FormatException();
            }

            m_ReadBitsData = new bool[m_Quantity];
            byte value;
                        
            for (int bytePos = 0; bytePos < quantity; bytePos++)
            {
                value =  stream[startIndex+2+bytePos];
                for( int bitPos = 0; bitPos < 8; ++bitPos )
                {
                    if ((bytePos * 8 + bitPos) >= m_Quantity)
                        break;

                    m_ReadBitsData[bytePos * 8 + bitPos] = (value & 0x01) == 1;
                    value >>= 1;
                }
            }

            return true;
        }

    }
}
