using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using OpenPop.Mime;
using WCFSocmedProcessing.Model;
using WCFSocmedProcessing.Repository;

namespace WCFSocmedProcessing
{

    #region TemplateFunction
    /*
    public bool SmartUpdateStatus(out string messageInfo)
        {
            messageInfo = "";
            var isSuccess = false;
            var isSuccessSaveLog = false;
            string message = "";
            int typeLog = 3; // nama tipe log
     
            //add another variable here
            //add logic here
            
            return isSuccess;
        }
    */
    #endregion

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {
        private static int _idSentCreateDraftMail = 0;

        

        /// <summary>
        /// Create Draft Mail From Vision Server
        /// </summary>
        /// <param name="messageToSent"></param>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        public bool CreateDraftMail(string messageToSent, out string messageInfo)
        {
            _idSentCreateDraftMail = 0;
            messageInfo = "";
            var isSuccess = false;
            var isSuccessSaveLog = false;
            string message = "";
            const int typeLog = 14; //Create Draft Mail

            //add another variable here
            var draft = new Sent();
            draft.FromName = ConfigurationManager.AppSettings["MailOwnerNameGmail"];
            draft.FromAddress = ConfigurationManager.AppSettings["MailAddressGmail"];
            draft.ToName = draft.FromName;
            draft.ToAddress = draft.FromAddress;
            var isTesting = bool.Parse(ConfigurationManager.AppSettings["IsTesting"]);
            if (!isTesting)
            {
                draft.ToName = ConfigurationManager.AppSettings["MailOwnerNameFacebook"];
                draft.ToAddress = ConfigurationManager.AppSettings["MailAddressFacebook"];
            }
            draft.Subject = messageToSent;
            draft.IsActive = true;  //not yet used
            draft.IsScheduled = false;
            draft.IsAttachment = false;
            draft.CreateDate = DateTime.Now;

            isSuccess = LumenChatterBotDBRepository.CreateDraftManual(draft, out _idSentCreateDraftMail, out message);
            if (isSuccess)
                messageInfo += message;
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
            if (isSuccessSaveLog)
                messageInfo += message;

            return isSuccess;
        }

        /// <summary>
        /// Upload Image with some information.
        /// </summary>
        /// <param name="file"></param>
        public void UploadImageStatus(DocInfo file)
        {
            var messageInfo = "";
            string message = "";
            bool isSuccess = false;
            bool isSuccessSaveLog = false;

            //see master_log_type DB
            const int typeLog = 11; //Upload Image

            var imageDirectory = ConfigurationManager.AppSettings["ImageDirectory"];
            var fileFullName = "";
            try
            {
                // If the directory doesn't exist, create it.
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }

                //chek file null or not
                bool isFileNull = file == null;
                if (!isFileNull)
                {
                    var tempName = string.IsNullOrEmpty(file.FileName) ? "temp" : file.FileName;
                    fileFullName = Guid.NewGuid() + "@" + tempName;
                    var writer = File.Create(imageDirectory + fileFullName);

                    var buffer = new byte[4096];
                    int byteRead;
                    while ((byteRead = file.FileContent.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        writer.Write(buffer, 0, byteRead);
                    }

                    file.FileContent.Close();
                    writer.Flush();
                    writer.Close();
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                // Fail silently
                message += ex.Message;
                isSuccess = false;
            }

            if (!isSuccess)
                messageInfo += message;

            //SaveLog() always done to save ERROR or OK status
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
            if (!isSuccessSaveLog)
                messageInfo += message;

            if (isSuccess)
            {
                isSuccess = LumenChatterBotDBRepository.SaveSentAttachment(fileFullName, _idSentCreateDraftMail, out message);
                if (!isSuccess)
                    messageInfo += message;
                isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
                if (!isSuccessSaveLog)
                    messageInfo += message;
            }

            ////Update Image Status
            //if (isSuccess)
            //{
            //    isSuccess = LumenChatterBotDBRepository.SentDraftMailById(_idSentCreateDraftMail, out message);
            //    if (!isSuccess)
            //        messageInfo += message;
            //    isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
            //    if (!isSuccessSaveLog)
            //        messageInfo += message;
            //}

        }

        /// <summary>
        /// Post Image Status Update to Facebook
        /// </summary>
        /// <param name="messageInfo"></param>
        public void PostImageStatusUpdate()
        {
            var messageInfo = "";
            var isSuccess = false;
            var isSuccessSaveLog = false;
            string message = "";
            const int typeLog = 15; //Post Image Status Update

            //add another variable here

            //Update Image Status

            isSuccess = LumenChatterBotDBRepository.SentDraftMailById(out message);
            if (!isSuccess)
                messageInfo += message;
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
            if (!isSuccessSaveLog)
                messageInfo += message;

            //return isSuccess;
        }


        /// <summary>
        /// Get email inbox from pop3 server and save inbox to DB
        /// </summary>
        public bool RetrieveInboxMail(out string messageInfo)
        {
            messageInfo = "";
            string message = "";
            List<Message> inboxList;
            bool isSuccess = false;
            bool isSuccessSaveLog = false;

            //see master_log_type DB
            const int typeLog = 1; //Retrieve Inbox Mail
            //const int saveInboxType = 2;
            isSuccess = EmailHandlingRepository.GetNewInbox(out message, out inboxList);
            var messageToSave = message;

            if (!isSuccess)
                messageInfo += message;

            //SaveLog() always done to save ERROR or OK status
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, messageToSave, typeLog, isSuccess);
            if (!isSuccessSaveLog)
                messageInfo += message;

            if (isSuccess)
            {
                isSuccess = LumenChatterBotDBRepository.SaveNewInbox(inboxList, out message);
                if (!isSuccess)
                    messageInfo += message;
                isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
                if (!isSuccessSaveLog)
                    messageInfo += message;
            }

            return isSuccess;
        }

        /// <summary>
        /// Filter from inbox tabel to facebook message format.
        /// Save filtered inbox to Inbox_Facebook.
        /// Set isFilterred = true if the message has been convert to inbox_facebook.
        /// </summary>
        public bool FilterInboxFacebook(out string messageInfo, DateTime? date)
        {
            messageInfo = "";
            var isSuccess = false;
            var isSuccessSaveLog = false;
            string message = "";
            const int typeLog = 4; //Filter Inbox Facebook

            //add another variable here
            var inboxFb = new List<Inbox_Facebook>();

            //add logic here
            isSuccess = LumenChatterBotDBRepository.FilterInboxToFacebookData(date, out inboxFb, out message);
            if (!isSuccess)
                messageInfo += message;
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
            if (!isSuccessSaveLog)
                messageInfo += message;

            //typeLog = 5;//Save_Inbox_Fb //com:13-05-2014
            isSuccess = LumenChatterBotDBRepository.SaveNewInboxFacebook(inboxFb, out message);
            if (!isSuccess)
                messageInfo += message;
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
            if (!isSuccessSaveLog)
                messageInfo += message;

            return isSuccess;
        }

        /// <summary>
        /// Analize Inbox_Facebook then define it's respon.
        /// </summary>
        /*
         * Load all Inbox_Facebook !IsAnalized
         * Analize and get respon each Inbox_Facebook
         * 
         */
        public bool AnalizeFacebookResponse(DateTime? date, out List<Sent> draftSent, out List<Sent_Facebook> draftSentFacebook, out List<int> idInboxList, out string messageInfo)
        {
            messageInfo = "";
            var isSuccess = false;
            var isSuccessSaveLog = false;
            string message = "";
            const int typeLog = 6; //Analize Facebook Response

            //add another variable here
            idInboxList = new List<int>();
            var inboxList = new List<View_Inbox_Facebook>();

            isSuccess = LumenChatterBotDBRepository.GetInboxFacebook(date, out inboxList, out message);
            if (isSuccess)
                messageInfo += message;
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
            if (isSuccessSaveLog)
                messageInfo += message;
            isSuccess = LumenChatterBotDBRepository.GetInboxFacebookResponse(inboxList, out draftSent,
                                                                             out draftSentFacebook, out message);
            if (isSuccess)
            {
                messageInfo += message;
                foreach (var x in inboxList)
                {
                    idInboxList.Add(x.Id);
                }
            }
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
            if (isSuccessSaveLog)
                messageInfo += message;

            return isSuccess;
        }

        /// <summary>
        /// Send draft to mail server.
        /// </summary>
        public bool SentDraftMail(out string messageInfo)
        {
            messageInfo = "";
            var isSuccess = false;
            var isSuccessSaveLog = false;
            string message = "";
            const int typeLog = 8; //Sent Draft Mail

            //add another variable here

            isSuccess = LumenChatterBotDBRepository.SentDraftMail(out message);
            if (isSuccess)
                messageInfo += message;
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
            if (isSuccessSaveLog)
                messageInfo += message;

            return isSuccess;
        }


        /// <summary>
        /// Save draft mail.
        /// </summary>
        /*
         * Collect sending draft from Inbox and Inbox_Facebook
         * Save draft to Sent and Sendt_Facebook DB.
         */
        public bool SaveDraftMail(List<Sent> draftSent, List<Sent_Facebook> draftSentFacebook, List<int> idInboxList, out string messageInfo)
        {
            messageInfo = "";
            var isSuccess = false;
            var isSuccessSaveLog = false;
            string message = "";
            const int typeLog = 7; //Save Mail Draft

            //add another variable here

            isSuccess = LumenChatterBotDBRepository.SaveSentDraft(draftSent, draftSentFacebook, idInboxList, out message);
            if (isSuccess)
                messageInfo += message;
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
            if (isSuccessSaveLog)
                messageInfo += message;

            return isSuccess;
        }

        /// <summary>
        /// Generate reply draft and save to DB.
        /// </summary>
        public bool SmartReplyDraftCreation(out string messageInfo)
        {
            var isSuccess = false;
            var isSaveLog = false;
            messageInfo = "";
            var message = "";
            const int typeLog = 5; //Smart Reply Draft Creation

            var sentList = new List<Sent>();
            var sentFbLlist = new List<Sent_Facebook>();
            var idInboxList = new List<int>();

            isSuccess = AnalizeFacebookResponse(null, out sentList, out sentFbLlist, out idInboxList, out message);
            //no need to saveLog-AnalizeFacebookResponse, it's done by AnalizeFacebookREsponse function

            if (isSuccess)
            {
                isSuccess = LumenChatterBotDBRepository.SaveSentDraft(sentList, sentFbLlist, idInboxList, out message);
                if (isSuccess)
                    messageInfo += message;
                isSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
                if (isSaveLog)
                    messageInfo += message;
            }

            return isSuccess;
        }

        /// <summary>
        /// Update status randomly.
        /// </summary>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        public bool SmartUpdateStatus(out string messageInfo)
        {
            messageInfo = "";
            var isSuccess = false;
            var isSuccessSaveLog = false;
            string message = "";
            string statusMessage = "";
            const int typeLog = 3;//Smart Update Status

            isSuccess = LumenChatterBotDBRepository.GetStatusAutomatic(out message, out statusMessage);
            if (!isSuccess)
                messageInfo += message;
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
            if (!isSuccessSaveLog)
                messageInfo += message;

            if (isSuccess)
            {
                isSuccess = EmailHandlingRepository.UpdateFacebookStatus(out message, statusMessage);
                if (!isSuccess)
                    messageInfo += message;
                isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, typeLog, isSuccess);
                if (!isSuccessSaveLog)
                    messageInfo += message;
            }

            return isSuccess;
        }

        /// <summary>
        /// Sent happy birthday expression today.
        /// </summary>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        public bool SentHappyBirthday(out string messageInfo)
        {
            var isSuccess = false;
            var isSuccessSaveLog = false;
            var message = "";
            messageInfo = message;
            const int logType = 9;//Sent Happy Birthday

            isSuccess = LumenChatterBotDBRepository.SentHappyBirthdayMail(out message);
            if (!isSuccess)
                messageInfo += message;
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, logType, isSuccess);
            if (isSuccessSaveLog)
                messageInfo += message;
            return isSuccess;
        }

        /// <summary>
        /// Update status about weather today.
        /// </summary>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        public bool SentWeatherInfoToday(out string messageInfo)
        {
            var isSuccess = false;
            var isSuccessSaveLog = false;
            var message = "";
            messageInfo = message;
            const int logType = 10;//Sent Weather Info Today

            isSuccess = LumenChatterBotDBRepository.SentWeatherInfoToday(out message);
            if (!isSuccess)
                messageInfo += message;
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, logType, isSuccess);
            if (isSuccessSaveLog)
                messageInfo += message;
            return isSuccess;
        }


        #region WCF SCHEDULER

        #region WCF SCHEDULER - DOCUMENTATION
        /*Scheduler in SocmedProcessing is a program to handle scheduler execution of function.
         * WCF Scheduler is a service that builded to give any information needed by SchedulerProgram.
         * This service will handle database connection, load and save to database mechanism, and other algorithm thant don't need
         * to show to other program. So the client will be easier just to get data needed.
         * 
         * Important information that should be given from WCF Scheduler to be consumed SchedulerProgram :
         * - time to execute function
         * - name of function to be executed
         * 
         * ------------------------------------------------------------------------------------------------------
         * SCHEDULER PROGRAM
         * Scheduler program is a program that will be exetuce by windows task manager every hour.
         * When executed, this program will connect to WCF Scheduler to load schedule list (contain : time and function name to execute) that have tobe done at next 60minutes.
         * Then Scheduler will save the list, and execute at certain time as writen in list.
         * When execute, the scheduler will connect to WCF again and send functionName tobe executed in WCF.
         * So, the execution to outside world is happen in WCF. scheduler just trigger the WCF to run a specific function.
         * 
         * --------------------------------------------------------------------------------------------------------
         * 
         * Back again to WCF in Scheduler Section
         * When ExecuteSchedule() is called by Scheduler Program, WCF then will proces to execute function that send as parameter by client.
         * Wcf will check the function need paramter or not, if needed than it will load from database first for parameter.
         * after that, wcf will call the specific function with switch-case mechanism followed by parameter if present.
         * 
         * Function executed, than call SaveLog() to save the result on DB.
         * Give konfirmation of bool return to client.
         * Mission Completed.
         * Client Satisfied.
         * 
         * If not succeed, client will execute the function once more with parameter isErrorBefore.
         * Make ExecuteSchedule() overload. with and no parameter.
         *  - ExecuteSchedule(string functionName)
         *  - ExecuteSchedule(string functionName, bool isErrorBefore)
         *  
         * ========================================================================================================== 
         * ===== SUMMARY =====
         * Function in WCF Scheduler Part/SEction :
         * 
         *  - bool LoadSchedule(out List<ViewScheduleDetail> listSchedule, dateTime dateNow) -- dateNow should use time written on db
         *  - bool GetNextSchedule(out List<ViewScheduleDetail> listSchedule, dateTime dateNow)
         *      -string GetGeneralVariable(string varName) -- load general var value with correspond param
         *      -V_Schedule GetScheduleInRange (int ScheduleMode, timespan officialTimeExecution ) -- load schedule in curtain time range 60 mnts
         *      -Master_Scheduler GetRandomSchedulerMode (string scheduleModeBefore) -- load mode schedule in random but dont same as before
         * 
         * Description : 
         * LoadScheduler function will check wheter dateNow is same as ScheduleDate in DB General_Variable.
         * True/Yes : 
         * use ScheduleMode (in DB General_Variable) value.
         * False :
         * load DB Master_Schedule, use random algorithm, pick a mode but diferent than before, than 
         * save this mode to ScheduleMode in DB General_Variable
         * save dateNow to ScheduleDate (General_Variable)
         * 
         * Load V_Schedule with param ScheduleMode used, time in range of now to next 60 minutes.
         * If present, sent ViewScheduleDetail list to listSchedule out param.
         * If no data, send null ViewScheduleDetail.
         * 
         * SaveLog()
         * 
         * 
         * Load scheduler list from DB Master_Scheduler and Master_SchedulerDetail apropriate with ScheduleMode used now.
         * ScheduleMode value is saved at DB General_Variable.
         * 
         *      
         * Things Important :
         *  - Mode of Scheduler
         *  - DB Master_Scheduler
         *  - DB Master_SchedulerDetail
         * 
         */
        #endregion

        /// <summary>
        /// Load schedule to execute function from database.
        /// </summary>
        /// <param name="dateClient"></param>
        /// <param name="listSchedule"></param>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        public bool LoadSchedule(DateTime dateClient, out List<View_ScheduleDetail> listSchedule, out string messageInfo)
        {
            listSchedule = new List<View_ScheduleDetail>();
            var isSuccess = false;
            var isSuccessSaveLog = false;
            var message = "";
            messageInfo = message;
            const int logType = 12;//Load Schedule

            isSuccess = LumenChatterBotDBRepository.GetNextSchedule(out message, dateClient, out listSchedule);
            if (!isSuccess)
                messageInfo += message;
            isSuccessSaveLog = LumenChatterBotDBRepository.SaveLog(out message, message, logType, isSuccess);
            if (isSuccessSaveLog)
                messageInfo += message;
            return isSuccess;
        }



        /// <summary>
        /// Execute certain function as written in schedule.
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        public bool ExecuteSchedule(string functionName, out string messageInfo)
        {
            var isSuccess = false;
            var message = "";
            messageInfo = "";
            //see Master_LogType
            const int updateFbType = 13;//Execute Schedule
            message = "ExecuteSchedule(string functionName, out string messageInfo) - OK : function called " + functionName + ".";

            switch (functionName)
            {
                case "SmartUpdateStatus()":
                    isSuccess = SmartUpdateStatus(out messageInfo);
                    //SaveLog() not execute because SmartUpdateStatus has SaveLog mechanism
                    break;
                case "RetrieveInboxMail()":
                    isSuccess = RetrieveInboxMail(out messageInfo);
                    //SaveLog() not execute because RetrieveInboxMail has SaveLog mechanism
                    break;
                case "FilterInboxFacebook()":
                    isSuccess = FilterInboxFacebook(out messageInfo, null);
                    //todo: implement FilterInboxFacebook with parameter
                    break;
                case "SmartReplyDraftCreation()":
                    isSuccess = SmartReplyDraftCreation(out messageInfo);
                    break;
                case "SentDraftMail()":
                    isSuccess = SentDraftMail(out messageInfo);
                    break;
                case "SentHappyBirthday()":
                    isSuccess = SentHappyBirthday(out messageInfo);
                    break;
                case "SentWeatherInfoToday()":
                    isSuccess = SentWeatherInfoToday(out messageInfo);
                    break;
                default:
                    message = "ExecuteSchedule(string functionName, out string messageInfo) - WARNING : No function " + functionName + " to call.";
                    messageInfo = message;
                    break;
            }
            isSuccess = LumenChatterBotDBRepository.SaveLog(out message, message, updateFbType, isSuccess);

            return isSuccess;
        }

        #endregion

        #region EXAMPLE CODE
        public string TestWcfHelloWord(int value, out string messageInfo)
        {
            messageInfo = "- NO ERROR -";
            return string.Format("You entered: {0}", value);
        }

        #endregion

    }
}
