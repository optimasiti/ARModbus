using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusServer.Messages
{
    public class ModbusMessage 
    {
        public enum ModbusCommand
        {
            ReadCoils              = 0x01,
            ReadHoldingRegisters  = 0x03, 
            WriteMultipleCoils    = 0x0F,
            WriteMultipleRegisters= 0x10
        };

        private readonly ModbusCommand m_Command;
        protected byte m_ErrorCode = 0;
        protected byte[] m_Stream;   
        
        public ModbusCommand Command { get { return m_Command; } }
        public virtual ushort Quantity { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public virtual ushort StartAddress { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        
        public ModbusMessage( ModbusCommand modbusCommand, byte[] stream, int startIndex, int streamSize )
        {
            m_Command = modbusCommand;
            m_Stream = new byte[streamSize - startIndex];

            Array.Copy( stream, startIndex, m_Stream, 0, streamSize - startIndex );
        }

        public void SetErrorCode( byte value )
        {
            m_ErrorCode = value;
        }

        protected byte[] ErrorResponseToStream()
        {
            return new byte[2] { (byte)((byte)m_Command + 0x80), m_ErrorCode };
        }

        public virtual void SetReadWordsData(ushort[] values)
        {
            throw new NotImplementedException();
        }

        public virtual void SetReadBitsData( bool[] values )
        {
            throw new NotImplementedException();
        }

        public virtual bool[] GetWriteBitsData()
        {
            throw new NotImplementedException();
        }

        public virtual ushort[] GetWriteWordsData()
        {
            throw new NotImplementedException();
        }

        public virtual byte[] ToStream()
        {
            throw new NotImplementedException(); 
        }
        
    }
}
