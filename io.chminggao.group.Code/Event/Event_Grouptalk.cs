using Native.Sdk.Cqp.Enum;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace onlne.loneme.group.Code.Event
{
    /// <summary>
    /// 群聊消息处理
    /// </summary>
    public class Event_Grouptalk : IGroupMessage
    {
        public static string[] mcQQ ;
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            string[] serverlist = { "dx.mc14.icraft.cc:47577", "wt.mc14.icraft.cc:47577", "yd.mc14.icraft.cc:47577", "bf.mc14.icraft.cc:47577" };
            if (e.Message.Text.Substring(0, e.Message.Text.Length >= 5 ? 5 : e.Message.Text.Length).Contains("点歌"))  //点歌
            {
                SendMusic(e.Message.Text, e);
                e.Handler = true;

            }
//            if(e.FromGroup== 222600436)
//            {
//                if(e.Message == "服务器")
//                {
//                    e.FromGroup.SendGroupMessage(" 服务器：\n\t" +
//"电信 dx.mc14.icraft.cc:47577 \n\t网通 wt.mc14.icraft.cc:47577\n\t移动 yd.mc14.icraft.cc:47577");
//                    e.FromGroup.SendGroupMessage(CheckStat(serverlist));
//                    e.Handler = true;
//                }
//            }
            if (e.Message.Text.Contains("mc服务器") && e.Message.Text.Substring(0, 5) == "mc服务器")
            {
                try
                {
                    string[] server = { e.Message.Text.Substring(5) };
                    e.FromGroup.SendGroupMessage(CheckStat(server));
                }
                catch (Exception ex)
                {
                    e.FromGroup.SendGroupMessage("请验证服务器地址正确");

                }
                e.Handler = true;
            }
        }


        /// <summary>
        /// 检测服务器
        /// </summary>
        /// <param name="serverlist">服务器列表</param>
        public static string CheckStat(string[] serverlist)
        {
            try
            {
                for (int i = 0; i < serverlist.Length; i++)
                {
                    string host = serverlist[i].Split(':')[0];
                    int port = int.Parse(serverlist[i].Split(':')[1]);
                    var mse = C_MinServer.CheckServer(host, port);
                    if (mse == null)
                        continue;
                    return "服务器版本：" + mse.MinecraftVersion.ToString() +"\n当前在线数：" + mse.CurrentPlayerCount;
                }
            }
            catch (Exception ex)
            {
                ;
            }
            return "服务器状态未知（已关闭/离线）";
        }

        /// <summary>
        /// 点歌
        /// </summary>
        /// <param name="mesg">消息</param>
        /// <param name="groupL">群号</param>
        /// <param name="qqL">点歌用户</param>
        public static void SendMusic(string mesg, CQGroupMessageEventArgs e)
        {
            long songId = 0;
            if (mesg.Contains("网易云点歌"))
            {
                if (mesg.IndexOf("网易云点歌") == 0)
                {
                    string wyresullt= SendWY(mesg.Substring(5), C_SeachMusic.GetMusicModel(mesg.Substring(5), C_MusicTP.wyMusic),out songId);
                    e.FromGroup.SendGroupMessage(Native.Sdk.Cqp.CQApi.CQCode_Music(songId, Native.Sdk.Cqp.Enum.CQMusicType.Netease, CQMusicStyle.New));
                }
            }
            else if (mesg.Contains("点歌"))
            {
                if (mesg.IndexOf("点歌") != 0)
                    return;
                string qqresullt = SendQQ(mesg.Substring(2), C_SeachMusic.GetMusicModel(mesg.Substring(2), C_MusicTP.qqMusic),out songId);
                e.FromGroup.SendGroupMessage(Native.Sdk.Cqp.CQApi.CQCode_Music(songId, Native.Sdk.Cqp.Enum.CQMusicType.Tencent, CQMusicStyle.New));
            }
        }

        /// <summary>
        /// 网易云点歌  返回 ""则没有查询到歌曲
        /// </summary>
        /// <param name="msg">歌名等信息</param>
        /// <param name="model">歌曲</param>
        public static string SendWY(string msg,  MusicesT model,out long songid)
        {
            
            try
            {
                if (model == null)
                {
                    songid = 0;
                    return "未搜索到歌曲";
                }
                if (msg.Contains("。。")) //判断是否存在歌手信息
                {
                    var singer = msg.Substring(msg.IndexOf("。。") + 2);
                    var song = msg.Substring(0, msg.IndexOf("。。"));
                    if (C_SeachMusic.GetWYSearch(song).Where(a => a.singers.Contains(singer)).FirstOrDefault() == null)
                    {
                        songid = 0;
                        return "未搜索到" + singer + "的《" + song + "》";
                    }
                    songid = C_SeachMusic.GetWYSearch(song).Where(a => a.singers.Contains(singer)).FirstOrDefault().songid;
                    return "";
                }
                songid = model.songid;
                return "";
            }
            catch (Exception ex)
            {
                songid = 0;
                return "";
            }
        }


        /// <summary>
        /// QQ音乐点歌     返回 ""则没有查询到歌曲
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="model"></param>
        public static string SendQQ(string msg, MusicesT model, out long songid)
        {
            try
            {
                if (model == null)
                {
                    songid = 0;
                    return "未搜索到歌曲";
                }
                if (msg.Contains("。。"))
                {
                    var singer = msg.Substring(msg.IndexOf("。。") + 2);
                    var song = msg.Substring(0, msg.IndexOf("。。"));
                    if (C_SeachMusic.GetQQlSearch(song).Where(a => a.singers.Contains(singer)).FirstOrDefault() == null)
                    {
                        songid = 0;
                        return "未搜索到" + singer + "的《" + song + "》";
                    }
                    songid = C_SeachMusic.GetQQlSearch(song).Where(a => a.singers.Contains(singer)).FirstOrDefault().songid;
                    return "";
                }
                songid = model.songid;
                return "";
            }
            catch (Exception ex)
            {
                songid = 0;
                return "未搜索到歌曲";
            }
        }


    }

}