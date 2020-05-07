using Native.Sdk.Cqp;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Tool.IniConfig.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlne.loneme.group.Code.Event
{
    /// <summary>
    /// 入群事件处理
    /// </summary>
    public class Event_SomeOneIn :IGroupMemberIncrease
    {
        public void GroupMemberIncrease(object sender, CQGroupMemberIncreaseEventArgs e)
        {
            IniObject iniObject = IniObject.Load(e.CQApi.AppDirectory + "config.ini", Encoding.UTF8);
            if ("711498146,222600436".IndexOf(e.FromGroup.Id.ToString())>=0)
            {
                string str = iniObject["InOne"]["YinZhiChen"].ToString().Replace(" ", "\n");
                
                e.FromGroup.SendGroupMessage(CQApi.CQCode_At(e.BeingOperateQQ) + str);
            }
        }
    }
    //public class Event_SomeOnIn1 : IReceiveGroupMemberPass
    //{
    //    public void ReceiveGroupMemberPass(object sender, CqGroupMemberIncreaseEventArgs e)
    //    {
    //        IniObject iniObject = IniObject.Load(e.CqApi.GetAppDirectory() + "config.ini", Encoding.Default);
    //        if (e.FromGroup == 222600436)
    //        {
    //            string str = iniObject["InOne"]["YinZhiChen"].ToString().Replace(" ", "\n");
    //            e.CqApi.SendGroupMessage(e.FromGroup,e.BeingOperateQQ + str);
    //        }
    //    }
    //}
}
