﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WCFSocmedProcessing.Model
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="LumenChatterBot")]
	public partial class LumenChatterBotDBDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility SendMail Definitions
    partial void OnCreated();
    partial void InsertKeyWordTbl(KeyWordTbl instance);
    partial void UpdateKeyWordTbl(KeyWordTbl instance);
    partial void DeleteKeyWordTbl(KeyWordTbl instance);
    partial void InsertResponseTbl(ResponseTbl instance);
    partial void UpdateResponseTbl(ResponseTbl instance);
    partial void DeleteResponseTbl(ResponseTbl instance);
    partial void InsertBehaivourTbl(BehaivourTbl instance);
    partial void UpdateBehaivourTbl(BehaivourTbl instance);
    partial void DeleteBehaivourTbl(BehaivourTbl instance);
    partial void InsertInboxReply(InboxReply instance);
    partial void UpdateInboxReply(InboxReply instance);
    partial void DeleteInboxReply(InboxReply instance);
    partial void InsertInboxDetail(InboxDetail instance);
    partial void UpdateInboxDetail(InboxDetail instance);
    partial void DeleteInboxDetail(InboxDetail instance);
    partial void InsertInboxMain(InboxMain instance);
    partial void UpdateInboxMain(InboxMain instance);
    partial void DeleteInboxMain(InboxMain instance);
    #endregion
		
		public LumenChatterBotDBDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["LumenChatterBotConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public LumenChatterBotDBDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LumenChatterBotDBDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LumenChatterBotDBDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LumenChatterBotDBDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<KeyWordTbl> KeyWordTbls
		{
			get
			{
				return this.GetTable<KeyWordTbl>();
			}
		}
		
		public System.Data.Linq.Table<ResponseTbl> ResponseTbls
		{
			get
			{
				return this.GetTable<ResponseTbl>();
			}
		}
		
		public System.Data.Linq.Table<BehaivourTbl> BehaivourTbls
		{
			get
			{
				return this.GetTable<BehaivourTbl>();
			}
		}
		
		public System.Data.Linq.Table<vw_KeyWordResponse> vw_KeyWordResponses
		{
			get
			{
				return this.GetTable<vw_KeyWordResponse>();
			}
		}
		
		public System.Data.Linq.Table<InboxReply> InboxReplies
		{
			get
			{
				return this.GetTable<InboxReply>();
			}
		}
		
		public System.Data.Linq.Table<InboxDetail> InboxDetails
		{
			get
			{
				return this.GetTable<InboxDetail>();
			}
		}
		
		public System.Data.Linq.Table<InboxMain> InboxMains
		{
			get
			{
				return this.GetTable<InboxMain>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.KeyWordTbl")]
	public partial class KeyWordTbl : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _KeyWord;
		
    #region Extensibility SendMail Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnKeyWordChanging(string value);
    partial void OnKeyWordChanged();
    #endregion
		
		public KeyWordTbl()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_KeyWord", DbType="VarChar(300)")]
		public string KeyWord
		{
			get
			{
				return this._KeyWord;
			}
			set
			{
				if ((this._KeyWord != value))
				{
					this.OnKeyWordChanging(value);
					this.SendPropertyChanging();
					this._KeyWord = value;
					this.SendPropertyChanged("KeyWord");
					this.OnKeyWordChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.ResponseTbl")]
	public partial class ResponseTbl : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private System.Nullable<int> _KeyWordId;
		
		private string _Response;
		
		private string _Behaviour;
		
    #region Extensibility SendMail Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnKeyWordIdChanging(System.Nullable<int> value);
    partial void OnKeyWordIdChanged();
    partial void OnResponseChanging(string value);
    partial void OnResponseChanged();
    partial void OnBehaviourChanging(string value);
    partial void OnBehaviourChanged();
    #endregion
		
		public ResponseTbl()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_KeyWordId", DbType="Int")]
		public System.Nullable<int> KeyWordId
		{
			get
			{
				return this._KeyWordId;
			}
			set
			{
				if ((this._KeyWordId != value))
				{
					this.OnKeyWordIdChanging(value);
					this.SendPropertyChanging();
					this._KeyWordId = value;
					this.SendPropertyChanged("KeyWordId");
					this.OnKeyWordIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Response", DbType="Text", UpdateCheck=UpdateCheck.Never)]
		public string Response
		{
			get
			{
				return this._Response;
			}
			set
			{
				if ((this._Response != value))
				{
					this.OnResponseChanging(value);
					this.SendPropertyChanging();
					this._Response = value;
					this.SendPropertyChanged("Response");
					this.OnResponseChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Behaviour", DbType="VarChar(100)")]
		public string Behaviour
		{
			get
			{
				return this._Behaviour;
			}
			set
			{
				if ((this._Behaviour != value))
				{
					this.OnBehaviourChanging(value);
					this.SendPropertyChanging();
					this._Behaviour = value;
					this.SendPropertyChanged("Behaviour");
					this.OnBehaviourChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.BehaivourTbl")]
	public partial class BehaivourTbl : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private string _Description;
		
    #region Extensibility SendMail Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    #endregion
		
		public BehaivourTbl()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="VarChar(100)")]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="VarChar(300)")]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.vw_KeyWordResponse")]
	public partial class vw_KeyWordResponse
	{
		
		private int _Id;
		
		private int _KeyWordId;
		
		private string _KeyWord;
		
		private string _Response;
		
		private string _Behaviour;
		
		public vw_KeyWordResponse()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", DbType="Int NOT NULL")]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this._Id = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_KeyWordId", DbType="Int NOT NULL")]
		public int KeyWordId
		{
			get
			{
				return this._KeyWordId;
			}
			set
			{
				if ((this._KeyWordId != value))
				{
					this._KeyWordId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_KeyWord", DbType="VarChar(300)")]
		public string KeyWord
		{
			get
			{
				return this._KeyWord;
			}
			set
			{
				if ((this._KeyWord != value))
				{
					this._KeyWord = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Response", DbType="Text", UpdateCheck=UpdateCheck.Never)]
		public string Response
		{
			get
			{
				return this._Response;
			}
			set
			{
				if ((this._Response != value))
				{
					this._Response = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Behaviour", DbType="VarChar(100)")]
		public string Behaviour
		{
			get
			{
				return this._Behaviour;
			}
			set
			{
				if ((this._Behaviour != value))
				{
					this._Behaviour = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.InboxReply")]
	public partial class InboxReply : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private System.Nullable<int> _IdInboxDetail;
		
		private System.Nullable<System.DateTime> _SentDate;
		
		private string _HeaderFrom;
		
		private string _HeaderTo;
		
		private string _HeaderReplyTo;
		
		private string _HeaderSubject;
		
		private string _Body;
		
		private System.Nullable<bool> _IsAttachment;
		
		private EntityRef<InboxDetail> _InboxDetail;
		
    #region Extensibility SendMail Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnIdInboxDetailChanging(System.Nullable<int> value);
    partial void OnIdInboxDetailChanged();
    partial void OnSentDateChanging(System.Nullable<System.DateTime> value);
    partial void OnSentDateChanged();
    partial void OnHeaderFromChanging(string value);
    partial void OnHeaderFromChanged();
    partial void OnHeaderToChanging(string value);
    partial void OnHeaderToChanged();
    partial void OnHeaderReplyToChanging(string value);
    partial void OnHeaderReplyToChanged();
    partial void OnHeaderSubjectChanging(string value);
    partial void OnHeaderSubjectChanged();
    partial void OnBodyChanging(string value);
    partial void OnBodyChanged();
    partial void OnIsAttachmentChanging(System.Nullable<bool> value);
    partial void OnIsAttachmentChanged();
    #endregion
		
		public InboxReply()
		{
			this._InboxDetail = default(EntityRef<InboxDetail>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdInboxDetail", DbType="Int")]
		public System.Nullable<int> IdInboxDetail
		{
			get
			{
				return this._IdInboxDetail;
			}
			set
			{
				if ((this._IdInboxDetail != value))
				{
					if (this._InboxDetail.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnIdInboxDetailChanging(value);
					this.SendPropertyChanging();
					this._IdInboxDetail = value;
					this.SendPropertyChanged("IdInboxDetail");
					this.OnIdInboxDetailChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SentDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> SentDate
		{
			get
			{
				return this._SentDate;
			}
			set
			{
				if ((this._SentDate != value))
				{
					this.OnSentDateChanging(value);
					this.SendPropertyChanging();
					this._SentDate = value;
					this.SendPropertyChanged("SentDate");
					this.OnSentDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HeaderFrom", DbType="VarChar(500)")]
		public string HeaderFrom
		{
			get
			{
				return this._HeaderFrom;
			}
			set
			{
				if ((this._HeaderFrom != value))
				{
					this.OnHeaderFromChanging(value);
					this.SendPropertyChanging();
					this._HeaderFrom = value;
					this.SendPropertyChanged("HeaderFrom");
					this.OnHeaderFromChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HeaderTo", DbType="VarChar(500)")]
		public string HeaderTo
		{
			get
			{
				return this._HeaderTo;
			}
			set
			{
				if ((this._HeaderTo != value))
				{
					this.OnHeaderToChanging(value);
					this.SendPropertyChanging();
					this._HeaderTo = value;
					this.SendPropertyChanged("HeaderTo");
					this.OnHeaderToChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HeaderReplyTo", DbType="VarChar(500)")]
		public string HeaderReplyTo
		{
			get
			{
				return this._HeaderReplyTo;
			}
			set
			{
				if ((this._HeaderReplyTo != value))
				{
					this.OnHeaderReplyToChanging(value);
					this.SendPropertyChanging();
					this._HeaderReplyTo = value;
					this.SendPropertyChanged("HeaderReplyTo");
					this.OnHeaderReplyToChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HeaderSubject", DbType="VarChar(1000)")]
		public string HeaderSubject
		{
			get
			{
				return this._HeaderSubject;
			}
			set
			{
				if ((this._HeaderSubject != value))
				{
					this.OnHeaderSubjectChanging(value);
					this.SendPropertyChanging();
					this._HeaderSubject = value;
					this.SendPropertyChanged("HeaderSubject");
					this.OnHeaderSubjectChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Body", DbType="Text", UpdateCheck=UpdateCheck.Never)]
		public string Body
		{
			get
			{
				return this._Body;
			}
			set
			{
				if ((this._Body != value))
				{
					this.OnBodyChanging(value);
					this.SendPropertyChanging();
					this._Body = value;
					this.SendPropertyChanged("Body");
					this.OnBodyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsAttachment", DbType="Bit")]
		public System.Nullable<bool> IsAttachment
		{
			get
			{
				return this._IsAttachment;
			}
			set
			{
				if ((this._IsAttachment != value))
				{
					this.OnIsAttachmentChanging(value);
					this.SendPropertyChanging();
					this._IsAttachment = value;
					this.SendPropertyChanged("IsAttachment");
					this.OnIsAttachmentChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="InboxDetail_InboxReply", Storage="_InboxDetail", ThisKey="IdInboxDetail", OtherKey="Id", IsForeignKey=true)]
		public InboxDetail InboxDetail
		{
			get
			{
				return this._InboxDetail.Entity;
			}
			set
			{
				InboxDetail previousValue = this._InboxDetail.Entity;
				if (((previousValue != value) 
							|| (this._InboxDetail.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._InboxDetail.Entity = null;
						previousValue.InboxReplies.Remove(this);
					}
					this._InboxDetail.Entity = value;
					if ((value != null))
					{
						value.InboxReplies.Add(this);
						this._IdInboxDetail = value.Id;
					}
					else
					{
						this._IdInboxDetail = default(Nullable<int>);
					}
					this.SendPropertyChanged("InboxDetail");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.InboxDetail")]
	public partial class InboxDetail : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private System.Nullable<int> _IdInboxMain;
		
		private System.Nullable<System.DateTime> _ReceivedDate;
		
		private string _HeaderFrom;
		
		private string _HeaderTo;
		
		private string _HeaderReplyTo;
		
		private string _HeaderSubject;
		
		private string _Body;
		
		private string _MainContent;
		
		private string _CommentatorName;
		
		private System.Nullable<bool> _IsReplied;
		
		private EntitySet<InboxReply> _InboxReplies;
		
		private EntityRef<InboxMain> _InboxMain;
		
    #region Extensibility SendMail Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnIdInboxMainChanging(System.Nullable<int> value);
    partial void OnIdInboxMainChanged();
    partial void OnReceivedDateChanging(System.Nullable<System.DateTime> value);
    partial void OnReceivedDateChanged();
    partial void OnHeaderFromChanging(string value);
    partial void OnHeaderFromChanged();
    partial void OnHeaderToChanging(string value);
    partial void OnHeaderToChanged();
    partial void OnHeaderReplyToChanging(string value);
    partial void OnHeaderReplyToChanged();
    partial void OnHeaderSubjectChanging(string value);
    partial void OnHeaderSubjectChanged();
    partial void OnBodyChanging(string value);
    partial void OnBodyChanged();
    partial void OnMainContentChanging(string value);
    partial void OnMainContentChanged();
    partial void OnCommentatorNameChanging(string value);
    partial void OnCommentatorNameChanged();
    partial void OnIsRepliedChanging(System.Nullable<bool> value);
    partial void OnIsRepliedChanged();
    #endregion
		
		public InboxDetail()
		{
			this._InboxReplies = new EntitySet<InboxReply>(new Action<InboxReply>(this.attach_InboxReplies), new Action<InboxReply>(this.detach_InboxReplies));
			this._InboxMain = default(EntityRef<InboxMain>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdInboxMain", DbType="Int")]
		public System.Nullable<int> IdInboxMain
		{
			get
			{
				return this._IdInboxMain;
			}
			set
			{
				if ((this._IdInboxMain != value))
				{
					if (this._InboxMain.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnIdInboxMainChanging(value);
					this.SendPropertyChanging();
					this._IdInboxMain = value;
					this.SendPropertyChanged("IdInboxMain");
					this.OnIdInboxMainChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ReceivedDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> ReceivedDate
		{
			get
			{
				return this._ReceivedDate;
			}
			set
			{
				if ((this._ReceivedDate != value))
				{
					this.OnReceivedDateChanging(value);
					this.SendPropertyChanging();
					this._ReceivedDate = value;
					this.SendPropertyChanged("ReceivedDate");
					this.OnReceivedDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HeaderFrom", DbType="VarChar(500)")]
		public string HeaderFrom
		{
			get
			{
				return this._HeaderFrom;
			}
			set
			{
				if ((this._HeaderFrom != value))
				{
					this.OnHeaderFromChanging(value);
					this.SendPropertyChanging();
					this._HeaderFrom = value;
					this.SendPropertyChanged("HeaderFrom");
					this.OnHeaderFromChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HeaderTo", DbType="VarChar(500)")]
		public string HeaderTo
		{
			get
			{
				return this._HeaderTo;
			}
			set
			{
				if ((this._HeaderTo != value))
				{
					this.OnHeaderToChanging(value);
					this.SendPropertyChanging();
					this._HeaderTo = value;
					this.SendPropertyChanged("HeaderTo");
					this.OnHeaderToChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HeaderReplyTo", DbType="VarChar(500)")]
		public string HeaderReplyTo
		{
			get
			{
				return this._HeaderReplyTo;
			}
			set
			{
				if ((this._HeaderReplyTo != value))
				{
					this.OnHeaderReplyToChanging(value);
					this.SendPropertyChanging();
					this._HeaderReplyTo = value;
					this.SendPropertyChanged("HeaderReplyTo");
					this.OnHeaderReplyToChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HeaderSubject", DbType="VarChar(1000)")]
		public string HeaderSubject
		{
			get
			{
				return this._HeaderSubject;
			}
			set
			{
				if ((this._HeaderSubject != value))
				{
					this.OnHeaderSubjectChanging(value);
					this.SendPropertyChanging();
					this._HeaderSubject = value;
					this.SendPropertyChanged("HeaderSubject");
					this.OnHeaderSubjectChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Body", DbType="Text", UpdateCheck=UpdateCheck.Never)]
		public string Body
		{
			get
			{
				return this._Body;
			}
			set
			{
				if ((this._Body != value))
				{
					this.OnBodyChanging(value);
					this.SendPropertyChanging();
					this._Body = value;
					this.SendPropertyChanged("Body");
					this.OnBodyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MainContent", DbType="Text", UpdateCheck=UpdateCheck.Never)]
		public string MainContent
		{
			get
			{
				return this._MainContent;
			}
			set
			{
				if ((this._MainContent != value))
				{
					this.OnMainContentChanging(value);
					this.SendPropertyChanging();
					this._MainContent = value;
					this.SendPropertyChanged("MainContent");
					this.OnMainContentChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CommentatorName", DbType="VarChar(300)")]
		public string CommentatorName
		{
			get
			{
				return this._CommentatorName;
			}
			set
			{
				if ((this._CommentatorName != value))
				{
					this.OnCommentatorNameChanging(value);
					this.SendPropertyChanging();
					this._CommentatorName = value;
					this.SendPropertyChanged("CommentatorName");
					this.OnCommentatorNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsReplied", DbType="Bit")]
		public System.Nullable<bool> IsReplied
		{
			get
			{
				return this._IsReplied;
			}
			set
			{
				if ((this._IsReplied != value))
				{
					this.OnIsRepliedChanging(value);
					this.SendPropertyChanging();
					this._IsReplied = value;
					this.SendPropertyChanged("IsReplied");
					this.OnIsRepliedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="InboxDetail_InboxReply", Storage="_InboxReplies", ThisKey="Id", OtherKey="IdInboxDetail")]
		public EntitySet<InboxReply> InboxReplies
		{
			get
			{
				return this._InboxReplies;
			}
			set
			{
				this._InboxReplies.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="InboxMain_InboxDetail", Storage="_InboxMain", ThisKey="IdInboxMain", OtherKey="Id", IsForeignKey=true)]
		public InboxMain InboxMain
		{
			get
			{
				return this._InboxMain.Entity;
			}
			set
			{
				InboxMain previousValue = this._InboxMain.Entity;
				if (((previousValue != value) 
							|| (this._InboxMain.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._InboxMain.Entity = null;
						previousValue.InboxDetails.Remove(this);
					}
					this._InboxMain.Entity = value;
					if ((value != null))
					{
						value.InboxDetails.Add(this);
						this._IdInboxMain = value.Id;
					}
					else
					{
						this._IdInboxMain = default(Nullable<int>);
					}
					this.SendPropertyChanged("InboxMain");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_InboxReplies(InboxReply entity)
		{
			this.SendPropertyChanging();
			entity.InboxDetail = this;
		}
		
		private void detach_InboxReplies(InboxReply entity)
		{
			this.SendPropertyChanging();
			entity.InboxDetail = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.InboxMain")]
	public partial class InboxMain : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private System.Nullable<System.DateTime> _ReceivedDate;
		
		private EntitySet<InboxDetail> _InboxDetails;
		
    #region Extensibility SendMail Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnReceivedDateChanging(System.Nullable<System.DateTime> value);
    partial void OnReceivedDateChanged();
    #endregion
		
		public InboxMain()
		{
			this._InboxDetails = new EntitySet<InboxDetail>(new Action<InboxDetail>(this.attach_InboxDetails), new Action<InboxDetail>(this.detach_InboxDetails));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ReceivedDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> ReceivedDate
		{
			get
			{
				return this._ReceivedDate;
			}
			set
			{
				if ((this._ReceivedDate != value))
				{
					this.OnReceivedDateChanging(value);
					this.SendPropertyChanging();
					this._ReceivedDate = value;
					this.SendPropertyChanged("ReceivedDate");
					this.OnReceivedDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="InboxMain_InboxDetail", Storage="_InboxDetails", ThisKey="Id", OtherKey="IdInboxMain")]
		public EntitySet<InboxDetail> InboxDetails
		{
			get
			{
				return this._InboxDetails;
			}
			set
			{
				this._InboxDetails.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_InboxDetails(InboxDetail entity)
		{
			this.SendPropertyChanging();
			entity.InboxMain = this;
		}
		
		private void detach_InboxDetails(InboxDetail entity)
		{
			this.SendPropertyChanging();
			entity.InboxMain = null;
		}
	}
}
#pragma warning restore 1591