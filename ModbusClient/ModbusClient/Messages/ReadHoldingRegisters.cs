using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusClient.Messages
{
    public class ReadHoldingRegisters : ModbusMessage
    {
        private readonly ushort m_StartAddress;
        private readonly ushort m_Quantity;
        ushort[] m_ReadWordsData;

        public ushort StartAddress { get { return m_StartAddress; } }
        public ushort Quantity { get { return m_Quantity; } }

        public ReadHoldingRegisters( ushort startAddress, ushort quantity ) : 
            base( ModbusCommand.ReadHoldingRegisters )
        {
            m_StartAddress = startAddress;
            m_Quantity = quantity;
        }

        public override ushort[] GetReadWordsData()
        {
            if (m_ReadWordsData == null)
                return null;

            ushort[] copy = new ushort[m_ReadWordsData.Length];
            m_ReadWordsData.CopyTo(copy, 0);
            return copy;
        }


        public override byte[] ToStream()
        {
            byte[] stream = new byte[5];

            stream[0] = 0x03;

            stream[1] = (byte)(m_StartAddress >> 8);
            stream[2] = (byte)(m_StartAddress & 0xFF);

            stream[3] = (byte)(m_Quantity >> 8);
            stream[4] = (byte)(m_Quantity & 0xFF);

            return stream;
        }
        
        public override bool DecodeResponse(byte[] stream, int startIndex )
        {
            if(stream == null || HasErrorCode(stream, startIndex ))
                return false;

            if( startIndex >= int.MaxValue - m_Quantity*2 - 2 ||
                stream.Length          != startIndex + m_Quantity*2 + 2   ||
                stream[startIndex]      != (byte)Command              || 
                stream[startIndex + 1]  != m_Quantity*2 )
            {
                throw new FormatException();
            }

            m_ReadWordsData = new ushort[m_Quantity];
            ushort value;

            for( int i = 0; i < m_Quantity; i++ )
            {
                value = (ushort)(stream[startIndex + i * 2 + 2]<<8);
                value = (ushort)(value | stream[startIndex + i * 2 + 2 + 1]);
                
                m_ReadWordsData[i] = value;
            }

            return true;
        }
    }
}
