//---------------------------------------------------------------------------------------
// Copyright (c) SH 2020-05-21
// Author: LJP
// Date: 2020-05-21
// Description: 字符串辅助方法
//---------------------------------------------------------------------------------------
using System.Text;





namespace ActClient
{
    public static class StringBuilderEx
    {
        /// <summary>
        /// 无CG tostring ,暂时还不能用
        /// </summary>
        /// 转出的字符格式有问题
        /// 如果能使用内存拷贝就更好了
        static public void ToStringEX(this StringBuilder string_builder, string output )
        {
            
            unsafe
            {
                fixed(char* ptr = output )
                {
                    char* temp = ptr;
                    int len = string_builder.Length;
                    for (int i = 0; i < len; i++)
                    {
                        *temp = string_builder[i];
                        temp += 1;
                    }
                    *temp = '\0';
                }
            }
        }
    }


    /// <summary>
    /// 字符串辅助方法
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// StringBuilder
        /// </summary>
        static readonly StringBuilder sb = new StringBuilder(512);
        static string   helpstring = new string(' ', 512);

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <returns></returns>
        static public string Format(string format, params object[] args)
        {
            sb.Length = 0;
            sb.AppendFormat(format, args);
            //sb.ToStringEX(helpstring);
            //return helpstring.Trim();
            return sb.ToString();
        }
    }
}