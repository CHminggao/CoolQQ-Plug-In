using Native.Csharp.App.Event;
using Native.Csharp.App.Event.Model;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlne.loneme.group.Code.Event
{
    public class Event_CqAppEnable : IAppEnable
    {
        public void AppEnable(object sender, CQAppEnableEventArgs e)
        {
            if (!sqliteHelper.HasConfig(e.CQApi.AppDirectory))
                sqliteHelper.CreateConfig(e.CQApi.AppDirectory);
        }
    }
}
