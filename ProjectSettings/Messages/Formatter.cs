using System;
using System.Collections.Generic;

namespace Common.Messages;

public enum Format : int
{
    Log = 0,
    Error
}

public class Formatter
{
    public const int TimeCharsCount = 10;
    public const int LabelCharsCount = 18;

    private Format m_Format;

    private string m_Time;
    private string m_Label;
    private string m_Message;

    private string m_TimeString = new(TimeCharsCount);
    private string m_LabelString = new(LabelCharsCount);


    public Formatter(Format format, string time, string label, string message)
    {
        m_Format = format;    
        m_Time = time;
        m_Label = label;
        m_Message = message;

        ApplyFormat();
    }   

    private void ApplyFormat()
    {
        
    }

    public override void ToString()
    {
        return m_TimeString + m_LabelString + m_Message;
    }

}