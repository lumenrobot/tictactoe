using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using OpenPop.Mime;
using WCFSocmedProcessing.Model;

namespace WCFSocmedProcessing
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        /// <summary>
        /// Only for testing hello word
        /// </summary>
        [OperationContract]
        string TestWcfHelloWord(int value, out string messageInfo);

        //ALL RETURN FOR EVERY OPERATION IS TRUE-FALSE ONLY
        //Email Operation

        [OperationContract]
        void PostImageStatusUpdate();

        [OperationContract]
        bool CreateDraftMail(string messageToSent, out string messageInfo);

        [OperationContract]
        void UploadImageStatus(DocInfo file);

        [OperationContract]
        bool RetrieveInboxMail(out string messageInfo);

        [OperationContract]
        bool FilterInboxFacebook(out string messageInfo, DateTime? date);

        [OperationContract]
        bool AnalizeFacebookResponse(DateTime? date, out List<Sent> draftSent, out List<Sent_Facebook> draftSentFacebook, out List<int> idInboxList, out string messageInfo);

        [OperationContract]
        bool SentDraftMail(out string messageInfo);

        [OperationContract]
        bool SaveDraftMail(List<Sent> draftSent, List<Sent_Facebook> draftSentFacebook, List<int> idInboxList, out string messageInfo);

        [OperationContract]
        bool SmartReplyDraftCreation(out string messageInfo);
        
        [OperationContract]
        bool SmartUpdateStatus(out string messageInfo);

        //[OperationContract]
        //bool SmartUpdateStatusWithAttachment(out string messageInfo); 

        [OperationContract]
        bool SentHappyBirthday(out string messageInfo);
        
        [OperationContract]
        bool SentWeatherInfoToday(out string messageInfo);
        
        [OperationContract]
        bool LoadSchedule(DateTime dateNow, out List<View_ScheduleDetail> listScheduler, out string messageInfo);

        [OperationContract]
        bool ExecuteSchedule(string functionName, out string messageInfo);
    }
}


[MessageContract]
public class DocInfo
{
    [MessageHeader]
    public string Id { get; set; }

    [MessageHeader]
    public string FileName { get; set; }

    [MessageHeader]
    public double FileSize { get; set; }

    [MessageBodyMember]
    public Stream FileContent { get; set; }
}