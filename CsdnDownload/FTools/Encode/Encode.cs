using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FTools.Encode
{
    /// <summary>
    /// 类型转换
    /// </summary>
    public static class Encode
    {
        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string UrlEncode(string param)
        {
            return System.Web.HttpUtility.UrlEncode(param, System.Text.Encoding.UTF8);
            

        }

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string UrlDecode(string code)
        {
            return System.Web.HttpUtility.UrlDecode(code, System.Text.Encoding.UTF8); 
           
        }

        public static string UnicodeEn(string param)
        {
            string outStr = string.Empty;
            try
            {
                for (int i = 0; i < param.Length; i++)
                {
                    outStr += "\\u" + ((int)param[i]).ToString("x");
                }
            }
            catch
            {
                outStr = "To Unicode Error";
            }
            return outStr;
            
        }

        public static string UnicodeDe(string param)
        {
            string[] strlist = param.Replace("\\", "").Split('u');
            string outStr = string.Empty;
            if (!param.Contains("\\u"))
            {
                return "正确的Unicode格式: \\u 与 10/16 进制数字";
            }
            for (int i = 1; i < strlist.Length; i++)
            {
                try
                {
                    outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    continue;
                }
            }

            return outStr;
        }
    }


    
}
