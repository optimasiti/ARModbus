using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusClient.Messages
{
    public abstract class ModbusMessage
    {
        public enum ModbusCommand
        {
            ReadCoils = 0x01,
            ReadHoldingRegisters = 0x03, 
            WriteMultipleCoils = 0x0F, 
            WriteMultipleRegisters = 0x10
        };

        private readonly ModbusCommand m_Command;
        private byte m_ModbusErrorCode = 0; 
               
        public ModbusCommand Command { get { return m_Command; } }
        public byte ModbusErrorCode { get { return m_ModbusErrorCode; } }

        protected ModbusMessage( ModbusCommand modbusCommand )
        {
            m_Command = modbusCommand;
        }

        public abstract byte[] ToStream();
        public abstract bool DecodeResponse(byte[] stream, int startIndex );

        public virtual ushort[] GetReadWordsData()
        {
            throw new NotImplementedException();
        }

        public virtual bool[] GetReadBitsData()
        {
            throw new NotImplementedException();
        }

        protected bool HasErrorCode(byte[] stream, int startIndex )
        {
            if (stream == null || startIndex >= int.MaxValue - 2 || stream.Length < (startIndex + 2))
                return false;

            if( stream[startIndex] == (byte)m_Command + 0x80 )
            {
                m_ModbusErrorCode = stream[startIndex+1];
                return true;
            }

            return false;
        }
    }
}
