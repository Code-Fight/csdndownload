using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using FTools.HTTP;

namespace Bll
{
    public class EmailBll
    {
        public EmailBll(CookieContainer _cc)
        {
            cc = _cc;//设置cookie
        }
        /// <summary>
        /// 自动处理Cookie对象
        /// </summary>
        public  CookieContainer cc { get; set; }

        /// <summary>
        /// 执行Http请求方法
        /// </summary>
        public void GetHtml()
        {
           
           
        }

        public void FirstQuest()
        {
            string url = "http://10minutemail.com/10MinuteMail/"; //请求地址
            string res = string.Empty; //请求结果,请求类型不是图片时有效

            HttpHelpers helper = new HttpHelpers(); //发起请求对象
            HttpItems items = new HttpItems(); //请求设置对象
            HttpResults hr = new HttpResults(); //请求结果
            items.ProxyIp = "120.195.201.140:80";
            items.URL = url; //设置请求地址
            items.Container = cc; //自动处理Cookie时,每次提交时对cc赋值即可
            hr = helper.GetHtml(items); //发起请求并得到结果
            res = hr.Html; //得到请求结果
            GetViewStateId(res);
            GetEmail(res);

        }

        /// <summary>
        /// 获取ViewStateId
        /// </summary>
        /// <param name="reString"></param>
        /// <returns></returns>
        private string GetViewStateId(string reString)
        {
            string regexCode = "</span><input type=\"hidden\" name=\"javax.faces.ViewState\" id=\"javax.faces.ViewState\" value=\"(.*?)\"";
            Regex reg = new Regex(regexCode);
            MatchCollection mc = reg.Matches(reString);
            for (int i = 0; i < mc.Count; i++)
            {
                GroupCollection gc = mc[i].Groups; //得到所有分组 
                for (int j = 1; j < gc.Count; j++) //多分组 匹配的原始文本不要
                {
                    string temp = gc[j].Value;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        return temp;
                    }
                }
            }
            return string.Empty;

        }

        /// <summary>
        /// 获取ViewStateId
        /// </summary>
        /// <param name="reString"></param>
        /// <returns></returns>
        private string GetEmail(string reString)
        {
            string regexCode = "<br/>(.*?) is your temporary e-mail address.";
            Regex reg = new Regex(regexCode);
            MatchCollection mc = reg.Matches(reString);
            for (int i = 0; i < mc.Count; i++)
            {
                GroupCollection gc = mc[i].Groups; //得到所有分组 
                for (int j = 1; j < gc.Count; j++) //多分组 匹配的原始文本不要
                {
                    string temp = gc[j].Value;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        return temp;
                    }
                }
            }
            return string.Empty;
        }
    }
}
