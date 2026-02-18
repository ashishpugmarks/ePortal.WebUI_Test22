using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ePortal.Persistence.Services;
using Oracle.ManagedDataAccess.Client;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Admin.Interface;

public class PORequest : IPORequest
{
    //DataManagement dmgmt = new DataManagement();

    private readonly IDataManagement dmgmt;
    private readonly IConnectionString objCnStr;

    public PORequest(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
    {
        dmgmt = _oDataMgmt;
        objCnStr = _objCnStr;
    }

    public DataSet ManagePORequest(string strEmpCode)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strCn = objCnStr.getConnectingString();

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGPOREQUEST";
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                objCmd.Parameters.Add("CUR_PENDINGLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                DataSet objDs = new DataSet();
                objAdr.Fill(objDs);
                return objDs;
            }
            finally
            {
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
    }
    //public DataSet ManagePOApproval(string strSupEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();

    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGPOAPPROVAL";
    //            objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //public DataSet ManagePORequestHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGPOREQUESTHIS";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGPO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //public DataSet ManagePOApprovalHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGPOAPPROVALHIS";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGPO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}

    //public DataSet ManagePRRequest(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();

    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGPRREQUEST";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //public DataSet ManagePRApproval(string strSupEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();

    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGPRAPPROVAL";
    //            objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //public DataSet ManagePRRequestHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGPRREQUESTHIS";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //public DataSet ManagePRApprovalHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGPRAPPROVALHIS";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //#region "IOM"
    //public DataSet ManageIOMRequest(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();

    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGIOMREQUEST";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //public DataSet ManageIOMApproval(string strSupEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();

    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGIOMAPPROVAL";
    //            objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //public DataSet ManageIOMRequestHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGIOMREQUESTHIS";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //public DataSet ManageIOMApprovalHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGIOMAPPROVALHIS";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //#endregion


    #region "Advance Cheque Request"
    public DataSet ManageACRRequest(string strEmpCode)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strCn = objCnStr.getConnectingString();

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGACRREQUEST";
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                objCmd.Parameters.Add("CUR_PENDINGLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                DataSet objDs = new DataSet();
                objAdr.Fill(objDs);
                return objDs;
            }
            finally
            {
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
    }
    public DataSet ManageACRApproval(string strSupEmpCode)
    {
       // ConnectionString objCnStr = new ConnectionString();
        string strCn = objCnStr.getConnectingString();

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGACRAPPROVAL";
                objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                DataSet objDs = new DataSet();
                objAdr.Fill(objDs);
                return objDs;
            }
            finally
            {
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
    }
    public DataSet ManageACRRequestHis(string strEmpCode)
    {
       // ConnectionString objCnStr = new ConnectionString();
        string strCn = objCnStr.getConnectingString();
        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGACRREQUESTHIS";
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                DataSet objDs = new DataSet();
                objAdr.Fill(objDs);
                return objDs;
            }
            finally
            {
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
    }
    public DataSet ManageACRApprovalHis(string strEmpCode)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strCn = objCnStr.getConnectingString();
        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGACRAPPROVALHIS";
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                DataSet objDs = new DataSet();
                objAdr.Fill(objDs);
                return objDs;
            }
            finally
            {
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
    }
#endregion

    //#region "SES/MRN"
    //public DataSet ManageSESRequest(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();

    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGSESREQUEST";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //public DataSet ManageSESApproval(string strSupEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();

    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGSESAPPROVAL";
    //            objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //public DataSet ManageSESRequestHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGSESREQUESTHIS";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //public DataSet ManageSESApprovalHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGSESAPPROVALHIS";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}

    public DataSet ManageICRequest(string strEmpCode)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strCn = objCnStr.getConnectingString();

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGICREQUEST";
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                objCmd.Parameters.Add("CUR_PENDINGLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                DataSet objDs = new DataSet();
                objAdr.Fill(objDs);
                return objDs;
            }
            finally
            {
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
    }
    public DataSet ManageICApproval(string strSupEmpCode)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strCn = objCnStr.getConnectingString();

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGICAPPROVAL";
                objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                DataSet objDs = new DataSet();
                objAdr.Fill(objDs);
                return objDs;
            }
            finally
            {
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
    }

    //public DataSet ManageICRequestHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_ICREQUESTHISTORY";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}

    //public DataSet ManageICApprovalHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_ICAPPROVALHISTORY";
    //            objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //#endregion

    #region Creative change
    //Below Added by aumento for creative Master=====================
    //public DataSet ManageCMAApproval(string strSupEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();

    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGCMAPPROVAL";
    //            objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //=========================================
    #endregion

    //Below Added by aumento for Calendar Master=====================================
    //public DataSet ManageCALENDARApprovalHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGCALMAPPROVALHIS";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}

    //public DataSet ManageCalMasApproval(string strSupEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();

    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGCALMSTAPPROVAL";
    //            objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //=============================================================================

    //Below Added by aumento for ISMS Master=====================
    //public DataSet ManageISMSMasApproval(string strSupEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();

    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGISMSMSTAPPROVAL";
    //            objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}

    //public DataSet ManageISMSApprovalHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.sproc_ad_pendingismsapprovalhis";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //=====================================================================

    //Below Added by aumento for Creative Master=====================================
    //public DataSet ManageCreativeApprovalHis(string strEmpCode)
    //{
    //    ConnectionString objCnStr = new ConnectionString();
    //    string strCn = objCnStr.getConnectingString();
    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            OracleCommand objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGCREMAPPROVALHIS";
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
    //            objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //            objCmd.BindByName = true;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            DataSet objDs = new DataSet();
    //            objAdr.Fill(objDs);
    //            return objDs;
    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}
    //=============================================================================

}

