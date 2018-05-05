using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace WebPro
{
    public static class IpSupport
    {
        public static string GetClientIp()
        {
            string userIP = "";
            ////可以透过代理服务器
            //userIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            ////判断是否有代理服务器
            //if (string.IsNullOrEmpty(userIP))
            //{
            //没有代理服务器,如果有代理服务器获取的是代理服务器的IP
            userIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}
            return userIP;
        }



        /// <summary>         
        /// 获取HTML源码信息(Porschev)         
        /// </summary>         
        /// <param name="url">获取地址</param>         
        /// <returns>HTML源码</returns>         
        public static string GetHtml(string url)
        {
            string str = "";
            try
            {
                Uri uri = new Uri(url);
                WebRequest wr = WebRequest.Create(uri);
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, System.Text.Encoding.Default);
                str = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                str = "";
            }
            return str;
        }

        /// <summary>         
        /// 通过IP得到IP所在地省市（Porschev）         
        /// </summary>         
        /// <param name="ip"></param>         
        /// <returns></returns>         
        public static string GetAdrByIp(string ip)
        {
            string ipInfo = "";
            try
            {
                string url = "http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=js&ip=" + ip;

                //获取返回值
                string result = GetHtml(url);
                //unicode解码
                result = Decode(result);
                //分离json标准字符串
                result = result.Split('=')[1].Trim().Substring(0, result.Split('=')[1].Trim().Length - 1);
                var rs = 0;
                if (!Int32.TryParse(result, out rs))
                {
                    Newtonsoft.Json.Linq.JObject obj = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(result);
                    if (obj != null)
                    {
                        ipInfo = obj["country"].ToString()
                        + "-" + obj["province"].ToString()
                        + "-" + obj["city"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                ipInfo = "";
            }
            return ipInfo;
        }

        /// <summary>
        /// 将Unicode编码转换成中文
        /// </summary>
        /// <param name='result'></param>
        /// <returns></returns>
        static Regex reUnicode = new Regex(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);

        public static string Decode(string s)
        {
            return reUnicode.Replace(s, m =>
            {
                short c;
                if (short.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out c))
                {
                    return "" + (char)c;
                }
                return m.Value;
            });
        }
    }
}