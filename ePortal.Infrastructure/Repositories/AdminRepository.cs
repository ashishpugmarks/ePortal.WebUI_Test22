using ePortal.DomainClasses;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;

namespace ePortal.Infrastructure.Repositories
{
    public class AdminRepository
    {
        private EPortalDBContext _DBContext;
        private SYKI _Syki;
        public AdminRepository(EPortalDBContext empLoginDBContext, ISessionService sessionService, EmployeeLoginRepository empRepo)
        {
            _DBContext = empLoginDBContext;
            _Syki = _DBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        public List<SelectListViewModel> GetLocation_OperationByTypeId(int typeId)
        {
            List<SelectListViewModel> iList = new List<SelectListViewModel>();
            if (typeId == 2) // Location
            {
                iList = (from data in _DBContext.SYSITE
                         where data.ACTIVE == 1
                         select new SelectListViewModel
                         {
                             Value = data.SYSITEID,
                             Text = data.DESCRIP,
                         }).OrderBy(o => o.Text).ToList();
            }
            else if (typeId == 3) // Operation
            {
                iList = (from data in _DBContext.ADORGLEVEL
                         where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID && data.ADORGLEVELTYPEID == 1
                         select new SelectListViewModel
                         {
                             Value = data.ADORGLEVELID,
                             Text = data.LEVELDESCRIP,
                         }).OrderBy(o => o.Text).ToList();
            }
            return iList;
        }

        public List<EmailGroupViewModel> GetEmailGroupList()
        {
            var iList = (from data in _DBContext.ADEMAIL_GROUPMST
                         join _AddBy in _DBContext.ADEMPLOYEE on data.ADDED_BY equals _AddBy.ADEMPCODE
                         join _VWAssociate in _DBContext.VW_ASSOCIATELVLDETAILS on data.ADDED_BY equals _VWAssociate.ADEMPCODE
                         where _VWAssociate.SYKI == _Syki.SYKIID
                         select new EmailGroupViewModel
                         {
                             EMAILGROUPMSTID = data.EMAILGROUPMSTID,
                             GROUP_TYPE = data.GROUP_TYPE,
                             GROUP_NAME = data.GROUP_NAME,
                             //STATUS = data.STATUS == 1 ? true : false,
                             STATUS = data.STATUS  ,
                             REMARK = data.REMARK,
                             ADDED_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             ADDED_BY = data.ADDED_BY,
                             ADDED_DATE = data.ADDED_DATE,
                         }).ToList();
            List<EmailGroupViewModel> newList = new List<EmailGroupViewModel>();
            foreach (EmailGroupViewModel obj in iList.OrderBy(o => o.GROUP_TYPE).ThenBy(t => t.GROUP_NAME))
            {
                if (obj.GROUP_TYPE == 2)
                {
                    obj.EmailMapp_List = (from gm in _DBContext.ADEMAIL_GROUPMAPPING
                                          join _location in _DBContext.SYSITE on gm.GROUP_VALUE equals _location.SYSITEID
                                          where gm.EMAILGROUPMSTID == obj.EMAILGROUPMSTID
                                          && gm.STATUS == 1
                                          select new EmailGroupMappViewModel
                                          {
                                              EMAILGROUPMAPID = gm.EMAILGROUPMAPID,
                                              EMAILGROUPMSTID = gm.EMAILGROUPMSTID,
                                              GROUP_VALUE = gm.GROUP_VALUE,
                                              GROUP_MAPP_NAME = _location.DESCRIP,
                                          }).ToList();
                }
                else if (obj.GROUP_TYPE == 3)
                {
                    obj.EmailMapp_List = (from gm in _DBContext.ADEMAIL_GROUPMAPPING
                                          join _OP in _DBContext.ADORGLEVEL on gm.GROUP_VALUE equals _OP.ADORGLEVELID
                                          where gm.EMAILGROUPMSTID == obj.EMAILGROUPMSTID
                                          && gm.STATUS == 1
                                          select new EmailGroupMappViewModel
                                          {
                                              EMAILGROUPMAPID = gm.EMAILGROUPMAPID,
                                              EMAILGROUPMSTID = gm.EMAILGROUPMSTID,
                                              GROUP_VALUE = gm.GROUP_VALUE,
                                              GROUP_MAPP_NAME = _OP.LEVELDESCRIP,
                                          }).ToList();
                }
                else
                {
                    obj.EmailMapp_List = new List<EmailGroupMappViewModel>();
                }
                newList.Add(obj);
            }
            return newList;
        }

        public Tuple<short, long> SaveEmailGroup(EmailGroupViewModel model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            using (IDbContextTransaction transaction = _DBContext.Database.BeginTransaction())
            {
                try
                {
                    ADEMAIL_GROUPMST DPH = new ADEMAIL_GROUPMST();
                    int FlagAdd = 0;
                    if (model.EMAILGROUPMSTID > 0)
                    {
                        DPH = _DBContext.ADEMAIL_GROUPMST.Where(x => x.EMAILGROUPMSTID == model.EMAILGROUPMSTID).SingleOrDefault();
                    }
                    else
                    {
                        
                        //if (_DBContext.ADEMAIL_GROUPMST.Any(x => x.GROUP_NAME.ToUpper() == model.GROUP_NAME.ToUpper()))
                        var flag = _DBContext.ADEMAIL_GROUPMST
                                 .AsEnumerable()
                                 .Any(x => x.GROUP_NAME.Equals(model.GROUP_NAME, StringComparison.OrdinalIgnoreCase));

                        if (flag)
                            {
                            retVal = 2;
                            return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                        }

                        DPH = new ADEMAIL_GROUPMST();
                        if (_DBContext.ADEMAIL_GROUPMST.Count() == 0)
                        {
                            DPH.EMAILGROUPMSTID = 1;
                        }
                        else
                        {
                            DPH.EMAILGROUPMSTID = _DBContext.ADEMAIL_GROUPMST.Max(x => x.EMAILGROUPMSTID) + 1;
                        }
                        FlagAdd = 1;
                    }

                    DPH.GROUP_TYPE = model.GROUP_TYPE;
                    DPH.GROUP_NAME = model.GROUP_NAME;
                    DPH.STATUS = model.STATUS==1 ? (short)1 : (short)0;
                    DPH.REMARK = model.REMARK;
                    DateTime _date = DateTime.Now;
                    if (FlagAdd == 1)
                    {
                        DPH.ADDED_BY = model.ADDED_BY;
                        DPH.ADDED_DATE = _date;
                    }
                    else
                    {
                        DPH.UPDATED_BY = model.UPDATED_BY;
                        DPH.UPDATED_DATE = _date;
                    }
                    _DBContext.Entry(DPH).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _DBContext.SaveChanges();

                    //// --- Save Group Mapping --- ////
                    string groupValues = SaveGroupMapping(DPH.EMAILGROUPMSTID, _date, model.ADDED_BY, model.GROUP_VALUE_ARRAY);

                    //// --- Save History --- ////
                    SaveHistory(DPH.EMAILGROUPMSTID, _date, model, groupValues);

                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.EMAILGROUPMSTID;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
                return _retVal_tuple;
            }
            return null;
        }

        public string SaveGroupMapping(long EmailGroupMstId, DateTime AddedDate, long? AddedBy, long[]? GroupArray)
        {
            string GroupValues = "";
            if (EmailGroupMstId > 0)
            {
                List<ADEMAIL_GROUPMAPPING> DeleteList = _DBContext.ADEMAIL_GROUPMAPPING.Where(x => x.EMAILGROUPMSTID == EmailGroupMstId).ToList();
                if (DeleteList.Count > 0)
                {
                    _DBContext.ADEMAIL_GROUPMAPPING.RemoveRange(DeleteList);
                    _DBContext.SaveChanges();
                }
            }

            if (GroupArray != null)
            {
                foreach (long _GroupVal in GroupArray)
                {
                    GroupValues += _GroupVal.ToString() + ",";

                    ADEMAIL_GROUPMAPPING DPAH = new ADEMAIL_GROUPMAPPING();
                    if (_DBContext.ADEMAIL_GROUPMAPPING.Count() == 0)
                    {
                        DPAH.EMAILGROUPMAPID = 1;
                    }
                    else
                    {
                        DPAH.EMAILGROUPMAPID = _DBContext.ADEMAIL_GROUPMAPPING.Max(x => x.EMAILGROUPMAPID) + 1;
                    }
                    DPAH.EMAILGROUPMSTID = EmailGroupMstId;
                    DPAH.GROUP_VALUE = _GroupVal;
                    DPAH.STATUS = 1;
                    DPAH.ADDED_BY = AddedBy;
                    DPAH.ADDED_DATE = AddedDate;
                    _DBContext.Entry(DPAH).State = EntityState.Added;
                    _DBContext.SaveChanges();
                }
            }
            return GroupValues;
        }

        public void SaveHistory(long ActionId, DateTime ActionDate, EmailGroupViewModel model, string GroupValues)
        {
            ADEMAIL_GROUPHISTORY DPAH = new ADEMAIL_GROUPHISTORY();
            if (_DBContext.ADEMAIL_GROUPHISTORY.Count() == 0)
            {
                DPAH.EMAILGROUPHISID = 1;
            }
            else
            {
                DPAH.EMAILGROUPHISID = _DBContext.ADEMAIL_GROUPHISTORY.Max(x => x.EMAILGROUPHISID) + 1;
            }
            DPAH.ACTIONID = ActionId;
            DPAH.GROUP_TYPE = model.GROUP_TYPE;
            DPAH.GROUP_NAME = model.GROUP_NAME;
            DPAH.GROUP_VALUE = GroupValues;
            DPAH.REMARK = model.REMARK;
            DPAH.ACTION_BY = model.ADDED_BY;
            DPAH.ACTION_DATE = ActionDate;
            _DBContext.Entry(DPAH).State = EntityState.Added;
            _DBContext.SaveChanges();
        }

        public EmailGroupViewModel GetDetailById(long id)
        {
            var _obj = (from data in _DBContext.ADEMAIL_GROUPMST.Where(x => x.EMAILGROUPMSTID == id)
                        join _AddBy in _DBContext.ADEMPLOYEE on data.ADDED_BY equals _AddBy.ADEMPCODE
                        join _VWAssociate in _DBContext.VW_ASSOCIATELVLDETAILS on data.ADDED_BY equals _VWAssociate.ADEMPCODE
                        where _VWAssociate.SYKI == _Syki.SYKIID
                        select new EmailGroupViewModel
                        {
                            EMAILGROUPMSTID = data.EMAILGROUPMSTID,
                            GROUP_TYPE = data.GROUP_TYPE,
                            GROUP_NAME = data.GROUP_NAME,
                            // STATUS = data.STATUS == 1 ? true : false,
                            STATUS = data.STATUS,
                            REMARK = data.REMARK,
                            ADDED_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            ADDED_BY = data.ADDED_BY,
                            ADDED_DATE = data.ADDED_DATE,
                        }).FirstOrDefault();
            if (_obj.GROUP_TYPE == 2)
            {
                _obj.EmailMapp_List = (from gm in _DBContext.ADEMAIL_GROUPMAPPING
                                       join _location in _DBContext.SYSITE on gm.GROUP_VALUE equals _location.SYSITEID
                                       where gm.EMAILGROUPMSTID == _obj.EMAILGROUPMSTID
                                       && gm.STATUS == 1
                                       select new EmailGroupMappViewModel
                                       {
                                           EMAILGROUPMAPID = gm.EMAILGROUPMAPID,
                                           EMAILGROUPMSTID = gm.EMAILGROUPMSTID,
                                           GROUP_VALUE = gm.GROUP_VALUE,
                                           GROUP_MAPP_NAME = _location.DESCRIP,
                                       }).ToList();
                _obj.GROUP_VALUE_ARRAY = _obj.EmailMapp_List.Select(s => (long)s.GROUP_VALUE).ToArray();
            }
            else if (_obj.GROUP_TYPE == 3)
            {
                _obj.EmailMapp_List = (from gm in _DBContext.ADEMAIL_GROUPMAPPING
                                       join _OP in _DBContext.ADORGLEVEL on gm.GROUP_VALUE equals _OP.ADORGLEVELID
                                       where gm.EMAILGROUPMSTID == _obj.EMAILGROUPMSTID
                                       && gm.STATUS == 1
                                       select new EmailGroupMappViewModel
                                       {
                                           EMAILGROUPMAPID = gm.EMAILGROUPMAPID,
                                           EMAILGROUPMSTID = gm.EMAILGROUPMSTID,
                                           GROUP_VALUE = gm.GROUP_VALUE,
                                           GROUP_MAPP_NAME = _OP.LEVELDESCRIP,
                                       }).ToList();
                _obj.GROUP_VALUE_ARRAY = _obj.EmailMapp_List.Select(s => (long)s.GROUP_VALUE).ToArray();
            }
            return _obj;
        }
    }
}
