using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bll.Email;
using FTools.AntiCode;
using FTools.Encode;
using FTools.HTTP;

namespace Bll
{
    /// <summary>
    /// CSDN账号注册逻辑
    /// </summary>
    public class RegBll
    {
        Email01Bll email01=new Email01Bll();
        System.Net.CookieContainer cc = new System.Net.CookieContainer();//自动处理Cookie对象
        private int antiIndex = 0;
        AntiHelper antiHelper = new AntiHelper();

        public string GetVcode()
        {
            while (true)
            {
                antiIndex = AntiHelper.LoadCdsFromFile(System.Environment.CurrentDirectory + "\\csdn.cds", "csdn20160126");
                string url = "http://passport.csdn.net/ajax/verifyhandler.ashx?rand=" + DateTime.Now.ToString("yyyyMMddHHmmss");//请求地址
                string res = string.Empty;//请求结果,请求类型不是图片时有效
                HttpHelpers helper = new HttpHelpers();//发起请求对象
                HttpItems items = new HttpItems();//请求设置对象
                HttpResults hr = new HttpResults();//请求结果
                items.URL = url;//设置请求地址
                items.Container = cc;//自动处理Cookie时,每次提交时对cc赋值即可
                items.ResultType = ResultType.Byte;//设置请求返回值类型为Byte
                StringBuilder vCodeResult = new StringBuilder('\0', 256);
                byte[] img = helper.GetHtml(items).ResultByte;
                if (AntiHelper.GetVcodeFromBuffer(antiIndex, img, img.Length, vCodeResult))
                {
                    items=new HttpItems();
                    items.URL = "http://passport.csdn.net/account/register?action=validateCode&validateCode=" + vCodeResult;
                    items.Container = cc;//自动处理Cookie时,每次提交时对cc赋值即可
                    hr = helper.GetHtml(items);//发起请求并得到结果
                    if (!hr.Html.Contains("false"))
                    {
                        return vCodeResult.ToString();
                        
                    }
                    
                }
                
                Thread.Sleep(1000);
            }
           
            
        }

        public void Reg()
        {
            string _email = email01.GetEmail();

        }

        private void Reg_SendEamil(string email,string vCode)
        {
            string username = email.Split('@')[0];
            string pwd = DateTime.Now.ToString("HHmmss")+username;
            StringBuilder postData=new StringBuilder();
            postData.Append("fromUrl=http%3A%2F%2Fwww.csdn.net%2F&userName=");
            postData.Append(username);
            postData.Append("&email=" + Encode.UrlEncode(email));
            postData.Append("&password=" + pwd);
            postData.Append("&confirmpassword=" + pwd);
            postData.Append("&validateCode=" + vCode + "&agree=on");
            HttpHelpers helper = new HttpHelpers(); //发起请求对象
            HttpItems items = new HttpItems(); //请求设置对象
            HttpResults hr = new HttpResults(); //请求结果
            items.URL = "http://passport.csdn.net/account/register?action=saveUser&isFrom=true"; //设置请求地址
            items.Postdata = postData.ToString();
            items.Method = "Post";
            items.Container = cc; //自动处理Cookie时,每次提交时对cc赋值即可
            hr = helper.GetHtml(items); //发起请求并得到结果
            if (hr.Html.Contains("请在24小时内点击邮件中的链接继续完成注册"))
            {
                
            }
        }
    }
}
