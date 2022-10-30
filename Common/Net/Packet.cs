using Common.Messages;
using System.Text;

namespace Common.Net;

public class Packet
{
    public static readonly int IntSize = sizeof(int);
    public static readonly int DoubleSize = sizeof(double);

    public readonly PacketType Type;

    private readonly List<byte> m_Bytes;
    private int m_Index = 0;

    public Packet()
    {
        m_Bytes = new List<byte>();
    }


    public Packet(PacketType type) : this()
    {
        Type = type;
    }

    public Packet(byte[] bytes)
    {
        m_Bytes = new List<byte>(bytes);
        Type = (PacketType)ReadFromStart<int>();
    }
    private byte[] ObjectToBytes(object obj, out int indexOffset)
    {
        byte[] bytes;

        switch (obj)
        {
            case string:
            case DateTime:
                string str = (string)obj ?? ((DateTime)obj).ToString(Settings.DateFormat);

                var strBytes = Encoding.UTF8.GetBytes(str);
                var length = strBytes.Length;
                var lenBytes = BitConverter.GetBytes(length).ToList();

                lenBytes.AddRange(strBytes);

                bytes = lenBytes.ToArray();
                indexOffset = IntSize + length;
                break;
            case int s:
                bytes = BitConverter.GetBytes(s);
                indexOffset = IntSize;
                break;
            case double s:
                bytes = BitConverter.GetBytes(s);
                indexOffset = DoubleSize;
                break;
            default:
                throw new NotImplementedException();
        }

        return bytes;
    }

    public void Write(object obj)
    {
        var bytes = ObjectToBytes(obj, out var offset);
        m_Bytes.AddRange(bytes);
        m_Index += offset;
    }


    public void Insert(object obj)
    {
        var bytes = ObjectToBytes(obj, out var offset);
        m_Bytes.InsertRange(0, bytes);
        m_Index += offset;
    }
    public void RemoveFromStart<T>()
    {
        var type = typeof(T);
        byte[] bytes;

        if (type == typeof(string) || type == typeof(DateTime))
        {
            bytes = m_Bytes.GetRange(m_Index, IntSize).ToArray();
            var length = BitConverter.ToInt32(bytes);
            m_Index -= IntSize;

            m_Bytes.RemoveRange(m_Index, length);
            m_Index -= length;
        }
        else if (type == typeof(int))
        {
            m_Bytes.RemoveRange(m_Index, IntSize);
            m_Index -= IntSize;
        }
        else if (type == typeof(double))
        {
            m_Bytes.RemoveRange(m_Index, DoubleSize);
            m_Index -= DoubleSize;
        }
    }


    public T ReadFromStart<T>()
    {
        var type = typeof(T);
        byte[] bytes;
        object? result = null;

        if (type == typeof(string) || type == typeof(DateTime))
        {
            bytes = m_Bytes.GetRange(m_Index, IntSize).ToArray();
            var length = BitConverter.ToInt32(bytes);
            m_Index += IntSize;

            bytes = m_Bytes.GetRange(m_Index, length).ToArray();
            result = Encoding.UTF8.GetString(bytes);
            m_Index += length;
        }
        else if (type == typeof(int))
        {
            bytes = m_Bytes.GetRange(m_Index, IntSize).ToArray();
            result = BitConverter.ToInt32(bytes);
            m_Index += IntSize;
        }
        else if (type == typeof(double))
        {
            bytes = m_Bytes.GetRange(m_Index, DoubleSize).ToArray();
            result = BitConverter.ToDouble(bytes);
            m_Index += DoubleSize;
        }

        return (T)result;
    }

    public byte[] GetBytes()
    {
        Insert((int)Type);
        return m_Bytes.ToArray();
    }

    public virtual Formatter ToFormatter()
    {
        return new Formatter();
    }
}
