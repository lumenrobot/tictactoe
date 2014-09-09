using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenPop.Mime;
using WCFSocmedProcessing.Model;

namespace WCFSocmedProcessing.Repository
{
    public class LumenChatterBotDBRepository
    {
        public static bool SaveNewInbox(List<Message> messages)
        {
            var isSuccess = false;
            var inboxMain = new InboxMain();
            var dc = new LumenChatterBotDBDataContext();

            try
            {
                inboxMain = (from a in dc.InboxMains where a.ReceivedDate == DateTime.Now select a).FirstOrDefault();
                if (inboxMain != null)
                {
                    //do nothing
                }
                else
                {
                    inboxMain.ReceivedDate = DateTime.Now;
                    dc.InboxMains.InsertOnSubmit(inboxMain);
                }

                if(messages.Any())
                {
                    foreach (var message in messages)
                    {
                        var inboxDetail = new InboxDetail();
                        inboxDetail.IdInboxMain = inboxMain.Id;
                        inboxDetail.HeaderTo = message.Headers.Subject;
                        dc.InboxDetails.InsertOnSubmit(inboxDetail);
                    }
                }
                isSuccess = true;

            }
            catch (Exception ex)
            {
                isSuccess = false;
            }
            return isSuccess;
        }
    }
}