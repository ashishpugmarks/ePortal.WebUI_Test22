using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;
using System.Web;
using ePortal.DomainClasses;
using ePortal.Application.Contracts;
using ePortal.Shared;
using ePortal.Infrastructure.Repositories;


namespace ePortal.Application.Services
{
    public class PRService : IPRService
    {

       private PRRepository _PRRepo;
       private CommonRepository _CommonRepo;
        Tuple<short, long> _retVal_tuple;

        public PRService(PRRepository objPRRepository, CommonRepository objCommonRepository)
        {
            _PRRepo = objPRRepository;
            _CommonRepo = objCommonRepository;
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);
        }

        public Employee_Details GetAuthEmpById(long empCode, int designationId, string designation, Employee_Details empDtl)
        {
            return _PRRepo.GetAuthEmpById(empCode, designationId, designation, empDtl);
        }

        public PRHeaderViewModel GetPRRequestById(long id)
        {
            return _PRRepo.GetPRRequestById(id);
        }

        public short PRApproval(PRAppHistoryViewModel PHVM, Employee_Details emp_dtl)
        {
            //// Start Added By Aumento :: SR80255
            //short retVal = _PRRepo.PRApproval(PHVM);   
            //if (retVal == 1)
            //{
            //    SendMailByApprovalAuthority(PHVM.PRID, PHVM.APPROVAL_STATUS, emp_dtl);
            //}
            //return retVal;

            Tuple<short, short> retVal = _PRRepo.PRApproval(PHVM);
            if (retVal.Item1 == 1)
            {
                SendMailByApprovalAuthority(PHVM.PRID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            if (retVal.Item2 == 1)
            {
                SendMailAfterAutoAssignBuyer(PHVM.PRID);
            }

            return retVal.Item1;
            //// End Added By Aumento :: SR80255
        }

        public short PRCancel(PRHeaderViewModel PHVM)
        {
            return _PRRepo.PRCancel(PHVM);
        }

        public Tuple<short, long> SavePRRequest(PRHeaderViewModel model)
        {
            Tuple<short, long> _tuple = _PRRepo.SavePRRequest(model);
            if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && model.IsFinalSubmit == 1)
            {
                SendMailByRequestor(_tuple.Item2);
            }
            return _tuple;
        }
        
        
        
        public List<PRAppAuthSeqViewModel> GetDefaultAuthority(long loginUser, decimal indentAmt, long PRTYPE, Employee_Details obj, long indent_type, bool ITServiceMatSISAppStatus, bool ISIPCheck,PRHeaderViewModel PRDATA) //SIS PR Change,IPCheck { PRDATA} Added by Aumento ::  SR68003
       // public List<PRAppAuthSeqViewModel> GetDefaultAuthority(long loginUser, decimal indentAmt, long PRTYPE, Employee_Details obj, long indent_type, bool ITServiceMatSISAppStatus,bool ISIPCheck) //SIS PR Change,IPCheck    //  Added by Aumento ::  SR68003
        {
            List<PRAppAuthSeqViewModel> FinalList = new List<PRAppAuthSeqViewModel>();
            List<PR_DGIT_PRADDAPP_MST> objauthaddedinlast = new List<PR_DGIT_PRADDAPP_MST>();
            List<PRAppAuthSeqViewModel> AuthList = new List<PRAppAuthSeqViewModel>();
            List<PR_DGIT_PRADDAPP_MST> objappmst = new List<PR_DGIT_PRADDAPP_MST>();
            List<PR_DGIT_PRADDAPP_MST> objauthaddedinFirst = new List<PR_DGIT_PRADDAPP_MST>();
            try
            {
                short APP_SEQNO = 0;
                short IS_SISPR = ITServiceMatSISAppStatus == false ? (short)0 : (short)1; //SIS PR Change

                short IS_IPCheck = ISIPCheck == false ? (short)0 : (short)2; //IP check for Honda logo or other

                AuthList = GetAuthority(loginUser, indentAmt, PRTYPE, obj);

                #region "Logic to get approver auth. like Plant PPC and Corporate PPC"

                if (!string.IsNullOrEmpty(obj.Section_Id) && Convert.ToInt32(obj.Section_Id) != 0)
                {
                    //objappmst = 
                    objappmst.AddRange(_PRRepo.GetPRAddApproverMaster(PRTYPE, Convert.ToInt32(obj.Section_Id)));
                }
                if (!string.IsNullOrEmpty(obj.Department_Id) && Convert.ToInt32(obj.Department_Id) != 0 /*&& objappmst.Count() <= 0*/)
                {
                    //objappmst = _PRRepo.GetPRAddApproverMaster(PRTYPE, Convert.ToInt32(obj.Department_Id));
                    objappmst.AddRange(_PRRepo.GetPRAddApproverMaster(PRTYPE, Convert.ToInt32(obj.Department_Id)));
                }
                if (!string.IsNullOrEmpty(obj.Division_Id) && Convert.ToInt32(obj.Division_Id) != 0 /*&& objappmst.Count() <= 0*/)
                {
                    // objappmst = _PRRepo.GetPRAddApproverMaster(PRTYPE, Convert.ToInt32(obj.Division_Id));
                    objappmst.AddRange(_PRRepo.GetPRAddApproverMaster(PRTYPE, Convert.ToInt32(obj.Division_Id)));
                }
                if (!string.IsNullOrEmpty(obj.Operation_Id) && Convert.ToInt32(obj.Operation_Id) != 0 /*&& objappmst.Count() <= 0*/)
                {
                    // objappmst = _PRRepo.GetPRAddApproverMaster(PRTYPE, Convert.ToInt32(obj.Operation_Id));
                    objappmst.AddRange(_PRRepo.GetPRAddApproverMaster(PRTYPE, Convert.ToInt32(obj.Operation_Id)));
                }
                #endregion

                objauthaddedinFirst = objappmst.Where(m => m.ISADDEDINLAST == 2).ToList();

                objauthaddedinlast = objappmst.Where(m => m.ISADDEDINLAST == 1).ToList();

                objappmst = objappmst.Where(m => m.ISADDEDINLAST != 1 && m.ISADDEDINLAST != 2).ToList();

                #region "Added in 1st place"

                objauthaddedinFirst = (from data in objauthaddedinFirst
                                       where (data.AMOUNTRANGE_FROM == null ? true : ((indentAmt >= data.AMOUNTRANGE_FROM && indentAmt <= data.AMOUNTRANGE_TO)))
                                       && (data.INDENTTYPE == null ? true : (data.INDENTTYPE != null && data.INDENTTYPE == indent_type))
                                       && (data.IS_SISPR == null ? true : (data.IS_SISPR != null && (data.IS_SISPR == IS_SISPR || data.IS_SISPR == IS_IPCheck)))//SIS PR Change,IP Check
                                       select data).ToList();

                if (objauthaddedinFirst.Count > 0)
                {

                    foreach (var objseq in objauthaddedinFirst.Select(m => m.APPSEQ).Distinct().OrderBy(m => m.Value))
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        foreach (PR_DGIT_PRADDAPP_MST PDPAM in objauthaddedinFirst.Where(m => m.APPSEQ == objseq.Value))
                        {
                            Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(PDPAM.APPROVER));
                            if (Emp_Dtl != null) //Aumento as on 16072024 if condition added
                            {
                                FinalList.Add(new PRAppAuthSeqViewModel
                                {
                                    ADEMPCODE = Emp_Dtl._ECode,
                                    ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                    ADDESIGNATION = Emp_Dtl._Desig,
                                    APP_SEQ = APP_SEQNO,
                                    APPTYPE = 2,
                                    FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                    PRINTORDER = PDPAM.ISPRINTREQUIRED,
                                    ISPARALELLAPP = PDPAM.ISPARALELLAPP,
                                    ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                                    ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                                    ACTIONFOR = PDPAM.ACTIONFOR,
                                    APP_TYPEINFO = PDPAM.APP_TYPEINFO
                                });
                            }
                        }
                    }
                }


                #endregion

                #region "Making Final List for Approval"
                foreach (PRAppAuthSeqViewModel _AuthObj in AuthList)
                {

                    #region Add Extra Authority
                    List<PR_DGIT_PRADDAPP_MST> oPlantPPC = new List<PR_DGIT_PRADDAPP_MST>();
                    List<PR_DGIT_PRADDAPP_MST> oPlantPPC_FN = new List<PR_DGIT_PRADDAPP_MST>();

                    oPlantPPC = (from data in objappmst
                                 where data.ACTUALDESGID == _AuthObj.ADDESIGNATIONID
                                 && ((data.AMOUNTRANGE_FROM == null || data.AMOUNTRANGE_TO == null) ? true : (data.ACTUALDESGID == _AuthObj.ADDESIGNATIONID && (indentAmt >= data.AMOUNTRANGE_FROM && indentAmt <= data.AMOUNTRANGE_TO)))
                                 && (data.INDENTTYPE == null ? true : (data.ACTUALDESGID == _AuthObj.ADDESIGNATIONID && data.INDENTTYPE == indent_type))
                                 && (data.IS_SISPR == null ? true : (data.IS_SISPR != null && data.IS_SISPR == IS_SISPR)) //SIS PR Change
                                 select data).ToList();

                    if (oPlantPPC.Count == 0)
                    {
                        oPlantPPC = (from data in objappmst
                                     where data.FUNCTIONDESID == _AuthObj.ADACTUAL_FUNCDESGID
                                     && ((data.AMOUNTRANGE_FROM == null || data.AMOUNTRANGE_TO == null) ? true : (data.FUNCTIONDESID == _AuthObj.ADACTUAL_FUNCDESGID && (indentAmt >= data.AMOUNTRANGE_FROM && indentAmt <= data.AMOUNTRANGE_TO)))
                                     && (data.INDENTTYPE == null ? true : (data.FUNCTIONDESID == _AuthObj.ADACTUAL_FUNCDESGID && data.INDENTTYPE == indent_type))
                                     && (data.IS_SISPR == null ? true : (data.IS_SISPR != null && data.IS_SISPR == IS_SISPR)) //SIS PR Change
                                     select data).ToList();
                    }

                    oPlantPPC_FN = (from data in objappmst
                                    where data.FUNCTIONDESID < _AuthObj.ADACTUAL_FUNCDESGID
                                    && ((data.AMOUNTRANGE_FROM == null || data.AMOUNTRANGE_TO == null) ? true : (data.FUNCTIONDESID < _AuthObj.ADACTUAL_FUNCDESGID && (indentAmt >= data.AMOUNTRANGE_FROM && indentAmt <= data.AMOUNTRANGE_TO)))
                                    && (data.INDENTTYPE == null ? true : (data.FUNCTIONDESID < _AuthObj.ADACTUAL_FUNCDESGID && data.INDENTTYPE == indent_type))
                                    && (data.IS_SISPR == null ? true : (data.IS_SISPR != null && data.IS_SISPR == IS_SISPR)) //SIS PR Change
                                    select data).ToList();

                    if (oPlantPPC_FN.Count == 0 && oPlantPPC.Count == 0)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        _AuthObj.APP_SEQ = APP_SEQNO;
                        FinalList.Add(_AuthObj);
                    }

                    if (oPlantPPC_FN.Count > 0)
                    {

                        foreach (var objseq in oPlantPPC_FN.Select(m => m.APPSEQ).Distinct().OrderBy(m => m.Value))
                        {
                            APP_SEQNO = (short)(APP_SEQNO + 1);
                            foreach (PR_DGIT_PRADDAPP_MST PDPAM in oPlantPPC_FN.Where(m => m.APPSEQ == objseq.Value))
                            {
                                Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(PDPAM.APPROVER));
                                FinalList.Add(new PRAppAuthSeqViewModel
                                {
                                    ADEMPCODE = Emp_Dtl._ECode,
                                    ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                    ADDESIGNATION = Emp_Dtl._Desig,
                                    APP_SEQ = APP_SEQNO,
                                    APPTYPE = 2,
                                    FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                    PRINTORDER = PDPAM.ISPRINTREQUIRED,
                                    ISPARALELLAPP = PDPAM.ISPARALELLAPP,
                                    ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                                    ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                                    ACTIONFOR = PDPAM.ACTIONFOR,
                                    APP_TYPEINFO = PDPAM.APP_TYPEINFO
                                });
                            }
                        }

                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        _AuthObj.APP_SEQ = APP_SEQNO;
                        FinalList.Add(_AuthObj);

                        ////--- Remove used record ---////
                        foreach (var _obj in oPlantPPC_FN)
                        {
                            objappmst.Remove(_obj);
                        }
                    }

                    if (oPlantPPC.Count > 0)
                    {
                        if (oPlantPPC_FN.Count == 0)
                        {
                            APP_SEQNO = (short)(APP_SEQNO + 1);
                            _AuthObj.APP_SEQ = APP_SEQNO;
                            FinalList.Add(_AuthObj);
                        }

                        foreach (var objseq in oPlantPPC.Select(m => m.APPSEQ).Distinct().OrderBy(m => m.Value))
                        {
                            APP_SEQNO = (short)(APP_SEQNO + 1);
                            foreach (PR_DGIT_PRADDAPP_MST PDPAM in oPlantPPC.Where(m => m.APPSEQ == objseq))
                            {
                                Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(PDPAM.APPROVER));
                                FinalList.Add(new PRAppAuthSeqViewModel
                                {
                                    ADEMPCODE = Emp_Dtl._ECode,
                                    ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                    ADDESIGNATION = Emp_Dtl._Desig,
                                    APP_SEQ = APP_SEQNO,
                                    APPTYPE = 2,
                                    FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                    PRINTORDER = PDPAM.ISPRINTREQUIRED,
                                    ISPARALELLAPP = PDPAM.ISPARALELLAPP,
                                    ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                                    ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                                    ACTIONFOR = PDPAM.ACTIONFOR,
                                    APP_TYPEINFO=PDPAM.APP_TYPEINFO
                                });
                            }
                        }

                        ////--- Remove used record ---////
                        foreach (var _obj in oPlantPPC)
                        {
                            objappmst.Remove(_obj);
                        }
                    }



                    #endregion
                }

                objauthaddedinlast = (from data in objauthaddedinlast
                                      where (data.AMOUNTRANGE_FROM == null ? true : ((indentAmt >= data.AMOUNTRANGE_FROM && indentAmt <= data.AMOUNTRANGE_TO)))
                                      && (data.INDENTTYPE == null ? true : (data.INDENTTYPE != null && data.INDENTTYPE == indent_type))
                                      && (data.IS_SISPR == null ? true : (data.IS_SISPR != null && data.IS_SISPR == IS_SISPR)) //SIS PR Change
                                      select data).ToList();

                if (objauthaddedinlast.Count > 0)
                {

                    foreach (var objseq in objauthaddedinlast.Select(m => m.APPSEQ).Distinct().OrderBy(m => m.Value))
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        foreach (PR_DGIT_PRADDAPP_MST PDPAM in objauthaddedinlast.Where(m => m.APPSEQ == objseq.Value))
                        {
                            Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(PDPAM.APPROVER));
                            FinalList.Add(new PRAppAuthSeqViewModel
                            {
                                ADEMPCODE = Emp_Dtl._ECode,
                                ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                ADDESIGNATION = Emp_Dtl._Desig,
                                APP_SEQ = APP_SEQNO,
                                APPTYPE = 2,
                                FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                PRINTORDER = PDPAM.ISPRINTREQUIRED,
                                ISPARALELLAPP = PDPAM.ISPARALELLAPP,
                                ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                                ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                                ACTIONFOR = PDPAM.ACTIONFOR,
                                IS_SISPR = PDPAM.IS_SISPR, //SIS PR Change
                                APP_TYPEINFO = PDPAM.APP_TYPEINFO //SIS PR Change
                            });
                        }
                    }
                }

                // START Added by Aumento ::SR68003

                //   INDENTTYPE: 1 - Service PR, 2 - Material PR
                if (indent_type == 1)
                {
                    List<PRAppAuthSeqViewModel> mappedFinanceUsers = _PRRepo.GetMappedFinanceUser(PRDATA.PR_ReleaseLocation);

                    if (mappedFinanceUsers.Any())  // Use Any() for better performance instead of Count > 0
                    {
                        short SeqSkip = (short)(ISIPCheck ? 2 : 1);

                        // Set APP_SEQ for all mappedFinanceUsers in one go
                        mappedFinanceUsers.ForEach(user => user.APP_SEQ = SeqSkip);

                        // Remove existing entries with the same APP_SEQ from FinalList
                        FinalList.RemoveAll(x => x.APP_SEQ == SeqSkip);

                        // Add updated finance users to FinalList
                        FinalList.AddRange(mappedFinanceUsers);
                    }

                }

                // END Added by Aumento ::SR68003


                #endregion
            }
            catch (Exception ex)
            {
                FinalList = new List<PRAppAuthSeqViewModel>();
            }
            return FinalList.OrderBy(m => m.APPTYPE).OrderBy(n => n.APP_SEQ).ToList();
        }

        public List<PRAppAuthSeqViewModel> GetAuthority(long loginUser, decimal indentAmt, long PRTYPE, Employee_Details obj)
        {
            List<PRAppAuthSeqViewModel> AuthList = new List<PRAppAuthSeqViewModel>();
            try
            {
                #region "Get Additinal Auth. for SIS & GCA"
                long isauthfound = 0, SMHEAD = 0, DMHEAD = 0, DHHEAD = 0;

                VW_DGIT_PRADDAUTH_MAP objhead = new VW_DGIT_PRADDAUTH_MAP();
                if (!string.IsNullOrEmpty(obj.Section_Id) && Convert.ToInt32(obj.Section_Id) != 0)
                {
                    objhead = _PRRepo.GetPRAdditionalApp(PRTYPE, Convert.ToInt32(obj.Section_Id));
                    if (objhead != null && objhead.ADEMPCODE != 0)
                    {
                        if (objhead.ADORGLEVELTYPEID == 4)
                        {
                            SMHEAD = objhead.ADEMPCODE;
                        }
                        if (objhead.ADORGLEVELTYPEID == 3)
                        {
                            DMHEAD = objhead.ADEMPCODE;
                        }
                        if (objhead.ADORGLEVELTYPEID == 2)
                        {
                            DHHEAD = objhead.ADEMPCODE;
                        }
                        isauthfound = 1;
                    }
                }
                if (!string.IsNullOrEmpty(obj.Department_Id) && Convert.ToInt32(obj.Department_Id) != 0 && isauthfound == 0)
                {
                    objhead = _PRRepo.GetPRAdditionalApp(PRTYPE, Convert.ToInt32(obj.Department_Id));
                    if (objhead != null && objhead.ADEMPCODE != 0)
                    {
                        if (objhead.ADORGLEVELTYPEID == 4)
                        {
                            SMHEAD = objhead.ADEMPCODE;
                        }
                        if (objhead.ADORGLEVELTYPEID == 3)
                        {
                            DMHEAD = objhead.ADEMPCODE;
                        }
                        if (objhead.ADORGLEVELTYPEID == 2)
                        {
                            DHHEAD = objhead.ADEMPCODE;
                        }
                        isauthfound = 1;
                    }
                }
                if (!string.IsNullOrEmpty(obj.Division_Id) && Convert.ToInt32(obj.Division_Id) != 0 && isauthfound == 0)
                {
                    objhead = _PRRepo.GetPRAdditionalApp(PRTYPE, Convert.ToInt32(obj.Division_Id));
                    if (objhead != null && objhead.ADEMPCODE != 0)
                    {
                        if (objhead.ADORGLEVELTYPEID == 4)
                        {
                            SMHEAD = objhead.ADEMPCODE;
                        }
                        if (objhead.ADORGLEVELTYPEID == 3)
                        {
                            DMHEAD = objhead.ADEMPCODE;
                        }
                        if (objhead.ADORGLEVELTYPEID == 2)
                        {
                            DHHEAD = objhead.ADEMPCODE;
                        }
                        isauthfound = 1;
                    }
                }
                #endregion          

                #region "Making Auth List"
                UserApprovalAuthority Obj = _CommonRepo.CheckApprovalAuthority(loginUser);
                short APP_SEQNO = 1;
                if (Obj != null)
                {
                    if (Obj.SectionManager != 0 && Obj.SectionManager != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.SectionManager));
                        AuthList.Add(new PRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = APP_SEQNO,
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                            PRINTORDER = 1,
                            ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                            ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                        });
                    }
                    if (SMHEAD != 0)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(SMHEAD));
                        AuthList.Add(new PRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = APP_SEQNO,
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                            PRINTORDER = 0,
                            ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                            ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                        });
                    }
                    if (Obj.DepartmentManager != 0 && Obj.DepartmentManager != null)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DepartmentManager));
                        AuthList.Add(new PRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = APP_SEQNO,
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                            PRINTORDER = 2,
                            ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                            ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                        });
                    }
                    if (DMHEAD != 0)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(DMHEAD));
                        AuthList.Add(new PRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = APP_SEQNO,
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                            PRINTORDER = 0,
                            ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                            ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                        });
                    }

                    if (indentAmt < 20001 && AuthList.Where(m => m.FNDESID == 2).Count() > 0)
                    {
                        return AuthList.OrderBy(m => m.APPTYPE).OrderBy(n => n.APP_SEQ).ToList();
                    }

                    if (Obj.Coordinator != 0 && Obj.Coordinator != null)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.Coordinator));
                        AuthList.Add(new PRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = APP_SEQNO,
                            APPTYPE = 1,
                            PRINTORDER = 3,
                            ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                            //ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                        });
                    }

                    if (Obj.DivisionHead != 0 && Obj.DivisionHead != null)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DivisionHead));
                        AuthList.Add(new PRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = APP_SEQNO,
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                            PRINTORDER = 4,
                            ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                            ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                        });
                    }

                    if (DHHEAD != 0)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(DHHEAD));
                        AuthList.Add(new PRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = APP_SEQNO,
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                            PRINTORDER = 0,
                            ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                            ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                        });
                    }

                    if (indentAmt < 200001 && AuthList.Where(m => m.FNDESID == 3).Count() > 0)
                    {
                        return AuthList.OrderBy(m => m.APPTYPE).OrderBy(n => n.APP_SEQ).ToList();
                    }

                    if (Obj.EXECoordinator != 0 && Obj.EXECoordinator != null)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.EXECoordinator));
                        AuthList.Add(new PRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = APP_SEQNO,
                            APPTYPE = 1,
                            FNDESID = 4,
                            PRINTORDER = 5,
                            ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                            //ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                        });
                    }

                    if (Obj.OperationHead != 0 && Obj.OperationHead != null)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        if (indentAmt < 300001)
                        {
                            Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.OperationHead));
                            if (Emp_Dtl != null)
                            {
                                AuthList.Add(new PRAppAuthSeqViewModel
                                {
                                    ADEMPCODE = Emp_Dtl._ECode,
                                    ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                    ADDESIGNATION = Emp_Dtl._Desig,
                                    APP_SEQ = APP_SEQNO,
                                    APPTYPE = 1,
                                    FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                    PRINTORDER = 6,
                                    ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                                    ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                                });
                            }
                        }
                        if (indentAmt >= 300001 && (Obj.OperationHead != Obj.Director))
                        {
                            Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.OperationHead));
                            if (Emp_Dtl != null)
                            {
                                AuthList.Add(new PRAppAuthSeqViewModel
                                {
                                    ADEMPCODE = Emp_Dtl._ECode,
                                    ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                    ADDESIGNATION = Emp_Dtl._Desig,
                                    APP_SEQ = APP_SEQNO,
                                    APPTYPE = 1,
                                    FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                    PRINTORDER = 6,
                                    ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                                    ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                                });
                            }
                        }
                    }

                    string strCPOValue = _CommonRepo.GetParameterValue("CPO");
                    string strCPODIR = strCPOValue.Split('~')[0];
                    string[] strCPOOperation = strCPOValue.Split('~')[1].Split(',');
                    if (Obj.OperationId != 0 && Obj.OperationId != null && (strCPOOperation.Contains(Obj.OperationId.ToString()) || strCPOOperation.Contains(Obj.DivisionId.ToString())) && indentAmt >= 200001)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        Employee_Details Emp_Dtl = _CommonRepo.GetEVPDetailById(Convert.ToInt64(Obj.OperationId));
                        if (Emp_Dtl != null)
                        {
                            AuthList.Add(new PRAppAuthSeqViewModel
                            {
                                ADEMPCODE = Emp_Dtl._ECode,
                                ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                ADDESIGNATION = Emp_Dtl._Desig,
                                APP_SEQ = APP_SEQNO,
                                APPTYPE = 1,
                                FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                PRINTORDER = 7,
                                ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                                //ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                            });
                        }
                    }
                    if (indentAmt < 300001)
                    {
                        return AuthList.OrderBy(m => m.APPTYPE).OrderBy(n => n.APP_SEQ).ToList();
                    }

                    string strValue = _CommonRepo.GetParameterValue("PPCHOOH");
                    string strPPCOH = strValue.Split('~')[0];
                    string[] strOHOperation = strValue.Split('~')[1].Split(',');
                    if (!string.IsNullOrEmpty(strPPCOH) && (strOHOperation.Contains(Obj.OperationId.ToString()) || strOHOperation.Contains(Obj.DivisionId.ToString())))
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(strPPCOH));
                        if (Emp_Dtl != null)
                        {
                            AuthList.Add(new PRAppAuthSeqViewModel
                            {
                                ADEMPCODE = Emp_Dtl._ECode,
                                ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                ADDESIGNATION = Emp_Dtl._Desig,
                                APP_SEQ = APP_SEQNO,
                                APPTYPE = 1,
                                FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                PRINTORDER = 8,
                                ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                                //ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                            });
                        }
                    }

                    if (Obj.Director != 0 && Obj.Director != null)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        short atype;
                        Employee_Details Emp_Dtl = _CommonRepo.GetDirectorDetailById(Convert.ToInt64(Obj.Director), Convert.ToInt64(Obj.OperationId), out atype);
                        AuthList.Add(new PRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = APP_SEQNO,
                            APPTYPE = atype,
                            FNDESID = 6,
                            PRINTORDER = 8,
                            ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                            //ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                        });
                    }

                    if (Obj.Director2 != 0 && Obj.Director2 != null)
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        short atype;
                        Employee_Details Emp_Dtl = _CommonRepo.GetDirectorDetailById(Convert.ToInt64(Obj.Director2), Convert.ToInt64(Obj.OperationId), out atype);
                        AuthList.Add(new PRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = APP_SEQNO,
                            APPTYPE = atype,
                            FNDESID = 7,
                            PRINTORDER = 8,
                            ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                            //ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                        });
                    }

                    if (Obj.OperationId != 0 && Obj.OperationId != null && (strCPOOperation.Contains(Obj.OperationId.ToString()) || strCPOOperation.Contains(Obj.DivisionId.ToString())))
                    {
                        APP_SEQNO = (short)(APP_SEQNO + 1);
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(strCPODIR));
                        if (Emp_Dtl != null)
                        {
                            AuthList.Add(new PRAppAuthSeqViewModel
                            {
                                ADEMPCODE = Emp_Dtl._ECode,
                                ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                ADDESIGNATION = Emp_Dtl._Desig,
                                APP_SEQ = APP_SEQNO,
                                APPTYPE = 1,
                                FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                PRINTORDER = 9,
                                ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                                //ADACTUAL_FUNCDESGID = Convert.ToInt16(Emp_Dtl._FnDesigId),
                            });
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                AuthList = new List<PRAppAuthSeqViewModel>();
            }
            return AuthList.OrderBy(m => m.APPTYPE).OrderBy(n => n.APP_SEQ).ToList();
        }

        public Tuple<short, List<PRDetailViewModel>> SaveAttachment(long addedDate, long poHid, List<PRDetailViewModel> modelList)
        {
            List<PRDetailViewModel> iList = new List<PRDetailViewModel>();
            short retVal = _PRRepo.SavePRDetails(addedDate, poHid, modelList);
            if (retVal == 1)
            {
                iList = _PRRepo.GetAttachmentDetail(poHid);
            }
            Tuple<short, List<PRDetailViewModel>> _tuple = new Tuple<short, List<PRDetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public Tuple<short, List<PRDetailViewModel>> DeleteAttachment(string fileName, string docType, long poHid)
        {
            List<PRDetailViewModel> iList = new List<PRDetailViewModel>();
            short retVal = _PRRepo.DeleteAttachment(fileName, docType, poHid);
            if (retVal == 1)
            {
                iList = _PRRepo.GetAttachmentDetail(poHid);
            }
            Tuple<short, List<PRDetailViewModel>> _tuple = new Tuple<short, List<PRDetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public short SendMailByRequestor(long poHeaderId)
        {
            short retVal = 0;
            try
            {
                EmailCore sendMail = new EmailCore();
                PRHeaderViewModel PHVM = _PRRepo.GetPRRequestById(poHeaderId);
                if (PHVM.prAppHis.Count > 0)
                {
                    List<PRAppHistoryViewModel> Auth_obj_lst = PHVM.prAppHis.Where(a => a.PRAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).ToList();
                    foreach (PRAppHistoryViewModel Auth_obj in Auth_obj_lst)
                    {
                        if (Auth_obj != null)
                        {
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            if (serverpath.isTestServer())
                                sendMail.MailTo = serverpath.getTestEMail();
                            else
                                sendMail.MailTo = Auth_obj.APP_EMAIL;

                            string struid = (Auth_obj.APPEMP_CODE).ToString();
                            string strid = poHeaderId.ToString();

                            string strSubject = "Purchase Request - " + PHVM.IndentNo + ", Requestor - " + PHVM.Emp_Detail._EName + "(" + PHVM.Emp_Detail._ECode + ")";
                            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PR Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                             "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a PR Approval Request in Employee Portal. Below are the details :</br></td></tr>" +
                                             "<tr><td width=125 height=23 valign=top>Indent Number: </td><td width=389 valign=top>" + PHVM.IndentNo + "</td></tr>" +
                                             "<tr><td width=125 height=23 valign=top>Remarks: </td><td width=389 valign=top>" + PHVM.ItemDetail + "</td></tr>" +
                                             "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=PR&A=PRApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                             //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to View the detail.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
                                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            try
                            {
                                bool status = sendMail.Send();
                            }
                            catch (Exception ex)
                            {
                                retVal = -1;
                            }
                            finally
                            {
                                retVal = 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }
        //added by aumento for SR86919
        public short AddReleaseHoldentry(PRHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                retVal = _PRRepo.AddNewEntryWithApprovalStatus(model.PRHEADERID, model.ADDEDBY);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }
        //added by aumento for SR86919
        public short SendMailByRequestorAfterHoldandUploadDoc(PRHeaderViewModel Model)
        {
            short retVal = 0;
            long poHeaderId = Model.PRHEADERID;
            try
            {
                EmailCore sendMail = new EmailCore();
                PRHeaderViewModel PHVM = _PRRepo.GetPRRequestById(Model.PRHEADERID);
                if (PHVM.prAppHis.Count > 0)
                {
                   
                    PRAppHistoryViewModel Auth_obj = PHVM.prAppHis.Where(a => a.PRAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 4  && !string.IsNullOrEmpty(a.APP_EMAIL) ).OrderByDescending(x => x.PRAPPHISTORY_ID).FirstOrDefault();
                    if (Auth_obj != null)
                        {
                            
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;

                        string struid = (Auth_obj.APPEMP_CODE).ToString();
                            string strid = poHeaderId.ToString();

                            string strSubject = "Purchase Request - " + PHVM.IndentNo + ", Requestor - " + PHVM.Emp_Detail._EName + "(" + PHVM.Emp_Detail._ECode + ")";
                            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:1000px;'>" +
                                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PR Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                             "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=1000px><tr><td colspan=2 width=900 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has uploaded documents in PR approval request as a justification which is hold by you in Employee Portal. Please take appropriate action. Below are the details:</br></td></tr>" +
                                             "<tr><td width=300 height=23 valign=top>Indent Number: </td><td width=700 valign=top>" + PHVM.IndentNo + "</td></tr>" +
                                             "<tr><td width=300 height=23 valign=top>Remarks: </td><td width=700 valign=top>" + PHVM.ItemDetail + "</td></tr>" +
                                             "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=PR&A=PRApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                             //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to approve the request.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
                                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            try
                            {
                                bool status = sendMail.Send();
                            }
                            catch (Exception ex)
                            {
                                retVal = -1;
                            }
                            finally
                            {
                                retVal = 1;
                            }
                        }
                    
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }


        public short SendMailByApprovalAuthority(long poHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                EmailCore sendMail = new EmailCore();
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : approvalStatus == 4 ? "Hold" : ""; //SIS PR Change

                #region Send mail next approval authority
                PRHeaderViewModel PHVM = _PRRepo.GetPRRequestById(poHeaderId);
                if (PHVM.prAppHis.Count > 0 && approvalStatus == 1)
                {
                    List<PRAppHistoryViewModel> Auth_obj_lst = PHVM.prAppHis.Where(a => a.PRAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).ToList();
                    foreach (PRAppHistoryViewModel Auth_obj in Auth_obj_lst)
                    {
                        if (Auth_obj != null)
                        {
                           
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            if (serverpath.isTestServer())
                                sendMail.MailTo = serverpath.getTestEMail();
                            else
                                 sendMail.MailTo = Auth_obj.APP_EMAIL;

                            string struid = (Auth_obj.APPEMP_CODE).ToString();
                            string strid = poHeaderId.ToString();

                            string strSubject = "Purchase Request - " + PHVM.IndentNo + ", Requestor - " + PHVM.Emp_Detail._EName + "(" + PHVM.Emp_Detail._ECode + ")";
                            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PR Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                             "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a PR Approval Request in Employee Portal. Below are the details :</br></td></tr>" +
                                             "<tr><td width=125 height=23 valign=top>Indent Number: </td><td width=389 valign=top>" + PHVM.IndentNo + "</td></tr>" +
                                             "<tr><td width=125 height=23 valign=top>Remarks: </td><td width=389 valign=top>" + PHVM.ItemDetail + "</td></tr>" +
                                             "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=PR&A=PRApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                             //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to View the detail.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
                                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            try
                            {
                                bool status = sendMail.Send();
                            }
                            catch (Exception ex)
                            {
                                retVal = -1;
                            }
                            finally
                            {
                                retVal = 1;
                            }
                        }
                    }
                }
                #endregion

                #region Send mail for requestor
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
                {
                    //SIS PR Change
                    string holdDate = "", holdRemarks = "";
                    //var holdDetails = PHVM.prAppHis.Where(x => x.APPROVAL_STATUS == 0 && x.HOLD_DATE != null).FirstOrDefault();
                    var holdDetails = PHVM.prAppHis.Where(x => x.APPROVAL_STATUS == 4 && x.PRID == poHeaderId).OrderByDescending(m=>m.PRAPPHISTORY_ID).FirstOrDefault(); //Aumento as on 16072024
                    if (holdDetails != null)
                    {
                        //holdDate = holdDetails.HOLD_DATE.Value.ToString("dd-MMM-yyyy");//Aumento as on 16072024 
                        holdRemarks = holdDetails.APPROVAL_REMARK; //Aumento as on 16072024 - Hold_remarks to approval_remarks
                    }
                    //SIS PR Change

                    
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId;

                    string strSubject = "PR Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PR Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your PR request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>Indent Number</td><td width=389 valign=top>" + PHVM.IndentNo + "</td></tr>" +
                                     "<tr><td width=125 valign=top>Remarks </td><td width=389 valign=top>" + PHVM.ItemDetail + "</td></tr>";
                    //SIS PR Change
                    if (approvalStatus == 4)
                    {
                        strBody = strBody + "<tr style='background-color:yellow;'><td width=125 valign=top>Hold Remarks </td><td width=389 valign=top>" + holdRemarks + "</td></tr>";  //Aumento as on 16072024 - Background color added
                        //"<tr><td width=125 valign=top>Hold Date </td><td width=389 valign=top>" + holdDate + "</td></tr>"; //Aumento as on 16072024 - Comment
                    }
                    //SIS PR Change
                    strBody = strBody + "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                    }
                    finally
                    {
                        retVal = 1;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public List<ADORGLEVEL> GetOrgLevelList(long typeId)
        {
            return _PRRepo.GetOrgLevelList(typeId);
        }

        public Tuple<short, List<ADORGLEVEL>> BindOperationForOPMap(long loginUser)
        {
            return _PRRepo.BindOperationForOPMap(loginUser);
        }

        public List<PR_Div_Dep_SecViewModel> BindDivision(long op_Id)
        {
            return _PRRepo.BindDivision(op_Id);
        }

        public List<PR_Div_Dep_SecViewModel> BindDepartment(long div_Id, long op_Id)
        {
            return _PRRepo.BindDepartment(div_Id, op_Id);
        }

        public List<PR_Div_Dep_SecViewModel> BindSection(long dep_Id, long div_Id, long op_Id)
        {
            return _PRRepo.BindSection(dep_Id, div_Id, op_Id);
        }

        public SearchIndent PRDashboard(SearchIndent PHVM)
        {
            return _PRRepo.PRDashboard(PHVM);
        }

        public PRPURStatusViewModel GetPRPURStatusById(long id)
        {
            return _PRRepo.GetPRPURStatusById(id);
        }

        public short UpdatePRStatus(PRPURStatusViewModel PPVM, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                EmailCore sendMail = new EmailCore();
                retVal = _PRRepo.UpdatePRStatus(PPVM);
                if (retVal == 1)
                {
                    string RequestStatus = PPVM.STATUS == 1 ? "Hold" : PPVM.STATUS == 2 ? "Send back" : "";
                    string RequestStatusHold = PPVM.STATUS == 1 ? " put on Hold" : PPVM.STATUS == 2 ? " send back" : "";
                    PRHeaderViewModel PHVM = _PRRepo.GetPRRequestById(PPVM.PRHEADERID);

                    #region Send mail for requestor
                    if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId) && !string.IsNullOrEmpty(RequestStatus))
                    {
                     
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        sendMail.MailTo = PHVM.Emp_Detail._EmailId;
                        if (!string.IsNullOrEmpty(employeeDetails._EmailId))
                        {
                            sendMail.MailCc = employeeDetails._EmailId;
                        }

                        /// --- Add Top two authority in mail CC. ---///
                        List<PRAppHistoryViewModel> AppHisList = PHVM.prAppHis.Where(h => h.APPROVAL_STATUS == 1).OrderBy(o => o.PRAPPHISTORY_ID).Take(2).ToList();
                        if (AppHisList.Count > 0)
                        {
                            foreach (PRAppHistoryViewModel appObj in AppHisList)
                            {
                                if (!string.IsNullOrEmpty(appObj.APP_EMAIL))
                                {
                                    if (string.IsNullOrEmpty(sendMail.MailCc)) { sendMail.MailCc = appObj.APP_EMAIL; }
                                    else { sendMail.MailCc += "," + appObj.APP_EMAIL; }
                                }
                            }
                        }

                        string strSubject = "PR Approval Status - " + RequestStatus;
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PR Buyer Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                         "<tr><td valign=top colspan =2>Your PR request has been <b>" + RequestStatusHold + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                         "<tr><td width=135 height=22 valign=top><b>Indent Number</b></td><td width=389 valign=top>" + PHVM.IndentNo + "</td></tr>" +
                                         "<tr><td width=135 valign=top><b>Remarks</b></td><td width=389 valign=top>" + PHVM.ItemDetail + "</td></tr>" +
                                         "<tr><td width=135 valign=top><b>" + RequestStatus + " Remarks</b></td><td width=389 valign=top>" + PPVM.REMARK + "</td></tr>" +
                                         "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
                                         "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool status = sendMail.Send();
                        }
                        catch (Exception ex)
                        {
                            retVal = -2;
                        }
                        finally
                        {
                            retVal = 1;
                        }
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                retVal = retVal == 1 ? (short)-2 : (short)-1;
            }
            return retVal;
        }

        public short ChangeSLACategory(long PRID, short selectedSLACategory, long Updatedby)
        {
            return _PRRepo.ChangeSLACategory(PRID, selectedSLACategory, Updatedby);
        }

        public SearchIndentUser PRUserDashboard(SearchIndentUser PHVM)
        {
            return _PRRepo.PRUserDashboard(PHVM);
        }
        public SearchIndentUser PRAgeingHis(SearchIndentUser PHVM)
        {
            return _PRRepo.PRAgeingHistory(PHVM);
        }

        public List<PR_Div_Dep_SecViewModel> BindPlant(long Plant_Id)
        {
            return _PRRepo.BindPlant(Plant_Id);
        }
        public long GetPRNextApprovalId(long PRID, long ecode)
        {
            return _PRRepo.GetPRNextApprovalId(PRID, ecode);
        }

        public short AssignBuyer(PRBuyerMapViewModel PBVM)
        {
            return _PRRepo.AssignBuyer(PBVM);
        }

        public PRBuyerMapViewModel GetPRBuyerByHeaderId(long id)
        {
            return _PRRepo.GetPRBuyerByHeaderId(id);
        }

        public List<PRBuyerMstViewModel> GetBuyerMstList(long? GPType, long? ADOrgLevelId)
        {
            return _PRRepo.GetBuyerMstList(GPType, ADOrgLevelId);
        }

        public short PRChangeCategory(long PRID, short selectedCategory, long Updatedby)
        {
            return _PRRepo.PRChangeCategory(PRID, selectedCategory, Updatedby);
        }

        public short UpdateAllocationStatus(long PRID, long Updatedby)
        {
            return _PRRepo.UpdateAllocationStatus(PRID, Updatedby);
        }

        public SearchIndent PRBuyerDashboard(SearchIndent VM)
        {
            return _PRRepo.PRBuyerDashboard(VM);
        }

        public List<DGIT_PRCAT_MST> BindPRCategory()
        {
            return _PRRepo.BindPRCategory();
        }
        public List<PR_DGIT_PRADDAPP_MST> Get_PRADDAPP_MST_List(PR_DGIT_PRADDAPP_MST PR)
        {
            return _PRRepo.Get_PRADDAPP_MST_List(PR);
        }

        public List<Employee_Details> PortalAutocompleteSuggestionsFunDesig(string term)
        {
            return _PRRepo.PortalAutocompleteSuggestionsFunDesig(term);
        }

        public List<Employee_Details> PortalAutocompleteSuggestionsActDesig(string term)
        {
            return _PRRepo.PortalAutocompleteSuggestionsActDesig(term);
        }

        public long AddEditDgitPraddapp_mst(List<PR_DGIT_PRADDAPP_MST> data)
        {
            return _PRRepo.AddEditDgitPraddapp_mst(data);
        }

        public List<AD_orglevel_type> getOrgLevelType()
        {
            return _PRRepo.getOrgLevelType();
        }

        public List<AD_orglevel> GetOrgUnitData(int id)
        {
            return _PRRepo.GetOrgUnitData(id);
        }

        //================================================Start=================================================
        //                                 Allocator Data Management 29-08-2022 (Aumento)
        //======================================================================================================
        public List<DGIT_PRCAT_MST> getDgitPRCatMst()
        {
            return _PRRepo.getDgitPRCatMst();
        }

        public List<PR_dgit_properationmap> Get_Properationmap_List(PR_dgit_properationmap PR)
        {
            return _PRRepo.Get_Properationmap_List(PR);
        }

        public List<AD_orglevel> GetOperationList()
        {
            return _PRRepo.GetOperationList();
        }

        public long SaveAllocatorMaster(List<PR_dgit_properationmap> data)
        {
            return _PRRepo.SaveAllocatorMaster(data);
        }

        public List<PR_dgit_properationmap> getPR_properationmapReport(PR_dgit_properationmap pr)
        {
            return _PRRepo.getPR_properationmapReport(pr);
        }
        public int Get_EmployeeMap_Count(PR_dgit_properationmap PR)
        {
            return _PRRepo.Get_EmployeeMap_Count(PR);
        }
        //=======================================End(Allocator)===============================

        //================================================Start=================================================
        //                                 Buyer Data Management 29-08-2022 (Aumento)
        //======================================================================================================
        public List<AD_orglevel> PortalAutocompleteOrgLevel(string Key)
        {
            return _PRRepo.PortalAutocompleteOrgLevel(Key);
        }

        public List<PRBuyerMstViewModel> getPRBuyerMstList(PRBuyerMstViewModel obj)
        {
            return _PRRepo.getPRBuyerMstList(obj);
        }

        public long SaveBuyerMaster(List<PRBuyerMstViewModel> data)
        {
            return _PRRepo.SaveBuyerMaster(data);
        }

        public List<PRBuyerMstViewModel> getPR_BuyermapReport(PRBuyerMstViewModel obj)
        {
            return _PRRepo.getPR_BuyermapReport(obj);
        }

        public List<Employee_Details> PortalAutocompleteSuggestionsEmployee(string term, int Catid, int Orgid)
        {
            return _PRRepo.PortalAutocompleteSuggestionsEmployee(term, Catid, Orgid);
        }

        //=======================================End (Buyer)===============================


        //---SR52365==============================
        public Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> BindDivision(long op_Id, long _syKi, long adempcode)
        {
            return _PRRepo.BindDivision(op_Id, _syKi, adempcode);
        }
        public Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> BindDepartment(long div_Id, long _syKi, long adempcode)
        {
            return _PRRepo.BindDepartment(div_Id, _syKi, adempcode);
        }
        public Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> BindSection(long div_Id, long dept_Id, long _syKi, long adempcode)
        {
            return _PRRepo.BindSection(div_Id, dept_Id, _syKi, adempcode);
        }
        public List<SYKI> GetKICodeList(long userId)
        {
            return _PRRepo.GetKICodeList(userId);
        }

        public SearchIndentUser PRPIUserDashboard(SearchIndentUser VM)
        {
            return _PRRepo.PRPIUserDashboard(VM);
        }

        public List<PRBuyerMstViewModel> GetABuyerMstList(long? ADOrgLevelId)
        {
            return _PRRepo.GetABuyerMstList(ADOrgLevelId);
        }
        //---==============================
		
		
		 //Added by aumento as on 19092024 for the SR71870============================================================
        public List<SYKI> GetKICodeList_ForPRDBOperationwise(long userId)
        {
            return _PRRepo.GetKICodeList_ForPRDBOperationwise(userId);
        }

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDivision_PRDB(long op_Id, long LoginEcode, long kiid)
        {
            return _PRRepo.BindDivision_PRDB(LoginEcode, kiid, op_Id);
        }

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindOperation(long Loginempcode, long KIID)
        {
            return _PRRepo.BindOperation(Loginempcode, KIID);
        }
        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindSection(long ecode, long kIID, long operationID, long divisionID, long dEPTID)
        {
            return _PRRepo.BindSection(ecode, kIID, operationID, divisionID, dEPTID);
        }

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDepartment(long ecode, long kIID, long operationID, long divisionID)
        {
            return _PRRepo.BindDepartment(ecode, kIID, operationID, divisionID);
        }

        public Employee_Details GetEmployeeDetail(long ecode, long KIID)
        {
            return _PRRepo.GetEmployeeDetail(ecode, KIID);
        }

        public SearchIndentUser PRDashboadReport(SearchIndentUser PHVM)
        {
            return _PRRepo.PRDashboadReport(PHVM);
        }

        // START Added by Aumento ::  SR68003
        public object GetPRReleasePlants()
        {
            return _PRRepo.GetPRReleasePlants();
        }

        //===========================================================================================================
        
        //// Start Added By Aumento :: SR80255
        public short SendMailAfterAutoAssignBuyer(long poHeaderId)
        {
            short retVal = 0;
            try
            {
                Employee_Details EmpObj = _PRRepo.GetDataForSendMailAfterAutoAssignBuyer(poHeaderId);
                PRHeaderViewModel PHVM = _PRRepo.GetPRRequestById(poHeaderId);

                if (EmpObj != null && EmpObj.EMail_Id != "")
                {

                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                        sendMail.MailTo = EmpObj.EMail_Id;


                    string struid = (EmpObj._ECode).ToString();
                    string strid = poHeaderId.ToString();

                    string strSubject = "Purchase Request - " + PHVM.IndentNo + ", Requestor - " + PHVM.Emp_Detail._EName + "(" + PHVM.Emp_Detail._ECode + ")";
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PR Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + EmpObj._EFirstName + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a PR Approval Request in Employee Portal and Assign you . Below are the details :</br></td></tr>" +
                                     "<tr><td width=125 height=23 valign=top>Indent Number: </td><td width=389 valign=top>" + PHVM.IndentNo + "</td></tr>" +
                                     "<tr><td width=125 height=23 valign=top>Remarks: </td><td width=389 valign=top>" + PHVM.ItemDetail + "</td></tr>" +
                                     "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=PR&A=PRApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                     //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to View the detail.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                    }
                    finally
                    {
                        retVal = 1;
                    }


                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }
        //// End Added By Aumento :: SR80255
        
        //Added by TTL on 24-May-2025 against SR99130 > CR6361 - Start
        public List<DateTime> GetHolidaysByPlant(DateTime startDate, DateTime endDate, long siteId)
        {
            return _PRRepo.GetHolidaysByPlant(startDate, endDate, siteId);
        }
        //Added by TTL on 24-May-2025 against SR99130 > CR6361 - End
    }
}
