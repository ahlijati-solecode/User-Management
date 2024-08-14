using User_Management.Constants;

namespace User_Management.Queries
{
    public class LgRefUserAccessHistQueries
    {
        #region SELECT QUERY
        public static string SelectGeneral = string.Format(@" refuseraccesshist.id, refuseraccesshist.idrefuseraccess, refuseraccesshist.isview, refuseraccesshist.iscreate, 
                    refuseraccesshist.isedit, refuseraccesshist.isdelete, isgolive, golivedate, apvstat, refuseraccesshist.createdby, refuseraccesshist.createddate,refuseraccesshist.notes");

        public static string SelectForApprovalChecking = string.Format(@"refuseraccesshist.id,mr.""name""");

        public static string SelectForApprovalReject = string.Format(@"lruah.idrefuseraccess AS id, lruah.isview, lruah.iscreate, lruah.isedit, lruah.isdelete,lruah.isgolive,lruah.apvstat as refuseraccessstat");

        public static string SelectForHistUpdate = string.Format(@"lruah.id,lruah.idrefuseraccess, lruah.golivedate, lruah.isgolive,lruah.apvstat as refuseraccessstat");

        public static string Query = string.Format(@"SELECT @select FROM lg_ref_user_access_hist refuseraccesshis @sort");

        public static string QueryCheckingAlreadySubmittedByRoleId = string.Format(@"SELECT @select FROM lg_ref_user_access_hist refuseraccesshist {0} WHERE @where limit 1",
            "JOIN md_ref_user_access refuseraccess ON refuseraccesshist.idrefuseraccess = refuseraccess.id");

        public static string QueryCheckRejectApproval = string.Format(@"SELECT @select FROM lg_ref_user_access_hist refuseraccesshist {0} {1} WHERE @where limit 1  ",
            "JOIN md_ref_user_access refuseraccess ON refuseraccesshist.idrefuseraccess = refuseraccess.id",
            "JOIN md_role mr ON refuseraccess.idrole = mr.id");

        public static string QueryForApprovalReject = string.Format(@"SELECT @select FROM lg_ref_user_access_hist lruah {0} WHERE lrua.idrole = @where {1} ORDER BY @sort",
                "JOIN md_ref_user_access lrua ON lruah.idrefuseraccess = lrua.id",
                " AND " + " isgolive = false AND apvstat = '");

        public static string QueryForHistUpdate = string.Format(@"SELECT @select FROM lg_ref_user_access_hist lruah {0} WHERE lrua.idrole = @where {1} ORDER BY @sort",
                "JOIN md_ref_user_access lrua ON lruah.idrefuseraccess = lrua.id",
                " AND " + " isgolive = false AND apvstat = ");
        #endregion
    }
}
