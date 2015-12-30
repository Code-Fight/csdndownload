﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;


namespace FTools.HTTP
{
    partial class HttpRequest
    {
        /// <summary>
        /// 请求结果类
        /// </summary>
        public HttpResults result = new HttpResults();
        /// <summary>
        ///  HttpWebRequest对象用来发起请求
        /// </summary>
        public HttpWebRequest request = null;
        /// <summary>
        ///  获取响应流的数据对象
        /// </summary>
        public HttpWebResponse response = null;
        /// <summary>
        ///    响应流对象
        /// </summary>
        public Stream streamResponse;
        /// <summary>
        /// 异步回调函数
        /// </summary>
        public Action<HttpResults> callBack;
        /// <summary>
        /// 基础请求设置类
        /// </summary>
        public HttpItems objHttpCodeItem;
        /// <summary>
        /// 辅助转换数据的内存流
        /// </summary>
        public MemoryStream MemoryStream = new MemoryStream();
        /// <summary>
        /// 信号量
        /// </summary>
        public int m_semaphore = 0;
        /// <summary>
        ///  默认的编码
        /// </summary>
        public Encoding encoding;
    }
    /// <summary>
    /// Http请求操作类 
    /// </summary>
    public class HttpHelpers
    {
        #region 预定义方法或者变更
        /// <summary>
        ///  HttpWebRequest对象用来发起请求
        /// </summary>
        private HttpWebRequest request = null;
        /// <summary>
        ///   获取响应流的数据对象
        /// </summary>
        private HttpWebResponse response = null;
        /// <summary>
        /// 根据相传入的数据，得到相应页面数据
        /// </summary>
        /// <param name="objHttpItems">请求设置参数</param>
        /// <returns>请求结果</returns>
        private HttpResults GetHttpRequestData(HttpItems objHttpItems)
        {
            //返回参数
            HttpResults result = new HttpResults();
            try
            {
                //配置请求参数
                SetRequest(objHttpItems);

                #region 得到请求的response

                result.CookieCollection = new CookieCollection();
                response = (HttpWebResponse)request.GetResponse();

                result.StatusCode = response.StatusCode;
                result.StatusDescription = response.StatusDescription;
                result.Header = response.Headers;
                if (response.Cookies != null)
                {
                    result.CookieCollection = response.Cookies;
                }
                if (response.ResponseUri != null)
                {
                    result.ResponseUrl = response.ResponseUri.ToString();
                }
                if (response.Headers["set-cookie"] != null)
                {
                    //自动剔除垃圾信息 直接返回正常数据
                    result.Cookie = new XJHTTP().ClearCookie(response.Headers["set-cookie"]);
                }
                //处理返回值Container
                result.Container = objHttpItems.Container;
                //急速请求,不获取响应结果
                if (objHttpItems.ResultType == ResultType.So)
                { 
                    return result;
                }
                MemoryStream _stream = new MemoryStream();
                //GZIIP处理
                if (response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    _stream = GetMemoryStream(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress));
                }
                else
                {
                    _stream = GetMemoryStream(response.GetResponseStream());

                }
                //获取Byte
                byte[] RawResponse = _stream.ToArray();
                //是否返回Byte类型数据
                if (objHttpItems.ResultType == ResultType.Byte)
                {
                    result.ResultByte = RawResponse;
                    return result;
                }
                //无视编码--自动识别编码
                if (objHttpItems.Encoding == null)
                {
                    string temp = Encoding.Default.GetString(RawResponse, 0, RawResponse.Length);
                    //<meta(.*?)charset([\s]?)=[^>](.*?)>
                    Match meta = Regex.Match(temp, "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string charter = (meta.Groups.Count > 2) ? meta.Groups[2].Value : string.Empty;
                    if (charter.IndexOf("\"", StringComparison.Ordinal) > 0)
                    {
                        charter = charter.Split('\"')[0];
                    }
                    if (charter.IndexOf(" ", StringComparison.Ordinal) > 0)
                    {
                        charter = charter.Split(' ')[0];
                    }
                    charter = charter.Replace("\"", string.Empty).Replace("'", string.Empty).Replace(";", string.Empty);
                    if (charter.Length > 0)
                    {
                        charter = charter.ToLower().Replace("iso-8859-1", "gbk");
                        if (response.CharacterSet != null && (string.IsNullOrEmpty(response.CharacterSet.Trim()) || response.CharacterSet.Trim().Contains("utf8")))
                        {
                            objHttpItems.Encoding = Encoding.UTF8;
                        }
                        else
                        {
                            if (charter != response.CharacterSet)
                            {
                                objHttpItems.Encoding = Encoding.GetEncoding(charter);
                            }
                            else
                            {
                                objHttpItems.Encoding = Encoding.GetEncoding(response.CharacterSet);
                            }

                        }
                    }
                    else
                    {

                        if (response.CharacterSet != null)
                        {
                            if (response.CharacterSet.ToLower().Trim() == "iso-8859-1")
                            {
                                // encoding = Encoding.GetEncoding("gbk"); 直接改为UTF8编码
                                objHttpItems.Encoding = Encoding.UTF8;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(response.CharacterSet.Trim()) || response.CharacterSet.Trim().Contains("utf8"))
                                {
                                    objHttpItems.Encoding = Encoding.UTF8;
                                }
                                else
                                {
                                    objHttpItems.Encoding = Encoding.GetEncoding(response.CharacterSet);
                                }
                            }
                        }
                        else
                        {
                            //如果识别失败.强制赋值为utf8
                            objHttpItems.Encoding = Encoding.UTF8;
                        }
                    }
                }
                //得到返回的HTML
                try
                {
                    if (RawResponse.Length > 0)
                    {
                        result.Html = objHttpItems.Encoding.GetString(RawResponse);
                    }
                    else
                    {
                        result.Html = "";
                    }
                    _stream.Close();
                    response.Close();
                }
                catch
                {
                    return null;
                }
                //最后释放流


                #endregion
            }
            catch (WebException ex)
            {
                //这里是在发生异常时返回的错误信息
                result.Html = ex.Message;
                response = (HttpWebResponse)ex.Response;
                if (response != null)
                {
                    result.StatusCode = response.StatusCode;
                    result.StatusDescription = response.StatusDescription;
                }
                else
                {
                    result.StatusCode = HttpStatusCode.NotFound;
                    result.StatusDescription = ex.Message;
                }
                if (response != null)
                {
                    #region 错误时读取服务器返回数据
                    MemoryStream _stream = new MemoryStream();
                    //GZIIP处理
                    if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _stream = GetMemoryStream(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress));
                    }
                    else
                    {
                        _stream = GetMemoryStream(response.GetResponseStream());

                    }
                    //获取Byte
                    byte[] RawResponse = _stream.ToArray();
                    //是否返回Byte类型数据
                    if (objHttpItems.ResultType == ResultType.Byte)
                    {
                        result.ResultByte = RawResponse;
                        return result;
                    }
                    //无视编码
                    if (objHttpItems.Encoding == null)
                    {
                        string temp = Encoding.Default.GetString(RawResponse, 0, RawResponse.Length);
                        //<meta(.*?)charset([\s]?)=[^>](.*?)>
                        Match meta = Regex.Match(temp, "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string charter = (meta.Groups.Count > 2) ? meta.Groups[2].Value : string.Empty;
                        if (charter.IndexOf("\"") > 0)
                        {
                            charter = charter.Split('\"')[0];
                        }
                        if (charter.IndexOf(" ") > 0)
                        {
                            charter = charter.Split(' ')[0];
                        }
                        charter = charter.Replace("\"", string.Empty).Replace("'", string.Empty).Replace(";", string.Empty);
                        if (charter.Length > 0)
                        {
                            charter = charter.ToLower().Replace("iso-8859-1", "gbk");
                            objHttpItems.Encoding = Encoding.GetEncoding(charter);
                        }
                        else
                        {

                            if (response.CharacterSet != null)
                            {
                                if (response.CharacterSet.ToLower().Trim() == "iso-8859-1")
                                {
                                    // encoding = Encoding.GetEncoding("gbk"); 直接改为UTF8编码
                                    objHttpItems.Encoding = Encoding.UTF8;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(response.CharacterSet.Trim()) || string.IsNullOrEmpty(charter))
                                    {
                                        objHttpItems.Encoding = Encoding.UTF8;
                                    }
                                    else
                                    {
                                        if (charter != response.CharacterSet)
                                        {
                                            objHttpItems.Encoding = Encoding.GetEncoding(charter);
                                        }
                                        else
                                        {
                                            objHttpItems.Encoding = Encoding.GetEncoding(response.CharacterSet);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //如果识别失败.强制赋值为utf8
                                objHttpItems.Encoding = Encoding.UTF8;
                            }
                        }
                    }
                    //得到返回的HTML
                    try
                    {
                        if (RawResponse.Length > 0)
                        {
                            result.Html = objHttpItems.Encoding.GetString(RawResponse);
                        }
                        else
                        {
                            result.Html = "";
                        }
                        _stream.Close();
                        response.Close();
                    }
                    catch
                    {
                        return null;
                    }
                    //最后释放流 
                    #endregion
                }
                return result;
            }
            if (objHttpItems.IsToLower)
            {
                result.Html = result.Html.ToLower();
            }
            return result;
        }
        /// <summary>
        /// 异步获取响应数据
        /// </summary>
        /// <param name="result"></param>
        private void AsyncResponseData(IAsyncResult result)
        {
            HttpRequest hrt = result.AsyncState as HttpRequest;
            if (System.Threading.Interlocked.Increment(ref hrt.m_semaphore) != 1)
                return;
            try
            {
                hrt.response = (HttpWebResponse)hrt.request.EndGetResponse(result);
                //增加异步Cookie处理遗漏bug
                if (hrt.response.Cookies != null)
                {
                    hrt.result.CookieCollection = hrt.response.Cookies;
                }
                if (hrt.response.Headers["set-cookie"] != null)
                {
                    //自动剔除垃圾信息 直接返回正常数据 
                    hrt.result.Cookie = new XJHTTP().ClearCookie(hrt.response.Headers["set-cookie"]);
                }
                if (hrt.response.ResponseUri != null)
                {
                    hrt.result.ResponseUrl = hrt.response.ResponseUri.ToString();
                }
                //处理返回值Container
                hrt.result.Container = hrt.request.CookieContainer;
                //处理header
                hrt.result.Header = hrt.response.Headers;
                if (hrt.response.ContentEncoding != null && hrt.response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    hrt.streamResponse = new GZipStream(hrt.response.GetResponseStream(), CompressionMode.Decompress);
                }
                else
                {
                    hrt.streamResponse = hrt.response.GetResponseStream();
                }
                hrt.MemoryStream = GetMemoryStream(hrt.streamResponse);
                hrt.result.StatusCode = hrt.response.StatusCode;
                hrt.result.StatusDescription = hrt.response.StatusDescription;
                AsyncCallBackData(hrt);
            }
            catch (WebException ex)
            {

                //这里是在发生异常时返回的错误信息
                hrt.result.Html = ex.Message;
                hrt.response = (HttpWebResponse)ex.Response;
                if (hrt.response != null)
                {
                    hrt.result.StatusCode = hrt.response.StatusCode;
                    hrt.result.StatusDescription = hrt.response.StatusDescription;
                }
                else
                {
                    hrt.result.StatusCode = HttpStatusCode.NotFound;
                    hrt.result.StatusDescription = ex.Message;
                }
                if (hrt.response != null)
                {
                    #region 错误时读取服务器返回数据
                    MemoryStream _stream = new MemoryStream();
                    //GZIIP处理
                    if (hrt.response.ContentEncoding != null && hrt.response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _stream = GetMemoryStream(new GZipStream(hrt.response.GetResponseStream(), CompressionMode.Decompress));
                    }
                    else
                    {
                        _stream = GetMemoryStream(hrt.response.GetResponseStream());

                    }
                    //获取Byte
                    byte[] RawResponse = _stream.ToArray();
                    //是否返回Byte类型数据
                    if (hrt.objHttpCodeItem.ResultType == ResultType.Byte)
                    {
                        hrt.result.ResultByte = RawResponse;
                        AsyncCallBackData(hrt);
                    }
                    //无视编码
                    if (hrt.encoding == null)
                    {
                        string temp = Encoding.Default.GetString(RawResponse, 0, RawResponse.Length);
                        //<meta(.*?)charset([\s]?)=[^>](.*?)>
                        Match meta = Regex.Match(temp, "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string charter = (meta.Groups.Count > 2) ? meta.Groups[2].Value : string.Empty;
                        if (charter.IndexOf("\"") > 0)
                        {
                            charter = charter.Split('\"')[0];
                        }
                        if (charter.IndexOf(" ") > 0)
                        {
                            charter = charter.Split(' ')[0];
                        }
                        charter = charter.Replace("\"", string.Empty).Replace("'", string.Empty).Replace(";", string.Empty);
                        if (charter.Length > 0)
                        {
                            charter = charter.ToLower().Replace("iso-8859-1", "gbk");
                            hrt.encoding = Encoding.GetEncoding(charter);
                        }
                        else
                        {

                            if (hrt.response.CharacterSet != null)
                            {
                                if (hrt.response.CharacterSet.ToLower().Trim() == "iso-8859-1")
                                {

                                    hrt.encoding = Encoding.UTF8;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(hrt.response.CharacterSet.Trim()))
                                    {
                                        hrt.encoding = Encoding.UTF8;
                                    }
                                    else
                                    {
                                        if (charter != response.CharacterSet)
                                        {
                                            hrt.encoding = Encoding.GetEncoding(charter);
                                        }
                                        else
                                        {
                                            hrt.encoding = Encoding.GetEncoding(response.CharacterSet);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //如果识别失败.强制赋值为utf8
                                hrt.encoding = Encoding.UTF8;
                            }
                        }
                    }
                    //得到返回的HTML
                    try
                    {
                        if (RawResponse.Length > 0)
                        {
                            if (hrt.encoding == null)
                            {
                                hrt.encoding = Encoding.UTF8;
                            }
                            hrt.result.Html = hrt.encoding.GetString(RawResponse);
                        }
                        else
                        {
                            hrt.result.Html = "";
                        }
                        _stream.Close();
                        hrt.response.Close();
                    }
                    catch
                    {
                        hrt.result.Html = ex.Message;
                        AsyncCallBackData(hrt);
                    }
                    //最后释放流 
                    #endregion
                }
                hrt.result.Html = ex.Message;
                AsyncCallBackData(hrt);
            }
        }
        /// <summary>
        /// 无视编码
        /// </summary>
        /// <param name="hrt">请求参数</param>
        /// <param name="RawResponse">响应值</param>
        /// <returns></returns>
        HttpRequest GetEncoding(HttpRequest hrt, ref byte[] RawResponse)
        {
            if (hrt.encoding == null)
            {
                string temp = Encoding.Default.GetString(RawResponse, 0, RawResponse.Length);
                Match meta = Regex.Match(temp, "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string charter = (meta.Groups.Count > 2) ? meta.Groups[2].Value : string.Empty;
                if (charter.IndexOf("\"", StringComparison.Ordinal) > 0)
                {
                    charter = charter.Split('\"')[0];
                }
                if (charter.IndexOf(" ", StringComparison.Ordinal) > 0)
                {
                    charter = charter.Split(' ')[0];
                }
                charter = charter.Replace("\"", string.Empty).Replace("'", string.Empty).Replace(";", string.Empty);
                if (charter.Length > 0)
                {
                    charter = charter.ToLower().Replace("iso-8859-1", "gbk").Replace("http-equiv=content-type", "");
                    hrt.encoding = Encoding.GetEncoding(charter.Trim());
                }
                else
                {

                    if (hrt.response.CharacterSet != null)
                    {
                        if (hrt.response.CharacterSet.ToLower().Trim() == "iso-8859-1")
                        {
                            hrt.encoding = Encoding.UTF8;

                        }
                        else
                        {
                            if (string.IsNullOrEmpty(hrt.response.CharacterSet.Trim()))
                            {
                                hrt.encoding = Encoding.UTF8;
                            }
                            else
                            {
                                hrt.encoding = Encoding.GetEncoding(hrt.response.CharacterSet);
                            }
                        }
                    }
                }
            }
            return hrt;
        }
        /// <summary>
        /// 异步 处理/解析数据方法
        /// </summary>
        /// <param name="hrt"></param>
        void AsyncCallBackData(HttpRequest hrt)
        {
            try
            {
                byte[] RawResponse = hrt.MemoryStream.ToArray();
                //无视编码
                hrt = GetEncoding(hrt, ref RawResponse);
                //是否返回Byte类型数据  
                if (hrt.objHttpCodeItem.ResultType == ResultType.Byte)
                {
                    hrt.result.ResultByte = RawResponse;
                }
                //得到返回的HTML
                try
                {
                    if (hrt.encoding == null)
                    {
                        //如果识别失败.强制赋值为utf8
                        hrt.encoding = Encoding.UTF8;
                    }
                    hrt.result.Html = Encoding.UTF8.GetString(RawResponse);
                    hrt.callBack.Invoke(hrt.result);
                }
                catch
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                hrt.result.Html = ex.Message;
                hrt.callBack.Invoke(hrt.result);
            }
        }

        /// <summary>
        ///  根据传入的参数,来异步发起请求
        /// </summary>
        /// <param name="objItems">请求设置参数</param>
        /// <param name="callBack">回调函数</param>
        private void AsyncGetHttpRequestData(HttpItems objItems, Action<HttpResults> callBack)
        {
            HttpRequest hrt = new HttpRequest();
            SetRequest(objItems);
            hrt.objHttpCodeItem = objItems;
            hrt.request = request;
            hrt.callBack = callBack;
            try
            {

                IAsyncResult m_ar = hrt.request.BeginGetResponse(AsyncResponseData, hrt);
                System.Threading.ThreadPool.RegisterWaitForSingleObject(m_ar.AsyncWaitHandle,
                    TimeoutCallback, hrt, objItems.Timeout, true);
            }
            catch
            {
                hrt.result.Html = "TimeOut";
            }
        }
        /// <summary>
        /// 超时回调
        /// </summary>
        /// <param name="state">HttpRequest对象</param>
        /// <param name="timedOut">超时判断</param>
        void TimeoutCallback(object state, bool timedOut)
        {
            HttpRequest pa = state as HttpRequest;
            if (timedOut)
            {
                if (System.Threading.Interlocked.Increment(ref pa.m_semaphore) == 1)
                {
                    pa.result.Html = "TimeOut";
                    pa.callBack.Invoke(pa.result);
                }
            }
        }
        /// <summary>
        /// 获取流中的数据转换为内存流处理
        /// </summary>
        /// <param name="streamResponse">流</param>
        private MemoryStream GetMemoryStream(Stream streamResponse)
        {
            MemoryStream _stream = new MemoryStream();
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = streamResponse.Read(buffer, 0, Length);
            while (bytesRead > 0)
            {
                _stream.Write(buffer, 0, bytesRead);
                bytesRead = streamResponse.Read(buffer, 0, Length);
            }
            return _stream;
        }

        /// <summary>
        /// 为请求准备参数
        /// </summary>
        ///<param name="objHttpItems">参数列表</param> 
        private void SetRequest(HttpItems objHttpItems)
        { 
            // 验证证书
            SetCer(objHttpItems);
            //设置Header参数
            if (objHttpItems.Header != null && objHttpItems.Header.Count > 0)
            {
                try
                {
                    request.Headers = objHttpItems.Header;
                }
                catch
                {
                    return;
                }
            }
            if (objHttpItems.IsAjax)
            {
                request.Headers.Add("x-requested-with: XMLHttpRequest");
            }
            // 设置代理
            SetProxy(objHttpItems);
            //请求方式Get或者Post
            request.Method = objHttpItems.Method;
            request.Timeout = objHttpItems.Timeout;
            request.ReadWriteTimeout = objHttpItems.ReadWriteTimeout;
            //Accept
            request.Accept = objHttpItems.Accept;
            //ContentType返回类型
            request.ContentType = objHttpItems.ContentType;
            //UserAgent客户端的访问类型，包括浏览器版本和操作系统信息
            request.UserAgent = objHttpItems.UserAgent;
            // 编码
            SetEncoding(objHttpItems);
            //设置Cookie
            SetCookie(objHttpItems);
            //来源地址
            request.Referer = objHttpItems.Referer;
            //是否执行跳转功能 
            request.AllowAutoRedirect = objHttpItems.Allowautoredirect;
            //设置最大连接
            if (objHttpItems.Connectionlimit > 0)
            {
                request.ServicePoint.ConnectionLimit = objHttpItems.Connectionlimit;
            }
            //设置 post数据在大于1024时是否分包
            request.ServicePoint.Expect100Continue = objHttpItems.Expect100Continue;
            //设置Post数据
            SetPostData(objHttpItems);
            

        }
        /// <summary>
        /// 设置证书
        /// </summary>
        /// <param name="objHttpItems">请求设置参数</param>
        private void SetCer(HttpItems objHttpItems)
        {
            if (!string.IsNullOrEmpty(objHttpItems.CerPath))
            {
                //这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                //初始化对像，并设置请求的URL地址
                request = (HttpWebRequest)WebRequest.Create(GetUrl(objHttpItems.URL));
                //创建证书文件
                X509Certificate objx509 = new X509Certificate(objHttpItems.CerPath);
                //添加到请求里
                request.ClientCertificates.Add(objx509);
            }
            else
            {
                //初始化对像，并设置请求的URL地址
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(GetUrl(objHttpItems.URL));
                }
                catch
                {
                    return;
                }
            }
        }
        /// <summary>
        /// 设置编码
        /// </summary>
        /// <param name="objHttpItems">Http参数</param>
        private void SetEncoding(HttpItems objHttpItems)
        {
            if (!string.IsNullOrEmpty(objHttpItems.EncodingStr))
            {
                //读取数据时的编码方式
                objHttpItems.Encoding = System.Text.Encoding.GetEncoding(objHttpItems.EncodingStr);
            }
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="objHttpItems">Http参数</param>
        private void SetCookie(HttpItems objHttpItems)
        {
            //获取当前的cookie

            if (!string.IsNullOrEmpty(objHttpItems.Cookie))
            {
                //Cookie
                request.Headers[HttpRequestHeader.Cookie] = objHttpItems.Cookie;
            }
            //设置Cookie

            if (objHttpItems.CookieCollection != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(objHttpItems.CookieCollection);
            }
            if (objHttpItems.Container != null)
            {
                request.CookieContainer = objHttpItems.Container;
            }
        }

        /// <summary>
        /// 设置Post数据
        /// </summary>
        /// <param name="objHttpItems">Http参数</param>
        private void SetPostData(HttpItems objHttpItems)
        {
            //验证在得到结果时是否有传入数据
            if (request.Method.Trim().ToLower().Contains("post"))
            {
                //写入Byte类型
                if (objHttpItems.PostDataType == PostDataType.Byte)
                {
                    //验证在得到结果时是否有传入数据
                    if (objHttpItems.PostdataByte != null && objHttpItems.PostdataByte.Length > 0)
                    {
                        request.ContentLength = objHttpItems.PostdataByte.Length;
                        request.GetRequestStream().Write(objHttpItems.PostdataByte, 0, objHttpItems.PostdataByte.Length);
                    }
                }//写入文件
                else if (objHttpItems.PostDataType == PostDataType.FilePath)
                {
                    StreamReader r = new StreamReader(objHttpItems.Postdata, objHttpItems.Encoding);
                    byte[] buffer =objHttpItems.PostEncoding.GetBytes(r.ReadToEnd()); 
                    r.Close();
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }
                else
                {
                    //验证在得到结果时是否有传入数据
                    if (!string.IsNullOrEmpty(objHttpItems.Postdata))
                    {
                        byte[] buffer = objHttpItems.PostEncoding.GetBytes(objHttpItems.Postdata);
                        request.ContentLength = buffer.Length;
                        request.GetRequestStream().Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }
        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="objHttpItems">参数对象</param>
        private void SetProxy(HttpItems objHttpItems)
        {
            if (!string.IsNullOrEmpty(objHttpItems.ProxyIp))
            {
                //设置代理服务器
                if (objHttpItems.ProxyIp.Contains(":"))
                {
                    string[] plist = objHttpItems.ProxyIp.Split(':');
                    WebProxy myProxy = new WebProxy(plist[0].Trim(), Convert.ToInt32(plist[1].Trim()));
                    //连接凭证
                    if (!string.IsNullOrEmpty(objHttpItems.ProxyUserName) && !string.IsNullOrEmpty(objHttpItems.ProxyPwd))
                    {
                        myProxy.Credentials = new NetworkCredential(objHttpItems.ProxyUserName, objHttpItems.ProxyPwd);
                    }
                    //给当前请求对象
                    request.Proxy = myProxy;
                }
                else
                {
                    WebProxy myProxy = new WebProxy(objHttpItems.ProxyIp, false);
                    if (!string.IsNullOrEmpty(objHttpItems.ProxyUserName) && !string.IsNullOrEmpty(objHttpItems.ProxyPwd))
                    {
                        myProxy.Credentials = new NetworkCredential(objHttpItems.ProxyUserName, objHttpItems.ProxyPwd);
                    }
                    //给当前请求对象
                    request.Proxy = myProxy;
                }
                //设置安全凭证
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
        }
        /// <summary>
        /// 回调验证证书问题
        /// </summary>
        /// <param name="sender">流对象</param>
        /// <param name="certificate">证书</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="errors">SslPolicyErrors</param>
        /// <returns>bool</returns>
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            // 总是接受    
            return true;
        }
        #endregion
        #region 普通类型
        /// <summary>    
        /// 传入一个正确或不正确的URl，返回正确的URL
        /// </summary>    
        /// <param name="URL">url</param>   
        /// <returns>
        /// </returns>
        public string GetUrl(string URL)
        {
            if (!(URL.Contains("http://") || URL.Contains("https://")))
            {
                URL = "http://" + URL;
            }
            return URL;
        }
        ///<summary>
        ///采用httpwebrequest方式访问网络,根据传入的URl地址，得到响应的数据字符串。
        ///</summary>
        ///<param name="objHttpItems">参数列表</param>
        ///<returns>HttpResults 为返回结果</returns>
        public HttpResults GetHtml(HttpItems objHttpItems)
        {

            //调用专门读取数据的类
            return GetHttpRequestData(objHttpItems);
        }
        /// <summary>
        /// 采用httpwebrequest方式访问网络,根据传入的URl地址，得到响应的数据字符串。
        /// </summary>
        /// <param name="objHttpItems">参数列表</param>
        /// <param name="cookie">自动维护的字符串cookie</param>
        /// <returns>HttpResults 为返回结果</returns>
        public HttpResults GetHtml(HttpItems objHttpItems, ref string cookie)
        {
            //调用专门读取数据的类
            HttpResults res = GetHttpRequestData(objHttpItems);
            cookie = new XJHTTP().UpdateCookie(cookie, res.Cookie);
            return res;
        }
        /// <summary> 
        ///采用异步方式访问网络,根据传入的URl地址，得到响应的数据字符串。 
        /// </summary>
        /// <param name="objHttpItems">参数列表</param>
        /// <param name="callBack">完成后的回调函数</param>
        public void AsyncGetHtml(HttpItems objHttpItems, Action<HttpResults> callBack)
        {
            //调用专门读取数据的类
            AsyncGetHttpRequestData(objHttpItems, callBack);
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="hr">GetHtml后的请求结果</param>
        /// <returns></returns>
        public Image GetImg(HttpResults hr)
        {

            return byteArrayToImage(hr.ResultByte);

        }

        /// <summary>
        /// 字节数组生成图片
        /// </summary>
        /// <param name="Bytes">字节数组</param>
        /// <returns>图片</returns>
        private Image byteArrayToImage(byte[] Bytes)
        {
            if (Bytes == null)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                Image outputImg = Image.FromStream(ms);
                return outputImg;
            }

        }
        #endregion
    }

    /// <summary>
    /// Http请求参考类 
    /// </summary>
    public class HttpItems
    {

        
        string _URL;
        /// <summary>
        /// 请求URL必须填写
        /// </summary>
        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }
        string _Method = "GET";
        /// <summary>
        /// 请求方式默认为GET方式
        /// </summary>
        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        int _Timeout = 15000;
        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        int _ReadWriteTimeout = 30000;
        /// <summary>
        /// 默认写入Post数据超时间
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _ReadWriteTimeout; }
            set { _ReadWriteTimeout = value; }
        }
        string _Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
        /// <summary>
        /// 请求标头值 默认为image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*
        /// </summary>
        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        string _ContentType = "application/x-www-form-urlencoded";
        /// <summary>
        /// 请求返回类型默认 application/x-www-form-urlencoded
        /// </summary>
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        string _UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:17.0) Gecko/20100101 Firefox/17.0";
        /// <summary>
        /// 客户端访问信息默认Mozilla/5.0 (Windows NT 6.1; WOW64; rv:17.0) Gecko/20100101 Firefox/17.0
        /// </summary>
        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        private Encoding _PostEncoding=Encoding.Default;
        /// <summary>
        /// Post数据时的编码 默认为 Default,如需必要 请勿修改
        /// </summary>
        public Encoding PostEncoding
        {
            get { return _PostEncoding; }
            set { _PostEncoding = value; }
        }
        /// <summary>
        ///  默认的编码类型,如果不初始化,则为null,每次自动识别 
        /// </summary>
        public Encoding Encoding = null;

        string _Encoding = string.Empty;
        /// <summary>
        /// 返回数据编码默认为NUll,可以自动识别
        /// </summary>
        public string EncodingStr
        {
            get { return _Encoding; }
            set { _Encoding = value; }
        }
        private PostDataType _PostDataType = PostDataType.String;
        /// <summary>
        /// Post的数据类型
        /// </summary>
        public PostDataType PostDataType
        {
            get { return _PostDataType; }
            set { _PostDataType = value; }
        }
        string _Postdata;
        /// <summary>
        /// Post请求时要发送的字符串Post数据
        /// </summary>
        public string Postdata
        {
            get { return _Postdata; }
            set { _Postdata = value; }
        }
        private byte[] _PostdataByte = null;
        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte
        {
            get { return _PostdataByte; }
            set { _PostdataByte = value; }
        }
        CookieCollection cookiecollection = null;
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return cookiecollection; }
            set { cookiecollection = value; }
        }
        private CookieContainer _Container = null;
        /// <summary>
        /// 自动处理cookie
        /// </summary>
        public CookieContainer Container
        {
            get { return _Container; }
            set { _Container = value; }
        }
        string _Cookie = string.Empty;
        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        public string Cookie
        {
            get { return _Cookie; }
            set { _Cookie = value; }
        }
        string _Referer = string.Empty;
        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer
        {
            get { return _Referer; }
            set { _Referer = value; }
        }
        string _CerPath = string.Empty;
        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CerPath
        {
            get { return _CerPath; }
            set { _CerPath = value; }
        }
        private Boolean isToLower = false;
        /// <summary>
        /// 是否设置为全文小写
        /// </summary>
        public Boolean IsToLower
        {
            get { return isToLower; }
            set { isToLower = value; }
        }
        private Boolean isAjax = false;
        /// <summary>
        /// 是否增加异步请求头
        /// </summary>
        public Boolean IsAjax
        {
            get { return isAjax; }
            set { isAjax = value; }
        }

        private Boolean allowautoredirect = true;
        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面
        /// </summary>
        public Boolean Allowautoredirect
        {
            get { return allowautoredirect; }
            set { allowautoredirect = value; }
        }
        /// <summary>
        /// 当该属性设置为 true 时，使用 POST 方法的客户端请求应该从服务器收到 100-Continue 响应，以指示客户端应该发送要发送的数据。此机制使客户端能够在服务器根据请求报头打算拒绝请求时，避免在网络上发送大量的数据
        /// </summary>
        private bool expect100Continue = false;
        /// <summary>
        /// 当该属性设置为 true 时，使用 POST 方法的客户端请求应该从服务器收到 100-Continue 响应，以指示客户端应该发送要发送的数据。此机制使客户端能够在服务器根据请求报头打算拒绝请求时，避免在网络上发送大量的数据 默认False
        /// </summary>
        public bool Expect100Continue
        {
            get { return expect100Continue; }
            set { expect100Continue = value; }
        }
        private int connectionlimit = 1024;
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit
        {
            get { return connectionlimit; }
            set { connectionlimit = value; }
        }
        private string proxyusername = string.Empty;
        /// <summary>
        /// 代理Proxy 服务器用户名
        /// </summary>
        public string ProxyUserName
        {
            get { return proxyusername; }
            set { proxyusername = value; }
        }
        private string proxypwd = string.Empty;
        /// <summary>
        /// 代理 服务器密码
        /// </summary>
        public string ProxyPwd
        {
            get { return proxypwd; }
            set { proxypwd = value; }
        }
        private string proxyip = string.Empty;
        /// <summary>
        /// 代理 服务IP
        /// </summary>
        public string ProxyIp
        {
            get { return proxyip; }
            set { proxyip = value; }
        }
        private ResultType resulttype = ResultType.String;
        /// <summary>
        /// 设置返回类型String和Byte
        /// </summary>
        public ResultType ResultType
        {
            get { return resulttype; }
            set { resulttype = value; }
        }
        private WebHeaderCollection header = new WebHeaderCollection();
        /// <summary>
        /// 头数据
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }

    }
    /// <summary>
    /// Http返回参数类
    /// </summary>
    public class HttpResults
    {

        /// <summary>
        /// 响应结果的URL(可获取自动跳转后地址)   
        /// 如果获取跳转后地址失败,请使用RedirectUrl属性,
        /// 并设置HttpItems对象的Allowautoredirect =false;
        /// </summary>
        public string ResponseUrl { get; set; }
        /// <summary>
        /// 获取重定向的URL 
        /// 使用本属性时,请先关闭自动跳转属性  
        /// 设置方法如下:
        /// 设置HttpItems对象的Allowautoredirect =false;
        /// </summary>
        public string RedirectUrl
        {
            get
            {

                if (Header != null && Header.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Header["location"]))
                    {
                        return Header["location"].ToString();
                    }
                    return string.Empty;
                }

                return string.Empty;
            }
        }

        CookieContainer _Container;
        /// <summary>
        /// 自动处理Cookie集合对象
        /// </summary>
        public CookieContainer Container
        {
            get { return _Container; }
            set { _Container = value; }
        }
        string _Cookie = string.Empty;
        /// <summary>
        /// Http请求返回的Cookie
        /// </summary>
        public string Cookie
        {
            get { return _Cookie; }
            set { _Cookie = value; }
        }
        CookieCollection cookiecollection = null;
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return cookiecollection; }
            set { cookiecollection = value; }
        }
        private string html = string.Empty;
        /// <summary>
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空
        /// </summary>
        public string Html
        {
            get { return html; }
            set { html = value; }
        }
        private byte[] resultbyte = null;
        /// <summary>
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
        /// </summary>
        public byte[] ResultByte
        {
            get { return resultbyte; }
            set { resultbyte = value; }
        }
        private WebHeaderCollection header = new WebHeaderCollection();
        /// <summary>
        /// 头数据
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }
        /// <summary>
        /// Http请求后的状态码
        /// </summary>
        public HttpStatusCode StatusCode;
        private string _StatusDescription;
        /// <summary>
        /// 详细状态描述
        /// </summary>
        public string StatusDescription
        {
            get { return _StatusDescription; }
            set { _StatusDescription = value; }
        }
    }

    /// <summary>
    /// 返回类型
    /// </summary>
    public enum ResultType
    {
        String,//表示只返回字符串
        Byte,//表示返回字符串和字节流
        So//急速请求,仅返回数据头
    }

    /// <summary>
    /// Post的数据格式默认为string
    /// </summary>
    public enum PostDataType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        String,//字符串
        /// <summary>
        /// 字节流
        /// </summary>
        Byte,//字符串和字节流
        /// <summary>
        /// 文件路径
        /// </summary>
        FilePath//表示传入的是文件
    }

    /// <summary>
    /// WinInet的方式请求数据
    /// </summary>
    public class Wininet
    {
        #region WininetAPI
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);
        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref SystemTime sysTime);

        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern int InternetOpen(string strAppName, int ulAccessType, string strProxy, string strProxyBypass, int ulFlags);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern int InternetConnect(int ulSession, string strServer, int ulPort, string strUser, string strPassword, int ulService, int ulFlags, int ulContext);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool InternetCloseHandle(int ulSession);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool HttpAddRequestHeaders(int hRequest, string szHeasers, uint headersLen, uint modifiers);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern int HttpOpenRequest(int hConnect, string szVerb, string szURI, string szHttpVersion, string szReferer, string accetpType, int dwflags, int dwcontext);
        [DllImport("wininet.dll")]
        private static extern bool HttpSendRequestA(int hRequest, string szHeaders, int headersLen, string options, int optionsLen);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool InternetReadFile(int hRequest, byte[] pByte, int size, out int revSize);
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);
        #endregion
        public string UserAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; 125LA; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
        private int _WininetTimeOut = 0;
        /// <summary>
        /// Wininet超时时间 默认0 不设置超时,由于是自行实现(微软没修复超时的bug) 所以如果设置后,每次请求都会暂停.
        /// </summary>
        public int WininetTimeOut
        {
            get { return _WininetTimeOut; }
            set { _WininetTimeOut = value; }
        }
        #region 重载方法
        /// <summary>
        /// 使用API获取本地时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetLocalTime()
        {
            SystemTime sysTime = new SystemTime();
            GetLocalTime(ref sysTime);
            DateTime dt = new DateTime(sysTime.wYear, sysTime.wMonth, sysTime.wDay, sysTime.wHour, sysTime.wMinute, sysTime.wSecond);
            return dt;
        }
        /// <summary>
        /// 使用API设置本地时间
        /// </summary>
        /// <param name="dt"></param>
        public void SetLocalTime(DateTime dt)
        {
            SystemTime sysTime = new SystemTime();
            sysTime.wYear = (ushort)dt.Year;
            sysTime.wMonth = (ushort)dt.Month;
            sysTime.wDay = (ushort)dt.Day;
            sysTime.wHour = (ushort)dt.Hour;
            sysTime.wMinute = (ushort)dt.Minute;
            sysTime.wSecond = (ushort)dt.Second;
            SetLocalTime(ref sysTime);
        }
        /// <summary>
        /// WinInet 方式GET
        /// </summary>
        /// <param name="Url">地址</param>
        /// <returns></returns>
        public string GetData(string Url)
        {
            using (MemoryStream ms = GetHtml(Url, ""))
            {
                if (ms != null)
                {
                    //无视编码
                    Match meta = Regex.Match(Encoding.Default.GetString(ms.ToArray()), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                    string c = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToUpper().Trim() : string.Empty;
                    if (c.IndexOf("\"") > 0)
                    {
                        c = c.Split('\"')[0];
                    }
                    if (c.Length > 2)
                    {
                        if (c.IndexOf("UTF-8") != -1)
                        {
                            return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
                        }
                    }
                    return Encoding.GetEncoding("GBK").GetString(ms.ToArray());
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 将内存流转换为字符串
        /// </summary>
        /// <param name="mstream">需要转换的流</param>
        /// <returns></returns>
        public string GetDataPro(MemoryStream mstream)
        {
            using (MemoryStream ms = mstream)
            {
                if (ms != null)
                {
                    //无视编码
                    Match meta = Regex.Match(Encoding.Default.GetString(ms.ToArray()), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                    string c = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToUpper().Trim() : string.Empty;
                    if (c.IndexOf("\"") > 0)
                    {
                        c = c.Split('\"')[0];
                    }
                    if (c.Length > 2)
                    {
                        if (c.IndexOf("UTF-8") != -1)
                        {
                            return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
                        }
                    }
                    return Encoding.GetEncoding("GBK").GetString(ms.ToArray());
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// POST
        /// </summary>
        /// <param name="Url">地址</param>
        /// <param name="postData">提交数据</param>
        /// <returns></returns>
        public string PostData(string Url, string postData, StringBuilder Header = null)
        {
            using (MemoryStream ms = GetHtml(Url, postData, Header))
            {
                //无视编码
                Match meta = Regex.Match(Encoding.Default.GetString(ms.ToArray()), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                string c = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToUpper().Trim() : string.Empty;
                if (c.IndexOf("\"") > 0)
                {
                    c = c.Split('\"')[0];
                }
                if (c.Length > 2)
                {
                    if (c.IndexOf("UTF-8") != -1)
                    {
                        return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
                    }
                }
                return Encoding.GetEncoding("GBK").GetString(ms.ToArray());
            }
        }
        /// <summary>
        /// GET（UTF-8）模式
        /// </summary>
        /// <param name="Url">地址</param>
        /// <returns></returns>
        public string GetUtf8(string Url)
        {
            using (MemoryStream ms = GetHtml(Url, ""))
            {
                return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
            }
        }
        /// <summary>
        /// POST（UTF-8）
        /// </summary>
        /// <param name="Url">地址</param>
        /// <param name="postData">提交数据</param>
        /// <returns></returns>
        public string PostUtf8(string Url, string postData, StringBuilder Header = null)
        {
            using (MemoryStream ms = GetHtml(Url, postData, Header))
            {
                return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
            }
        }
        /// <summary>
        /// 获取网页图片(Image)
        /// </summary>
        /// <param name="Url">图片地址</param>
        /// <returns></returns>
        public Image GetImage(string Url)
        {
            using (MemoryStream ms = GetHtml(Url, ""))
            {
                if (ms == null)
                {
                    return null;
                }
                Image img = Image.FromStream(ms);
                return img;
            }
        }
        /// <summary>
        /// 获取网页图片(Image)
        /// </summary>
        /// <param name="mstream">Stream流</param>
        /// <returns></returns>
        public Image GetImage(MemoryStream mstream)
        {
            using (MemoryStream ms = mstream)
            {
                if (ms == null)
                {
                    return null;
                }
                Image img = Image.FromStream(ms);
                return img;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 重载提交数据(允许使用代理)
        /// </summary>
        /// <param name="Url">请求地址</param>
        /// <param name="Postdata">提交的数据(Get时为空)</param>
        /// <param name="proxy">代理地址(IP:端口 例如 127.0.0.1:8888)</param>
        /// <param name="subcookie">设置cookie,例如  JS增加的cookie.如非需要可不填写</param>
        /// <param name="subheader">设置数据头,如非需要可不填写</param>
        /// <param name="isMoved">是否允许跳转(允许3xx 跳转)</param>
        /// <returns></returns>
        public MemoryStream GetHtmlPro(string Url, string Postdata, string proxy = null, string subcookie = null, string subheader = null, bool isMoved = true)
        {
            try
            {
                //声明部分变量
                Uri uri = new Uri(Url);
                //判断访问方式
                string Method = "GET";
                if (string.IsNullOrEmpty(Postdata))
                {
                    Method = "GET";
                }
                else
                {
                    Method = "POST";
                }
                //判断是否是https
                bool https = Url.Substring(0, 5) == "https" ? true : false;
                //判断是否开启代理访问
                int hSession = 0;//会话句柄
                if (string.IsNullOrEmpty(proxy))
                {
                    hSession = InternetOpen(UserAgent, 1, string.Empty, string.Empty, 0);
                }
                else if (https)
                {
                    hSession = InternetOpen(UserAgent, 3, proxy, string.Empty, 0);
                }
                else
                {
                    hSession = InternetOpen(UserAgent, 3, "http=" + proxy, string.Empty, 0);
                }

                if (hSession == 0)
                {
                    InternetCloseHandle(hSession);
                    return null;//Internet句柄获取失败则返回
                }
                int hConnect = InternetConnect(hSession, uri.Host, uri.Port, string.Empty, string.Empty, 3, 0, 0);//连接句柄
                if (hConnect == 0)
                {
                    InternetCloseHandle(hConnect);
                    InternetCloseHandle(hSession);
                    return null;//Internet连接句柄获取失败则返回
                }

                //请求标记
                uint gettype = 2147483648 | 16;
                //是否带入cookie
                if (!string.IsNullOrEmpty(subcookie))
                {
                    gettype = gettype | 524288;
                }
                //是否重定向,true重定向，false不重定向
                if (!isMoved)
                {
                    gettype = gettype | 2097152;
                }
                if (https)
                {
                    gettype = gettype | 8388608;
                }
                else
                {
                    gettype = gettype | 16384;
                }

                //取HTTP请求句柄
                int hRequest = HttpOpenRequest(hConnect, Method, uri.PathAndQuery, "HTTP/1.1", string.Empty, string.Empty, (int)gettype, 0);//请求句柄
                if (hRequest == 0)
                {
                    InternetCloseHandle(hRequest);
                    InternetCloseHandle(hConnect);
                    InternetCloseHandle(hSession);
                    return null;//HTTP请求句柄获取失败则返回
                }

                //添加HTTP头
                string sb = string.Empty;
                if (string.IsNullOrEmpty(subheader))
                {
                    //如果提交协议头为空则使用内置的
                    sb += "Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\n";
                    sb += "Content-Type:application/x-www-form-urlencoded\r\n";
                    sb += "Accept-Language:zh-cn\r\n";
                    sb += "Referer:" + Url;
                }
                else
                {
                    sb = subheader;
                }

                //获取返回数据
                if (string.Equals(Method, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    HttpSendRequestA(hRequest, sb.ToString(), sb.Length, string.Empty, 0);
                }
                else
                {
                    HttpSendRequestA(hRequest, sb.ToString(), sb.Length, Postdata, Postdata.Length);
                }

                //处理返回数据
                int revSize = 0;//计次
                byte[] bytes = new byte[1024];
                MemoryStream ms = new MemoryStream();
                while (true)
                {
                    bool readResult = InternetReadFile(hRequest, bytes, 1024, out revSize);
                    if (readResult && revSize > 0)
                    {
                        ms.Write(bytes, 0, revSize);
                    }
                    else
                    {
                        break;
                    }
                }
                InternetCloseHandle(hRequest);
                InternetCloseHandle(hConnect);
                InternetCloseHandle(hSession);
                return ms;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 请求数据
        /// </summary>
        /// <param name="Url">请求地址</param>
        /// <param name="Postdata">提交的数据</param>
        /// <param name="Header">请求头</param>
        /// <returns></returns>
        private MemoryStream GetHtml(string Url, string Postdata, StringBuilder Header = null)
        {
            try
            {
                //声明部分变量
                Uri uri = new Uri(Url);
                string Method = "GET";
                if (Postdata != "")
                {
                    Method = "POST";
                }
                string UserAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; 125LA; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
                int hSession = InternetOpen(UserAgent, 1, "", "", 0);//会话句柄
                if (hSession == 0)
                {
                    InternetCloseHandle(hSession);
                    return null;//Internet句柄获取失败则返回
                }
                int hConnect = InternetConnect(hSession, uri.Host, uri.Port, "", "", 3, 0, 0);//连接句柄
                if (hConnect == 0)
                {
                    InternetCloseHandle(hConnect);
                    InternetCloseHandle(hSession);
                    return null;//Internet连接句柄获取失败则返回
                }
                //请求标记
                int gettype = -2147483632;
                if (Url.Substring(0, 5) == "https")
                {
                    gettype = -2139095024;
                }
                else
                {
                    gettype = -2147467248;
                }
                //取HTTP请求句柄
                int hRequest = HttpOpenRequest(hConnect, Method, uri.PathAndQuery, "HTTP/1.1", "", "", gettype, 0);//请求句柄
                if (hRequest == 0)
                {
                    InternetCloseHandle(hRequest);
                    InternetCloseHandle(hConnect);
                    InternetCloseHandle(hSession);
                    return null;//HTTP请求句柄获取失败则返回
                }
                //添加HTTP头
                StringBuilder sb = new StringBuilder();
                if (Header == null)
                {
                    sb.Append("Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\n");
                    sb.Append("Content-Type:application/x-www-form-urlencoded\r\n");
                    sb.Append("Accept-Language:zh-cn\r\n");
                    sb.Append("Referer:" + Url);
                }
                else
                {
                    sb = Header;
                }
                //获取返回数据
                if (string.Equals(Method, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    if (_WininetTimeOut > 0)
                    {
                        System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                        {

                            HttpSendRequestA(hRequest, sb.ToString(), sb.Length, "", 0);
                        }));
                        th.Start();
                        th.Join(_WininetTimeOut); //阻塞
                    }
                    else
                    {
                        HttpSendRequestA(hRequest, sb.ToString(), sb.Length, "", 0);
                    }
                }
                else
                {
                    if (_WininetTimeOut > 0)
                    {
                        System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                        {

                            HttpSendRequestA(hRequest, sb.ToString(), sb.Length, Postdata, Postdata.Length);
                        }));
                        th.Start();
                        th.Join(_WininetTimeOut); //阻塞
                    }
                    else
                    {
                        HttpSendRequestA(hRequest, sb.ToString(), sb.Length, Postdata, Postdata.Length);
                    }

                }
                //处理返回数据
                int revSize = 0;//计次
                byte[] bytes = new byte[1024];
                MemoryStream ms = new MemoryStream();
                while (true)
                {
                    bool readResult = InternetReadFile(hRequest, bytes, 1024, out revSize);
                    if (readResult && revSize > 0)
                    {
                        ms.Write(bytes, 0, revSize);
                    }
                    else
                    {
                        break;
                    }
                }
                InternetCloseHandle(hRequest);
                InternetCloseHandle(hConnect);
                InternetCloseHandle(hSession);
                return ms;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region 获取webbrowser的cookies
        /// <summary>
        /// 取出cookies
        /// </summary>
        /// <param name="url">完整的链接格式</param>
        /// <returns></returns>
        public string GetCookies(string url)
        {
            uint datasize = 256;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;

                cookieData = new StringBuilder((int)datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, IntPtr.Zero))
                    return null;
            }
            return cookieData.ToString() + ";";
        }
        #endregion

        #region String与CookieContainer互转
        /// <summary>
        /// 遍历CookieContainer
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;

        }
        /// <summary>
        /// 将String转CookieContainer
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public CookieContainer StringToCookie(string url, string cookie)
        {
            string[] arrCookie = cookie.Split(';');
            CookieContainer cookie_container = new CookieContainer();    //加载Cookie
            //try
            //{
            //    foreach (string sCookie in arrCookie)
            //    {
            //        if (sCookie.IndexOf("expires") > 0)
            //            continue;
            //        cookie_container.SetCookies(new Uri(url), sCookie);
            //    }
            //}
            //catch
            //{
            foreach (string sCookie in arrCookie)
            {
                if (!string.IsNullOrEmpty(sCookie))
                {
                    Cookie ck = new Cookie();
                    ck.Name = sCookie.Split('=')[0].Trim();
                    ck.Value = sCookie.Split('=')[1].Trim();
                    ck.Domain = url;
                    cookie_container.Add(ck);
                }
            }

            //}
            return cookie_container;
        }
        /// <summary>
        /// 将CookieContainer转换为string类型
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public string CookieToString(CookieContainer cc)
        {
            System.Collections.Generic.List<Cookie> lstCookies = new System.Collections.Generic.List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            StringBuilder sb = new StringBuilder();
            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies)
                    {
                        sb.Append(c.Name).Append("=").Append(c.Value).Append(";");
                    }
            }
            return sb.ToString();
        }
        #endregion
    }
    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMiliseconds;
    }
    /// <summary>
    /// HTTP类库
    /// </summary>
    public class XJHTTP
    {
        HttpItems item = new HttpItems();
        HttpHelpers http = new HttpHelpers();
        Wininet wnet = new Wininet();
        HttpResults hr;
        /// <summary>
        /// 使用API获取本地时间
        /// </summary>
        /// <returns>返回本地电脑的Datatime数据</returns>
        public DateTime GetLocalTime()
        {
            return wnet.GetLocalTime();
        }
        /// <summary>
        /// 使用API设置本地时间
        /// </summary>
        /// <param name="dt">需要设置的时间</param>
        public void SetLocalTime(DateTime dt)
        {
            wnet.SetLocalTime(dt);
        }
        /// <summary>
        /// 获取服务器返回的时间,如果Header中没有Date则返回当前时间
        /// </summary>
        /// <param name="hrs">请求结果对象</param>
        /// <returns>返回本地时区Datatime数据</returns>
        public DateTime GetServerTime(HttpResults hrs)
        {
            try
            {
                return GetTime4Gmt(hrs.Header["Date"].ToString());
            }
            catch
            {

                return DateTime.Now;
            }

        }
        /// <summary>
        /// 本地时间转成GMT时间 (参数如果不传入则为当前时间)
        /// 本地时间为：2011-9-29 15:04:39
        /// 转换后的时间为：Thu, 29 Sep 2011 07:04:39 GMT
        /// </summary>
        /// <param name="dt">参数如果不传入则为当前时间 DateTime.Now</param>
        /// <returns></returns>
        public string GetTimeToGMTString(DateTime dt = default(DateTime))
        {
            if (dt == default(DateTime))
            {
                dt = DateTime.Now;
            }
            return dt.ToUniversalTime().ToString("r");
        }

        /// <summary>
        ///本地时间转成GMT格式的时间(参数如果不传入则为当前时间)
        ///本地时间为：2011-9-29 15:04:39
        ///转换后的时间为：Thu, 29 Sep 2011 15:04:39 GMT+0800
        /// </summary>
        /// <param name="dt">参数如果不传入则为当前时间 DateTime.Now</param>
        /// <returns></returns>
        public string GetTimeToGMTFormat(DateTime dt = default(DateTime))
        {

            if (dt == default(DateTime))
            {
                dt = DateTime.Now;
            }
            return dt.ToString("r") + dt.ToString("zzz").Replace(":", "");
        }

        /// <summary>  
        /// GMT时间转成本地时间  
        /// DateTime dt1 = GMT2Local("Thu, 29 Sep 2011 07:04:39 GMT");
        /// 转换后的dt1为：2011-9-29 15:04:39
        /// DateTime dt2 = GMT2Local("Thu, 29 Sep 2011 15:04:39 GMT+0800");
        /// 转换后的dt2为：2011-9-29 15:04:39
        /// </summary>  
        /// <param name="gmt">字符串形式的GMT时间</param>  
        /// <returns></returns>  
        public DateTime GetTime4Gmt(string gmt)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                string pattern = "";
                if (gmt.IndexOf("+0") != -1)
                {
                    gmt = gmt.Replace("GMT", "");
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss zzz";
                }
                if (gmt.ToUpper().IndexOf("GMT") != -1)
                {
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
                }
                if (pattern != "")
                {
                    dt = DateTime.ParseExact(gmt, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    dt = dt.ToLocalTime();
                }
                else
                {
                    dt = Convert.ToDateTime(gmt);
                }
            }
            catch
            {
            }
            return dt;
        }
        /// <summary>
        /// 取文本右边 
        /// 默认取出右边所有文本,如果需要取固定长度请设置 length参数
        /// 异常则返回空字符串
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="right">需要确认位置的字符串</param>
        /// <param name="length">默认0,如果设置按照设置的值取出数据</param>
        /// <returns></returns>
        public string Right(string str, string right, int length = 0)
        {
            int pos = str.IndexOf(right, StringComparison.Ordinal);
            if (pos < 0) return "";
            int len = str.Length;
            if (len - pos - right.Length <= 0) return "";
            string result = "";
            if (length == 0)
            {
                result = str.Substring(pos + right.Length, len - (pos + right.Length));
            }
            else
            {
                result = str.Substring(pos + right.Length, length);
            }
            return result;
        }
        /// <summary>
        ///  取文本左边
        ///  默认取出左边所有文本,如果需要取固定长度请设置 length参数
        /// 异常则返回空字符串
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="left">需要确认位置的字符串</param>
        /// <param name="length">默认0,如果设置按照设置的值取出数据</param>
        /// <returns></returns>
        public string Left(string str, string left, int length = 0)
        {
            var pos = str.IndexOf(left, StringComparison.Ordinal);
            if (pos < 0) return "";
            string result = "";
            if (length == 0)
            {
                result = str.Substring(0, pos);
            }
            else
            {
                result = str.Substring(length, pos);
            }
            return result;
        }

        /// <summary>
        /// Unicode字符转汉字. 
        /// 默认处理 \u7384\u673a\u8bba\u575b 
        /// 如果遇到 &7384 &673a &8bba &575b  请对SplitString赋值为&
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="SplitString">默认为u</param>
        /// <returns></returns>
        public string FromUnicodeString(string str, string SplitString = "u")
        {
            //最直接的方法Regex.Unescape(str);
            StringBuilder strResult = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
            {
                string[] strlist = str.Replace("\\", "").Split(new string[] { SplitString }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < strlist.Length; i++)
                {
                    try
                    {
                        int charCode = Convert.ToInt32(strlist[i], 16);
                        strResult.Append((char)charCode);
                    }
                    catch (Exception)
                    {

                        continue; //跳过转换失败的字符
                    }

                }
            }
            return strResult.ToString();
        }
        /// <summary>
        /// 将StringCookie 添加到 CookieContainer对象中
        /// 当CookieContainer出现问题时请调用本方法修复 CookieContainer对象
        /// 更新失败时,返回原来的CookieContainer对象
        /// 
        /// </summary>
        /// <param name="cookie_container">需要更新的cookie_container</param>
        /// <param name="cookie">字符串cookie</param>
        /// <param name="domain">domain默认为空</param>
        /// <returns></returns>
        public CookieContainer AddCookieToContainer(CookieContainer cookie_container, string cookie, string domain = "")
        {

            if (cookie_container == null)
            {
                cookie_container = new CookieContainer();
            }
            Regex reg = new Regex("domain=(.*?);");
            string rdomain = string.Empty;
            if (reg.IsMatch(cookie))
            {
                rdomain = reg.Match(cookie).Groups[1].Value;
            }
            else
            {
                //匹配失败 如果cookie_container中的数据为0那么直接返回,本次更新失败
                if (cookie_container.Count > 0)
                {
                    try
                    {
                        rdomain = new XJHTTP().GetAllCookie(cookie_container)[0].Domain; //得到原cookie_container中的domain.  如果失败,则返回原先对象
                    }
                    catch
                    {
                        return cookie_container;
                    }

                }
                else
                {
                    rdomain = domain;
                }
            }
            string[] arrCookie = cookie.Split(';');
            foreach (string sCookie in arrCookie)
            {

                string vsCookie = sCookie.Replace("HttpOnly,", "").Trim(); //剔除httponly
                if (vsCookie.Contains("expires")) //去除过滤时间
                {
                    continue;
                }
                if (vsCookie.Split(',').Length > 1) //去掉,分隔的cookie
                {
                    vsCookie = vsCookie.Split(',')[1];
                }
                try
                {
                    if (!string.IsNullOrEmpty(vsCookie))
                    {
                        Cookie ck = new Cookie();
                        ck.Name = vsCookie.Split('=')[0].Trim();
                        ck.Value = vsCookie.Split('=')[1].Trim();
                        ck.Domain = rdomain;
                        cookie_container.Add(ck);
                    }
                }
                catch
                {
                    continue;
                }

            }
            return cookie_container;
        }
        /// <summary>
        /// 执行js代码(JS代码,参数,调用方法名,方法名[默认Eval 可选Run])
        /// 
        /// </summary>
        /// <param name="reString">JS代码</param>
        /// <param name="para">参数</param>
        /// <param name="MethodName">调用方法名</param>
        /// <param name="Method">方法名:默认Eval 可选Run</param>
        /// <returns></returns>
        public string RunJsMethod(string reString, string para, string MethodName, string Method = "Eval")
        {
            try
            {
                Type obj = Type.GetTypeFromProgID("ScriptControl");
                if (obj == null) return string.Empty;
                object ScriptControl = Activator.CreateInstance(obj);
                obj.InvokeMember("Language", BindingFlags.SetProperty, null, ScriptControl, new object[] { "JScript" });
                obj.InvokeMember("AddCode", BindingFlags.InvokeMethod, null, ScriptControl, new object[] { reString });
                object objx = obj.InvokeMember(Method, BindingFlags.InvokeMethod, null, ScriptControl, new object[] { string.Format("{0}({1})", MethodName, para) }).ToString();//执行结果
                if (objx == null)
                {
                    return string.Empty;
                }
                return objx.ToString();
            }
            catch (Exception ex)
            {
                string ErrorInfo = string.Format("执行JS出现错误:   \r\n 错误描述: {0} \r\n 错误原因: {1} \r\n 错误来源:{2}", ex.Message, ex.InnerException.Message, ex.InnerException.Source);//异常信息
                return ErrorInfo;
            }
        }


        /// <summary>
        /// 获取所有的A标签
        /// </summary>
        /// <param name="html">要分析的Html代码</param>
        /// <returns>返回一个List存储所有的A标签</returns>
        public List<AItem> GetAList(string html)
        {
            List<AItem> list = null;
            string ra = RegexString.Alist;
            if (Regex.IsMatch(html, ra, RegexOptions.IgnoreCase))
            {
                list = new List<AItem>();
                foreach (Match item in Regex.Matches(html, ra, RegexOptions.IgnoreCase))
                {
                    AItem a = null;
                    try
                    {
                        a = new AItem()
                        {
                            Href = item.Groups[1].Value,
                            Text = item.Groups[2].Value,
                            Html = item.Value,
                            Type = AType.Text
                        };
                        List<ImgItem> imglist = GetImgList(a.Text);
                        if (imglist != null && imglist.Count > 0)
                        {
                            a.Type = AType.Img;
                            a.Img = imglist[0];
                        }
                    }
                    catch { a = null; }
                    if (a != null)
                    {
                        list.Add(a);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 获取所有的Img标签
        /// </summary>
        /// <param name="html">要分析的Html代码</param>
        /// <returns>返回一个List存储所有的Img标签</returns>
        public List<ImgItem> GetImgList(string html)
        {
            List<ImgItem> list = null;
            string ra = RegexString.ImgList;
            if (Regex.IsMatch(html, ra, RegexOptions.IgnoreCase))
            {
                list = new List<ImgItem>();
                foreach (Match item in Regex.Matches(html, ra, RegexOptions.IgnoreCase))
                {
                    ImgItem a = null;
                    try
                    {
                        a = new ImgItem()
                        {
                            Src = item.Groups[1].Value,
                            Html = item.Value
                        };
                    }
                    catch { a = null; }
                    if (a != null)
                    {
                        list.Add(a);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 过滤html标签
        /// </summary>
        /// <param name="html">html的内容</param>
        /// <returns>处理后的文本</returns>
        public string StripHTML(string html)
        {
            html = Regex.Replace(html, RegexString.Nscript, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            html = Regex.Replace(html, RegexString.Style, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            html = Regex.Replace(html, RegexString.Script, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            html = Regex.Replace(html, RegexString.Html, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return html;
        }
        /// <summary>
        /// 过滤html中所有的换行符号
        /// </summary>
        /// <param name="html">html的内容</param>
        /// <returns>处理后的文本</returns>
        public string ReplaceNewLine(string html)
        {
            return Regex.Replace(html, RegexString.NewLine, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
        /// <summary>
        /// 取文本中间的其他写法
        /// </summary>
        /// <param name="html">原始Html</param>
        /// <param name="s">开始字符串</param>
        /// <param name="e">结束字符串</param>
        /// <returns></returns>
        public string GetMidHtml(string html, string s, string e)
        {
            string rx = string.Format("{0}{1}{2}", s, RegexString.AllHtml, e);
            if (Regex.IsMatch(html, rx, RegexOptions.IgnoreCase))
            {
                Match match = Regex.Match(html, rx, RegexOptions.IgnoreCase);
                if (match != null && match.Groups.Count > 0)
                {
                    return match.Groups[1].Value.Trim();
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 提取网页Title
        /// </summary>
        /// <param name="html">Html</param>
        /// <returns>返回Title</returns>
        public string GetHtmlTitle(string html)
        {
            if (Regex.IsMatch(html, RegexString.HtmlTitle))
            {
                return Regex.Match(html, RegexString.HtmlTitle).Groups[1].Value.Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        #region Json序列化方法 Framework 2.0下无效 ,可注释不使用
        /// <summary>
        /// 将指定的Json字符串转为指定的T类型对象  
        /// </summary>
        /// <param name="jsonstr">字符串</param>
        /// <returns>转换后的对象，失败为Null</returns>
        public object JsonToObject<T>(string jsonstr)
        {
            try
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                return jss.Deserialize<T>(jsonstr);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 将指定的对象转为Json字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>转换后的字符串失败为空字符串</returns>
        public string ObjectToJson(object obj)
        {
            try
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                return jss.Serialize(obj);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        #endregion

        /// <summary>
        /// 打开指定URL openType:0使用IE打开,!=0 使用默认浏览器打开
        /// </summary>
        /// <param name="url">需要打开的地址</param>
        /// <param name="openType">0使用IE,其他使用默认</param>
        public void OpenUrl(string url, int openType = 0)
        {
            // 调用ie打开网页
            if (openType == 0)
            {
                System.Diagnostics.Process.Start("IEXPLORE.EXE", url);
            }
            else
            {
                System.Diagnostics.Process.Start(url);
            }
        }


        ///
        /// 设置cookie
        ///
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
        ///
        /// 获取cookie
        ///
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookie(
        string url, string name, StringBuilder data, ref int dataSize);
        /// <summary>
        /// 设置IE cookie
        /// </summary>
        /// <param name="GetUrl">URL</param>
        /// <param name="NewCookie">Cookie</param>
        public void SetIeCookie(string GetUrl, string NewCookie)
        {
            //获取旧的
            StringBuilder cookie = new StringBuilder(new String(' ', 2048));
            int datasize = cookie.Length;
            bool b = InternetGetCookie(GetUrl, null, cookie, ref datasize);
            //删除旧的
            foreach (string fileName in System.IO.Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.Cookies)))
            {
                if (fileName.ToLower().IndexOf(GetUrl) > 0)
                {
                    System.IO.File.Delete(GetUrl);
                }
            }
            //生成新的
            foreach (string c in NewCookie.Split(';'))
            {
                string[] item = c.Split('=');
                string name = item[0];
                string value = item[1] + ";expires=Sun,22-Feb-2099 00:00:00 GMT";
                InternetSetCookie(GetUrl, name, value);
                InternetSetCookie(GetUrl, name, value);
                InternetSetCookie(GetUrl, name, value);
            }
        }
        /// <summary>
        /// 删除IE COOKIE
        /// </summary>
        public void ClearIECookie()
        {
            CleanAll();
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
        }
        #region 删除IE COOKIE具体代码

        public const int INTERNET_OPTION_END_BROWSER_SESSION = 42;
        #region Web清理
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        /*
         * 7 个静态函数
         * 私有函数
         * private bool FileDelete()    : 删除文件
         * private void FolderClear()   : 清除文件夹内的所有文件
         * private void RunCmd(): 运行内部命令
         *
         * 公有函数
         * public void CleanCookie()    : 删除Cookie
         * public void CleanHistory()   : 删除历史记录
         * public void CleanTempFiles() : 删除临时文件
         * public void CleanAll()       : 删除所有
         *
         *
         *
         * */
        //private
        /// <summary>
        /// 删除一个文件，System.IO.File.Delete()函数不可以删除只读文件，这个函数可以强行把只读文件删除。
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>成功为true</returns>
        public bool FileDelete(string path)
        {
            //first set the File\'s ReadOnly to 0
            //if EXP, restore its Attributes
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            System.IO.FileAttributes att = 0;
            bool attModified = false;
            try
            {
                //### ATT_GETnSET
                att = file.Attributes;
                file.Attributes &= (~System.IO.FileAttributes.ReadOnly);
                attModified = true;
                file.Delete();

            }
            catch
            {
                if (attModified)
                    file.Attributes = att;
                return false;

            }
            return true;
        }
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        public void FolderClear(string path)
        {
            System.IO.DirectoryInfo diPath = new System.IO.DirectoryInfo(path);
            foreach (System.IO.FileInfo fiCurrFile in diPath.GetFiles())
            {
                FileDelete(fiCurrFile.FullName);

            }
            foreach (System.IO.DirectoryInfo diSubFolder in diPath.GetDirectories())
            {
                FolderClear(diSubFolder.FullName); // Call recursively for all subfolders

            }

        }
        /// <summary>
        /// 删除历史记录  Win7+ 需要管理员权限
        /// </summary>
        public void CleanHistory()
        {
            string[] theFiles = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.History), "*", System.IO.SearchOption.AllDirectories);
            foreach (string s in theFiles)
                FileDelete(s);
            RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 1");
        }
        /// <summary>
        ///  删除临时文件  Win7+ 需要管理员权限
        /// </summary>
        public void CleanTempFiles()
        {
            FolderClear(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
            RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 8");
        }
        /// <summary>
        /// 删除Cookie文件 Win7+ 需要管理员权限
        /// </summary>
        public void CleanCookie()
        {
            string[] theFiles = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies), "*", System.IO.SearchOption.AllDirectories);
            foreach (string s in theFiles)
                FileDelete(s);
            RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 2");
        }
        ///
        /// 删除全部 历史记录,Cookie,临时文件
        ///
        public void CleanAll()
        {
            CleanHistory();
            CleanCookie();
            CleanTempFiles();
        }

        #endregion
        /// <summary>
        /// 调用CMD执行命令
        /// </summary>
        /// <param name="cmd"></param>
        public void RunCmd(string cmd)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            // 关闭Shell的使用
            p.StartInfo.UseShellExecute = false;
            // 重定向标准输入
            p.StartInfo.RedirectStandardInput = true;
            // 重定向标准输出
            p.StartInfo.RedirectStandardOutput = true;
            //重定向错误输出
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.WriteLine("exit");
        }

        #endregion

        /// <summary>
        /// 将字符串转换为base64格式 默认UTF8编码
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public string GetString2Base64(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return Convert.ToBase64String(encoding.GetBytes(str));
        }
        /// <summary>
        /// base64字符串转换为普通格式 默认UTF8编码
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public string GetStringbyBase64(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] buffer = Convert.FromBase64String(str);
            return encoding.GetString(buffer);
        }
        /// <summary>
        /// 将byte数组转换为AscII字符
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public string GetAscii2string(byte[] b)
        {
            string str = "";
            for (int i = 7; i < 19; i++)
            {
                str += (char)b[i];
            }
            return str;
        }
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        /// <summary>
        /// 将字节数组转化为十六进制字符串，每字节表示为两位
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string Bytes2HexString(byte[] bytes, int start, int len)
        {
            string tmpStr = "";

            for (int i = start; i < (start + len); i++)
            {
                tmpStr = tmpStr + bytes[i].ToString("X2");
            }

            return tmpStr;
        }
        /// <summary>
        /// 字符串转16进制
        /// </summary>
        /// <param name="mHex"></param>
        /// <returns>返回十六进制代表的字符串</returns>
        public string HexToStr(string mHex) // 返回十六进制代表的字符串 
        {
            byte[] bTemp = System.Text.Encoding.Default.GetBytes(mHex);
            string strTemp = "";
            for (int i = 0; i < bTemp.Length; i++)
            {
                strTemp += bTemp[i].ToString("X");
            }
            return strTemp;


        }
        /// <summary>
        /// 将十六进制字符串转化为字节数组
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] HexString2Bytes(string src)
        {
            byte[] retBytes = new byte[src.Length / 2];

            for (int i = 0; i < src.Length / 2; i++)
            {
                retBytes[i] = byte.Parse(src.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return retBytes;
        }
        /// <summary>
        /// 文件下载[如果连接不是绝对路径存在跳转默认会自动跳转]
        /// 会自动分析协议头中的filename
        /// 如果分析失败则直接存储为默认名[默认为:.zip格式].
        /// 成功返回true;
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="paths">保存绝对路径 如:c://download//</param>
        /// <param name="cc">Cookie</param>
        /// <param name="defaultName">默认后缀</param>
        /// <returns></returns>
        public bool DonwnLoad(string url, string paths, CookieContainer cc, string defaultName = ".zip")
        {
            try
            {
                item.URL = url;
                item.Allowautoredirect = true;
                item.ResultType = ResultType.Byte;
                item.Container = cc;
                //item.Container = wnet.StringToCookie(url, wnet.GetCookies(url));
                hr = http.GetHtml(item);
                string strname = hr.Header["Content-Disposition"].Split(new string[] { "filename=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                byte[] buffer = hr.ResultByte;
                try
                {
                    File.WriteAllBytes(paths + strname, buffer);
                }
                catch
                {
                    File.WriteAllBytes(paths + defaultName, buffer);
                }
                return true;

            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 文件下载[如果连接不是绝对路径存在跳转默认会自动跳转]
        /// 会自动分析协议头中的filename
        /// 如果分析失败则直接存储为默认名[默认为:.zip格式].
        /// 成功返回true;
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="paths">保存绝对路径 如:c://download//</param>
        /// <param name="cc">Ref Cookie 返回处理后的Cookie可再次使用</param> 
        /// <param name="Encoder">编码默认utf8</param>
        /// <param name="defaultName">默认后缀[zip]</param>
        /// <returns></returns>
        public bool WebClientDonwnLoad(string Url, string paths, string Referer, ref CookieContainer cc, Encoding Encoder = null, string defaultName = ".zip")
        {
            try
            {
                if (Encoder == null)
                {
                    Encoder = Encoding.UTF8;
                }
                WebClient myClient = new WebClient();
                myClient.Headers.Add("Accept: */*");
                myClient.Headers.Add("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; .NET4.0E; .NET4.0C; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; SE 2.X MetaSr 1.0)");
                myClient.Headers.Add("Accept-Language: zh-cn");
                myClient.Headers.Add("Content-Type: multipart/form-data");
                myClient.Headers.Add("Accept-Encoding: gzip, deflate");
                myClient.Headers.Add("Cache-Control: no-cache");
                myClient.Headers.Add(CookieTostring(cc));
                myClient.Encoding = Encoder;
                byte[] buffer = myClient.DownloadData(Url);
                string strname = myClient.Headers["Content-Disposition"].Split(new string[] { "filename=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                //处理cookie
                cc = StringToCookie(Url, myClient.ResponseHeaders["Set-Cookie"].ToString());
                try
                {
                    File.WriteAllBytes(paths + strname, buffer);
                }
                catch
                {
                    File.WriteAllBytes(paths + defaultName, buffer);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        /// <summary>
        /// WebClient Post 上传
        /// 用于上传类型为multipart/form-data 
        /// 如果上传失败,请检查协议头是否有自定义协议头.如Ajax头
        /// </summary>
        /// <param name="Url">上传地址</param>
        /// <param name="Referer">referer</param>
        /// <param name="PostData"></param>
        /// <param name="cc">Cookie</param>
        /// <param name="Encoder">编码默认utf8</param> 
        /// <returns></returns>
        public string UploadPost(string Url, string Referer, string PostData, ref CookieContainer cc, Encoding Encoder = null)
        {
            if (Encoder == null)
            {
                Encoder = Encoding.UTF8;
            }
            string result = "";
            WebClient myClient = new WebClient();
            myClient.Headers.Add("Accept: */*");
            myClient.Headers.Add("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; .NET4.0E; .NET4.0C; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; SE 2.X MetaSr 1.0)");
            myClient.Headers.Add("Accept-Language: zh-cn");
            myClient.Headers.Add("Content-Type: multipart/form-data");
            myClient.Headers.Add("Accept-Encoding: gzip, deflate");
            myClient.Headers.Add("Cache-Control: no-cache");
            myClient.Headers.Add(CookieTostring(cc));
            myClient.Encoding = Encoder;
            result = myClient.UploadString(Url, PostData);
            //处理cookie
            cc = StringToCookie(Url, myClient.ResponseHeaders["Set-Cookie"].ToString());
            return result;

        }
        /// <summary>
        /// 普通请求.直接返回标准结果
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <returns>返回结果</returns>
        public HttpResults GetHtml(string url)
        {
            item.URL = url;
            return http.GetHtml(item);
        }
        /// <summary>
        /// 普通请求.直接返回标准结果
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="cc">当前Cookie</param>
        /// <returns></returns>
        public HttpResults GetHtml(string url, CookieContainer cc)
        {
            item.URL = url;
            item.Container = cc;
            return http.GetHtml(item);
        }
        /// <summary>
        ///  普通请求.直接返回标准结果
        /// </summary>
        /// <param name="picurl">图片请求地址</param>
        /// <param name="referer">上一次请求地址</param>
        /// <param name="cc">当前Cookie</param>
        /// <returns></returns>
        public HttpResults GetImage(string picurl, string referer, CookieContainer cc)
        {
            item.URL = picurl;
            item.Referer = referer;
            item.Container = cc;
            item.ResultType = ResultType.Byte;
            return http.GetHtml(item);
        }
        /// <summary>
        /// 普通请求.直接返回Image格式图像
        /// </summary>
        /// <param name="picurl">图片请求地址</param>
        /// <param name="referer">上一次请求地址</param>
        /// <param name="cc">当前Cookie</param>
        /// <returns></returns>
        public Image GetImageByImage(string picurl, string referer, CookieContainer cc)
        {
            item.URL = picurl;
            item.Referer = referer;
            item.Container = cc;
            item.ResultType = ResultType.Byte;
            return http.GetImg(http.GetHtml(item));
        }
        /// <summary>
        /// 普通请求.直接返回标准结果
        /// </summary>
        /// <param name="posturl">post地址</param>
        /// <param name="referer">上一次请求地址</param>
        /// <param name="postdata">请求数据</param>
        /// <param name="IsAjax">是否需要异步标识</param>
        /// <param name="cc">当前Cookie</param>
        /// <returns></returns>
        public HttpResults PostHtml(string posturl, string referer, string postdata, bool IsAjax, CookieContainer cc)
        {
            item.URL = posturl;
            item.Referer = referer;
            item.Method = "Post";
            item.IsAjax = IsAjax;
            item.ResultType = ResultType.String;
            item.Postdata = postdata;
            item.Container = cc;
            return http.GetHtml(item);
        }
        /// <summary>
        /// 获取当前请求所有Cookie
        /// </summary>
        /// <param name="items"></param>
        /// <returns>Cookie集合</returns>
        public List<Cookie> GetAllCookieByHttpItems(HttpItems items)
        {
            return wnet.GetAllCookies(items.Container);
        }

        /// <summary>
        /// 获取CookieContainer 中的所有对象
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public List<Cookie> GetAllCookie(CookieContainer cc)
        {
            return wnet.GetAllCookies(cc);
        }
        /// <summary>
        /// 将 CookieContainer 对象转换为字符串类型
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public string CookieTostring(CookieContainer cc)
        {
            return wnet.CookieToString(cc);
        }
        /// <summary>
        /// 将文字Cookie转换为CookieContainer 对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public CookieContainer StringToCookie(string url, string cookie)
        {
            return wnet.StringToCookie(url, cookie);
        }
        /// <summary>
        /// 异步POST请求 通过回调返回结果
        /// </summary>
        /// <param name="objHttpItems">请求项</param>
        /// <param name="callBack">回调地址</param>
        public void AsyncPostHtml(HttpItems objHttpItems, Action<HttpResults> callBack)
        {
            http.AsyncGetHtml(objHttpItems, callBack);
        }
        /// <summary>
        /// 异步GET请求 通过回调返回结果
        /// </summary>
        /// <param name="objHttpItems">请求项</param>
        /// <param name="callBack">回调地址</param>
        public void AsyncGetHtml(HttpItems objHttpItems, Action<HttpResults> callBack)
        {
            http.AsyncGetHtml(objHttpItems, callBack);
        }
        /// <summary>
        /// WinInet方式GET请求  直接返回网页内容
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public string GetHtmlByWininet(string url)
        {
            return wnet.GetData(url);
        }
        /// <summary>
        /// WinInet方式GET请求(UTF8)  直接返回网页内容
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public string GetHtmlByWininetUTF8(string url)
        {
            return wnet.GetUtf8(url);
        }
        /// <summary>
        /// WinInet方式POST请求  直接返回网页内容
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <param name="postdata">提交内容</param>
        /// <returns></returns>
        public string POSTHtmlByWininet(string url, string postdata)
        {
            return wnet.PostData(url, postdata);
        }
        /// <summary>
        /// WinInet方式POST请求  直接返回网页内容
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <param name="postdata">提交内容</param>
        /// <returns></returns>
        public string POSTHtmlByWininetUTF8(string url, string postdata)
        {
            return wnet.PostData(url, postdata);
        }
        /// <summary>
        /// WinInet方式请求 图片  直接返回Image
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <returns></returns>
        public Image GetImageByWininet(string url)
        {
            return wnet.GetImage(url);
        }

        /// <summary>
        /// 获取JS时间戳 13位
        /// </summary>
        /// <returns></returns>
        public string GetTimeByJs()
        {
            Type obj = Type.GetTypeFromProgID("ScriptControl");
            if (obj == null) return null;
            object ScriptControl = Activator.CreateInstance(obj);
            obj.InvokeMember("Language", BindingFlags.SetProperty, null, ScriptControl, new object[] { "JScript" });
            string js = "function time(){return new Date().getTime()}";
            obj.InvokeMember("AddCode", BindingFlags.InvokeMethod, null, ScriptControl, new object[] { js });
            return obj.InvokeMember("Eval", BindingFlags.InvokeMethod, null, ScriptControl, new object[] { "time()" }).ToString();
        }
        /// <summary>  
        /// 获取时间戳 C# 10位 
        /// </summary>  
        /// <returns></returns>  
        public string GetTimeByCSharp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        /// <summary>
        /// 合并Cookie，将cookie2与cookie1合并更新 返回字符串类型Cookie
        /// </summary>
        /// <param name="cookie1">旧cookie</param>
        /// <param name="cookie2">新cookie</param>
        /// <returns></returns>
        public string UpdateCookie(string cookie1, string cookie2)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<string, string> dicCookie = new Dictionary<string, string>();
            //遍历cookie1
            if (!string.IsNullOrEmpty(cookie1))
            {
                foreach (string cookie in cookie1.Replace(',', ';').Split(';'))
                {
                    if (!string.IsNullOrEmpty(cookie) && cookie.IndexOf('=') > 0)
                    {
                        string key = cookie.Split('=')[0].Trim();
                        string value = cookie.Substring(key.Length + 1).Trim();
                        dicCookie.Add(key, cookie);
                    }
                }
            }

            if (!string.IsNullOrEmpty(cookie2))
            {
                //遍历cookie2
                foreach (string cookie in cookie2.Replace(',', ';').Split(';'))
                {
                    if (!string.IsNullOrEmpty(cookie) && cookie.IndexOf('=') > 0)
                    {
                        string key = cookie.Split('=')[0].Trim();
                        string value = cookie.Substring(key.Length + 1).Trim();
                        if (dicCookie.ContainsKey(key))
                        {
                            dicCookie[key] = cookie;
                        }
                        else
                        {
                            dicCookie.Add(key, cookie);
                        }
                    }
                }
            }
            int i = 0;
            foreach (var item in dicCookie)
            {
                i++;
                if (i < dicCookie.Count)
                {
                    sb.Append(item.Value + ";");
                }
                else
                {
                    sb.Append(item.Value);
                }
            }
            return sb.ToString();

        }
        /// <summary>
        /// 清理string类型Cookie.剔除无用项返回结果为null时遇见错误.
        /// </summary>
        /// <param name="Cookies"></param>
        /// <returns></returns>
        public string ClearCookie(string Cookies)
        {
            try
            {
                string rStr = string.Empty;
                Cookies = Cookies.Replace(";", "; ");
                Regex r = new Regex("(?<=,)(?<cookie>[^ ]+=(?!deleted;)[^;]+);");
                Match m = r.Match("," + Cookies);
                while (m.Success)
                {
                    rStr += m.Groups["cookie"].Value + ";";
                    m = m.NextMatch();
                }
                return rStr;
            }
            catch
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 从Wininet中获取Cookie对象
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetCookieByWininet(string url)
        {
            return wnet.GetCookies(url);
        }
        /// <summary>
        /// 字符串MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string EncryptMD5String(string str)
        {
            using (MD5 md5String = MD5.Create())
            {
                StringBuilder sb = new StringBuilder();
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                byte[] md5Encrypt = md5String.ComputeHash(bytes);
                for (int i = 0; i < md5Encrypt.Length; i++)
                {
                    sb.Append(md5Encrypt[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        /// <summary>
        /// 取文本中间
        /// </summary>
        /// <param name="allStr">原字符</param>
        /// <param name="firstStr">前面的文本</param>
        /// <param name="lastStr">后面的文本</param>
        /// <returns></returns>
        public string GetStringMid(string allStr, string firstStr, string lastStr)
        {
            //取出前面的位置
            int index1 = allStr.IndexOf(firstStr);
            //取出后面的位置
            int index2 = allStr.IndexOf(lastStr, index1 + firstStr.Length);

            if (index1 < 0 || index2 < 0)
            {
                return "";
            }
            //定位到前面的位置
            index1 = index1 + firstStr.Length;
            //判断要取的文本的长度
            index2 = index2 - index1;

            if (index1 < 0 || index2 < 0)
            {
                return "";
            }
            //取出文本
            return allStr.Substring(index1, index2);
        }
        /// <summary>
        /// 批量取文本中间
        /// </summary>
        /// <param name="allStr">原字符</param>
        /// <param name="firstStr">前面的文本</param>
        /// <param name="lastStr">后面的文本</param>
        /// <param name="regexCode">默认为万能表达式(.*?)</param>
        /// <returns></returns>
        public List<string> GetStringMids(string allStr, string firstStr, string lastStr, string regexCode = "(.*?)")
        {
            List<string> list = new List<string>();
            string reString = string.Format("{0}{1}{2}", firstStr, regexCode, lastStr);
            Regex reg = new Regex(reString);
            MatchCollection mc = reg.Matches(allStr);
            for (int i = 0; i < mc.Count; i++)
            {
                GroupCollection gc = mc[i].Groups; //得到所有分组 
                for (int j = 1; j < gc.Count; j++) //多分组
                {
                    string temp = gc[j].Value;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        list.Add(temp);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// URL加密适用于淘宝中文编码算法
        /// </summary>
        /// <param name="str">明文</param>
        /// <returns>密文</returns>
        public string UrlEncoding(string str)
        {
            byte[] buff = Encoding.Default.GetBytes(str);
            string s = "";
            for (int ix = 0; ix < buff.Length; ix++)
            {
                s += "%" + buff[ix].ToString("x2");
            }
            s = s.ToUpper(); //%62%62%73%2E%6D%73%64%6E%35%2E%63%6F%6D%D0%FE%BB%FA%C2%DB%CC%B3%B3%F6%C6%B7
            return s;
        }
        /// <summary>
        /// Url编码,encoding默认为utf8编码
        /// </summary>
        /// <param name="str">需要编码的字符串</param>
        /// <param name="encoding">指定编码类型</param>
        /// <returns>编码后的字符串</returns>
        public string UrlEncoding(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                return System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            }
            else
            {
                return System.Web.HttpUtility.UrlEncode(str, encoding);
            }
        }
        /// <summary>
        /// URL解密适用于淘宝中文编码算法
        /// </summary>
        /// <param name="str">密文</param>
        /// <returns>明文</returns>
        public string DeUrlMethod(string str)
        {
            //改进后更适合的算法
            List<byte> li2 = new List<byte>();
            string[] strs = str.Split('%');
            for (int j = 0; j < strs.Length; j++)
            {
                if (!string.IsNullOrEmpty(strs[j]))
                {
                    li2.Add(Convert.ToByte(strs[j], 16));
                }
            }
            string res = Encoding.Default.GetString(li2.ToArray());//bbs.msdn5.com玄机论坛出品
            return res;
        }
        /// <summary>
        /// Url解码,encoding默认为utf8编码
        /// </summary>
        /// <param name="str">需要解码的字符串</param>
        /// <param name="encoding">指定解码类型</param>
        /// <returns>解码后的字符串</returns>
        public string UrlDecoding(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                return System.Web.HttpUtility.UrlDecode(str, Encoding.UTF8);
            }
            else
            {
                return System.Web.HttpUtility.UrlDecode(str, encoding);
            }
        }




    }
    /// <summary>
    /// 正则表达式静态类
    /// </summary>
    public class RegexString
    {
        /// <summary>
        /// 获取所有的A链接
        /// </summary>
        public static readonly string Alist = "<a[\\s\\S]+?href[=\"\']([\\s\\S]+?)[\"\'\\s+][\\s\\S]+?>([\\s\\S]+?)</a>";
        /// <summary>
        /// 获取所有的Img标签
        /// </summary>
        public static readonly string ImgList = "<img[\\s\\S]*?src=[\"\']([\\s\\S]*?)[\"\'][\\s\\S]*?>([\\s\\S]*?)>";
        /// <summary>
        /// 所有的Nscript
        /// </summary>
        public static readonly string Nscript = "<nscript[\\s\\S]*?>[\\s\\S]*?</nscript>";
        /// <summary>
        /// 所有的Style
        /// </summary>
        public static readonly string Style = "<style[\\s\\S]*?>[\\s\\S]*?</style>";
        /// <summary>
        /// 所有的Script
        /// </summary>
        public static readonly string Script = "<script[\\s\\S]*?>[\\s\\S]*?</script>";
        /// <summary>
        /// 所有的Html
        /// </summary>
        public static readonly string Html = "<[\\s\\S]*?>";
        /// <summary>
        /// 换行符号
        /// </summary>
        public static readonly string NewLine = Environment.NewLine;
        /// <summary>
        ///获取网页编码
        /// </summary>
        public static readonly string Enconding = "<meta[^<]*charset=([^<]*)[\"']";
        /// <summary>
        /// 所有Html
        /// </summary>
        public static readonly string AllHtml = "([\\s\\S]*?)";
        /// <summary>
        /// title
        /// </summary>
        public static readonly string HtmlTitle = "<title>([\\s\\S]*?)</title>";
    }
    /// <summary>
    /// 图片对象 
    /// </summary>
    public class ImgItem
    {
        /// <summary>
        /// 图片网址
        /// </summary>
        public string Src { get; set; }
        /// <summary>
        /// 图片标签Html
        /// </summary>
        public string Html { get; set; }
    }
    /// <summary>
    /// A连接对象   
    /// </summary>
    public class AItem
    {
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Href { get; set; }
        /// <summary>
        /// 链接文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 链接的图片 
        /// </summary>
        public ImgItem Img { get; set; }
        /// <summary>
        /// 整个连接Html
        /// </summary>
        public string Html { get; set; }
        /// <summary>
        /// A链接的类型 文本/图像
        /// </summary>
        public AType Type { get; set; }
    }
    public enum AType
    {
        /// <summary>
        /// 文本链接(默认)
        /// </summary>
        Text,
        /// <summary>
        /// 图片链接
        /// </summary>
        Img
    }
}
