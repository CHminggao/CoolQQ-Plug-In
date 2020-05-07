using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Native.Tool.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace onlne.loneme.group.Code
{
    /// <summary>
    /// 搜索音乐
    /// </summary>
    public static class C_SeachMusic
    {
        /// <summary>
        /// QQ 音乐根据歌名搜索
        /// </summary>
        /// <param name="name">歌名</param>
        /// <returns></returns>
        public static List<MusicesT> GetQQlSearch(string name)
        {
            string url = $@"https://c.y.qq.com/soso/fcgi-bin/client_search_cp?aggr=1&cr=1&flag_qc=0&p=1&n=30&w={name}";
            WebRequest wRequest = WebRequest.Create(url);
            wRequest.Method = "GET";
            wRequest.ContentType = "text/html;charset=UTF-8";
            WebResponse wResponse = wRequest.GetResponse();
            Stream stream = wResponse.GetResponseStream();
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
            string str = reader.ReadToEnd();   //url返回的值
            reader.Close();
            wResponse.Close();

            #region 使用正则表达式
            //Regex reg = new Regex("(?i)(?<=songmid\":\")[^\"]*(?=\")");
            //MatchCollection mc = reg.Matches(str);
            //List<long> list = new List<long>();
            //foreach (Match m in mc)
            //{
            //    list.Add(Convert.ToInt64( m.Value));
            //    break;
            //}
            #endregion

            #region 使用JObject解析json
            str = str.Substring(0, str.Length - 1).Substring(9);
            JObject jo = (JObject)JsonConvert.DeserializeObject(str);
            var list = jo["data"]["song"]["list"];
            List<MusicesT> musices = new List<MusicesT>();
            foreach (var l in list)
            {
                MusicesT mu = new MusicesT();
                mu.songid = Convert.ToInt64(l["songid"].ToString());

                var singer = l["singer"];
                foreach (var s in singer)
                {
                    if (mu.singers == "")
                        mu.singers = s["name"].ToString();
                    else
                        mu.singers += "," + s["name"].ToString();
                }
                musices.Add(mu);
            }
            #endregion


            return musices;
            // return $"https://i.y.qq.com/v8/playsong.html?songmid={list[0]}";
        }

        /// <summary>
        /// 网易云音乐搜索歌名
        /// </summary>
        /// <param name="name">歌名</param>
        /// <returns></returns>
        public static List<MusicesT> GetWYSearch(string name)
        {
            string url = $@"http://music.163.com/api/search/get/web?csrf_token=&hlpretag=&hlposttag=&s={name}&type=1&offset=0&total=true&limit=30";
            WebRequest wRequest = WebRequest.Create(url);
            wRequest.Method = "GET";
            wRequest.ContentType = "text/html;charset=UTF-8";
            WebResponse wResponse = wRequest.GetResponse();
            Stream stream = wResponse.GetResponseStream();
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
            string str = reader.ReadToEnd();   //url返回的值
            reader.Close();
            wResponse.Close();

            //str = str.Substring(0, str.Length - 1).Substring(9);
            JObject jo = (JObject)JsonConvert.DeserializeObject(str);
            var list = jo["result"]["songs"];
            List<MusicesT> musices = new List<MusicesT>();
            foreach (var l in list)
            {
                MusicesT mu = new MusicesT();
                mu.songid = Convert.ToInt64(l["id"].ToString());

                var singer = l["artists"];
                foreach (var s in singer)
                {
                    if (mu.singers == "")
                        mu.singers = s["name"].ToString();
                    mu.singers += "," + s["name"].ToString();
                }
                musices.Add(mu);
            }
            return musices;
        }

        /// <summary>
        /// 搜索并返回第一条结果
        /// </summary>
        /// <param name="name">歌名</param>
        /// <param name="c_MusicTP">平台选择</param>
        /// <returns></returns>
        public static MusicesT GetMusicModel(string name,C_MusicTP c_MusicTP=C_MusicTP.qqMusic)
        {
            if (c_MusicTP == C_MusicTP.qqMusic)
            {
                    return GetQQlSearch(name).Count!=0? GetQQlSearch(name)[0]:null;
            }
            else
            {
                return GetWYSearch(name).Count != 0 ? GetWYSearch(name)[0] : null;
            }
        }
    }

    /// <summary>
    /// 搜索类型
    /// </summary>
    public enum C_MusicTP
    {
        /// <summary>
        /// qq音乐
        /// </summary>
        qqMusic,

        /// <summary>
        /// 网易云音乐
        /// </summary>
        wyMusic
    }

    /// <summary>
    /// 音乐列表
    /// </summary>
    public class MusicesT
    {
        public long songid;
        public string singers;
    }
}
