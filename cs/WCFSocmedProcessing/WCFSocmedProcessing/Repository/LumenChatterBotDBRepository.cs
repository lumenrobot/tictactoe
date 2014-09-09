using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using OpenPop.Mime;
using WCFSocmedProcessing.Model;

namespace WCFSocmedProcessing.Repository
{
    public class LumenChatterBotDBRepository
    {
        private const string className = "LumenChatterBotDBRepository";
        private const string ok = "OK";
        private const string err = "ERROR";
        private const string scheduleDateVar = "ScheduleDate";
        private const string scheduleModeVar = "ScheduleMode";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="messageInfo"></param>
        /// <param name="fileName"> </param>
        /// <returns></returns>
        public static bool SaveSentAttachment(string fileName, int id, out string messageInfo)
        {
            var isSuccess = false;
            var dc = new LumenChatterBotDBDataContext();
            messageInfo = className + ".SaveSentAttachment(string fileName,int id, out string messageInfo) - ";

            try
            {
                var draft = (from a in dc.Sents where a.Id == id select a).FirstOrDefault();
                draft.IsAttachment = true;

                var attachment = new Sent_Attachment();
                attachment.FileName = fileName;
                attachment.FilePath = ConfigurationManager.AppSettings["ImageDirectory"] + fileName;
                attachment.IdSent = id;

                dc.Sent_Attachments.InsertOnSubmit(attachment);
                dc.SubmitChanges();

                var draftToPost =
                    (from a in dc.General_Variables where a.VariableName == "DraftToPost" select a).FirstOrDefault();

                if(draftToPost!=null)
                {
                    draftToPost.Value = id.ToString();
                    dc.SubmitChanges();
                }
                else
                {
                    draftToPost=new General_Variable();
                    draftToPost.VariableName = "DraftToPost";
                    draftToPost.Value = id.ToString();
                    dc.General_Variables.InsertOnSubmit(draftToPost);
                    dc.SubmitChanges();
                }

                isSuccess = true;
                messageInfo += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                messageInfo += err;
                messageInfo += " : " + ex.Message;
            }

            return isSuccess;
        }

        /// <summary>
        /// Save list of inbox to DB
        /// <param name="messages">List of message email</param>
        /// <param name="messageInfo">Message string for log information</param>
        /// </summary>
        /// <returns>True if method run well, false if error occur on method.</returns>
        public static bool SaveNewInbox(List<Message> messages, out string messageInfo)
        {
            var isSuccess = false;
            var dc = new LumenChatterBotDBDataContext();
            messageInfo = className + ".SaveNewInbox(List<Message> messages,out string messageInfo) - ";

            try
            {
                foreach (var a in messages)
                {
                    var inbox = new Inbox();
                    DateTime receiveDate;
                    var isParsed = DateTime.TryParse(a.Headers.Date, out receiveDate);
                    if (!isParsed)
                    {
                        receiveDate = DateTime.Now;
                        //todo: error notification
                    }
                    inbox.ReceivedDate = receiveDate;
                    inbox.FromName = a.Headers.From.DisplayName;
                    inbox.FromAddress = a.Headers.From.Address;
                    //inbox.CcName = "";
                    //inbox.CcAddress = "";
                    //inbox.ToName = "";
                    //inbox.ToAddress = "";
                    inbox.Subject = a.Headers.Subject;

                    MessagePart plainText = a.FindFirstPlainTextVersion();
                    var builder = new StringBuilder();
                    builder.Append(plainText.GetBodyAsText());
                    string bodyTextPlain = builder.ToString();
                    inbox.BodyTextPlain = bodyTextPlain;
                    //inbox.BodyHtml = "";
                    //inbox.BodyComposite = "";
                    inbox.IsAttachment = a.MessagePart.IsAttachment;
                    inbox.IsFiltered = false;

                    if (a.Headers.ReplyTo != null)
                    {
                        inbox.ReplyToName = a.Headers.ReplyTo.DisplayName;
                        inbox.ReplyToAddress = a.Headers.ReplyTo.Address;
                    }

                    dc.Inboxes.InsertOnSubmit(inbox);
                    dc.SubmitChanges();

                }

                isSuccess = true;
                messageInfo += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                messageInfo += err;
                messageInfo += " : " + ex.Message;
            }

            return isSuccess;
        }

        /// <summary>
        /// Save log to DB. If SaveLog it's self error, than it will write log to a file.
        /// <param name="message">Out message for erro info.</param>
        /// <param name="messageToSave">Log information to save if any.</param>
        /// <param name="logTypeId">Log type id, see Master DB about log type.</param>
        /// <param name="status">Status of method to save in log.</param>
        /// </summary>
        public static bool SaveLog(out string message, string messageToSave, int logTypeId, bool status)
        {
            var isSuccess = false;
            message = "";
            try
            {
                var dc = new LumenChatterBotDBDataContext();
                var logType = (from a in dc.Master_LogTypes where a.Id == logTypeId select a).FirstOrDefault();
                var eventLog = new EventLog();
                eventLog.CreateDate = DateTime.Now;
                if (logType != null)
                {
                    eventLog.LogType = logType.TypeName;
                }
                else
                {
                    eventLog.LogType = "- Not Define -";
                }
                eventLog.Message = messageToSave;
                eventLog.IsError = !status;

                dc.EventLogs.InsertOnSubmit(eventLog);
                dc.SubmitChanges();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = ex.Message;
                //todo: save error to file 
            }
            return isSuccess;
        }

        /// <summary>
        /// Get status to post to FB randomly.
        /// <param name="message">Out message for error info.</param>
        /// <param name="statusMessageToPost">Message to post to socmed.</param>
        /// </summary>
        public static bool GetStatusAutomatic(out string message, out string statusMessageToPost)
        {
            var isSuccess = false;
            message = className + ".GetStatusAutomatic() - ";
            const int idMKeyStatus = 1; //kata mutiara
            const int totalMessage = 50;
            const int randomMax = 1000000;
            const string varStatusName = "ListStatusId";
            var statusIndex = 0;
            statusMessageToPost = "";
            var isRepeatedPost = false;

            try
            {
                var dc = new LumenChatterBotDBDataContext();
                var rawListStr =
                    (from a in dc.General_Variables where a.VariableName == varStatusName select a).FirstOrDefault();
                if (rawListStr.Value == null)
                {
                    statusIndex = 1;
                }
                else
                {
                    var arrayStr = rawListStr.Value.Split(',');
                    var listIndexDb = new List<int>();
                    foreach (var str in arrayStr)
                    {
                        int num = 0;
                        var isParsed = int.TryParse(str, out num);
                        if (isParsed)
                        {
                            listIndexDb.Add(num);
                        }
                    }

                    // random generate algorithm

                    do
                    {
                        Random rnd = new Random();
                        int rndNum = rnd.Next(randomMax);
                        statusIndex = rndNum % totalMessage;
                        var exist = listIndexDb.Contains(statusIndex);

                        if (exist)
                        {
                            isRepeatedPost = true;
                        }
                        else
                        {
                            isRepeatedPost = false;
                        }

                    } while (isRepeatedPost);
                }

                statusMessageToPost =
                    (from a in dc.Master_StatusDetails
                     where a.IdMKeyStatus == idMKeyStatus && a.Id == statusIndex
                     select a.Message).FirstOrDefault();
                rawListStr.Value += "," + statusIndex.ToString();
                dc.SubmitChanges();

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }
            return isSuccess;
        }

        /// <summary>
        /// Get Status Automatic With Attachment
        /// </summary>
        /// <param name="message"></param>
        /// <param name="statusMessageToPost"></param>
        /// <returns></returns>
        public static bool GetStatusAutomaticWithAttachment(out string message, out string statusMessageToPost)
        {
            var isSuccess = false;
            message = className + ".GetStatusAutomaticWithAttachment() - ";
            const int idMKeyStatus = 1; //kata mutiara
            const int totalMessage = 50;
            const int randomMax = 1000000;
            const string varStatusName = "ListStatusId";
            var statusIndex = 0;
            statusMessageToPost = "";
            var isRepeatedPost = false;

            try
            {
                var dc = new LumenChatterBotDBDataContext();
                var rawListStr =
                    (from a in dc.General_Variables where a.VariableName == varStatusName select a).FirstOrDefault();
                if (rawListStr.Value == null)
                {
                    statusIndex = 1;
                }
                else
                {
                    var arrayStr = rawListStr.Value.Split(',');
                    var listIndexDb = new List<int>();
                    foreach (var str in arrayStr)
                    {
                        int num = 0;
                        var isParsed = int.TryParse(str, out num);
                        if (isParsed)
                        {
                            listIndexDb.Add(num);
                        }
                    }

                    // random generate algorithm

                    do
                    {
                        Random rnd = new Random();
                        int rndNum = rnd.Next(randomMax);
                        statusIndex = rndNum % totalMessage;
                        var exist = listIndexDb.Contains(statusIndex);

                        if (exist)
                        {
                            isRepeatedPost = true;
                        }
                        else
                        {
                            isRepeatedPost = false;
                        }

                    } while (isRepeatedPost);
                }

                statusMessageToPost =
                    (from a in dc.Master_StatusDetails
                     where a.IdMKeyStatus == idMKeyStatus && a.Id == statusIndex
                     select a.Message).FirstOrDefault();
                rawListStr.Value += "," + statusIndex.ToString();
                dc.SubmitChanges();

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }
            return isSuccess;
        }

        /// <summary>
        /// Get schedule list randomly.
        /// <param name="scheduleModeBefore">scheduler mode last used.</param>
        /// </summary>
        public static int GetRandomSchedulerMode(string scheduleModeBefore)
        {
            int mode = 1;
            try
            {
                var isParsed = int.TryParse(scheduleModeBefore, out mode);
                var dc = new LumenChatterBotDBDataContext();
                var result =
                    (from a in dc.Master_Schedulers where a.Id != mode select a).ToList();

                if (result.Any())
                {
                    Random rnd = new Random();
                    int rndInt = rnd.Next(1000000);
                    var indexList = rndInt % result.Count;
                    mode = result[indexList].Id;
                }

            }
            catch (Exception)
            {
                //todo : implement catch
                throw;
            }
            return mode;
        }

        /// <summary>
        /// Get key-value from GeneralVariable list.
        /// <param name="varName">string variable name(key).</param>
        /// </summary>
        public static string GetGeneralVariable(string varName)
        {
            var result = "";
            try
            {
                var dc = new LumenChatterBotDBDataContext();
                result =
                    (from a in dc.General_Variables where a.VariableName == varName select a.Value).
                        FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = "";
                //todo: implement 
                throw;
            }
            return result;
        }

        /// <summary>
        /// Create new GeneralVariable or change the value if exist.
        /// <param name="varName">string variable name(key).</param>
        /// </summary>
        public static bool SetGeneralVariable(string varName, string varValue)
        {
            var isSuccess = false;
            try
            {
                var dc = new LumenChatterBotDBDataContext();
                var result =
                    (from a in dc.General_Variables where a.VariableName == varName select a).
                        FirstOrDefault();

                //create new general variable if not exist
                if (result == null)
                {
                    result = new General_Variable();
                    result.VariableName = varName;
                    result.Value = varValue;
                    dc.General_Variables.InsertOnSubmit(result);
                }
                else
                {
                    result.Value = varValue;
                }

                dc.SubmitChanges();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                //todo: implement 
                throw;
            }
            return isSuccess;
        }

        /// <summary>
        /// Get schedule for next 60 minutes forward.
        /// <param name="message">out for error/success status.</param>
        /// <param name="dateClient">date and time client request schedule.</param>
        /// <param name="listSchedule">out list schedule and function to be execute at certain time.</param>
        /// </summary>
        public static bool GetNextSchedule(out string message, DateTime dateClient, out List<View_ScheduleDetail> listSchedule)
        {
            TimeSpan clientTimeSpan = new TimeSpan(dateClient.Hour, dateClient.Minute, dateClient.Second);
            TimeSpan nextClientTimeSpanPeriod = clientTimeSpan.Add(new TimeSpan(1, 0, 0));
            listSchedule = new List<View_ScheduleDetail>();
            var isSuccess = false;
            message = className + ".GetNextSchedule(out string message, DateTime dateNow, out List<View_ScheduleDetail> listScheduler) - ";

            try
            {
                var dc = new LumenChatterBotDBDataContext();
                var lastExecuteDate = DateTime.Now;

                var lastExecuteDateStr = GetGeneralVariable(scheduleDateVar);
                var isParsed = false;
                var isSameDay = false;

                //Check that clientRequest == DB Last execution date
                //if yes, it means in same day, if no then load new mode
                if (lastExecuteDateStr.Any())
                {
                    isParsed = DateTime.TryParse(lastExecuteDateStr, out lastExecuteDate);
                }

                //if sameday, you dont need to get new mode, just run the same mode.
                var scheduleMode = 0;
                var scheduleModeStr = GetGeneralVariable(scheduleModeVar);
                var isParsedScheduleMode = int.TryParse(scheduleModeStr, out scheduleMode);

                if (isParsedScheduleMode)
                {
                    isSameDay = (lastExecuteDate.Date == dateClient.Date);
                    if (isSameDay)
                    {
                        //do nothing
                    }
                    else
                    {
                        //get new schedulemode
                        scheduleMode = GetRandomSchedulerMode(scheduleModeStr);
                        scheduleModeStr = scheduleMode.ToString();
                        var changedDB = SetGeneralVariable(scheduleModeVar, scheduleModeStr);
                        changedDB = SetGeneralVariable(scheduleDateVar, DateTime.Now.ToString());
                    }
                }

                //Get listSchedule event next 60 minutes
                listSchedule = (from a in dc.View_ScheduleDetails
                                where a.ExecuteTime > clientTimeSpan && a.ExecuteTime <= nextClientTimeSpanPeriod
                                && a.IsMScheduleActive == true && a.IdMScheduler == scheduleMode
                                && a.IsScheduleDetailActive == true
                                orderby a.ExecuteTime ascending
                                select a).ToList();

                isSuccess = true;
                message += ok;
            }
            catch (Exception)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        #region Mail and Facebook

        //template function
        /*
        /// <summary>
        /// Save draft mail to database (Sent, Sent_Facebook) and change inbox that is analized.
        /// <param name="sentDraftList">list sent draft.</param>
        /// <param name="sentFbDraftList">list sent_fb draft.</param>
        /// <param name="idInboxList">list id inbox to change isAnalized property.</param>
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool SaveSentDraft(List<Sent> sentDraftList, List<Sent_Facebook> sentFbDraftList, List<int> idInboxList , out string message)
        {
            inboxFb=new List<Inbox_Facebook>();
            message = "";
            var isSuccess = false;
            message = className + ".FilterInboxToFacebookData(out string message, DateTime dateNow, out List<View_ScheduleDetail> listScheduler) - ";
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }
            try
            {
                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }
         */
        /// <summary>
        /// Sent happybirthday to friends.
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool SentWeatherInfoToday(out string message)
        {
            message = "";
            var isSuccess = false;
            message = className + ".SentWeatherInfoToday(out string message) - ";

            //variable lainnya

            try
            {
                var dc = new LumenChatterBotDBDataContext();

                var weatherToday = (from a in dc.Inboxes
                                    where a.Subject.Contains("bandung") && a.Subject.Contains("today")
                                    && a.ReceivedDate.Value.Date == DateTime.Today.Date
                                    orderby a.ReceivedDate descending
                                    select a).FirstOrDefault();

                if (weatherToday != null)
                {
                    var isSent = EmailHandlingRepository.UpdateFacebookStatus(out message, weatherToday.Subject);

                    if (isSent)
                    {
                        var sent = new Sent();
                        sent.IsActive = false;
                        sent.Subject = weatherToday.Subject;
                        sent.FromName = "Lumen Lumen";
                        sent.ToName = "Facebook";
                        sent.CreateDate = DateTime.Now;
                        sent.SentDate = sent.CreateDate;
                        sent.IsAttachment = false;
                        sent.IsScheduled = false;

                        dc.Sents.InsertOnSubmit(sent);
                        dc.SubmitChanges();
                    }



                }

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// Sent happybirthday to friends.
        /// <param name="isTesting">bool, true for development, false for real sending to fb .</param>
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool SentHappyBirthdayMail(out string message)
        {
            message = "";
            var isSuccess = false;
            message = className + ".SentHappyBirthdayMail(out string message) - ";

            //variable lainnya

            try
            {
                var dc = new LumenChatterBotDBDataContext();

                var happyFriend = (from a in dc.Master_FacebookFriends
                                   where a.Birthday.HasValue && a.Birthday.Value.Date.Day == DateTime.Now.Date.Day
                                   && a.Birthday.Value.Date.Month == DateTime.Now.Date.Month
                                   select a).ToList();

                if (happyFriend.Any())
                {
                    foreach (var friend in happyFriend)
                    {
                        var happyExpression =
                        (from a in dc.View_Master_KeyReplies
                         where a.NameMKeyReply == "happy birthday expression"
                         select a).ToList();

                        if (happyExpression.Any())
                        {
                            var ran = new Random();
                            int ranNum = ran.Next(1000000);
                            int happyIndex = ranNum % happyExpression.Count;
                            var happyMessage = happyExpression[happyIndex].Message;
                            if (friend.FbId.Any())
                            {
                                happyMessage += " @[" + friend.FbId + ":]";
                            }
                            else
                            {
                                happyMessage += "@" + friend.FbName;
                            }

                            var isSent = EmailHandlingRepository.UpdateFacebookStatus(out message, happyMessage);

                            if (isSent)
                            {
                                var sent = new Sent();
                                sent.IsActive = false;
                                sent.Subject = happyMessage;
                                sent.FromName = "Lumen Lumen";
                                sent.ToName = "Facebook";
                                sent.CreateDate = DateTime.Now;
                                sent.SentDate = sent.CreateDate;
                                sent.IsAttachment = false;
                                sent.IsScheduled = false;

                                dc.Sents.InsertOnSubmit(sent);
                                dc.SubmitChanges();
                            }
                        }
                    }

                }

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// Sent Draft Mail By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool SentDraftMailById(out string message)
        {
            message = "";
            message = className + ".SentDraftMailById(out string message) - ";
            var isSuccess = false;


            //variable lainnya

            try
            {
                var msg = "";
                var dc = new LumenChatterBotDBDataContext();
                var draftToPost =
                    (from a in dc.General_Variables where a.VariableName == "DraftToPost" select a).FirstOrDefault();
                int id = 0;
                var isParsed = int.TryParse(draftToPost.Value,out id);
                var viewSentDraft = (from a in dc.View_Sent_Facebooks where a.Id == id select a).FirstOrDefault();
                var isTesting = bool.Parse(ConfigurationManager.AppSettings["IsTesting"]);

                if (viewSentDraft != null)
                {
                    var isSent = false;
                    if(viewSentDraft.IsAttachment.Value)
                    {
                        var viewSentDraftWithAttachment = (from a in dc.View_Sent_Attachments where a.IdSent == id select a).FirstOrDefault();
                        isSent = EmailHandlingRepository.UpdateFacebookStatusWithAttachment(viewSentDraft.Subject, viewSentDraftWithAttachment.FilePath, out msg);
                    }
                    else
                    {
                        isSent = EmailHandlingRepository.UpdateFacebookStatus(out msg, viewSentDraft.Subject);
                    }
                    if (isSent)
                    {
                        if (!isTesting)
                        {
                            var item = (from b in dc.Sents where b.Id == id select b).FirstOrDefault();
                            if (item != null)
                            {
                                item.SentDate = DateTime.Now;
                                item.IsActive = false;
                            }
                            draftToPost.Value = "0";
                            dc.SubmitChanges();
                        }
                    }
                    else
                    {
                        message += "draftId = " + id + " " + msg + " ; ";
                    }
                }

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// SentDraftMailById
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool SentDraftMailById(int id, out string message)
        {
            message = "";
            message = className + ".SentDraftMailById(int id, out string message) - ";
            var isSuccess = false;


            //variable lainnya

            try
            {
                var msg = "";
                var dc = new LumenChatterBotDBDataContext();
                var viewSentDraft = (from a in dc.View_Sent_Attachments where a.IdSent == id select a).FirstOrDefault();
                var isTesting = bool.Parse(ConfigurationManager.AppSettings["IsTesting"]);

                if (viewSentDraft != null)
                {
                    var isSent = EmailHandlingRepository.UpdateFacebookStatusWithAttachment(viewSentDraft.Subject, viewSentDraft.FilePath, out msg);
                    if (isSent)
                    {
                        if (!isTesting)
                        {
                            var item = (from b in dc.Sents where b.Id == id select b).FirstOrDefault();
                            if (item != null)
                            {
                                item.SentDate = DateTime.Now;
                                item.IsActive = false;
                                dc.SubmitChanges();
                            }
                        }
                    }
                    else
                    {
                        message += "draftId = " + id + " " + msg + " ; ";
                    }
                }

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// Sent draft to server and fill checklist Sent DB.
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool SentDraftMail(out string message)
        {
            message = "";
            message = className + ".SentDraftMail( out string message) - ";
            var isSuccess = false;


            //variable lainnya
            var draftList = new List<View_Sent_Facebook>();
            var sentMailList = new List<Sent>();

            try
            {
                var msg = "";
                isSuccess = GetDraftSentToFacebook(null, out draftList, out msg);

                if (isSuccess)
                {
                    foreach (var a in draftList)
                    {
                        var isSent = EmailHandlingRepository.SendMail(out msg, a);
                        if (isSent)
                        {
                            var draft = new Sent();
                            draft.Id = a.Id;
                            draft.SentDate = DateTime.Now;
                            sentMailList.Add(draft);
                        }
                        else
                        {
                            message += "draftId = " + a.Id + " " + msg + " ; ";
                        }
                    }
                    var isTesting = bool.Parse(ConfigurationManager.AppSettings["IsTesting"]);
                    if (!isTesting)
                    {
                        if (sentMailList.Any())
                        {
                            var dc = new LumenChatterBotDBDataContext();
                            foreach (var sent in sentMailList)
                            {
                                var item = (from b in dc.Sents where b.Id == sent.Id select b).FirstOrDefault();
                                if (item != null)
                                {
                                    item.SentDate = sent.SentDate;
                                    item.IsActive = false;
                                    dc.SubmitChanges();
                                }
                            }
                        }
                    }

                }

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// Save draft mail to database (Sent, Sent_Facebook) manually.
        /// <param name="draft">draft.</param>
        /// <param name="id">id saved draft.</param>
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool CreateDraftManual(Sent draft, out int id, out string message)
        {
            const int idMessage = 6;
            id = 0;
            message = "";
            var isSuccess = false;
            message = className + ".CreateDraftManual(Sent draft, out string message) - ";

            try
            {
                var dc = new LumenChatterBotDBDataContext();

                if (draft != null)
                {
                    //save sent
                    dc.Sents.InsertOnSubmit(draft);
                    dc.SubmitChanges();
                    id = draft.Id;

                    //save sent_facebook
                    Sent_Facebook draftFb = new Sent_Facebook();
                    draftFb.Id = draft.Id;
                    draftFb.IdMessageType = idMessage;
                    draftFb.Message = draft.Subject;
                    draftFb.ToNameFb = draft.ToName;
                    draftFb.ToAddressFb = draft.ToAddress;
                    dc.Sent_Facebooks.InsertOnSubmit(draftFb);
                    dc.SubmitChanges();

                    var draftToPost =
                    (from a in dc.General_Variables where a.VariableName == "DraftToPost" select a).FirstOrDefault();

                    if (draftToPost != null)
                    {
                        draftToPost.Value = id.ToString();
                        dc.SubmitChanges();
                    }
                    else
                    {
                        draftToPost = new General_Variable();
                        draftToPost.VariableName = "DraftToPost";
                        draftToPost.Value = id.ToString();
                        dc.General_Variables.InsertOnSubmit(draftToPost);
                        dc.SubmitChanges();
                    }
                }

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// Save draft mail to database (Sent, Sent_Facebook) and change inbox that is analized.
        /// <param name="sentDraftList">list sent draft.</param>
        /// <param name="sentFbDraftList">list sent_fb draft.</param>
        /// <param name="idInboxList">list id inbox to change isAnalized property.</param>
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool SaveSentDraft(List<Sent> sentDraftList, List<Sent_Facebook> sentFbDraftList, List<int> idInboxList, out string message)
        {
            message = "";
            var isSuccess = false;
            message = className + ".SaveSentDraft(List<Sent> sentDraftList, List<Sent_Facebook> sentFbDraftList, List<int> idInboxList , out string message) - ";

            try
            {
                var dc = new LumenChatterBotDBDataContext();

                if (sentDraftList.Any() && sentFbDraftList.Any() && (sentDraftList.Count == sentFbDraftList.Count))
                {
                    for (var i = 0; i < sentDraftList.Count; i++)
                    {
                        //save sent
                        Sent draft = sentDraftList[i];
                        dc.Sents.InsertOnSubmit(draft);
                        dc.SubmitChanges();

                        //save sent_facebook
                        Sent_Facebook draftFb = sentFbDraftList[i];
                        draftFb.Id = draft.Id;
                        dc.Sent_Facebooks.InsertOnSubmit(draftFb);
                        dc.SubmitChanges();

                        //change value isAnalized inboxFacebook
                        var inboxFb =
                            (from a in dc.Inbox_Facebooks where a.Id == idInboxList[i] select a).FirstOrDefault();
                        inboxFb.IsAnalized = true;
                        dc.SubmitChanges();
                    }
                }

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// Save draft mail to database (Sent, Sent_Facebook).
        /// <param name="sentDraftList">list sent draft.</param>
        /// <param name="sentFbDraftList">list sent_fb draft.</param>
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool SaveSentDraft(List<Sent> sentDraftList, List<Sent_Facebook> sentFbDraftList, out string message)
        {
            message = "";
            var isSuccess = false;
            message = className + ".SaveSentDraft(List<Sent> sentDraftList, List<Sent_Facebook> sentFbDraftList, out string message) - ";

            try
            {
                var dc = new LumenChatterBotDBDataContext();

                if (sentDraftList.Any() && sentFbDraftList.Any() && (sentDraftList.Count == sentFbDraftList.Count))
                {
                    for (var i = 0; i < sentDraftList.Count; i++)
                    {
                        Sent draft = sentDraftList[i];

                        dc.Sents.InsertOnSubmit(draft);
                        dc.SubmitChanges();

                        Sent_Facebook draftFb = sentFbDraftList[i];
                        draftFb.Id = draft.Id;
                        dc.Sent_Facebooks.InsertOnSubmit(draftFb);
                        dc.SubmitChanges();
                    }
                }

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// Get draft to sent to fb.
        /// <param name="date">date and time filter.</param>
        /// <param name="draftToFb">out list draft to sent to fb.</param>
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool GetDraftSentToFacebook(DateTime? date, out List<View_Sent_Facebook> draftToFb, out string message)
        {
            draftToFb = new List<View_Sent_Facebook>();
            message = "";
            var isSuccess = false;
            message = className + ".GetDraftSentToFacebook(DateTime? date, out List<View_Sent_Facebook> draftToFb, out string message) - ";

            try
            {
                var dc = new LumenChatterBotDBDataContext();

                //load all inboxFb !IsAnalized 
                if (!date.HasValue)
                {
                    draftToFb = (from a in dc.View_Sent_Facebooks
                                 where a.IsActive == true && a.IsScheduled != true
                                 orderby a.CreateDate ascending
                                 select a).ToList();
                }
                else
                {
                    //load invoxFb !IsAnalized at certain date
                    draftToFb = (from a in dc.View_Sent_Facebooks
                                 where a.IsActive == true && a.IsScheduled != true
                                 && a.CreateDate.Value.Date == date.Value.Date
                                 orderby a.CreateDate ascending
                                 select a).ToList();
                }

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }


        /// <summary>
        /// Get inbox facebook !IsAnalized.
        /// <param name="inboxFb">out list inboxFb.</param>
        /// <param name="reply">reply text.</param>
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool GenerateSentDraft(View_Inbox_Facebook inboxFb, string reply, out Sent sentMail, out Sent_Facebook sentMailFb, out string message)
        {
            message = "";
            var isSuccess = false;
            message = className + ".GenerateSentDraft(View_Inbox_Facebook inboxFb, string reply, out string message) - ";
            sentMail = new Sent();
            sentMailFb = new Sent_Facebook();

            try
            {
                sentMail.CreateDate = DateTime.Now;
                sentMail.FromAddress = ConfigurationManager.AppSettings["MailAddressGmail"];
                sentMail.FromName = ConfigurationManager.AppSettings["MailOwnerNameGmail"];
                sentMail.ToName = inboxFb.ReplyToName;
                sentMail.ToAddress = inboxFb.ReplyToAddress;
                sentMail.Subject = "Re: " + inboxFb.Subject;
                sentMail.BodyTextPlain = reply;
                sentMail.IsAttachment = false;
                sentMail.IsScheduled = false;
                sentMail.IsActive = true;

                //sentMailFacebook
                sentMailFb.IdMessageType = inboxFb.IdMessageType;
                sentMailFb.Message = reply;
                sentMailFb.ToNameFb = inboxFb.FromNameFb;

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                sentMail = new Sent();
                sentMailFb = new Sent_Facebook();
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// Get inbox facebook !IsAnalized.
        /// <param name="inboxFb">out list inboxFb.</param>
        /// <param name="sentDraftList">out list sent draft.</param>
        /// <param name="sentFbDraftList">out list sent_fb draft.</param>
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool GetInboxFacebookResponse(List<View_Inbox_Facebook> inboxFb, out List<Sent> sentDraftList, out List<Sent_Facebook> sentFbDraftList, out string message)
        {
            message = "";
            var isSuccess = false;
            message = className + ".GetInboxFacebookResponse(List<View_Inbox_Facebook> inboxFb, out string message) - ";
            var responseReply = "";
            sentDraftList = new List<Sent>();
            sentFbDraftList = new List<Sent_Facebook>();

            try
            {
                var dc = new LumenChatterBotDBDataContext();

                if (inboxFb.Any())
                {
                    foreach (var a in inboxFb)
                    {
                        var response = (from b in dc.View_Master_KeyReplies
                                        where b.NameMKeyReply == a.Message
                                        select b).ToList();
                        if (response.Any())
                        {
                            //get unique response 
                            var random = new Random();
                            int responseIndex = random.Next(0, 1000000);
                            var responChosen = response[responseIndex % response.Count];
                            responseReply = responChosen.Message;
                        }
                        else
                        {
                            response = (from b in dc.View_Master_KeyReplies
                                        where b.NameMKeyReply == "-no refference-"
                                        select b).ToList();
                            if (response.Any())
                            {
                                var random = new Random();
                                int responseIndex = random.Next(0, 1000000);
                                var responChosen = response[responseIndex % response.Count];
                                responseReply = responChosen.Message;

                            }
                            else
                            {
                                responseReply = ".....do nothing.....";
                            }
                        }

                        var sentDraft = new Sent();
                        var sentFbDraft = new Sent_Facebook();
                        var infoDraft = "";
                        var isDraftCreated = GenerateSentDraft(a, responseReply, out sentDraft, out sentFbDraft,
                                                               out infoDraft);
                        if (isDraftCreated)
                        {
                            sentDraftList.Add(sentDraft);
                            sentFbDraftList.Add(sentFbDraft);
                        }
                    }
                }



                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// Get inbox facebook !IsAnalized.
        /// <param name="date">date and time filter.</param>
        /// <param name="inboxFb">out list inboxFb.</param>
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool GetInboxFacebook(DateTime? date, out List<View_Inbox_Facebook> inboxFb, out string message)
        {
            inboxFb = new List<View_Inbox_Facebook>();
            message = "";
            var isSuccess = false;
            message = className + ".GetInboxFacebook(DateTime? date, out List<View_Inbox_Facebook> inboxFb, out string message) - ";

            try
            {
                var dc = new LumenChatterBotDBDataContext();

                //load all inboxFb !IsAnalized 
                if (!date.HasValue)
                {
                    inboxFb = (from a in dc.View_Inbox_Facebooks
                               where a.IsAnalized != true && a.ReplyToName != "noreply"
                               orderby a.ReceivedDate ascending
                               select a).ToList();
                }
                else
                {
                    //load invoxFb !IsAnalized at certain date
                    inboxFb = (from a in dc.View_Inbox_Facebooks
                               where a.IsAnalized != true && a.ReceivedDate.Value.Date == date.Value.Date
                               && a.ReplyToName != "noreply"
                               orderby a.ReceivedDate ascending
                               select a).ToList();
                }

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// Get schedule for next 60 minutes forward.
        /// <param name="date">date and time filter.</param>
        /// <param name="inboxFb">out list inboxFb.</param>
        /// <param name="message">out for error/success status.</param>
        /// </summary>
        public static bool FilterInboxToFacebookData(DateTime? date, out List<Inbox_Facebook> inboxFb, out string message)
        {
            inboxFb = new List<Inbox_Facebook>();
            message = "";
            var isSuccess = false;
            message = className + ".FilterInboxToFacebookData(DateTime? date, out List<Inbox_Facebook> inboxFb, out string message) - ";

            try
            {
                var dc = new LumenChatterBotDBDataContext();
                var inbox = new List<Inbox>();
                if (!date.HasValue)
                {
                    inbox = (from a in dc.Inboxes
                             where a.FromName.ToUpper() == "FACEBOOK" && a.ReplyToName != null
                                   && a.IsFiltered != true
                             orderby a.ReceivedDate ascending
                             select a).ToList();
                }
                else
                {
                    inbox = (from a in dc.Inboxes
                             where a.FromName.ToUpper() == "FACEBOOK" && a.ReplyToName != null
                                   && a.IsFiltered != true && a.ReceivedDate.Value.Date == date.Value.Date
                             orderby a.ReceivedDate ascending
                             select a).ToList();
                }

                var messageType = (from a in dc.Master_MessageTypes select a).ToList();

                if (inbox.Any() && messageType.Any())
                {
                    foreach (var x in inbox)
                    {
                        var fb = new Inbox_Facebook();
                        //get sender name
                        var sender = "";
                        var idMsgType = 0;
                        var mainMsg = "";
                        if (x.Subject.Any())
                        {
                            //split by messageType key
                            var isSplited = false;

                            //try to split with all keyword to find sender
                            //than filter message by switch case mechanism depent on param
                            foreach (Master_MessageType t in messageType)
                            {
                                if (!isSplited)
                                {
                                    string param = t.TypeName.Trim();
                                    try
                                    {
                                        if (x.Subject.Contains(param))
                                        {
                                            sender = x.Subject.Split(new[] { param }, StringSplitOptions.None)[0];
                                            sender = sender.Trim();
                                            idMsgType = t.Id;
                                            isSplited = true;

                                            //split message to get message main content
                                            try
                                            {
                                                switch (param)
                                                {
                                                    case "confirmed":
                                                        break;
                                                    case "commented":
                                                        mainMsg = x.BodyTextPlain.Split('"')[1];
                                                        break;
                                                    case "mentioned":
                                                        mainMsg = x.BodyTextPlain.Split('"')[1];
                                                        break;
                                                    case "shared":
                                                        break;
                                                    case "wants":
                                                        break;
                                                    case "posted":
                                                        mainMsg = x.BodyTextPlain.Split('"')[1];
                                                        break;
                                                    default:
                                                        break;

                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                mainMsg = "";
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        isSplited = false;
                                        sender = "";
                                        idMsgType = 0;
                                    }
                                }
                            }
                        }
                        fb.FromNameFb = sender;
                        fb.Id = x.Id;
                        fb.IdMessageType = idMsgType;
                        fb.IsAnalized = false;
                        fb.Message = mainMsg.Trim();

                        //add to list inboxFb
                        inboxFb.Add(fb);
                    }
                }

                isSuccess = true;
                message += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message += err;
            }

            return isSuccess;
        }

        /// <summary>
        /// Save list of inbox to DB
        /// <param name="inboxFb">List of facebook inbox message</param>
        /// <param name="messageInfo">Message string for log information</param>
        /// </summary>
        /// <returns>True if method run well, false if error occur on method.</returns>
        public static bool SaveNewInboxFacebook(List<Inbox_Facebook> inboxFb, out string messageInfo)
        {
            var isSuccess = false;
            var dc = new LumenChatterBotDBDataContext();
            messageInfo = className + ".SaveNewInboxFacebook(List<Inbox_Facebook> inboxFb, out string messageInfo) - ";

            try
            {
                if (inboxFb.Any())
                {
                    foreach (var a in inboxFb)
                    {
                        var mainInbox = (from b in dc.Inboxes where b.Id == a.Id select b).FirstOrDefault();
                        mainInbox.IsFiltered = true;
                        dc.Inbox_Facebooks.InsertOnSubmit(a);
                        dc.SubmitChanges();

                    }
                }

                isSuccess = true;
                messageInfo += ok;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                messageInfo += err;
                messageInfo += " : " + ex.Message;
            }

            return isSuccess;
        }


        #endregion
    }
}