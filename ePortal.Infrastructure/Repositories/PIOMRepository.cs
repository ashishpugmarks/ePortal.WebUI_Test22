using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spire.Pdf.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Infrastructure.Repositories
{
    public class PIOMRepository
    {
        private readonly EPortalDBContext _dbContext;
        private readonly ICProcessBDContext _icDbContext;
        private readonly SYKI _syki;
        private readonly ILogger<PIOMRepository> _logger;

        public PIOMRepository(EPortalDBContext dbContext, ICProcessBDContext icDbContext, ILogger<PIOMRepository> logger)
        {
            _dbContext = dbContext;
            _icDbContext = icDbContext;
            _syki = _dbContext.SYKI.FirstOrDefault(x => x.ACTIVE == 1);
            _logger = logger;
        }

        public Tuple<short, long> SavePIOMRequest(IOMPHeaderViewModel model)
        {
            short retVal = 0;
            long retHeaderId = 0;

            // --- Flatten all approvers for validation ---
            var allApprovers = model.Stages?
                .SelectMany(s => s.ParallelGroups)
                .SelectMany(pg => pg.Approvers)
                .ToList() ?? new List<IOMPAppAuthSeqViewModel>();

            // --- Director Validation ---
            var directorDesgList = _dbContext.SYPARAMETERS
                .Where(m => m.PARAMNAME == "APPNOTE_DIRECTOR_DESG")
                .ToList();

            string directorDesgIds = "";
            if ((model.APP_TYPE == 1 && allApprovers.Any(m => m.Header == "Director" || m.Header == "Senior Director"))
                || model.APP_TYPE == 2)
            {
                if (directorDesgList.Any())
                    directorDesgIds = directorDesgList.First().PARAMVALUE;
            }

            if (allApprovers.Any() && !string.IsNullOrEmpty(directorDesgIds))
            {
                long[] desgIds = Array.ConvertAll(directorDesgIds.Split(','), long.Parse);
                long[] empCodes = model.APP_TYPE == 1
                    ? allApprovers.Where(m => m.Header == "Director" || m.Header == "Senior Director")
                                  .Select(n => n.ADEMPCODE).ToArray()
                    : allApprovers.Select(n => n.ADEMPCODE).ToArray();

                var directorRecord = _dbContext.VW_ASSOCIATELVLDETAILS
                    .Where(d => d.SYKI == _syki.SYKIID && d.ACTIVE == 1 && empCodes.Contains(d.ADEMPCODE))
                    .Select(d => new { d.ADEMPCODE, ADDESIGNATIONID = (long)d.ADDESIGNATIONID, d.OPERATIONID })
                    .Where(d => desgIds.Contains(d.ADDESIGNATIONID))
                    .ToList();

                foreach (var direc in directorRecord)
                {
                    var operDir = _dbContext.VW_ASSOCIATELVLDETAILS
                        .Where(m => m.SYKI == _syki.SYKIID
                                 && m.ACTIVE == 1
                                 && m.OPERATIONID == direc.OPERATIONID
                                 && (m.ADDESIGNATIONID == 27 || m.ADDESIGNATIONID == 34))
                        .ToList();

                    long operDirCnt = directorRecord.Count(m => m.OPERATIONID == direc.OPERATIONID);
                    if (operDir.Count != operDirCnt)
                        return new Tuple<short, long>(2, direc.ADEMPCODE);
                }
            }

            // --- Transaction Start ---
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                // --- Save Header ---
                DGIT_PIOMHEADER header = SaveOrUpdateHeader(model, out int flagAdd);

                // --- Save Details ---
                if (model.iompDetail?.Any() == true)
                {
                    SaveIOMDetails(model.ADDEDBY, header.IOMHEADERID, model.iompDetail);
                }

                // --- Save Stages → Groups → Approvers ---
                if (model.Stages?.Any() == true)
                {
                    if (model.IOMHEADERID > 0)
                    {
                        DeleteExistingStages(model.IOMHEADERID);
                    }
                    foreach (var stage in model.Stages)
                    {
                        long stageId = SaveStage(model.ADDEDBY, header.IOMHEADERID, stage, 1);

                        foreach (var group in stage.ParallelGroups)
                        {
                            long groupId = SaveParallelGroup(model.ADDEDBY, stageId, group);

                            // --- Sequential Approvers ---
                            if (group.Approvers?.Any() == true)
                            {
                                SaveSequentialApprovers(model.ADDEDBY, header.IOMHEADERID, groupId, group.Approvers, stage.StageOrder);
                            }

                            // --- Optional: Group-level headers ---
                            if (group.Headers?.Any() == true)
                            {
                                SaveAppHeader(model.ADDEDBY, header.IOMHEADERID, group.Headers);
                            }
                        }
                    }
                }

                transaction.Commit();
                retVal = 1;
                retHeaderId = header.IOMHEADERID;
            }
            catch (Exception)
            {
                transaction.Rollback();
                retVal = -1;
            }

            return new Tuple<short, long>(retVal, retHeaderId);
        }

        private DGIT_PIOMHEADER SaveOrUpdateHeader(IOMPHeaderViewModel model, out int flagAdd)
        {
            DGIT_PIOMHEADER header;
            flagAdd = 0;

            if (model.IOMHEADERID > 0)
            {
                header = _dbContext.DGIT_PIOMHEADER.SingleOrDefault(x => x.IOMHEADERID == model.IOMHEADERID);
            }
            else
            {
                header = new DGIT_PIOMHEADER();
                if (_dbContext.DGIT_PIOMHEADER.Count() == 0)
                {
                    header.IOMHEADERID = 1;
                }
                else
                {
                    header.IOMHEADERID = _dbContext.DGIT_PIOMHEADER.Max(x => x.IOMHEADERID) + 1;
                }
                flagAdd = 1;


            }

            header.IOM_DESC = model.IOMDesc;
            header.PROCESS_STATUS = model.PROCESS_STATUS;
            header.STATUS = model.STATUS;
            header.APP_TYPE = model.APP_TYPE;
            header.ISEDITABLE = 1;

            if (flagAdd == 1)
            {
                header.ADDEDBY = model.ADDEDBY;
                header.DATEADDED = DateTime.Now;
                _dbContext.Entry(header).State = EntityState.Added;
            }
            else
            {
                header.UPDATEDBY = model.UPDATEDBY;
                header.UPDATEDATE = DateTime.Now;
                _dbContext.Entry(header).State = EntityState.Modified;
            }

            _dbContext.SaveChanges();
            return header;
        }
        private long SaveStage(long addedBy, long iomId, IOMStageViewModel stage, short AdditionalType = 1)
        {
            try
            {
                DGIT_PIOM_STAGE dbStage = new()
                {
                    STAGE_ID = _dbContext.DGIT_PIOM_STAGE.Count() > 0
                        ? _dbContext.DGIT_PIOM_STAGE.Max(x => x.STAGE_ID) + 1
                        : 1,
                    IOMHEADERID = iomId,
                    STAGE_NAME = stage.StageName,
                    STAGE_ORDER = stage.StageOrder,
                    STATUS = 1,
                    ADDEDBY = addedBy,
                    ADDITIONALTYPE = AdditionalType,
                    DATEADDED = DateTime.Now
                };

                _dbContext.DGIT_PIOM_STAGE.Add(dbStage);
                _dbContext.SaveChanges();

                return dbStage.STAGE_ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private long SaveParallelGroup(long addedBy, long stageId, IOMParallelGroupViewModel group)
        {



            DGIT_PIOM_SEQ_APPROVAL dbGroup = new()
            {
                PARALLEL_GROUP_ID = _dbContext.DGIT_PIOM_SEQ_APPROVAL.Count() > 0
                    ? _dbContext.DGIT_PIOM_SEQ_APPROVAL.Max(x => x.PARALLEL_GROUP_ID) + 1
                    : 1,
                STAGE_ID = stageId,
                GROUP_NAME = group.GroupName,
                STATUS = 1,
                ADDEDBY = addedBy,
                DATEADDED = DateTime.Now
            };

            _dbContext.DGIT_PIOM_SEQ_APPROVAL.Add(dbGroup);
            _dbContext.SaveChanges();

            return dbGroup.PARALLEL_GROUP_ID;
        }
        private void SaveSequentialApprovers(long addedBy, long headerId, long groupId, List<IOMPAppAuthSeqViewModel> approvers, int stageOrder)
        {
            try
            {
                // Delete existing approvers for this header + group
                var lastId = _dbContext.DGIT_PIOMAPPAUTHSEQ.Count() > 0
        ? _dbContext.DGIT_PIOMAPPAUTHSEQ.Max(x => x.IOMAPPAUTH_ID)
        : 1;

                foreach (var appr in approvers.OrderBy(a => a.APP_SEQ))
                {
                    lastId++;
                    DGIT_PIOMAPPAUTHSEQ entity = new()
                    {

                        IOMAPPAUTH_ID = lastId,
                        IOMHEADERID = headerId,
                        PARALLEL_GROUP_ID = groupId,
                        IOMAPPHEADER = appr.Header,
                        ADEMPCODE = appr.ADEMPCODE,
                        APP_SEQ = appr.APP_SEQ,
                        APPTYPE = appr.APPTYPE,
                        STATUS = 1,
                        ADDEDBY = addedBy,
                        ADDEDDATE = DateTime.Now
                    };

                    _dbContext.DGIT_PIOMAPPAUTHSEQ.Add(entity);
                    _dbContext.SaveChanges();
                    if (stageOrder == 0 && appr.APP_SEQ == 0)
                    {
                        SaveIOMAppHis(addedBy, headerId, appr, entity.IOMAPPAUTH_ID, groupId);
                    }

                }


            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        public short SaveIOMDetails(long addedBy, long headerId, List<IOMPDetailViewModel> details)
        {

            short retVal = 0;
            foreach (IOMPDetailViewModel PDVM in details)
            {
                DGIT_PIOMDETAIL DPD = new DGIT_PIOMDETAIL();
                int FlagAdd = 0;
                if (headerId > 0)
                {
                    if (PDVM.DOC_TYPE == "IOM")
                    {
                        DPD = _dbContext.DGIT_PIOMDETAIL.Where(x => x.IOMHEADERID == headerId && x.DOC_TYPE == PDVM.DOC_TYPE).FirstOrDefault();
                        if (DPD != null)
                        {
                            _dbContext.DGIT_PIOMDETAIL.Remove(DPD);
                            _dbContext.SaveChanges();
                        }
                    }

                    DPD = new DGIT_PIOMDETAIL();
                    if (_dbContext.DGIT_PIOMDETAIL.Count() == 0)
                    {
                        DPD.IOMDTL_ID = 1;
                    }
                    else
                    {
                        DPD.IOMDTL_ID = _dbContext.DGIT_PIOMDETAIL.Max(x => x.IOMDTL_ID) + 1;
                    }
                    FlagAdd = 1;

                    DPD.IOMHEADERID = headerId;
                    DPD.DOC_TYPE = PDVM.DOC_TYPE;
                    DPD.FILENAME = PDVM.FILENAME;
                    DPD.ADDITIONAL_INFO = PDVM.ADDITIONAL_INFO;
                    DPD.STATUS = 1;
                    if (FlagAdd == 1)
                    {
                        DPD.ADDEDBY = addedBy;
                        DPD.ADDEDDATE = DateTime.Now;
                    }
                    else
                    {
                        DPD.UPDATEDBY = addedBy;
                        DPD.UPDATEDATE = DateTime.Now;
                    }
                    _dbContext.Entry(DPD).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _dbContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;

        }

        public void SaveAppHeader(long addedBy, long headerId, List<IOMPAppHeaderViewModel> headers)
        {
            // Delete existing headers for this IOM
            var existing = _dbContext.DGIT_PIOMAPPHEADER
                .Where(x => x.IOMHEADERID == headerId)
                .ToList();

            if (existing.Any())
            {
                _dbContext.DGIT_PIOMAPPHEADER.RemoveRange(existing);
                _dbContext.SaveChanges();
            }

            foreach (var h in headers.OrderBy(x => x.Seq_Order))
            {
                DGIT_PIOMAPPHEADER entity = new()
                {
                    IOMAPPHEADERID = _dbContext.DGIT_PIOMAPPHEADER.Any()
                        ? _dbContext.DGIT_PIOMAPPHEADER.Max(x => x.IOMAPPHEADERID) + 1
                        : 1,
                    IOMHEADERID = headerId,
                    IOMAPPHEADER = h.IOMAPPHEADER,
                    APPHEADERDESC = h.APPHEADERDESC,
                    STATUS = 1,
                    ADDEDBY = addedBy,
                    ADDEDDATE = DateTime.Now
                };

                _dbContext.DGIT_PIOMAPPHEADER.Add(entity);
            }

            _dbContext.SaveChanges();
        }
        public void SaveIOMAppHis(long addedBy, long iomHeaderId, IOMPAppAuthSeqViewModel approver, long seqid, long parallelGroupId)
        {
            try
            {
                // Check if this approver is part of the authorization sequence
                int approverCount = _dbContext.DGIT_PIOMAPPAUTHSEQ
          .Count(x => x.IOMHEADERID == iomHeaderId
                   && x.ADEMPCODE == approver.ADEMPCODE
                   && x.PARALLEL_GROUP_ID == parallelGroupId);

                bool isValidApprover = approverCount > 0;

                if (!isValidApprover)
                    return; // approver not part of this IOM sequence → skip

                // Check if history record already exists (pending status)
                var existing = _dbContext.DGIT_PIOMAPPHISTORY
                    .FirstOrDefault(x => x.IOMHEADERID == iomHeaderId
                                      && x.ADEMPCODE == approver.ADEMPCODE

                                      && x.APPROVAL_STATUS == 0);

                bool isNew = existing == null;
                var entity = existing ?? new DGIT_PIOMAPPHISTORY();

                if (isNew)
                {
                    entity.IOMAPPHISTORY_ID = _dbContext.DGIT_PIOMAPPHISTORY.Count() > 0
                        ? _dbContext.DGIT_PIOMAPPHISTORY.Max(x => x.IOMAPPHISTORY_ID) + 1
                        : 1;

                    entity.IOMHEADERID = iomHeaderId;
                    entity.ADEMPCODE = approver.ADEMPCODE;
                    entity.APPTYPE = approver.APPTYPE;
                    entity.APPROVAL_STATUS = 0;
                    entity.APPROVAL_REMARK = string.Empty;
                    entity.PIOMAPPAUTHSEQ_ID = seqid;
                    entity.ADDEDBY = addedBy;
                    entity.ADDEDDATE = DateTime.Now;

                    _dbContext.DGIT_PIOMAPPHISTORY.Add(entity);
                }
                else
                {
                    entity.UPDATEBY = addedBy;
                    entity.UPDATEDATE = DateTime.Now;
                    _dbContext.Entry(entity).State = EntityState.Modified;
                }

                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        // Used in update flow to delete all existing stages, groups, and approvers before saving new data.
        private void DeleteExistingStages(long headerId)
        {
            // Step 0: Delete history records for approvers (if any, e.g., from SaveIOMAppHis)
            var approverIds = _dbContext.DGIT_PIOMAPPAUTHSEQ
                .Where(x => x.IOMHEADERID == headerId)
                .Select(x => x.IOMAPPAUTH_ID)
                .ToList();
            if (approverIds.Any())
            {
                var historyToDelete = _dbContext.DGIT_PIOMAPPHISTORY
                    .Where(h => approverIds.Contains(h.PIOMAPPAUTHSEQ_ID));
                _dbContext.DGIT_PIOMAPPHISTORY.RemoveRange(historyToDelete);
                _dbContext.SaveChanges();
            }

            // Step 1: Delete approvers
            var approversToDelete = _dbContext.DGIT_PIOMAPPAUTHSEQ.Where(x => x.IOMHEADERID == headerId);
            _dbContext.DGIT_PIOMAPPAUTHSEQ.RemoveRange(approversToDelete);
            _dbContext.SaveChanges();

            // Step 2: Delete groups
            var stageIdsToDelete = _dbContext.DGIT_PIOM_STAGE.Where(s => s.IOMHEADERID == headerId).Select(s => s.STAGE_ID).ToList();
            if (stageIdsToDelete.Any())
            {
                var groupsToDelete = _dbContext.DGIT_PIOM_SEQ_APPROVAL.Where(g => stageIdsToDelete.Contains(g.STAGE_ID));
                _dbContext.DGIT_PIOM_SEQ_APPROVAL.RemoveRange(groupsToDelete);
                _dbContext.SaveChanges();
            }

            // Step 3: Delete stages
            var stagesToDelete = _dbContext.DGIT_PIOM_STAGE.Where(s => s.IOMHEADERID == headerId);
            _dbContext.DGIT_PIOM_STAGE.RemoveRange(stagesToDelete);
            _dbContext.SaveChanges();
        }

        public IOMPHeaderViewModel GetPIOMRequestById(long id)
        {
            var _obj = (from data in _dbContext.DGIT_PIOMHEADER.Where(x => x.IOMHEADERID == id)
                        join emp in _dbContext.ADEMPLOYEE on data.ADDEDBY equals emp.ADEMPCODE
                        join _VWA in _dbContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _syki.SYKIID) on data.ADDEDBY equals _VWA.ADEMPCODE into _vwdt
                        from _VWAssociate in _vwdt.DefaultIfEmpty()
                        join cat in _dbContext.DGIT_IOMCATMST on data.IOMCATID equals cat.IOMCATMSTID into catLeft
                        from catData in catLeft.DefaultIfEmpty()
                        select new IOMPHeaderViewModel
                        {
                            IOMHEADERID = data.IOMHEADERID,
                            IOMDesc = data.IOM_DESC,
                            PROCESS_STATUS = data.PROCESS_STATUS,
                            APP_TYPE = data.APP_TYPE,
                            STATUS = data.STATUS,
                            ADDEDBY = data.ADDEDBY,
                            ISEDITABLE = data.ISEDITABLE,
                            ADDEDBYNAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            Emp_Detail = new Employee_Details
                            {
                                _ECode = emp.ADEMPCODE,
                                _EName = emp.FIRSTNAME + " " + emp.LASTNAME,
                                _EmailId = emp.EMAILID ?? "",
                                _DOB = DateTime.Now,
                                _SecDescrip = _VWAssociate.SECTION,
                                _DepDesc = _VWAssociate.DEPARTMENT,
                                _DivDesc = _VWAssociate.DIVISION,
                                _OpDesc = _VWAssociate.OPERATION,
                                _SiteId = _VWAssociate.SYSITEID
                            }

                        }).FirstOrDefault();

            if (_obj == null)
                return null;

            // --- Load Details ---
            _obj.iompDetail = _dbContext.DGIT_PIOMDETAIL
                .Where(d => d.IOMHEADERID == _obj.IOMHEADERID && d.STATUS == 1)
                .Select(d => new IOMPDetailViewModel
                {
                    IOMDTL_ID = d.IOMDTL_ID,
                    IOMHEADERID = d.IOMHEADERID,
                    DOC_TYPE = d.DOC_TYPE,
                    FILENAME = d.FILENAME,
                    ADDITIONAL_INFO = d.ADDITIONAL_INFO,
                    ADDEDBY = d.ADDEDBY,
                    ADDEDDATE = d.ADDEDDATE
                }).ToList();

            // --- Load Stages ---
            var stages = _dbContext.DGIT_PIOM_STAGE
                .Where(s => s.IOMHEADERID == _obj.IOMHEADERID)
                .OrderBy(s => s.STAGE_ORDER)
                .ToList();

            _obj.Stages = new List<IOMStageViewModel>();

            foreach (var stage in stages)
            {
                var stageVm = new IOMStageViewModel
                {
                    StageID = stage.STAGE_ID,
                    StageName = stage.STAGE_NAME,
                    StageOrder = stage.STAGE_ORDER,
                    STATUS = stage.STATUS,
                    AdditinalType = stage.ADDITIONALTYPE,
                    ParallelGroups = new List<IOMParallelGroupViewModel>()
                };

                // --- Load Parallel Groups for this stage ---
                var groups = _dbContext.DGIT_PIOM_SEQ_APPROVAL
                    .Where(g => g.STAGE_ID == stage.STAGE_ID)
                    .ToList();

                foreach (var grp in groups)
                {
                    var groupVm = new IOMParallelGroupViewModel
                    {
                        ParallelGroupID = grp.PARALLEL_GROUP_ID,
                        GroupName = grp.GROUP_NAME,
                        STATUS = grp.STATUS,
                        Approvers = new List<IOMPAppAuthSeqViewModel>()
                    };

                    // --- Load Sequential Approvers ---
                    var approverData = (from auth in _dbContext.DGIT_PIOMAPPAUTHSEQ
                                        join _VWD in _dbContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _syki.SYKIID)
                                            on auth.ADEMPCODE equals _VWD.ADEMPCODE into _VWTMP
                                        from _Vw in _VWTMP.DefaultIfEmpty()
                                        join aemp in _dbContext.ADEMPLOYEE on auth.ADEMPCODE equals aemp.ADEMPCODE
                                        join desg in _dbContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals desg.ADDESIGNATIONID into desgTmp
                                        from desgData in desgTmp.DefaultIfEmpty()
                                        where auth.IOMHEADERID == _obj.IOMHEADERID &&
                                              auth.PARALLEL_GROUP_ID == grp.PARALLEL_GROUP_ID
                                        orderby auth.APP_SEQ
                                        select new
                                        {
                                            auth.IOMAPPAUTH_ID,
                                            auth.ADEMPCODE,
                                            aemp.FIRSTNAME,
                                            aemp.LASTNAME,
                                            Desg = desgData.DESCRIP,
                                            auth.APP_SEQ,
                                            auth.APPTYPE,
                                            auth.ADDEDDATE,
                                            auth.IOMAPPHEADER
                                        }).ToList();
                    var approvers = approverData.Select(x => new IOMPAppAuthSeqViewModel
                    {
                        IOMAPPAUTH_ID = x.IOMAPPAUTH_ID,
                        ADEMPCODE = x.ADEMPCODE,
                        ADEMPNAME = $"{x.FIRSTNAME} {x.LASTNAME}", // C# concatenation (safe)
                        ADDESIGNATION = string.IsNullOrEmpty(x.Desg) ? "(Inactive)" : x.Desg,
                        APP_SEQ = x.APP_SEQ,
                        APPTYPE = x.APPTYPE,
                        ADDEDDATE = x.ADDEDDATE,
                        Header = x.IOMAPPHEADER
                    }).ToList();
                    groupVm.Approvers.AddRange(approvers);

                    // --- Load Approval Headers if exist ---
                    var headers = _dbContext.DGIT_PIOMAPPHEADER
                        .Where(h => h.IOMHEADERID == _obj.IOMHEADERID)
                        .OrderBy(h => h.IOMAPPHEADERID)
                        .Select(h => new IOMPAppHeaderViewModel
                        {
                            IOMAPPHEADERID = h.IOMAPPHEADERID,
                            IOMHEADERID = h.IOMHEADERID,
                            IOMAPPHEADER = h.IOMAPPHEADER,
                            APPHEADERDESC = h.APPHEADERDESC,
                            STATUS = h.STATUS
                        }).ToList();

                    groupVm.Headers = headers;

                    stageVm.ParallelGroups.Add(groupVm);
                }

                _obj.Stages.Add(stageVm);
            }

            var iomHistoryList = _dbContext.DGIT_PIOMAPPHISTORY.ToList();

            // --- Load Approval History ---
            _obj.iompAppHis = (from hist in _dbContext.DGIT_PIOMAPPHISTORY.Where(x => x.IOMHEADERID == _obj.IOMHEADERID)
                               join emp in _dbContext.ADEMPLOYEE on hist.ADEMPCODE equals emp.ADEMPCODE
                               select new IOMPAppHistoryViewModel
                               {

                                   IOMAPPHISTORY_ID = hist.IOMAPPHISTORY_ID,
                                   IOMID = hist.IOMHEADERID,
                                   EmpCode = hist.ADEMPCODE,
                                   ApprovalStatus = hist.APPROVAL_STATUS,
                                   ApprovalRemark = hist.APPROVAL_REMARK,
                                   EmpName = emp.FIRSTNAME + " " + emp.LASTNAME + " [" + emp.ADEMPCODE + "]",
                                   Email = emp.EMAILID ?? "",
                                   AddedDate = hist.ADDEDDATE,
                                   UpdatedBy = hist.UPDATEBY,
                                   UpdatedDate = hist.UPDATEDATE,
                                   ApprovalDate = hist.APP_DATE,
                                   Seq_id = hist.PIOMAPPAUTHSEQ_ID,
                                   AppType = hist.APPTYPE,

                               }).OrderBy(x => x.IOMAPPHISTORY_ID).ToList();

            // --- Parallel Flow Logic: Handle Send-Backs, Enrich History, and Populate StageHis ---
            // (Fixed: Removed assignments to read-only properties IsGroupCompleted/IsStageCompleted)

            long lastSendBackAppHisId = 0;
            var lastSendBackHistory = _obj.iompAppHis
                .Where(h => h.IOMID == _obj.IOMHEADERID && h.ApprovalStatus == 2)  // Status 2 = send-back
                .OrderByDescending(h => h.IOMAPPHISTORY_ID)
                .FirstOrDefault();
            if (lastSendBackHistory != null)
            {
                lastSendBackAppHisId = lastSendBackHistory.IOMAPPHISTORY_ID;
            }

            // Step 2: Helper dict with ValueTuple key (type-safe)
            var historyDict = _obj.iompAppHis
                .GroupBy(h => (h.EmpCode, h.Seq_id))  // Tuple: (EmpCode, Seq_id)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.IOMAPPHISTORY_ID).ToList());

            // Step 3: Process Stages Sequentially, Enrich & Populate StageHis
            _obj.StageHis = new List<IOMStageViewModel>();
            foreach (var stage in _obj.Stages.OrderBy(s => s.StageOrder))
            {
                var stageHis = new IOMStageViewModel
                {
                    StageID = stage.StageID,
                    StageName = stage.StageName,
                    StageOrder = stage.StageOrder,
                    AdditinalType = stage.AdditinalType,
                    ParallelGroups = new List<IOMParallelGroupViewModel>(stage.ParallelGroups.Select(pg => new IOMParallelGroupViewModel  // Shallow copy for safety
                    {
                        ParallelGroupID = pg.ParallelGroupID,
                        GroupName = pg.GroupName,
                        GroupOrder = pg.GroupOrder,
                        STATUS = pg.STATUS,
                        Approvers = new List<IOMPAppAuthSeqViewModel>(pg.Approvers),  // Copy approvers
                        Headers = pg.Headers  // Reference ok if read-only
                    })),
                    StageHistory = new List<IOMPAppHistoryViewModel>()
                };

                bool stageHasSendBack = false;
                bool isStageCompleted = true;  // Temp var for logic, but not assigning to property

                // Inner: Parallel Groups (independent)
                foreach (var group in stageHis.ParallelGroups.OrderBy(g => g.GroupOrder))
                {
                    bool isGroupCompleted = true;  // Temp var for logic

                    // Inner: Sequential Approvers
                    foreach (var approver in group.Approvers.OrderBy(a => a.APP_SEQ))
                    {
                        // Check before send-back record
                        bool isBeforeSendBackRecord = false;
                        var approverKey = (approver.ADEMPCODE, approver.IOMAPPAUTH_ID);  // Tuple key
                        List<IOMPAppHistoryViewModel> approverHistory = null;
                        if (historyDict.TryGetValue(approverKey, out approverHistory))
                        {
                            isBeforeSendBackRecord = approverHistory.Any(h => h.IOMAPPHISTORY_ID <= lastSendBackAppHisId);
                            if (approverHistory.Any(h => h.ApprovalStatus == 2)) stageHasSendBack = true;
                        }

                        // Main condition for adding pending
                        bool shouldAddPending = false;
                        if (isBeforeSendBackRecord)
                        {
                            shouldAddPending = approverHistory == null || !approverHistory.Any(h => h.IOMAPPHISTORY_ID > lastSendBackAppHisId);
                        }
                        else
                        {
                            shouldAddPending = approverHistory == null || !approverHistory.Any();
                        }

                        if (shouldAddPending)
                        {
                            var dummyEntry = new IOMPAppHistoryViewModel
                            {
                                IOMAPPHISTORY_ID = 0,
                                IOMID = _obj.IOMHEADERID,
                                Seq_id = approver.IOMAPPAUTH_ID,
                                EmpCode = approver.ADEMPCODE,
                                EmpName = approver.ADEMPNAME + $" [{approver.ADEMPCODE}]",
                                ApprovalStatus = 0,
                                ApprovalRemark = "",
                                AddedDate = DateTime.Now,
                                AppType = approver.APPTYPE,
                                Header = approver.Header,
                                IsGroupCompleted = false,
                                IsStageCompleted = false
                            };
                            _obj.iompAppHis.Add(dummyEntry);
                            stageHis.StageHistory.Add(dummyEntry);
                        }

                        // Update status
                        short currentStatus = approverHistory?.FirstOrDefault()?.ApprovalStatus ?? 0;
                        approver.STATUS = currentStatus;
                        if (currentStatus != 1) isGroupCompleted = false;

                        // Reset next if send-back
                        if (currentStatus == 2)
                        {
                            var nextIndex = group.Approvers.IndexOf(approver);
                            for (int i = nextIndex + 1; i < group.Approvers.Count; i++)
                            {
                                group.Approvers[i].STATUS = 0;
                            }
                            isGroupCompleted = false;
                            stageHasSendBack = true;
                        }

                        // Add real history to stage (inside loop, scope safe)
                        if (approverHistory != null)
                        {
                            stageHis.StageHistory.AddRange(approverHistory);
                        }
                    }

                    // No assignment to IsGroupCompleted (read-only, auto-computes from Approvers.STATUS)
                    // Use temp var isGroupCompleted for logic only
                    if (!isGroupCompleted) isStageCompleted = false;
                }

                // No assignment to IsStageCompleted (read-only, auto-computes from ParallelGroups)
                // Use temp var isStageCompleted for logic only

                if (stageHasSendBack) stageHis.STATUS = 2;  // Flag for UI

                _obj.StageHis.Add(stageHis);
            }

            // Optional: Update overall status (this uses the computed properties)
            //_obj.PROCESS_STATUS = _obj.Stages.All(s => s.IsStageCompleted) ? (short)1 : (short)0;

            // Sort global history
            _obj.iompAppHis = _obj.iompAppHis.OrderBy(h => h.IOMAPPHISTORY_ID).ToList();

            return _obj;
        }

        //public IOMPHeaderViewModel GetPIOMRequestById(long id)
        //{
        //    var _obj = (from data in _dbContext.DGIT_PIOMHEADER.Where(x => x.IOMHEADERID == id)
        //                join emp in _dbContext.ADEMPLOYEE on data.ADDEDBY equals emp.ADEMPCODE
        //                join _VWA in _dbContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _syki.SYKIID) on data.ADDEDBY equals _VWA.ADEMPCODE into _vwdt
        //                from _VWAssociate in _vwdt.DefaultIfEmpty()
        //                join cat in _dbContext.DGIT_IOMCATMST on data.IOMCATID equals cat.IOMCATMSTID into catLeft
        //                from catData in catLeft.DefaultIfEmpty()
        //                select new IOMPHeaderViewModel
        //                {
        //                    IOMHEADERID = data.IOMHEADERID,
        //                    IOMDesc = data.IOM_DESC,
        //                    PROCESS_STATUS = data.PROCESS_STATUS,
        //                    APP_TYPE = data.APP_TYPE,
        //                    STATUS = data.STATUS,
        //                    ADDEDBY = data.ADDEDBY,
        //                    ADDEDBYNAME = emp.FIRSTNAME + " " + emp.LASTNAME,
        //                    DATEADDED = data.DATEADDED,
        //                    Emp_Detail = new Employee_Details
        //                    {
        //                        _ECode = emp.ADEMPCODE,
        //                        _EName = emp.FIRSTNAME + " " + emp.LASTNAME,
        //                        _EmailId = emp.EMAILID ?? "",
        //                        _DOB = DateTime.Now,
        //                        _SecDescrip = _VWAssociate.SECTION,
        //                        _DepDesc = _VWAssociate.DEPARTMENT,
        //                        _DivDesc = _VWAssociate.DIVISION,
        //                        _OpDesc = _VWAssociate.OPERATION,
        //                        _SiteId = _VWAssociate.SYSITEID
        //                    }

        //                }).FirstOrDefault();

        //    if (_obj == null)
        //        return null;

        //    // --- Load Details ---
        //    _obj.iompDetail = _dbContext.DGIT_PIOMDETAIL
        //        .Where(d => d.IOMHEADERID == _obj.IOMHEADERID && d.STATUS == 1)
        //        .Select(d => new IOMPDetailViewModel
        //        {
        //            IOMDTL_ID = d.IOMDTL_ID,
        //            IOMHEADERID = d.IOMHEADERID,
        //            DOC_TYPE = d.DOC_TYPE,
        //            FILENAME = d.FILENAME,
        //            ADDITIONAL_INFO = d.ADDITIONAL_INFO,
        //            ADDEDBY = d.ADDEDBY,
        //            ADDEDDATE = d.ADDEDDATE
        //        }).ToList();

        //    // --- Load Stages ---
        //    var stages = _dbContext.DGIT_PIOM_STAGE
        //        .Where(s => s.IOMHEADERID == _obj.IOMHEADERID)
        //        .OrderBy(s => s.STAGE_ORDER)
        //        .ToList();

        //    _obj.Stages = new List<IOMStageViewModel>();

        //    foreach (var stage in stages)
        //    {
        //        var stageVm = new IOMStageViewModel
        //        {
        //            StageID = stage.STAGE_ID,
        //            StageName = stage.STAGE_NAME,
        //            StageOrder = stage.STAGE_ORDER,
        //            ParallelGroups = new List<IOMParallelGroupViewModel>()
        //        };

        //        // --- Load Parallel Groups for this stage ---
        //        var groups = _dbContext.DGIT_PIOM_SEQ_APPROVAL
        //            .Where(g => g.STAGE_ID == stage.STAGE_ID)
        //            .ToList();

        //        foreach (var grp in groups)
        //        {
        //            var groupVm = new IOMParallelGroupViewModel
        //            {
        //                ParallelGroupID = grp.PARALLEL_GROUP_ID,
        //                GroupName = grp.GROUP_NAME,
        //                Approvers = new List<IOMPAppAuthSeqViewModel>()
        //            };

        //            // --- Load Sequential Approvers ---
        //            var approverData = (from auth in _dbContext.DGIT_PIOMAPPAUTHSEQ
        //                                join _VWD in _dbContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _syki.SYKIID)
        //                                    on auth.ADEMPCODE equals _VWD.ADEMPCODE into _VWTMP
        //                                from _Vw in _VWTMP.DefaultIfEmpty()
        //                                join aemp in _dbContext.ADEMPLOYEE on auth.ADEMPCODE equals aemp.ADEMPCODE
        //                                join desg in _dbContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals desg.ADDESIGNATIONID into desgTmp
        //                                from desgData in desgTmp.DefaultIfEmpty()
        //                                where auth.IOMHEADERID == _obj.IOMHEADERID &&
        //                                      auth.PARALLEL_GROUP_ID == grp.PARALLEL_GROUP_ID
        //                                orderby auth.APP_SEQ
        //                                select new
        //                                {
        //                                    auth.IOMAPPAUTH_ID,
        //                                    auth.ADEMPCODE,
        //                                    aemp.FIRSTNAME,
        //                                    aemp.LASTNAME,
        //                                    Desg = desgData.DESCRIP,
        //                                    auth.APP_SEQ,
        //                                    auth.APPTYPE,
        //                                    auth.ADDEDDATE,
        //                                    auth.IOMAPPHEADER
        //                                }).ToList();
        //            var approvers = approverData.Select(x => new IOMPAppAuthSeqViewModel
        //            {
        //                IOMAPPAUTH_ID = x.IOMAPPAUTH_ID,
        //                ADEMPCODE = x.ADEMPCODE,
        //                ADEMPNAME = $"{x.FIRSTNAME} {x.LASTNAME}", // C# concatenation (safe)
        //                ADDESIGNATION = string.IsNullOrEmpty(x.Desg) ? "(Inactive)" : x.Desg,
        //                APP_SEQ = x.APP_SEQ,
        //                APPTYPE = x.APPTYPE,
        //                ADDEDDATE = x.ADDEDDATE,
        //                Header = x.IOMAPPHEADER
        //            }).ToList();
        //            groupVm.Approvers.AddRange(approvers);

        //            // --- Load Approval Headers if exist ---
        //            var headers = _dbContext.DGIT_PIOMAPPHEADER
        //                .Where(h => h.IOMHEADERID == _obj.IOMHEADERID)
        //                .OrderBy(h => h.IOMAPPHEADERID)
        //                .Select(h => new IOMPAppHeaderViewModel
        //                {
        //                    IOMAPPHEADERID = h.IOMAPPHEADERID,
        //                    IOMHEADERID = h.IOMHEADERID,
        //                    IOMAPPHEADER = h.IOMAPPHEADER,
        //                    APPHEADERDESC = h.APPHEADERDESC,
        //                    STATUS = h.STATUS
        //                }).ToList();

        //            groupVm.Headers = headers;

        //            stageVm.ParallelGroups.Add(groupVm);
        //        }

        //        _obj.Stages.Add(stageVm);
        //    }

        //    var iomHistoryList = _dbContext.DGIT_PIOMAPPHISTORY.ToList();

        //    // --- Load Approval History ---
        //    _obj.iompAppHis = (from hist in _dbContext.DGIT_PIOMAPPHISTORY.Where(x => x.IOMHEADERID == _obj.IOMHEADERID)
        //                       join emp in _dbContext.ADEMPLOYEE on hist.ADEMPCODE equals emp.ADEMPCODE
        //                       select new IOMPAppHistoryViewModel
        //                       {

        //                           IOMAPPHISTORY_ID = hist.IOMAPPHISTORY_ID,
        //                           IOMID = hist.IOMHEADERID,
        //                           EmpCode = hist.ADEMPCODE,
        //                           ApprovalStatus = hist.APPROVAL_STATUS,
        //                           ApprovalRemark = hist.APPROVAL_REMARK,
        //                           EmpName = emp.FIRSTNAME + " " + emp.LASTNAME + " [" + emp.ADEMPCODE + "]",
        //                           Email = emp.EMAILID ?? "",
        //                           AddedDate = hist.ADDEDDATE,
        //                           UpdatedBy = hist.UPDATEBY,
        //                           UpdatedDate = hist.UPDATEDATE,
        //                           ApprovalDate = hist.APP_DATE,
        //                           Seq_id = hist.PIOMAPPAUTHSEQ_ID,
        //                           AppType = hist.APPTYPE,

        //                       }).OrderBy(x => x.IOMAPPHISTORY_ID).ToList();

        //    return _obj;
        //}


        public List<IOMPDetailViewModel> GetAttachmentDetail(long _IOMHEADERID)
        {
            try
            {

                var data = (from _PIOMDetail in _dbContext.DGIT_PIOMDETAIL.Where(d => d.IOMHEADERID == _IOMHEADERID)
                            where _PIOMDetail.STATUS == 1
                            select new IOMPDetailViewModel
                            {
                                IOMDTL_ID = _PIOMDetail.IOMDTL_ID,
                                IOMHEADERID = _PIOMDetail.IOMHEADERID,
                                DOC_TYPE = _PIOMDetail.DOC_TYPE,
                                ADDITIONAL_INFO = _PIOMDetail.ADDITIONAL_INFO ?? "",
                                FILENAME = _PIOMDetail.FILENAME,
                            }).ToList();
                return data;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        public short DeleteAttachment(string fileName, string docType, long IOMHEADERID)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(docType) && IOMHEADERID > 0)
            {
                DGIT_PIOMDETAIL DT = _dbContext.DGIT_PIOMDETAIL.Where(x => x.FILENAME == fileName && x.DOC_TYPE == docType && x.IOMHEADERID == IOMHEADERID).FirstOrDefault();
                if (DT != null)
                {
                    _dbContext.DGIT_PIOMDETAIL.Remove(DT);
                    _dbContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public short PIOMApproval(IOMPAppHistoryViewModel PHVM, IOMPHeaderViewModel iomObj, Employee_Details emp_dtl)
        {
            short retVal = 0;

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_PIOMAPPHISTORY DPAH = new DGIT_PIOMAPPHISTORY();
                    int DPAH_Approval_Status = -1;

                    if (PHVM.IOMID > 0)
                    {

                        // Find current approval history record for user with pending or hold
                        DPAH = _dbContext.DGIT_PIOMAPPHISTORY
                            .Where(x => x.ADEMPCODE == PHVM.EmpCode && x.IOMHEADERID == PHVM.IOMID && (x.APPROVAL_STATUS == 0 || x.APPROVAL_STATUS == 5)).OrderByDescending(x => x.IOMAPPHISTORY_ID)
                            .FirstOrDefault();

                        DPAH_Approval_Status = DPAH != null ? DPAH.APPROVAL_STATUS : DPAH_Approval_Status;

                        if (DPAH != null && DPAH.APPROVAL_STATUS == 0)
                        {
                            // Update approval history
                            DPAH.APPROVAL_STATUS = PHVM.ApprovalStatus;
                            DPAH.APPROVAL_REMARK = PHVM.ApprovalRemark;
                            DPAH.UPDATEBY = PHVM.UpdatedBy;
                            DPAH.APP_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _dbContext.Entry(DPAH).State = EntityState.Modified;
                            _dbContext.SaveChanges();
                        }

                        if (DPAH != null && DPAH_Approval_Status == 5
                         && (PHVM.ApprovalStatus == 5 || PHVM.ApprovalStatus == 1 || PHVM.ApprovalStatus == 2 || PHVM.ApprovalStatus == 3))
                        {

                            var currentStage = iomObj.Stages
                                .FirstOrDefault(stage => stage.ParallelGroups
                                    .Any(pg => pg.Approvers.Any(a => a.ADEMPCODE == PHVM.EmpCode)));

                            if (currentStage != null)
                            {
                                // Find the parallel group in that stage where approver belongs
                                var currentParallelGroup = currentStage.ParallelGroups
                                    .FirstOrDefault(pg => pg.Approvers.Any(a => a.ADEMPCODE == PHVM.EmpCode));

                                if (currentParallelGroup != null)
                                {
                                    // Find the matching approver in the sequence within this parallel group
                                    var currentApprover = currentParallelGroup.Approvers
                                        .FirstOrDefault(a => a.ADEMPCODE == PHVM.EmpCode);

                                    if (currentApprover != null)
                                    {
                                        // Save approval history update for this approver
                                        SavePIOMAppHisApproval(PHVM.AddedBy, PHVM.IOMID, currentApprover, PHVM.ApprovalRemark, PHVM.ApprovalStatus);
                                    }
                                }
                            }
                        }

                        if (PHVM.ApprovalStatus == 1) // Approved
                        {
                            var authSeq = _dbContext.DGIT_PIOMAPPAUTHSEQ
                                .FirstOrDefault(x => x.IOMHEADERID == PHVM.IOMID && x.ADEMPCODE == PHVM.EmpCode);

                            if (authSeq != null)
                            {
                                authSeq.STATUS = PHVM.ApprovalStatus;
                                authSeq.UPDATEBY = PHVM.UpdatedBy;
                                authSeq.UPDATEDATE = DateTime.Now;
                                _dbContext.Entry(authSeq).State = EntityState.Modified;
                                _dbContext.SaveChanges();
                            }
                            var pendingStages = iomObj.Stages.Where(stage => stage.STATUS == 1).ToList();
                            var currentStage = pendingStages
                                .FirstOrDefault(stage => stage.ParallelGroups
                                    .Any(pg => pg.Approvers.Any(a => a.ADEMPCODE == PHVM.EmpCode)));

                            if (currentStage != null)
                            {
                                var currentParallelGroup = currentStage.ParallelGroups
                                    .FirstOrDefault(pg => pg.Approvers.Any(a => a.ADEMPCODE == PHVM.EmpCode));

                                if (currentParallelGroup != null)
                                {
                                    var approvers = currentParallelGroup.Approvers.OrderBy(a => a.APP_SEQ).ToList();
                                    var currentApproverIndex = approvers.FindIndex(a => a.ADEMPCODE == PHVM.EmpCode);

                                    if (currentApproverIndex >= 0 && currentApproverIndex < approvers.Count - 1)
                                    {
                                        var nextApprover = approvers[currentApproverIndex + 1];
                                        SaveIOMAppHis(PHVM.AddedBy, PHVM.IOMID, nextApprover, nextApprover.IOMAPPAUTH_ID, currentParallelGroup.ParallelGroupID.Value);
                                        SendMailByApprovalAuthority(PHVM, PHVM.ApprovalStatus, emp_dtl, nextApprover.IOMAPPAUTH_ID);
                                    }

                                    // ✅ Check if current parallel group is completed
                                    bool isGroupCompleted = CheckIfGroupCompleted(currentParallelGroup.ParallelGroupID.Value);

                                    currentParallelGroup.STATUS = isGroupCompleted ? (short)0 : (short)1;

                                    var dbParallelGroup = _dbContext.DGIT_PIOM_PARALLEL_GROUP
                                        .FirstOrDefault(g => g.PARALLEL_GROUP_ID == currentParallelGroup.ParallelGroupID);

                                    if (dbParallelGroup != null)
                                    {
                                        dbParallelGroup.STATUS = currentParallelGroup.STATUS;
                                        dbParallelGroup.UPDATEDATE = DateTime.Now;
                                        dbParallelGroup.UPDATEBY = PHVM.UpdatedBy ?? 0;
                                        _dbContext.Entry(dbParallelGroup).State = EntityState.Modified;
                                        _dbContext.SaveChanges();
                                    }

                                    bool isStageCompleted = currentStage.StageID.HasValue && CheckIfStageCompleted(PHVM.IOMID, currentStage.StageID.Value);
                                    currentStage.STATUS = isStageCompleted ? (short)0 : (short)1;

                                    var dbStage = _dbContext.DGIT_PIOM_STAGE
                                        .FirstOrDefault(s => s.STAGE_ID == currentStage.StageID);

                                    if (dbStage != null)
                                    {
                                        dbStage.STATUS = currentStage.STATUS;
                                        dbStage.UPDATEDATE = DateTime.Now;
                                        dbStage.UPDATEBY = PHVM.UpdatedBy ?? 0;
                                        _dbContext.Entry(dbStage).State = EntityState.Modified;
                                        _dbContext.SaveChanges();
                                    }

                                    // ✅ If stage completed -> move to next stage
                                    if (isStageCompleted)
                                    {
                                        var nextStage = iomObj.Stages.FirstOrDefault(s => s.StageOrder == currentStage.StageOrder + 1);
                                        if (nextStage != null)
                                        {
                                            foreach (var pg in nextStage.ParallelGroups)
                                            {
                                                var firstApprover = pg.Approvers.OrderBy(a => a.APP_SEQ).FirstOrDefault();
                                                if (firstApprover != null)
                                                {
                                                    SaveIOMAppHis(PHVM.AddedBy, PHVM.IOMID, firstApprover, firstApprover.IOMAPPAUTH_ID, pg.ParallelGroupID.Value);
                                                    SendMailByApprovalAuthority(PHVM, PHVM.ApprovalStatus, emp_dtl, firstApprover.IOMAPPAUTH_ID);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        DGIT_PIOMHEADER DPH = _dbContext.DGIT_PIOMHEADER.SingleOrDefault(x => x.IOMHEADERID == PHVM.IOMID);
                        if (DPH != null)
                        {
                            // Check if all stages in the document are completed using DB-driven logic
                            bool allStagesCompleted = CheckIfDocumentCompleted(PHVM.IOMID);
                            DPH.ISEDITABLE = 0;
                            // Update PROCESS_STATUS based on current approval action
                            if (PHVM.ApprovalStatus == 1) // Approved
                            {
                                // If all stages completed, mark document as Complete (2)
                                // Otherwise, mark as Work in Progress (1)
                                DPH.PROCESS_STATUS = allStagesCompleted ? (short)2 : (short)1;
                            }
                            else if (PHVM.ApprovalStatus == 2)
                                DPH.PROCESS_STATUS = 0; // Sent Back by approver
                            else if (PHVM.ApprovalStatus == 3)
                                DPH.PROCESS_STATUS = 3; // Rejected
                            else if (PHVM.ApprovalStatus == 5)
                                DPH.PROCESS_STATUS = 5; // On Hold or special status
                            else
                                DPH.PROCESS_STATUS = 1; // Default: Work in Progress

                            // Update modification info
                            DPH.UPDATEDBY = PHVM.UpdatedBy;
                            DPH.UPDATEDATE = DateTime.Now;

                            // Mark entity as modified and persist changes to DB
                            _dbContext.Entry(DPH).State = EntityState.Modified;
                            _dbContext.SaveChanges();

                            // NOTE for other devs:
                            // PROCESS_STATUS values:
                            // 0 = Sent Back
                            // 1 = Work in Progress
                            // 2 = Complete
                            // 3 = Rejected
                            // 5 = Hold
                            // 6 = isEditable
                            //
                            // This block ensures the document header status reflects the actual
                            // state of all stages and approvals in the workflow, using the DB-driven
                            // completion checks rather than in-memory properties.
                        }

                        retVal = 1;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Ideally log ex
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        bool CheckIfStageCompleted(long iomHeaderId, long stageId)
        {
            // Get all parallel groups for the stage
            var parallelGroups = _dbContext.DGIT_PIOM_SEQ_APPROVAL
                .Where(sa => sa.STAGE_ID == stageId)
                .Select(sa => sa.PARALLEL_GROUP_ID)
                .ToList();

            // Get all approvers for those parallel groups
            var auths = _dbContext.DGIT_PIOMAPPAUTHSEQ
                .Where(a => parallelGroups.Contains(a.PARALLEL_GROUP_ID))
                .Select(a => a.IOMAPPAUTH_ID)
                .ToList();

            // Count of total approvers in that stage
            var totalApprovers = auths.Count();

            // Count of approvers who have approved
            var approvedApprovers = _dbContext.DGIT_PIOMAPPHISTORY
                .Where(h => auths.Contains(h.PIOMAPPAUTHSEQ_ID) && h.APPROVAL_STATUS == 1)
                .Select(h => h.PIOMAPPAUTHSEQ_ID)
                .Distinct()
                .Count();

            return totalApprovers > 0 && totalApprovers == approvedApprovers;
        }

        bool CheckIfGroupCompleted(long parallelGroupId)
        {
            // Get all approvers in the parallel group
            var auths = _dbContext.DGIT_PIOMAPPAUTHSEQ
                .Where(a => a.PARALLEL_GROUP_ID == parallelGroupId)
                .Select(a => a.IOMAPPAUTH_ID)
                .ToList();

            if (!auths.Any()) return false;

            // Count approvers who have approved
            var approvedCount = _dbContext.DGIT_PIOMAPPHISTORY
                .Where(h => auths.Contains(h.PIOMAPPAUTHSEQ_ID) && h.APPROVAL_STATUS == 1)
                .Select(h => h.PIOMAPPAUTHSEQ_ID)
                .Distinct()
                .Count();

            return approvedCount == auths.Count;
        }

        bool CheckIfDocumentCompleted(long iomHeaderId)
        {
            var stageIds = _dbContext.DGIT_PIOM_STAGE
                .Where(s => s.IOMHEADERID == iomHeaderId)
                .Select(s => s.STAGE_ID)
                .ToList();

            if (!stageIds.Any()) return false;

            return stageIds.All(stageId => CheckIfStageCompleted(iomHeaderId, stageId));
        }

        public void SavePIOMAppHisApproval(long AddedBy, long PIOMHEADERID, IOMPAppAuthSeqViewModel PSVM, string Remark, short ApprovalStatus)
        {
            DGIT_PIOMAPPHISTORY DPAH = new DGIT_PIOMAPPHISTORY();
            int FlagAdd = 0;
            DPAH = _dbContext.DGIT_PIOMAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.IOMHEADERID == PIOMHEADERID && d.APPROVAL_STATUS == 0).FirstOrDefault();
            int APPauthseq = _dbContext.DGIT_PIOMAPPAUTHSEQ.Where(m => m.IOMHEADERID == PIOMHEADERID && m.ADEMPCODE == PSVM.ADEMPCODE).Count();
            if (DPAH == null && APPauthseq > 0)
            {
                DPAH = new DGIT_PIOMAPPHISTORY();
                if (_dbContext.DGIT_PIOMAPPHISTORY.Count() == 0)
                {
                    DPAH.IOMAPPHISTORY_ID = 1;
                }
                else
                {
                    DPAH.IOMAPPHISTORY_ID = _dbContext.DGIT_PIOMAPPHISTORY.Max(x => x.IOMAPPHISTORY_ID) + 1;
                }
                FlagAdd = 1;

                DPAH.IOMHEADERID = PIOMHEADERID;
                DPAH.PIOMAPPAUTHSEQ_ID = PSVM.IOMAPPAUTH_ID;
                DPAH.ADEMPCODE = PSVM.ADEMPCODE;
                DPAH.APPTYPE = PSVM.APPTYPE;
                DPAH.APPROVAL_STATUS = ApprovalStatus;
                DPAH.APPROVAL_REMARK = Remark;
                DPAH.APP_DATE = DateTime.Now;
                if (FlagAdd == 1)
                {
                    DPAH.ADDEDBY = AddedBy;
                    DPAH.ADDEDDATE = DateTime.Now;
                }
                else
                {
                    DPAH.UPDATEBY = AddedBy;
                    DPAH.UPDATEDATE = DateTime.Now;
                }
                _dbContext.Entry(DPAH).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                _dbContext.SaveChanges();
            }
        }

        public short SaveApprovalHoldToPending(IOMPAppHistoryViewModel PHVM)
        {
            short retVal = 0;

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var datatest = _dbContext.DGIT_PIOMAPPHISTORY.ToList();
                    // Check if approval note is already pending for this IOMID, EmpCode, SequenceNo (stage), and Seq_id (group)
                    int pendingCount = _dbContext.DGIT_PIOMAPPHISTORY.Count(x =>
                        x.IOMHEADERID == PHVM.IOMID &&
                        x.ADEMPCODE == PHVM.EmpCode &&
                        x.APPROVAL_STATUS == 0 &&
                        x.PIOMAPPAUTHSEQ_ID == PHVM.Seq_id
                    );

                    bool isAlreadyPending = pendingCount > 0;


                    if (isAlreadyPending)
                        return -1;

                    DGIT_PIOMAPPHISTORY DPAH = new DGIT_PIOMAPPHISTORY();

                    var totalCount = _dbContext.DGIT_PIOMAPPHISTORY.Count();

                    DPAH.IOMAPPHISTORY_ID = totalCount > 0
                        ? _dbContext.DGIT_PIOMAPPHISTORY.Max(x => x.IOMAPPHISTORY_ID) + 1
                        : 1;


                    DPAH.IOMHEADERID = PHVM.IOMID;
                    DPAH.ADEMPCODE = PHVM.EmpCode;
                    DPAH.APPTYPE = PHVM.AppType;
                    DPAH.APPROVAL_STATUS = 0;
                    DPAH.APPROVAL_REMARK = string.Empty;
                    DPAH.PIOMAPPAUTHSEQ_ID = PHVM.Seq_id;

                    DPAH.ADDEDBY = PHVM.AddedBy;
                    DPAH.ADDEDDATE = DateTime.Now;

                    _dbContext.Entry(DPAH).State = EntityState.Added;
                    _dbContext.SaveChanges();

                    var DPH = _dbContext.DGIT_PIOMHEADER.SingleOrDefault(x => x.IOMHEADERID == PHVM.IOMID);
                    if (DPH != null)
                    {
                        DPH.PROCESS_STATUS = 1; // Pending status
                        DPH.UPDATEDBY = PHVM.UpdatedBy;
                        DPH.UPDATEDATE = DateTime.Now;
                        _dbContext.Entry(DPH).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }

                    transaction.Commit();
                    retVal = 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    retVal = -1;
                }
            }
            return retVal;
        }

        public short PIOMCancel(IOMPHeaderViewModel PHVM)
        {
            short retVal = 0;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (PHVM.IOMHEADERID > 0)
                    {
                        //////// Update Process Status ////////
                        DGIT_PIOMHEADER DPH = new DGIT_PIOMHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _dbContext.DGIT_PIOMHEADER.Where(x => x.IOMHEADERID == PHVM.IOMHEADERID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.PROCESS_STATUS = 4;
                            DPH.UPDATEDBY = PHVM.UPDATEDBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _dbContext.Entry(DPH).State = EntityState.Modified;
                            _dbContext.SaveChanges();
                        }
                        retVal = 1;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public List<IOMPHeaderViewModel> GetPIOMPrevAuthority(long Adempcode)
        {
            var iomHeaderIds = _dbContext.DGIT_PIOMAPPAUTHSEQ
                .Where(a => a.ADDEDBY == Adempcode && a.STATUS == 1)
                .Select(a => a.IOMHEADERID)
                .Distinct()
                .OrderByDescending(id => id)
                .Take(5)
                .ToList();

            if (!iomHeaderIds.Any())
                return new List<IOMPHeaderViewModel>();

            var mainHeaders = _dbContext.DGIT_PIOMHEADER
                .Where(h => iomHeaderIds.Contains(h.IOMHEADERID) && h.STATUS == 1)
                .OrderByDescending(h => h.DATEADDED)
                .ToList();

            var viewModels = new List<IOMPHeaderViewModel>();

            foreach (var header in mainHeaders)
            {
                var vm = new IOMPHeaderViewModel
                {
                    IOMHEADERID = header.IOMHEADERID,
                    IOMDesc = header.IOM_DESC,
                    STATUS = header.STATUS,
                    PROCESS_STATUS = header.PROCESS_STATUS,
                    DATEADDED = header.DATEADDED,
                    ADDEDBY = header.ADDEDBY,
                    UPDATEDATE = header.UPDATEDATE,
                    UPDATEDBY = header.UPDATEDBY,
                    Stages = new List<IOMStageViewModel>()
                };

                var stages = _dbContext.DGIT_PIOM_STAGE
                    .Where(s => s.IOMHEADERID == header.IOMHEADERID)
                    .OrderBy(s => s.STAGE_ORDER)
                    .ToList();

                foreach (var stageEntity in stages)
                {
                    var stageVm = new IOMStageViewModel
                    {
                        StageID = stageEntity.STAGE_ID,
                        StageName = stageEntity.STAGE_NAME,
                        StageOrder = stageEntity.STAGE_ORDER,
                        STATUS = stageEntity.STATUS,
                        ParallelGroups = new List<IOMParallelGroupViewModel>()
                    };

                    var parallelGroups = _dbContext.DGIT_PIOM_SEQ_APPROVAL
                        .Where(pg => pg.STAGE_ID == stageEntity.STAGE_ID)
                        .OrderBy(pg => pg.PARALLEL_GROUP_ID)
                        .ToList();

                    for (int groupIndex = 0; groupIndex < parallelGroups.Count; groupIndex++)
                    {
                        var pgEntity = parallelGroups[groupIndex];
                        var pgVm = new IOMParallelGroupViewModel
                        {
                            ParallelGroupID = pgEntity.PARALLEL_GROUP_ID,
                            GroupName = pgEntity.GROUP_NAME,
                            GroupOrder = groupIndex + 1,
                            STATUS = pgEntity.STATUS,
                            Headers = new List<IOMPAppHeaderViewModel>(),
                            Approvers = new List<IOMPAppAuthSeqViewModel>()
                        };

                        var approvers = (from a in _dbContext.DGIT_PIOMAPPAUTHSEQ
                                         where a.PARALLEL_GROUP_ID == pgEntity.PARALLEL_GROUP_ID
                                               && a.IOMHEADERID == header.IOMHEADERID
                                         join addBy in _dbContext.ADEMPLOYEE on a.ADEMPCODE equals addBy.ADEMPCODE
                                         join vw in _dbContext.VW_ASSOCIATELVLDETAILS on a.ADEMPCODE equals vw.ADEMPCODE
                                         join desg in _dbContext.ADDESIGNATION on vw.ADDESIGNATIONID equals desg.ADDESIGNATIONID
                                         where vw.SYKI == _syki.SYKIID
                                         orderby a.APP_SEQ
                                         select new
                                         {
                                             a.IOMAPPAUTH_ID,
                                             a.IOMHEADERID,
                                             a.ADEMPCODE,
                                             a.APP_SEQ,
                                             a.STATUS,
                                             a.APPTYPE,
                                             a.ADDEDBY,
                                             a.ADDEDDATE,
                                             a.UPDATEBY,
                                             a.UPDATEDATE,
                                             a.IOMAPPHEADER,
                                             EmpName = addBy.FIRSTNAME + " " + addBy.LASTNAME,
                                             ADDESIGNATION = desg.DESCRIP
                                         }).ToList();

                        foreach (var app in approvers)
                        {
                            var appVm = new IOMPAppAuthSeqViewModel
                            {
                                IOMAPPAUTH_ID = app.IOMAPPAUTH_ID,
                                IOMID = app.IOMHEADERID,
                                ADEMPCODE = app.ADEMPCODE,
                                ADEMPNAME = app.EmpName ?? string.Empty,
                                ADDESIGNATION = app.ADDESIGNATION ?? string.Empty,
                                Header = app.IOMAPPHEADER,
                                APP_SEQ = app.APP_SEQ,
                                STATUS = app.STATUS,
                                APPTYPE = app.APPTYPE,
                                ADDEDBY = app.ADDEDBY,
                                ADDEDDATE = app.ADDEDDATE,
                                UPDATEBY = app.UPDATEBY,
                                UPDATEDATE = app.UPDATEDATE,
                            };
                            pgVm.Approvers.Add(appVm);
                        }

                        var headerStrings = approvers.Select(a => a.IOMAPPHEADER).Distinct().ToList();
                        foreach (var hStr in headerStrings)
                        {
                            var headEntity = _dbContext.DGIT_PIOMAPPHEADER
                                .FirstOrDefault(h => h.IOMHEADERID == header.IOMHEADERID
                                                    && h.IOMAPPHEADER == hStr
                                                    && h.STATUS == 1);
                            if (headEntity != null)
                            {
                                var headVm = new IOMPAppHeaderViewModel
                                {
                                    IOMAPPHEADERID = headEntity.IOMAPPHEADERID,
                                    IOMHEADERID = headEntity.IOMHEADERID,
                                    IOMAPPHEADER = headEntity.IOMAPPHEADER,
                                    APPHEADERDESC = headEntity.APPHEADERDESC,
                                    STATUS = headEntity.STATUS,
                                    Seq_Order = 0,
                                    ADDEDBY = headEntity.ADDEDBY,
                                    ADDEDDATE = headEntity.ADDEDDATE,
                                    UPDATEBY = headEntity.UPDATEBY,
                                    UPDATEDATE = headEntity.UPDATEDATE
                                };
                                pgVm.Headers.Add(headVm);
                            }
                        }

                        stageVm.ParallelGroups.Add(pgVm);
                    }

                    vm.Stages.Add(stageVm);
                }

                viewModels.Add(vm);
            }

            return viewModels;
        }

        public short SendMailByApprovalAuthority(IOMPAppHistoryViewModel IHVM, short approvalStatus, Employee_Details employeeDetails, long seqid)
        {
            long iomHeaderId = long.Parse(IHVM.IOMID.ToString());
            IOMPHeaderViewModel PHVM = GetPIOMRequestById(IHVM.IOMID);
            short PROCESS_STATUS = PHVM.PROCESS_STATUS;
            short retVal = 0;

            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved"
                    : approvalStatus == 2 ? "Send back"
                    : approvalStatus == 3 ? "Rejected"
                    : approvalStatus == 5 ? "Hold"
                    : "";

                _logger.LogInformation("SendMailByApprovalAuthority started. IOMID: {IOMID}, ApprovalStatus: {ApprovalStatus}, RequestStatus: {RequestStatus}",
                    IHVM.IOMID, approvalStatus, RequestStatus);

                #region Send mail next approval authority
                if (PHVM.iompDetail.Count > 0 && approvalStatus == 1)
                {
                    _logger.LogInformation("Preparing to send mail to next approval authority. SeqID: {SeqID}", seqid);

                    var Auth_obj = PHVM.iompAppHis
                        .Where(a => a.Seq_id == seqid && !string.IsNullOrEmpty(a.Email))
                        .FirstOrDefault();

                    if (Auth_obj != null)
                    {
                        _logger.LogInformation("Next approval authority found: EmpCode={EmpCode}, Email={Email}", Auth_obj.EmpCode, Auth_obj.Email);

                        commanEmail sendMail = new commanEmail
                        {
                            MailFrom = "portal.admin@honda.hmsi.in",
                            MailTo = serverpath.isTestServer() ? serverpath.getTestEMail() : Auth_obj.Email
                        };

                        string struid = Auth_obj.EmpCode.ToString();
                        string strid = iomHeaderId.ToString();

                        sendMail.MailSubject = $"IOM Request from - {PHVM.Emp_Detail._EName}, Employee Code - {PHVM.Emp_Detail._ECode}";
                        sendMail.MailBody = $"<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                            $"<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IOM Request from {PHVM.Emp_Detail._EName} - Emp Code ({PHVM.Emp_Detail._ECode})</font></b></td></tr>" +
                            $"<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear {Auth_obj.EmpCode} - [" + Auth_obj.EmpCode + "] San ,</br></br>{PHVM.Emp_Detail._EName} San has raised a IOM Approval Request in Employee Portal. Below are the details :</td></tr>" +
                            $"<tr><td width=125 height=22 valign=top>IOM Description :</td><td width=389 valign=top>{PHVM.IOMDesc}</td></tr>" +
                            $"<tr><td valign=top colspan=2>Please click on <a href={serverpath.getServerPath()}/Login/EmailApproval/?Wid={struid}&C=PIOM&A=PIOMApproval&Tid={strid}> Employee Portal</a> link to approve the request.</td></tr>" +
                            $"<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                        try
                        {
                            _logger.LogInformation("Sending mail to next authority: {Email}", sendMail.MailTo);
                            Thread bgThread = new Thread(() =>
                            {
                                try
                                {
                                    sendMail.Send();
                                    _logger.LogInformation("Mail sent successfully to {Email}", sendMail.MailTo);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error sending mail to next approval authority {Email}", sendMail.MailTo);
                                }
                            });
                            bgThread.Start();
                            retVal = 1;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error creating thread for next approval authority email");
                            retVal = -1;
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No next approval authority found for SeqID: {SeqID}", seqid);
                    }
                }
                #endregion

                #region Send mail for requestor
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId) && PROCESS_STATUS != 2)
                {
                    _logger.LogInformation("Preparing to send mail to requestor: {Email}", PHVM.Emp_Detail._EmailId);

                    commanEmail sendMail = new commanEmail
                    {
                        MailFrom = "portal.admin@honda.hmsi.in",
                        MailTo = PHVM.Emp_Detail._EmailId,
                        MailSubject = $"IOM Approval Status - {RequestStatus}",
                        MailBody = $"<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                            $"<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IOM Approval Status {RequestStatus} - Emp Code ({PHVM.Emp_Detail._ECode})</font></b></td></tr>" +
                            $"<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                            $"<tr><td valign=top colspan=2>Your IOM request has been <b>{RequestStatus}</b> by {employeeDetails.Employee_Name} San. The request details are as follows:</td></tr>" +
                            $"<tr><td width=125 height=22 valign=top>IOM Description</td><td width=389 valign=top>{PHVM.IOMDesc}</td></tr>" +
                            (approvalStatus == 5 ? $"<tr style='background-color:yellow;'><td width=125 height=22 valign=top>Remarks</td><td width=389 valign=top>{IHVM.ApprovalRemark}</td></tr>" : "") +
                            $"<tr><td valign=top colspan=2>Please login <a href={serverpath.getServerPath()}Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
                            $"<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>"
                    };

                    try
                    {
                        _logger.LogInformation("Sending mail to requestor: {Email}", sendMail.MailTo);
                        Thread bgThread = new Thread(() =>
                        {
                            try
                            {
                                sendMail.Send();
                                _logger.LogInformation("Mail sent successfully to requestor {Email}", sendMail.MailTo);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error sending mail to requestor {Email}", sendMail.MailTo);
                            }
                        });
                        bgThread.Start();
                        retVal = 1;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating thread for requestor email");
                        retVal = -1;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMailByApprovalAuthority for IOMID: {IOMID}", IHVM.IOMID);
                retVal = -1;
            }

            _logger.LogInformation("SendMailByApprovalAuthority completed. ReturnValue: {RetVal}", retVal);
            return retVal;
        }

        public Tuple<short, long> SaveAdditionalIOMRequest(IOMPHeaderViewModel model)
        {
            short retVal = 0;
            long retHeaderId = 0;

            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                //  Fetch header and update basic info ---
                var DPH = _dbContext.DGIT_PIOMHEADER
                    .FirstOrDefault(x => x.IOMHEADERID == model.IOMHEADERID);

                if (DPH == null)
                    throw new Exception("Invalid IOM Header ID.");

                DPH.PROCESS_STATUS = model.PROCESS_STATUS;
                DPH.UPDATEDBY = model.UPDATEDBY;
                DPH.UPDATEDATE = DateTime.Now;

                _dbContext.SaveChanges();

                //  Save Additional Approvers ---
                var result = SaveAdditionalApprovers(model.ADDEDBY, DPH.IOMHEADERID, model.Stages, useOuterTransaction: true);

                if (result.Item1 <= 0)
                    throw new Exception("Error in saving additional approvers.");

                transaction.Commit();
                retVal = 1;
                retHeaderId = DPH.IOMHEADERID;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("already exist"))
                    retVal = -2;
                else
                    retVal = -1;

                transaction.Rollback();
                Console.WriteLine($"Error in SaveAdditionalIOMRequest: {ex.Message}");
            }

            return new Tuple<short, long>(retVal, retHeaderId);
        }


        public Tuple<short, long> SaveAdditionalApprovers(
            long addedBy,
            long iomHeaderId,
            List<IOMStageViewModel> stages,
            bool useOuterTransaction = false)
        {
            short retVal = 0;
            long retHeaderId = 0;
            //  Fetch existing approvers once ---
            var existingEmpCodes = _dbContext.DGIT_PIOMAPPAUTHSEQ
                .Where(a => a.IOMHEADERID == iomHeaderId)
                .Select(a => a.ADEMPCODE)
                .ToHashSet();

            int maxStageOrder = _dbContext.DGIT_PIOM_STAGE
                .Where(s => s.IOMHEADERID == iomHeaderId)
                .Select(s => (int?)s.STAGE_ORDER)
                .Max() ?? 0;

            short maxAdditionalType = _dbContext.DGIT_PIOM_STAGE
                .Where(s => s.IOMHEADERID == iomHeaderId)
                .Select(s => (short?)s.ADDITIONALTYPE)
                .Max() ?? 1;

            bool isFirstStage = true;
            maxAdditionalType++;
            //  Loop through stages and save ---
            foreach (var stage in stages ?? Enumerable.Empty<IOMStageViewModel>())
            {
                maxStageOrder++;
                stage.StageOrder = maxStageOrder;

                long stageId = SaveStage(addedBy, iomHeaderId, stage, maxAdditionalType);

                foreach (var group in stage.ParallelGroups ?? Enumerable.Empty<IOMParallelGroupViewModel>())
                {
                    long groupId = SaveParallelGroup(addedBy, stageId, group);

                    //  Check duplicates ---
                    if (group.Approvers?.Any() == true)
                    {
                        var duplicateApprovers = group.Approvers
                            .Where(a => existingEmpCodes.Contains(a.ADEMPCODE))
                            .Select(a => a.ADEMPCODE)
                            .ToList();

                        if (duplicateApprovers.Any())
                            throw new Exception("already exist");

                        //  Save new approvers ---
                        SaveSequentialAdditionalApprovers(addedBy, iomHeaderId, groupId, group.Approvers, stage.StageOrder, isFirstStage);

                        // Add new approvers to in-memory set
                        foreach (var emp in group.Approvers)
                            existingEmpCodes.Add(emp.ADEMPCODE);
                    }
                }
                isFirstStage = false;
            }

            retVal = 1;
            retHeaderId = iomHeaderId;

            return new Tuple<short, long>(retVal, retHeaderId);
        }

        private void SaveSequentialAdditionalApprovers(long addedBy, long headerId, long groupId, List<IOMPAppAuthSeqViewModel> approvers, int stageOrder, bool isFirstStage)
        {
            try
            {
                // Delete existing approvers for this header + group
                var lastId = _dbContext.DGIT_PIOMAPPAUTHSEQ.Count() > 0
        ? _dbContext.DGIT_PIOMAPPAUTHSEQ.Max(x => x.IOMAPPAUTH_ID)
        : 1;

                foreach (var appr in approvers.OrderBy(a => a.APP_SEQ))
                {
                    lastId++;
                    DGIT_PIOMAPPAUTHSEQ entity = new()
                    {

                        IOMAPPAUTH_ID = lastId,
                        IOMHEADERID = headerId,
                        PARALLEL_GROUP_ID = groupId,
                        IOMAPPHEADER = appr.Header,
                        ADEMPCODE = appr.ADEMPCODE,
                        APP_SEQ = appr.APP_SEQ,
                        APPTYPE = appr.APPTYPE,
                        STATUS = 1,
                        ADDEDBY = addedBy,
                        ADDEDDATE = DateTime.Now
                    };

                    _dbContext.DGIT_PIOMAPPAUTHSEQ.Add(entity);
                    _dbContext.SaveChanges();
                    if (isFirstStage && appr.APP_SEQ == 0)
                    {
                        SaveIOMAppHis(addedBy, headerId, appr, entity.IOMAPPAUTH_ID, groupId);
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public long GetPIOMNextApprovalId(long IOMID, long ecode)
        {
            long retval = 0;


            var _obj = (from _PO in _dbContext.DGIT_PIOMHEADER
                        join _PAH in _dbContext.DGIT_PIOMAPPHISTORY on _PO.IOMHEADERID equals _PAH.IOMHEADERID
                        join _CAT in _dbContext.DGIT_PIOMCATMST on _PO.IOMCATID equals _CAT.IOMCATMSTID into _catTMP
                        from _catd in _catTMP.DefaultIfEmpty()
                        where _PO.PROCESS_STATUS == 1 && _PAH.APPROVAL_STATUS == 0 && _PAH.ADEMPCODE == ecode
                        select new
                        {
                            _PO.IOMHEADERID,
                            _PO.DATEADDED,
                            ISHIGHLIGHTED = (_catd == null ? 0 : _catd.ISHIGHLIGHTED)
                        }
                        ).OrderByDescending(M => M.ISHIGHLIGHTED).ThenBy(p => p.DATEADDED);
            var lsthigh = _obj.Where(m => m.ISHIGHLIGHTED == 1 && m.IOMHEADERID > IOMID);
            if (lsthigh.Count() > 0)
            {
                retval = lsthigh.FirstOrDefault().IOMHEADERID;

            }
            else
            {
                var lstnonhigh = _obj.Where(m => m.ISHIGHLIGHTED == 0);
                retval = lstnonhigh.FirstOrDefault().IOMHEADERID;
            }
            return retval;
        }



    }
}
