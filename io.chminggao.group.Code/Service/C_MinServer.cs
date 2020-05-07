using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace onlne.loneme.group.Code { 

    /// <summary>
    /// 我的世界服务器状态检测
    /// </summary>
    public class C_MinServer
    {
        /// <summary>
        /// 服务器Motd
        /// </summary>
        public string ServerMotd { get; private set; }

        /// <summary>
        /// 服务器最多玩家
        /// </summary>
        public int MaxPlayerCount { get; private set; }

        /// <summary>
        /// 服务器当前在线玩家
        /// </summary>
        public int CurrentPlayerCount { get; private set; }

        /// <summary>
        /// 服务器版本
        /// </summary>
        public Version MinecraftVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="motd">服务器Motd</param>
        /// <param name="maxplayers">服务器最多玩家</param>
        /// <param name="playercount">服务器当前在线玩家</param>
        /// <param name="mcversion">服务器版本</param>
        public C_MinServer(string motd, int maxplayers, int playercount, Version mcversion)
        {
            this.ServerMotd = motd;
            this.MaxPlayerCount = maxplayers;
            this.CurrentPlayerCount = playercount;
            this.MinecraftVersion = mcversion;
        }


        /// <summary>
        /// 检测MC服务器是否正常
        /// </summary>
        /// <param name="host">域名或者ip</param>
        /// <param name="port">端口</param>
        public static C_MinServer CheckServer(string host, int port)
        {
            try
            {
                string ip = null;
                IPAddress tip;
                //IPHostEntry iph = Dns.GetHostEntry(host);  //原转域名到ip
                //验证是否为域名 
                if(!IPAddress.TryParse(host, out tip))
                {
                    IPHostEntry iph = Dns.GetHostEntry(host);
                    foreach (IPAddress ip1 in iph.AddressList) // 取到ip地址
                    {
                        ip = ip1.ToString();
                        break;
                    }
                }
                else
                {
                    ip = tip.ToString();
                }
                
                C_MinServer a = null;
                IPAddress c = IPAddress.Parse(ip);
                IPEndPoint b = new IPEndPoint(c, port);
                if (b == null)
                {
                    //地址错误
                    return null;
                }
                string[] packertdat = null;
                using (TcpClient client = new TcpClient())
                {
                    //client.Connect(b);
                    var begin= client.BeginConnect(c,port,null,null);
                    var success = begin.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(0.5));
                    if (!success)
                    {
                        return null;
                    }
                    client.EndConnect(begin);
                    using (NetworkStream ns = client.GetStream())
                    {
                        ns.Write(new byte[] { 0xFe, 0x01 }, 0, 2);
                        byte[] buff = new byte[2048];
                        int br = ns.Read(buff, 0, buff.Length);
                        if (buff[0] != 0xFF)
                        {
                            //无效包
                            return null;
                        }
                        string packet = Encoding.BigEndianUnicode.GetString(buff, 3, br - 3);
                        if (!packet.StartsWith("§"))
                        {
                            //无效数据
                            return null;
                        }
                        packertdat = packet.Split('\u0000');
                        ns.Close();
                    }
                    client.Close();
                    return new C_MinServer(packertdat[3], int.Parse(packertdat[5]), int.Parse(packertdat[4]), Version.Parse(packertdat[2]));
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("无法连接到服务器");
                return null;
                //throw new Exception("连接问题，查看InnerException以获得详细信息", ex);
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine("接收到的数据无效");
                return null;
                //throw new Exception("接收到的数据无效，查看InnerException以获取详细信息", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("有一个问题");
                return null;
                //throw new Exception("有一个问题，查看InnerException以获得详细信息", ex);
            }
        }
    }
}
