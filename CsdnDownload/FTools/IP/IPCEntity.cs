using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTools.IP
{
    /// <summary>
    /// ip采集信息
    /// </summary>
    public class IPCEntity
    {
        /// <summary>
        /// url 地址用PageIndex代替 没有不写即可
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 正则 如果ip和port在一起，直接一个就可以，如果不在一起请使用两个（.*？） 不要把正则分开
        /// </summary>
        public string RexString { get; set; }

        /// <summary>
        /// 如果要翻页取 填写要取的页数
        /// </summary>
        public int PageIndexCount { get; set; }
        
    }
}
