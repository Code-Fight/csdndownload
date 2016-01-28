using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using FTools.HTTP;

namespace Bll.Email
{
    /// <summary>
    /// 10分钟邮箱：https://10minutemail.net/
    /// </summary>
    public class Email01Bll
    {
        System.Net.CookieContainer cc = new System.Net.CookieContainer();//自动处理Cookie对象


        #region 获取一个邮箱地址

        /// <summary>
        /// 获取一个邮箱地址
        /// </summary>
        /// <returns></returns>
        public string GetEmail()
        {
            string url = "https://10minutemail.net/"; //请求地址
            string res = string.Empty; //请求结果,请求类型不是图片时有效

            HttpHelpers helper = new HttpHelpers(); //发起请求对象
            HttpItems items = new HttpItems(); //请求设置对象
            HttpResults hr = new HttpResults(); //请求结果
            items.URL = url; //设置请求地址
            items.Container = cc; //自动处理Cookie时,每次提交时对cc赋值即可
            hr = helper.GetHtml(items); //发起请求并得到结果
            res = hr.Html; //得到请求结果
            Regex reg = new Regex("data-clipboard-text=\"(.*?)\"><i class=\"fa");
            MatchCollection mc = reg.Matches(res);
            //string GroupsResult = string.Empty;
            for (int i = 0; i < mc.Count; i++)
            {
                GroupCollection gc = mc[i].Groups; //得到所有分组 
                for (int j = 1; j < gc.Count; j++) //多分组 匹配的原始文本不要
                {
                    string temp = gc[j].Value;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        //GroupsResult += string.Format("Groups 【{0}】   Values 【{1}】", (j).ToString(), temp); //获取结果
                        return temp;
                    }
                }
            }
            return string.Empty;
        }

        #endregion


        #region 获取激活链接

        /// <summary>
        /// 获取激活链接
        /// </summary>
        /// <returns></returns>
        public string GetRegLinkByEmail()
        {
            string ret;
            do
            {
                Thread.Sleep(500); //2秒 尝试有尝试一次获取激活链接
                ret = GetNewMail();
                if (ret.Length>0)
                {
                    ret = ret.Replace("amp;", "");
                }
                if (ret.Contains("操作超时"))
                {
                    ret = GetNewMail();
                }
            } while (!(ret.Length > 0));
            return ret;
        }

        private string GetNewMail()
        {
            string url = "https://10minutemail.net/mailbox.ajax.php?_=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                //请求地址
            string res = string.Empty; //请求结果,请求类型不是图片时有效
            HttpHelpers helper = new HttpHelpers(); //发起请求对象
            HttpItems items = new HttpItems(); //请求设置对象
            HttpResults hr = new HttpResults(); //请求结果
            items.URL = url; //设置请求地址
            items.IsAjax = true;
            items.Container = cc; //自动处理Cookie时,每次提交时对cc赋值即可
            hr = helper.GetHtml(items); //发起请求并得到结果
            res = hr.Html; //得到请求结果
            if (!res.Contains("CSDN"))
            {
                return string.Empty;
            }
            Regex reg = new Regex("<a href=\"(.*?)\">");
            MatchCollection mc = reg.Matches(res);
            List<string> GroupsResult = new List<string>();
            for (int i = 0; i < mc.Count; i++)
            {
                GroupCollection gc = mc[i].Groups; //得到所有分组 
                for (int j = 1; j < gc.Count; j++) //多分组 匹配的原始文本不要
                {
                    string temp = gc[j].Value;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        GroupsResult.Add(temp); //获取结果
                    }
                }
            }
            foreach (var temp in GroupsResult)
            {
                if (!temp.Contains("welcome"))
                {
                    items.URL = "https://10minutemail.net/" + temp; //设置请求地址
                    //items.IsAjax = true;
                    items.Container = cc; //自动处理Cookie时,每次提交时对cc赋值即可
                    hr = helper.GetHtml(items); //发起请求并得到结果
                    res = hr.Html; //得到请求结果
                    if (res.Contains("感谢您注册CSDN社区"))
                    {
                        return GetJiHuoHerf(res);
                    }
                    break;
                }
            }
            return string.Empty;
        }

        private string GetJiHuoHerf(string res)
        {
            Regex reg = new Regex("感谢您注册CSDN社区，您只需要点击下面链接，激活您的帐户，您便可以享受CSDN各项服务。<br /><br /><a href=\"(.*?)\">");
            MatchCollection mc = reg.Matches(res);
            List<string> GroupsResult = new List<string>();
            for (int i = 0; i < mc.Count; i++)
            {
                GroupCollection gc = mc[i].Groups; //得到所有分组 
                for (int j = 1; j < gc.Count; j++) //多分组 匹配的原始文本不要
                {
                    string temp = gc[j].Value;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        GroupsResult.Add(temp); //获取结果
                    }
                }
            }
            if (GroupsResult.Count == 1)
            {
                return GroupsResult[0];
            }
            return string.Empty;
        }

        #endregion

    }
}
