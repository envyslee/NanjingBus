using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingBus.Method
{
    public class Decode
    {
        /// <summary>
        /// 解码包含特殊字符的字符串（包含/u）
        /// </summary>
        /// <param name="srcStr">包含特殊字符的字符串（包含/u）</param>
        /// <returns></returns>
        public static String DecodeStr(String srcStr)
        {
            int currentPos = 0;

            while (srcStr.Contains("/u"))
            {
                currentPos = srcStr.IndexOf("/u");
                string tempStr = srcStr.Substring(currentPos, 6);
                char convertChar = (char)(Convert.ToInt32(tempStr.Substring(2, 4), 16));
                srcStr = srcStr.Replace(tempStr, convertChar.ToString());
            }

            return srcStr;
        }
    }
}
