using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTools.HTTP;

namespace Bll.IP
{
    public class IpCollectionHttpBll
    {


        //TODO:写采集




        /// <summary>
        /// 访问网页
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetHtml(string url)
        {
            System.Net.CookieContainer cc = new System.Net.CookieContainer();//自动处理Cookie对象
            HttpHelpers helper = new HttpHelpers();//发起请求对象
            HttpItems items = new HttpItems();//请求设置对象
            HttpResults hr = new HttpResults();//请求结果
            items.URL = url;//设置请求地址
            items.Container = cc;//自动处理Cookie时,每次提交时对cc赋值即可
            hr = helper.GetHtml(items);//发起请求并得到结果
            return hr.Html;//得到请求结果
        }
    }
}
