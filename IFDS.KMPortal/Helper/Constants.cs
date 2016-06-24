using System;

namespace IFDS.KMPortal.Helper
{
    public static class Constants
    {
        public static string FAQLIST = "FAQ";
        public static string DISCUSSIONS_LIST = "Discussions List";
        public static string KMPORTAL_DEPT_SITES = "KMPortal Department Sites";
        public static string CREATED = "Created";
        public static string CREATEDBY = "Created_x0020_By";
        public static string ID = "ID";
        public static string DEPT_NAME = "DepartmentName";
        public static string FILE_EXTENSION = "FileExtension";
        public static string CONTENT_TYPE_ID = "ContentTypeId";
        public static string TITLE = "Title";
        public static string AUTHOR = "Author";
        public static string MEMEBER = "Member";
        public static string MEMEBER_STATUS = "MemberStatus";
        public static string DISPLAY_NAME = "Displayname";
        public static string WEB = "web";
        public static string REPUTATION_SCORE = "ReputationScore";
        public static string DISCUSSION_VIEWS = "Column1";
        public static string FAQ_VIEWS = "Column2";
        public static string PATH = "Path";
        public static string REPLY_COUNT = "ReplyCount";
        public static string FILE_REF = "FileRef";


        public static string FILE__LEAF_REF = "FileLeafRef";

        public static string[] OFFICE_EXT = { "doc", "docx", "xls", "ppt", "pptx" };
        public static string OFFICE_EXT_TYPE = "File_x0020_Type";

        public static string ITEM_CHILD_COUNT = "ItemChildCount";
        public static string LAST_MODIFIED_TIME = "LastModifiedTime";
        public static string ANSWER = "Answer";
        public static string SITE_URL = "SiteURL";
        public static string MODIFIED = "Modified";
        public static string ROW_LIMIT = "5";
        public static string NEWFORM = "NewForm.aspx";
        public static string ASPX_FILE_EXTENSION = "aspx";
        public static string FILE__DIR_REF = "FileDirRef";
        public static string IMAGE = "Image";

        
        public static string NO_DATA_ERRMSG = "No Data Available";
        public static int CHAR_MAXLENGTH = 300;
        public static string GLOBAL_SEARCH = "Global Search";
        public static string LOCAL_SEARCH = "Department Search";

        public static Guid approvalWorkflowBaseId = new Guid("8ad4d8f0-93a7-4941-9657-cf3706f00409");
        public static string ApprovalStatus = "_ModerationStatus";
        public static string IsWorkflowAttached = "IsWorkflowAttached";
        public static string IsModerationEnabled = "IsModerationEnabled";
        public static string WorkflowFieldName = "WorkflowFieldName";

        public static string LibrariesToExclude = "Form Templates;Images;Pages;Site Assets;Site Collection Documents;Site Collection Images;Site Pages;Style Library;Templates";
        public static string IFDSdbConnection ="IFDSConnection";                

        public static string DepartmentTaxonomyText = "department";
        public static string ProductTaxonomyText = "taxonomy";
        public static string Count = "Count";

        /* TOPCONTRIBUTORS AND LEARNERS SECTION */

        public static string SortViewsLifeTime = "ViewsLifeTime";
        public static string ResultTableType = "TableType";
        public static string DEFAULT_PERSONAL_URL = "#";
        public static string WORK_EMAIL = "WorkEmail";
        public static string WORK_PHONE = "WorkPhone";
        public static string PERSONAL_SITE = "PersonalSite";
        public static string PERSONAL_SPACE = "PersonalSpace";
        public static string GIFTED_BADGE_TEXT = "GiftedBadgeText";
        public static string NUMBER_OF_REPLIES = "NumberOfReplies";
        public static string NUMBER_OF_DISUCSSIONS = "NumberOfDiscussions";
    }
}
