using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatikSDavidom.Components.Packet
{
    public class Packet
    {
        private readonly int StringSize = 4;
        private readonly int IntSize = 4;
        private readonly int DoubleSize = 8;

        private List<byte> m_Bytes;
        private int m_Index;

        public Packet()
        {
            m_Bytes = new List<byte>();
        }

        public Packet(byte[] bytes)
        {
            m_Bytes = new List<byte>(bytes);
            m_Index = m_Bytes.Count;
        }

        public void Write(object obj)
        {
            byte[] bytes;

            switch (obj)
            {
                case string s:
                    m_Index += StringSize;
                    bytes = Encoding.UTF8.GetBytes(s);
                    break;
                case int s:
                    m_Index += IntSize;
                    bytes = BitConverter.GetBytes(s);
                    break;
                case double s:
                    m_Index += DoubleSize;
                    bytes = BitConverter.GetBytes(s);
                    break;
                case DateTime s:
                    m_Index += StringSize;
                    bytes = Encoding.UTF8.GetBytes(s.ToString("HH:mm"));
                    break;
                default:
                    throw new NotImplementedException();
            }

            m_Bytes.AddRange(bytes);
        }

        public object? ReadFromEnd<T>()
        {
            var type = typeof(T);
            byte[] bytes;
            object? result = null;

            if (type == typeof(string) || type == typeof(DateTime))
            {
                bytes = m_Bytes.GetRange(m_Index - StringSize, StringSize).ToArray();
                result = Encoding.UTF8.GetString(bytes);
                m_Index -= StringSize;
            }
            else if (type == typeof(int))
            {
                bytes = m_Bytes.GetRange(m_Index - IntSize, IntSize).ToArray();
                result = BitConverter.ToInt32(bytes);
                m_Index -= IntSize;
            }
            else if (type == typeof(double))
            {
                bytes = m_Bytes.GetRange(m_Index - DoubleSize, DoubleSize).ToArray();
                result = BitConverter.ToDouble(bytes);
                m_Index -= DoubleSize;
            }

            return result;
        }

        public byte[] GetBytes()
        {
            return m_Bytes.ToArray();
        }
    }
}
