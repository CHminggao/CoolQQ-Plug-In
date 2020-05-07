using Native.Tool.IniConfig.Linq;
using System;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace Native.Csharp.App.Event.Model
{
    public class sqliteHelper
    {
        //管理员
        public static long admin;

        #region 初始化配置文件

        /// <summary>
        /// 判断配置文件是否存在
        /// </summary>
        /// <param name="rootpath"></param>
        /// <returns></returns>
        public static bool HasConfig(string rootpath)
        {
            try
            {
                string filespath = rootpath + "config.ini";
                bool has = File.Exists(filespath);
                return has;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// 创建配置文件
        /// </summary>
        /// <returns></returns>
        public static bool CreateConfig(string rootpath)
        {
            try
            {
                IniObject iObject = new IniObject()
                {
                    new IniSection("admin")
                    {
                        {"qq","363384882" }
                    },
                    new IniSection ("Create")
                    {
                        { "DateT", DateTime.Now}
                    },
                    new IniSection("InOne")
                    {
                        {"YinZhiChen","" }
                    }
                };

                iObject.Save(rootpath + "config.ini");
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        #endregion 初始化配置文件

        #region 数据库操作
/*
        private static string sqlconnect = "Data Source = thingsdata.db; Version=3;";

        private static SQLiteConnection Sqlite = new SQLiteConnection(sqlconnect);

        /// <summary>
        /// 初始化数据库文件 初始化管理员
        /// </summary>
        public static void GetIni()
        {
            IniObject iObject = IniObject.Load(Sdk.Cqp.CqApi.GetAppDirectory() + "config.ini", Encoding.Default);
            IniValue value = iObject["sqlite"]["path"];
            sqlconnect = "Data Source = " + value.ToString() + "; Version=3;";
            IniValue value1 = iObject["admin"]["qq"];
            admin = !string.IsNullOrEmpty(value1.ToString()) ? value1.ToInt64() : 0;
            if (admin == 0)
            {
                Sdk.Cqp.CQLog.(Sdk.Cqp.CQLog.Error, "配置文件", "管理员设置错误");
            }

            Sqlite = new SQLiteConnection(sqlconnect);
        }

        /// <summary>
        /// 获取群信息
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static GroupModel GetGroups()
        {
            GroupModel model = new GroupModel();

            string sqlcom = "select * from Group ";
            using (SQLiteCommand cmd = new SQLiteCommand(sqlcom, Sqlite))
            {
                try
                {
                    Sqlite.Open();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            model.Id = reader.GetInt32(0);
                        }
                    }
                    Sqlite.Close();
                }
                catch (Exception ex)
                {
                    Sqlite.Close();
                    Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Warning, "获取群信息", "发生错误！");
                }
                return model;
            }
            //SQLiteCommand cmd = new SQLiteCommand(sqlcom, Sqlite);
        }

        /// <summary>
        /// 新增群
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SaveGroup(GroupModel model)
        {
            string sqlcom = "insert into expSign(Group) values('" + model.Group + "')";
            using (SQLiteCommand cmd = new SQLiteCommand(sqlcom, Sqlite))
            {
                try
                {
                    Sqlite.Open();
                    cmd.ExecuteNonQuery();
                    Sqlite.Close();
                    return true;
                }
                catch (Exception)
                {
                    Sqlite.Close();
                    return false;
                }
            }
        }

        /// <summary>
        /// 是否包含群
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static bool HasGroup(long group)
        {
            bool isit = false;
            string sqlcom = "select * from Group Where Group="+group;
            using (SQLiteCommand cmd = new SQLiteCommand(sqlcom, Sqlite))
            {
                try
                {
                    Sqlite.Open();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        isit = true;
                    }
                    Sqlite.Close();
                }
                catch (Exception ex)
                {
                    Sqlite.Close();
                    Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Warning, "获取群信息", "发生错误！");
                }
                return isit;
            }
        }

        public static string GetDataStr()
        {
            string time = DateTime.Now.Date.ToString();
            if (DateTime.Now.Minute > 30)
            {
                time += "|" + (DateTime.Now.Hour + 1).ToString();
            }
            else
            {
                time += "|" + DateTime.Now.Hour.ToString();
            }
            return time;
        }*/

        #endregion 数据库操作
    }
}