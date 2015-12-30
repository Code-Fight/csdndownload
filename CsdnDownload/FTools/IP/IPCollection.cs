using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using FTools.HTTP;

namespace FTools.IP
{
    /// <summary>
    /// ip采集
    /// </summary>
    public class IPCollection
    {
        #region 获取ip

        /// <summary>
        /// 获取IP地址--入口
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public string[] GetIp(IPCEntity mode)
        {
            string html = string.Empty;
            List<string> retIpList = new List<string>();
            if (mode == null)
            {
                return null;
            }
            if (mode.Url.Contains("PageIndex"))
            {
                for (int i = 0; i < mode.PageIndexCount; i++)
                {
                    html = GetHtml(mode.Url.Replace("PageIndex", (i + 1).ToString()));
                    if (string.IsNullOrEmpty(html))
                    {
                        return null;
                    }
                    retIpList.AddRange(RexIpArry(html, mode.RexString));
                }
                return retIpList.ToArray();
            }
            html = GetHtml(mode.Url);
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            retIpList.AddRange(RexIpArry(html, mode.RexString));
            return retIpList.ToArray();
        }

        /// <summary>
        /// 获取源码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetHtml(string url)
        {
            CookieContainer cc = new CookieContainer(); //自动处理Cookie对象
            HttpHelpers helper = new HttpHelpers(); //发起请求对象
            HttpItems items = new HttpItems(); //请求设置对象
            HttpResults hr = new HttpResults(); //请求结果
            items.URL = url; //设置请求地址
            items.Container = cc; //自动处理Cookie时,每次提交时对cc赋值即可
            hr = helper.GetHtml(items); //发起请求并得到结果
            return hr.Html; //得到请求结果
        }

        /// <summary>
        /// 通过正则过滤ip
        /// </summary>
        /// <param name="html"></param>
        /// <param name="rexstring"></param>
        /// <returns></returns>
        private string[] RexIpArry(string html, string rexstring)
        {
            Regex reg = new Regex(rexstring);
            MatchCollection mc = reg.Matches(html);
            List<string> retList = new List<string>();
            for (int i = 0; i < mc.Count; i++)
            {
                GroupCollection gc = mc[i].Groups; //得到所有分组 
                if (gc.Count > 1)
                {
                    retList.Add(gc[1].Value + ":" + gc[2].Value);
                }
                else
                {
                    retList.Add(gc[1].Value);
                }

            }
            return retList.ToArray();
        }

        #endregion

    }
}
