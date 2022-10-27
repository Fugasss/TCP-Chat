using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messages;

public class Formatter
{
    public const int TimeCharsCount = 12;
    public const int LabelCharsCount = 20;

    private string m_Time;
    private string m_Label;
    private string m_Message;

    private char[] m_TimeString = new char[TimeCharsCount];
    private char[] m_LabelString = new char[LabelCharsCount];

    public Formatter(string time, string label, string message)
    {
        m_Time = time;
        m_Label = label;
        m_Message = message;

        ApplyFormat(m_TimeString, m_Time);
        ApplyFormat(m_LabelString, m_Label);
    }

    private void ApplyFormat(char[] str, string input)
    {
        int i = -1;

        if (str.Length > input.Length)
        {
            while (i++ < input.Length - 1)
                str[i] = input[i];
        }
        else
        {
            while (i++ < str.Length - 4)
                str[i] = input[i];

            str[str.Length - 3] = '.';
            str[str.Length - 2] = '.';
            str[str.Length - 1] = ' ';
        }
    }

    public override string ToString()
    {
        var time = new string(m_TimeString);
        var label = new string(m_LabelString);
        var result = time.AlignCenter(TimeCharsCount) + label.AlignCenter(LabelCharsCount) + m_Message;
        return result;
    }
}