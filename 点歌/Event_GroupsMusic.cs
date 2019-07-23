using Native.Csharp.App.EventArgs;
using Native.Csharp.App.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Event
{
    public class Event_GroupsMusic : IReceiveGroupMessage
    {
        public void ReceiveGroupMessage(object sender, CqGroupMessageEventArgs e)
        {
            if (e.Message.Contains("网易云点歌"))
            {
                if (e.Message.IndexOf("网易云点歌") == 0)
                {
                    var msgs = e.Message.Substring(5).ToString();
                    ToSongsWY(msgs, e);
                }
            }
            else if(e.Message.Contains("点歌"))
            {

                string msg = e.Message;
                if (!msg.Contains("点歌"))
                    return;
                if (msg.Substring(0, 2).ToString() != "点歌")
                    return;
                var msgs = msg.Substring(2).ToString();
                ToSongsQQ(msgs, e);
            }
            if (e.Message.Contains("一首") && e.Message.Contains("送给")) //未某人点歌
            {
                try
                {
                    var songs = e.Message.Substring(2, e.Message.IndexOf("送给") -2);
                    var per = e.Message.Substring(e.Message.IndexOf("送给") + 2);
                    if (per == "我" || per == "自己")
                    {


                        ToSomeoneSongsQQ(songs, e.FromQQ, e);
                    }
                    else if(per=="你们")
                    {
                        ToSomeoneSongsQQ(songs, -1, e);
                    }
                    else if(per!=null)
                    {
                        if(per.Substring(0,10)=="[CQ:at,qq=")
                        {
                            ToSomeoneSongsQQ(songs, 0, e,per);
                        }
                        else
                        {

                        }
                    }

                }
                catch (Exception)
                {
                    return;
                    throw;
                }
            }

        }

        #region  网易云音乐点歌 
        /// <summary>
        /// 网易云音乐点歌
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="e"></param>
        private void ToSongsWY(string msgs, CqGroupMessageEventArgs e)
        {
            try
            {
                int seeid = -1;
                if (msgs.Contains("。。"))
                {
                    var singer = msgs.Substring(msgs.IndexOf("。。") + 2);
                    var song = msgs.Substring(0, msgs.IndexOf("。。"));
                    var list = GetWYSearch(song);
                    if (list == null)
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "未搜索到歌曲");
                        return;
                    }

                    var musi = list.Where(a => a.singers.Contains(singer)).FirstOrDefault();
                    if (musi == null)
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "未搜索到" + singer + "的《" + song + "》");
                        return;
                    }

                    var music = Common.CqApi.CqCode_Music(musi.songid, "163");
                    seeid = Common.CqApi.SendGroupMessage(e.FromGroup, music);


                }
                else
                {
                    var list = GetWYSearch(msgs);
                    if (list == null)
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "未搜索到歌曲");
                        return;
                    }

                    var music = Common.CqApi.CqCode_Music(list[0].songid, "163");
                    seeid = Common.CqApi.SendGroupMessage(e.FromGroup, music);
                }
                if (seeid < 0)
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "歌曲发送失败");
            }
            catch (Exception ex)
            {
                return;
                throw ex;
            }
        }

        /// <summary>
        /// 网易云音乐搜索歌名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private List<Musices> GetWYSearch(string name)
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
            List<Musices> musices = new List<Musices>();
            foreach (var l in list)
            {
                Musices mu = new Musices();
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

        #endregion

        #region qq点歌
        /// <summary>
        /// qq点歌
        /// </summary>
        /// <param name="e"></param>
        private void ToSongsQQ(string msgs, CqGroupMessageEventArgs e)
        {
            try
            {
                int seeid = -1;
                if (msgs.Contains("。。"))
                {
                    var singer = msgs.Substring(msgs.IndexOf("。。") + 2);
                    var song = msgs.Substring(0, msgs.IndexOf("。。"));
                    var list = GetUrlAllSearch(song);
                    if (list == null)
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "未搜索到歌曲");
                        return;
                    }

                    var musi = list.Where(a => a.singers.Contains(singer)).FirstOrDefault();
                    if (musi == null)
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "未搜索到" + singer + "的《" + song + "》");
                        return;
                    }

                    var music = Common.CqApi.CqCode_Music(musi.songid);
                    seeid = Common.CqApi.SendGroupMessage(e.FromGroup, music);


                }
                else
                {
                    var list = GetUrlAllSearch(msgs);
                    if (list == null)
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "未搜索到歌曲");
                        return;
                    }

                    var music = Common.CqApi.CqCode_Music(list[0].songid);
                    seeid = Common.CqApi.SendGroupMessage(e.FromGroup, music);
                }
                if (seeid < 0)
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "歌曲发送失败");
            }
            catch (Exception ex)
            {
                return;
                throw ex;
            }
        }

        /// <summary>
        /// QQ 音乐根据歌名搜索
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private List<Musices> GetUrlAllSearch(string name)
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
            List<Musices> musices = new List<Musices>();
            foreach (var l in list)
            {
                Musices mu = new Musices();
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
        /// 为某人点歌
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="e"></param>
        private void ToSomeoneSongsQQ(string msgs,long atQQ, CqGroupMessageEventArgs e,string qqcode="")
        {
            try
            {
                
                int seeid = -1;

                var list = GetUrlAllSearch(msgs);
                if (list == null)
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "未搜索到歌曲");
                    return;
                }

                var music = Common.CqApi.CqCode_Music(list[0].songid);
                seeid = Common.CqApi.SendGroupMessage(e.FromGroup, music);
                if (seeid < 0)
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "歌曲发送失败");
                else
                {
                    if(qqcode=="")
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(atQQ));
                    else
                        Common.CqApi.SendGroupMessage(e.FromGroup, qqcode);
                }
            }
            catch (Exception ex)
            {
                return;
                throw ex;
            }
        }



        #endregion
    }
    public class Musices
    {
        public long songid;
        public string singers;
    }

}
