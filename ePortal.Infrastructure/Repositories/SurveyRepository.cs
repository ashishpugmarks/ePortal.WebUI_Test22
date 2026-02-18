using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Infrastructure.Repositories
{
    public class SurveyRepository
    {
        private EPortalDBContext _ServeyDBContext;
        private SYKI _Syki;

        public SurveyRepository(EPortalDBContext ServeyDBContext)
        {
            _ServeyDBContext = ServeyDBContext;
            _Syki = _ServeyDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }


        public IEnumerable<SurveyViewModel> SurveyList()
        {
            //sa CR6927
            var v_financialYear = _Syki.FINANCIALYEAR.Split('-');
            var startYear = v_financialYear[0];
            var endYear = v_financialYear[1];
            DateTime ReqDateFrom = DateTime.ParseExact("01-APR-" + startYear.Trim(), "dd-MMM-yyyy", null);
            DateTime ReqDateTo = DateTime.ParseExact("31-MAR-" + endYear.Trim() + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            //ea CR6927

            IEnumerable<SurveyViewModel> iList;
            //iList = (from data in _ServeyDBContext.HRSECURITYSURVEY.OrderBy(r => r.HRSECURITYSURVEYID).Where(r => r.ACTIVE == 1).ToList() //Survey enhancement
            iList = (from data in _ServeyDBContext.HRSECURITYSURVEY.OrderByDescending(r => r.HRSECURITYSURVEYID).ToList() //Survey enhancement
                     where (data.DATEADDED >= ReqDateFrom)  //CR6927
                    && (data.DATEADDED <= ReqDateTo)  //CR6927
                     select new SurveyViewModel
                     {
                         HRSECURITYSURVEYID = data.HRSECURITYSURVEYID,
                         HRSURVEYDESC = data.HRSURVEYDESC,
                         HRSURVEYDESCHINDI = data.HRSURVEYDESCHINDI,
                         HRSURVEYDESCKANNADA = data.HRSURVEYDESCKANNADA,
                         HRSURVEYDESCGUJARATI = data.HRSURVEYDESCGUJARATI,
                         HRSURVEYDESCJAPANESE = data.HRSURVEYDESCJAPANESE,
                         //ACTIVE = data.ACTIVE == 0 ? false : true,
                         ACTIVE = data.ACTIVE, //Survey enhancement
                         HRSURVEYSTARTDATE = data.HRSURVEYSTARTDATE,
                         HRSURVEYENDDATE = data.HRSURVEYENDDATE,
                         RETEST = data.RETEST
                     });
            return iList;
        }

        //public SurveyViewModel GetEditById(int? id)
        //{
        //    DateTime CutoffDate = new DateTime(2024, 6, 3);//Added by Aumento on 01-06-2024 :: SR71845
        //    SurveyViewModel AVM = new SurveyViewModel();
        //    AVM = (from data in _ServeyDBContext.HRSECURITYSURVEY
        //           where data.HRSECURITYSURVEYID == id
        //           select new SurveyViewModel
        //           {
        //               HRSECURITYSURVEYID = data.HRSECURITYSURVEYID,
        //               HRSURVEYDESC = data.HRSURVEYDESC,
        //               HRSURVEYDESCHINDI = data.HRSURVEYDESCHINDI,
        //               HRSURVEYDESCKANNADA = data.HRSURVEYDESCKANNADA,
        //               HRSURVEYDESCGUJARATI = data.HRSURVEYDESCGUJARATI,
        //               HRSURVEYDESCJAPANESE = data.HRSURVEYDESCJAPANESE,
        //               HRSURVEYSTARTDATE = data.HRSURVEYSTARTDATE,
        //               HRSURVEYENDDATE = data.HRSURVEYENDDATE,
        //               SURVEYRESULTMAIL = data.SURVEYRESULTMAIL == 0 || data.SURVEYRESULTMAIL == null ? false : true,
        //               FEEDBACKSTATUS = data.FEEDBACKSTATUS == 0 || data.SURVEYRESULTMAIL == null ? false : true,
        //               //ACTIVE = data.ACTIVE == 0 || data.SURVEYRESULTMAIL == null ? false : true,
        //               ACTIVE = data.ACTIVE == 0 || data.SURVEYRESULTMAIL == null ? 0 : data.ACTIVE,
        //               HRSURVEYTITAL = data.HRSURVEYTITAL,
        //               HRSURVEY_HEADER = data.HRSURVEYHEADER,
        //               HRSURVEY_HEADER_HINDI = data.HRSURVEYHEADERHINDI,
        //               HRSURVEY_HEADER_KANNADA = data.HRSURVEYHEADERKANNADA,
        //               HRSURVEY_HEADER_GUJARATI = data.HRSURVEYHEADERGUJARATI,
        //               HRSURVEY_HEADER_JAPANESE = data.HRSURVEYHEADERJAPANESE,
        //               ISREMARKSMANDATORY = data.ISREMARKSMANDATORY == 1 ? true : false,
        //               FUNCTIONAL_DESIGNATIONS = (data.DATEADDED >= CutoffDate) ? data.FUNCTIONAL_DESIGNATION : null, //Updated by Aumento on 01-06-2024 :: SR71845
        //               LOCATIONS = data.LOCATION,
        //               OPERATION_VALUE = data.OPERATION,
        //               DIVISION_VALUE = data.DIVISION,
        //               SurveyType = (long)(data.SURVEYTYPE == null ? 1 : data.SURVEYTYPE),
        //               LoginCNT = (long)(data.LOGINCNT == null ? 0 : data.LOGINCNT),
        //               ISHCGSURVEY = (data.ISHCGSURVEY == 1 ? true : false),
        //               Questionlist = (from _qList in _ServeyDBContext.HRSECURITYQUSLIST
        //                               where _qList.HRSECURITYSURVEYID == data.HRSECURITYSURVEYID && _qList.ACTIVE == 1
        //                               select new QuestionViewModel
        //                               {
        //                                   QusId = _qList.HRSECURITYQUSLISTID,
        //                                   QUSDESCRIPTION = _qList.QUSDESCRIPTION,
        //                                   QUSDESCRIPTIONHINDI = _qList.QUSDESCRIPTIONHINDI,
        //                                   QUSDESCRIPTIONKANNADA = _qList.QUSDESCRIPTIONKANNADA,
        //                                   QUSDESCRIPTIONGUJARATI = _qList.QUSDESCRIPTIONGUJARATI,
        //                                   QUSDESCRIPTIONJAPANESE = _qList.QUSDESCRIPTIONJAPANESE,
        //                                   OPTIONTYPE = _qList.OPTIONTYPE,
        //                                   ISSUBQUESTION = _qList.ISSUBQUESTION,
        //                                   SUBQUESTIONPARENTID = _qList.SUBQUESTIONPARENTID,
        //                                   TRIGGEROPTIONIDS = _qList.TRIGGEROPTIONIDS,
        //                                   Status = _qList.ACTIVE,
        //                                   MappingList = (from _AnsList in _ServeyDBContext.HRSECURITYANSLIST
        //                                                  where _AnsList.HRSECURITYQUSLISTID == _qList.HRSECURITYQUSLISTID
        //                                                  select new AnswerViewModel
        //                                                  {
        //                                                      AnsId = _AnsList.HRSECURITYANSLISTID,
        //                                                      ANSDESCRIPTION = _AnsList.ANSDESCRIPTION,
        //                                                      ANSDESCRIPTIONHINDI = _AnsList.ANSDESCRIPTIONHINDI,
        //                                                      ANSDESCRIPTIONKANNADA = _AnsList.ANSDESCRIPTIONKANNADA,
        //                                                      ANSDESCRIPTIONGUJARATI = _AnsList.ANSDESCRIPTIONGUJARATI,
        //                                                      ANSDESCRIPTIONJAPANESE = _AnsList.ANSDESCRIPTIONJAPANESE,
        //                                                      ISCORRECT = _AnsList.ISCORRECT,
        //                                                      Status = _AnsList.ACTIVE,
        //                                                      ISFAMILYDECREQ = (_AnsList.ISFAMILYDEC_REQ == 1 ? true : false),
        //                                                  }).ToList(),
        //                               }).ToList(),
        //               HRSURVEYMODULETYPE = data.HRSURVEYMODULETYPE == null ? "FBS" : data.HRSURVEYMODULETYPE //Survey enhancement
        //           }).FirstOrDefault();

        //    return AVM;
        //}

        public SurveyViewModel GetEditById(int? id)
        {
            DateTime CutoffDate = new DateTime(2024, 6, 3);
            SurveyViewModel AVM;

            // Step 1: Get the survey data
            var data = _ServeyDBContext.HRSECURITYSURVEY
                .Where(s => s.HRSECURITYSURVEYID == id)
                .FirstOrDefault();

            if (data == null) return null;

            // Step 2: Get all questions for this survey
            var questionEntities = _ServeyDBContext.HRSECURITYQUSLIST
                .Where(q => q.HRSECURITYSURVEYID == data.HRSECURITYSURVEYID && q.ACTIVE == 1)
                .ToList();

            // Step 3: Get all answers for these questions
            var questionIds = questionEntities.Select(q => q.HRSECURITYQUSLISTID).ToList();

            var answerEntities = _ServeyDBContext.HRSECURITYANSLIST
                .Where(a => questionIds.Contains(a.HRSECURITYQUSLISTID))
                .ToList();

            // Step 4: Map answers to questions
            var questions = questionEntities.Select(q => new QuestionViewModel
            {
                QusId = q.HRSECURITYQUSLISTID,
                QUSDESCRIPTION = q.QUSDESCRIPTION,
                QUSDESCRIPTIONHINDI = q.QUSDESCRIPTIONHINDI,
                QUSDESCRIPTIONKANNADA = q.QUSDESCRIPTIONKANNADA,
                QUSDESCRIPTIONGUJARATI = q.QUSDESCRIPTIONGUJARATI,
                QUSDESCRIPTIONJAPANESE = q.QUSDESCRIPTIONJAPANESE,
                OPTIONTYPE = q.OPTIONTYPE,
                ISSUBQUESTION = q.ISSUBQUESTION,
                SUBQUESTIONPARENTID = q.SUBQUESTIONPARENTID,
                TRIGGEROPTIONIDS = q.TRIGGEROPTIONIDS,
                Status = q.ACTIVE,
                MappingList = answerEntities
                    .Where(a => a.HRSECURITYQUSLISTID == q.HRSECURITYQUSLISTID)
                    .Select(a => new AnswerViewModel
                    {
                        AnsId = a.HRSECURITYANSLISTID,
                        ANSDESCRIPTION = a.ANSDESCRIPTION,
                        ANSDESCRIPTIONHINDI = a.ANSDESCRIPTIONHINDI,
                        ANSDESCRIPTIONKANNADA = a.ANSDESCRIPTIONKANNADA,
                        ANSDESCRIPTIONGUJARATI = a.ANSDESCRIPTIONGUJARATI,
                        ANSDESCRIPTIONJAPANESE = a.ANSDESCRIPTIONJAPANESE,
                        ISCORRECT = a.ISCORRECT,
                        Status = a.ACTIVE,
                        ISFAMILYDECREQ = a.ISFAMILYDEC_REQ == 1
                    }).ToList()
            }).ToList();

            // Step 5: Build the final SurveyViewModel
            AVM = new SurveyViewModel
            {
                HRSECURITYSURVEYID = data.HRSECURITYSURVEYID,
                HRSURVEYDESC = data.HRSURVEYDESC,
                HRSURVEYDESCHINDI = data.HRSURVEYDESCHINDI,
                HRSURVEYDESCKANNADA = data.HRSURVEYDESCKANNADA,
                HRSURVEYDESCGUJARATI = data.HRSURVEYDESCGUJARATI,
                HRSURVEYDESCJAPANESE = data.HRSURVEYDESCJAPANESE,
                HRSURVEYSTARTDATE = data.HRSURVEYSTARTDATE,
                HRSURVEYENDDATE = data.HRSURVEYENDDATE,                
                SURVEYRESULTMAIL = data.SURVEYRESULTMAIL != null && data.SURVEYRESULTMAIL != 0,
                FEEDBACKSTATUS = data.FEEDBACKSTATUS != null && data.FEEDBACKSTATUS != 0,
                ACTIVE = data.ACTIVE ?? 0,
                HRSURVEYTITAL = data.HRSURVEYTITAL,
                HRSURVEY_HEADER = data.HRSURVEYHEADER,
                HRSURVEY_HEADER_HINDI = data.HRSURVEYHEADERHINDI,
                HRSURVEY_HEADER_KANNADA = data.HRSURVEYHEADERKANNADA,
                HRSURVEY_HEADER_GUJARATI = data.HRSURVEYHEADERGUJARATI,
                HRSURVEY_HEADER_JAPANESE = data.HRSURVEYHEADERJAPANESE,
                ISREMARKSMANDATORY = data.ISREMARKSMANDATORY == 1,
                FUNCTIONAL_DESIGNATIONS = data.DATEADDED >= CutoffDate ? data.FUNCTIONAL_DESIGNATION : null,
                LOCATIONS = data.LOCATION,
                OPERATION_VALUE = data.OPERATION,
                DIVISION_VALUE = data.DIVISION,
                SurveyType = (long)(data.SURVEYTYPE ?? 1),
                LoginCNT = (long)(data.LOGINCNT ?? 0),
                ISHCGSURVEY = data.ISHCGSURVEY == 1,
                Questionlist = questions,
                HRSURVEYMODULETYPE = data.HRSURVEYMODULETYPE ?? "FBS"
            };

            return AVM;
        }


        public long CreateSurvey(SurveyViewModel SV)
        {
            long retVal = 0;

            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = DateTime.Now.Date;
            
            startDate = DateTime.ParseExact(SV.HRSURVEYSTARTDATE_STRING, "dd-MMM-yyyy", null);
            endDate = DateTime.ParseExact(SV.HRSURVEYENDDATE_STRING, "dd-MMM-yyyy", null);


            try
            {
                HRSECURITYSURVEY _SQ = _ServeyDBContext.HRSECURITYSURVEY.Where(x => x.HRSECURITYSURVEYID == SV.HRSECURITYSURVEYID).FirstOrDefault();
                int FlagAdd = 0;
                if (_SQ == null)
                {
                    _SQ = new HRSECURITYSURVEY();
                    if (_ServeyDBContext.HRSECURITYSURVEY.ToList().Count == 0)
                    {
                        _SQ.HRSECURITYSURVEYID = 1;
                        FlagAdd = 1;
                    }
                    else
                    {
                        _SQ.HRSECURITYSURVEYID = _ServeyDBContext.HRSECURITYSURVEY.Max(r => r.HRSECURITYSURVEYID) + 1;
                        FlagAdd = 1;
                    }
                }
                _SQ.HRSURVEYDESC = SV.HRSURVEYDESC;
                _SQ.HRSURVEYDESCHINDI = SV.HRSURVEYDESCHINDI;
                _SQ.HRSURVEYDESCKANNADA = SV.HRSURVEYDESCKANNADA;
                _SQ.HRSURVEYDESCGUJARATI = SV.HRSURVEYDESCGUJARATI;
                _SQ.HRSURVEYDESCJAPANESE = SV.HRSURVEYDESCJAPANESE;
                _SQ.HRSURVEYSTARTDATE = startDate; // SV.HRSURVEYSTARTDATE;
                _SQ.HRSURVEYENDDATE = endDate;     // SV.HRSURVEYENDDATE;
                _SQ.DATEADDED = DateTime.Now;
                _SQ.HRSURVEYTITAL = SV.HRSURVEYTITAL;
                _SQ.ACTIVE = Convert.ToInt16(SV.ACTIVE);
                _SQ.RETEST = 0;
                _SQ.ADDEDBY = SV.ADDEDBY;
                _SQ.HRSURVEYHEADER = SV.HRSURVEY_HEADER;
                _SQ.HRSURVEYHEADERHINDI = SV.HRSURVEY_HEADER_HINDI;
                _SQ.HRSURVEYHEADERKANNADA = SV.HRSURVEY_HEADER_KANNADA;
                _SQ.HRSURVEYHEADERGUJARATI = SV.HRSURVEY_HEADER_GUJARATI;
                _SQ.HRSURVEYHEADERJAPANESE = SV.HRSURVEY_HEADER_JAPANESE;
                _SQ.ISREMARKSMANDATORY = Convert.ToInt16(SV.ISREMARKSMANDATORY);
                _SQ.SURVEYTYPE = SV.SurveyType;
                _SQ.LOGINCNT = SV.LoginCNT;
                _SQ.ISHCGSURVEY = (SV.ISHCGSURVEY == true ? 1 : 0);
                bool vInMAIL = SV.SURVEYRESULTMAIL;
                long vOutMAIL = Convert.ToInt64(vInMAIL);
                _SQ.SURVEYRESULTMAIL = vOutMAIL;

                bool vInSTATUS = SV.FEEDBACKSTATUS;
                short vOutSTATUS = Convert.ToInt16(vInSTATUS);
                _SQ.FEEDBACKSTATUS = vOutSTATUS;

                //_SQ.SURVEYID = _SQ.HRSECURITYSURVEYID - 1;
                string fnIds = "";
                if (SV.FUNCTIONAL_DESIGNATION != null)
                {
                    foreach (var val in SV.FUNCTIONAL_DESIGNATION)
                    {
                        fnIds += val.ToString() + ",";
                    }
                    ;
                }
                _SQ.FUNCTIONAL_DESIGNATION = fnIds;

                string LocationIds = "";
                if (SV.LOCATION != null)
                {
                    foreach (var val in SV.LOCATION)
                    {
                        LocationIds += val.ToString() + ",";
                    }
                    ;
                }
                _SQ.LOCATION = LocationIds;

                string operationIds = "";
                if (SV.OPERATION != null)
                {
                    foreach (var val in SV.OPERATION)
                    {
                        operationIds += val.ToString() + ",";
                    }
                    ;
                }
                _SQ.OPERATION = operationIds;

                string divisionIds = "";
                if (SV.DIVISION != null)
                {
                    foreach (var val in SV.DIVISION)
                    {
                        divisionIds += val.ToString() + ",";
                    }
                    ;
                }
                _SQ.DIVISION = divisionIds;


                _SQ.HRSURVEYMODULETYPE = SV.HRSURVEYMODULETYPE; //Survey enhancement

                _ServeyDBContext.Entry(_SQ).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                _ServeyDBContext.SaveChanges();

                #region Add Question and Answer

                foreach (var item in SV.Questionlist)
                {

                    HRSECURITYQUSLIST HRQUESTION = new HRSECURITYQUSLIST();
                    if (HRQUESTION.HRSECURITYQUSLISTID == 0)
                    {
                        HRQUESTION = new HRSECURITYQUSLIST();
                        if (_ServeyDBContext.HRSECURITYQUSLIST.ToList().Count == 0)
                        {
                            HRQUESTION.HRSECURITYQUSLISTID = 1;
                            FlagAdd = 1;
                        }
                        else
                        {
                            HRQUESTION.HRSECURITYQUSLISTID = _ServeyDBContext.HRSECURITYQUSLIST.Max(r => r.HRSECURITYQUSLISTID) + 1;
                            FlagAdd = 1;
                        }
                    }
                    HRQUESTION.ACTIVE = 1;
                    HRQUESTION.QUSDESCRIPTION = item.QUSDESCRIPTION;
                    HRQUESTION.QUSDESCRIPTIONHINDI = item.QUSDESCRIPTIONHINDI;
                    HRQUESTION.QUSDESCRIPTIONKANNADA = item.QUSDESCRIPTIONKANNADA;
                    HRQUESTION.QUSDESCRIPTIONGUJARATI = item.QUSDESCRIPTIONGUJARATI;
                    HRQUESTION.QUSDESCRIPTIONJAPANESE = item.QUSDESCRIPTIONJAPANESE;
                    HRQUESTION.OPTIONTYPE = Convert.ToInt16(item.OPTIONTYPE);
                    HRQUESTION.HRSECURITYSURVEYID = _SQ.HRSECURITYSURVEYID;
                    HRQUESTION.DATEADDED = SV.DATEADDED;
                    HRQUESTION.ADDEDBY = SV.ADDEDBY;
                    _ServeyDBContext.Entry(HRQUESTION).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _ServeyDBContext.SaveChanges();

                    foreach (var ansObj in item.MappingList)
                    {
                        HRSECURITYANSLIST HRANSWER = new HRSECURITYANSLIST();
                        if (HRANSWER.HRSECURITYANSLISTID == 0)
                        {
                            HRANSWER = new HRSECURITYANSLIST();
                            if (_ServeyDBContext.HRSECURITYANSLIST.ToList().Count == 0)
                            {
                                HRANSWER.HRSECURITYANSLISTID = 1;
                                FlagAdd = 1;
                            }
                            else
                            {
                                HRANSWER.HRSECURITYANSLISTID = _ServeyDBContext.HRSECURITYANSLIST.Max(r => r.HRSECURITYANSLISTID) + 1;
                                FlagAdd = 1;
                            }
                        }
                        HRANSWER.HRSECURITYQUSLISTID = HRQUESTION.HRSECURITYQUSLISTID;
                        HRANSWER.ANSDESCRIPTION = ansObj.ANSDESCRIPTION;
                        HRANSWER.ANSDESCRIPTIONHINDI = ansObj.ANSDESCRIPTIONHINDI;
                        HRANSWER.ANSDESCRIPTIONKANNADA = ansObj.ANSDESCRIPTIONKANNADA;
                        HRANSWER.ANSDESCRIPTIONGUJARATI = ansObj.ANSDESCRIPTIONGUJARATI;
                        HRANSWER.ANSDESCRIPTIONJAPANESE = ansObj.ANSDESCRIPTIONJAPANESE;
                        HRANSWER.ISCORRECT = ansObj.ISCORRECT;
                        HRANSWER.ACTIVE = ansObj.Status;
                        HRANSWER.DATEADDED = SV.DATEADDED;
                        HRANSWER.ADDEDBY = SV.ADDEDBY;
                        HRANSWER.ISFAMILYDEC_REQ = (ansObj.ISFAMILYDECREQ ? 1 : 0);
                        _ServeyDBContext.Entry(HRANSWER).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                        _ServeyDBContext.SaveChanges();

                    }
                }

                #endregion

                retVal = _SQ.HRSECURITYSURVEYID;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }
        public SurveyViewModel Detail(int id)
        {
            DateTime CutoffDate = new DateTime(2024, 6, 3);//Added by Aumento on 01-06-2024 :: SR71845
            SurveyViewModel AVM = new SurveyViewModel();
            HRSECURITYSURVEY SAM = new HRSECURITYSURVEY();
            SAM = _ServeyDBContext.HRSECURITYSURVEY.Find((long)id);
            if (SAM != null)
            {
                AVM.HRSECURITYSURVEYID = SAM.HRSECURITYSURVEYID;
                AVM.HRSURVEYDESC = SAM.HRSURVEYDESC;
                AVM.HRSURVEYDESCHINDI = SAM.HRSURVEYDESCHINDI;
                AVM.HRSURVEYDESCKANNADA = SAM.HRSURVEYDESCKANNADA;
                AVM.HRSURVEYDESCGUJARATI = SAM.HRSURVEYDESCGUJARATI;
                AVM.HRSURVEYDESCJAPANESE = SAM.HRSURVEYDESCJAPANESE;
                AVM.HRSURVEYSTARTDATE = SAM.HRSURVEYSTARTDATE;
                AVM.HRSURVEYENDDATE = SAM.HRSURVEYENDDATE;
                AVM.DATEADDED = SAM.DATEADDED;
                AVM.ADDEDBY = Convert.ToInt64(SAM.ADDEDBY);
                AVM.HRSURVEYTITAL = SAM.HRSURVEYTITAL;
                AVM.HRSURVEY_HEADER = SAM.HRSURVEYHEADER;
                AVM.HRSURVEY_HEADER_HINDI = SAM.HRSURVEYHEADERHINDI;
                AVM.HRSURVEY_HEADER_KANNADA = SAM.HRSURVEYHEADERKANNADA;
                AVM.HRSURVEY_HEADER_GUJARATI = SAM.HRSURVEYHEADERGUJARATI;
                AVM.HRSURVEY_HEADER_JAPANESE = SAM.HRSURVEYHEADERJAPANESE;
                AVM.ISREMARKSMANDATORY = SAM.ISREMARKSMANDATORY == 1 ? true : false;
                //AVM.ISHCGSURVEY = SAM.ISHCGSURVEY == 1 ? true : false;
                long? vISHCGSURVEY = SAM.ISHCGSURVEY;
                if (vISHCGSURVEY == 1)
                {
                    AVM.ISHCGSURVEY = true;
                }
                else
                {
                    AVM.ISHCGSURVEY = false;
                }
                long? vInMAIL = SAM.SURVEYRESULTMAIL;
                if (vInMAIL == 1)
                {
                    AVM.SURVEYRESULTMAIL = true;
                }
                else
                {
                    AVM.SURVEYRESULTMAIL = false;
                }
                long? vInSTATUS = SAM.FEEDBACKSTATUS;
                if (vInSTATUS == 1)
                {
                    AVM.FEEDBACKSTATUS = true;
                }
                else
                {
                    AVM.FEEDBACKSTATUS = false;
                }
                AVM.ACTIVE = Convert.ToInt64(SAM.ACTIVE);
                //short? _active = Convert.ToInt16(SAM.ACTIVE);
                //if (_active == 1)
                //{
                //    AVM.ACTIVE = true;
                //}
                //else
                //{
                //    AVM.ACTIVE = false;
                //}
                // AVM.FUNCTIONAL_DESIGNATIONS = SAM.FUNCTIONAL_DESIGNATION; //Comment by Aumento on 01-06-2024 :: SR71845
                AVM.FUNCTIONAL_DESIGNATIONS = (SAM.DATEADDED >= CutoffDate) ? SAM.FUNCTIONAL_DESIGNATION : null; //Updated by Aumento on 01-06-2024 :: SR71845
                if (AVM.FUNCTIONAL_DESIGNATIONS != null)
                {
                    string[] _fnDesination = AVM.FUNCTIONAL_DESIGNATIONS.Split(',').Select(sValue => sValue.TrimEnd(',')).ToArray();
                    AVM.FUNCTIONAL_DESIGNATION = _fnDesination;
                }
                AVM.LOCATIONS = SAM.LOCATION;
                if (AVM.LOCATIONS != null)
                {
                    string[] _location = AVM.LOCATIONS.Split(',').Select(sValue => sValue.Trim(',')).ToArray();
                    AVM.LOCATION = _location;
                }
                AVM.OPERATION_VALUE = SAM.OPERATION;
                if (AVM.OPERATION_VALUE != null)
                {
                    string[] _operation = AVM.OPERATION_VALUE.Split(',').Select(sValue => sValue.Trim(',')).ToArray();
                    AVM.OPERATION = _operation;
                }
                AVM.DIVISION_VALUE = SAM.DIVISION;
                if (AVM.DIVISION_VALUE != null)
                {
                    string[] _division = AVM.DIVISION_VALUE.Split(',').Select(sValue => sValue.Trim(',')).ToArray();
                    AVM.DIVISION = _division;
                }

                AVM.Questionlist = (from _qList in _ServeyDBContext.HRSECURITYQUSLIST
                                    where _qList.HRSECURITYSURVEYID == SAM.HRSECURITYSURVEYID
                                    select new QuestionViewModel
                                    {
                                        QusId = _qList.HRSECURITYQUSLISTID,
                                        QUSDESCRIPTION = _qList.QUSDESCRIPTION,
                                        QUSDESCRIPTIONHINDI = _qList.QUSDESCRIPTIONHINDI,
                                        QUSDESCRIPTIONKANNADA = _qList.QUSDESCRIPTIONKANNADA,
                                        QUSDESCRIPTIONGUJARATI = _qList.QUSDESCRIPTIONGUJARATI,
                                        QUSDESCRIPTIONJAPANESE = _qList.QUSDESCRIPTIONJAPANESE,

                                        OPTIONTYPE = _qList.OPTIONTYPE,
                                        ISSUBQUESTION = _qList.ISSUBQUESTION,
                                        SUBQUESTIONPARENTID = _qList.SUBQUESTIONPARENTID,
                                        TRIGGEROPTIONIDS = _qList.TRIGGEROPTIONIDS,
                                        Status = _qList.ACTIVE,

                                        MappingList = (from _anslist in _ServeyDBContext.HRSECURITYANSLIST
                                                       where _anslist.HRSECURITYQUSLISTID == _qList.HRSECURITYQUSLISTID
                                                       select new AnswerViewModel
                                                       {
                                                           AnsId = _anslist.HRSECURITYANSLISTID,
                                                           ANSDESCRIPTION = _anslist.ANSDESCRIPTION,
                                                           ANSDESCRIPTIONHINDI = _anslist.ANSDESCRIPTIONHINDI,
                                                           ANSDESCRIPTIONKANNADA = _anslist.ANSDESCRIPTIONKANNADA,
                                                           ANSDESCRIPTIONGUJARATI = _anslist.ANSDESCRIPTIONGUJARATI,
                                                           ANSDESCRIPTIONJAPANESE = _anslist.ANSDESCRIPTIONJAPANESE,
                                                           ISCORRECT = _anslist.ISCORRECT,
                                                           Status = _anslist.ACTIVE,
                                                       }).ToList(),
                                    }).ToList();
                AVM.HRSURVEYMODULETYPE = SAM.HRSURVEYMODULETYPE == null ? "FBS" : SAM.HRSURVEYMODULETYPE; //Survey enhancement
            }
            return AVM;
        }


        public short AddPercentage(SurveyViewModel SV, int id)
        {
            short retVal = 0;
            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = DateTime.Now.Date;

            startDate = DateTime.ParseExact(SV.HRSURVEYSTARTDATE_STRING, "dd-MMM-yyyy", null);
            endDate = DateTime.ParseExact(SV.HRSURVEYENDDATE_STRING, "dd-MMM-yyyy", null);

            try
            {
                HRSECURITYSURVEY _SQ = _ServeyDBContext.HRSECURITYSURVEY.Where(x => x.HRSECURITYSURVEYID == id).FirstOrDefault();
                if (_SQ != null)
                {
                    //HRSECURITYSURVEY _survey = new HRSECURITYSURVEY();
                    //if (_survey.HRSECURITYSURVEYID == 0)
                    //{
                    //    _survey.HRSECURITYSURVEYID = _ServeyDBContext.HRSECURITYSURVEY.Max(r => r.HRSECURITYSURVEYID) + 1;
                    //}

                    _SQ.HRSURVEYDESC = SV.HRSURVEYDESC;
                    _SQ.HRSURVEYDESCHINDI = SV.HRSURVEYDESCHINDI;
                    _SQ.HRSURVEYDESCKANNADA = SV.HRSURVEYDESCKANNADA;
                    _SQ.HRSURVEYDESCGUJARATI = SV.HRSURVEYDESCGUJARATI;
                    _SQ.HRSURVEYDESCJAPANESE = SV.HRSURVEYDESCJAPANESE;
                    _SQ.HRSURVEYSTARTDATE = startDate;  // SV.HRSURVEYSTARTDATE;
                    _SQ.HRSURVEYENDDATE = endDate;      // SV.HRSURVEYENDDATE;
                    _SQ.DATEADDED = DateTime.Now;
                    _SQ.HRSURVEYTITAL = SV.HRSURVEYTITAL;
                    _SQ.ACTIVE = Convert.ToInt16(SV.ACTIVE);
                    //_SQ.RETEST = 1;
                    _SQ.RETEST = 0;
                    _SQ.ADDEDBY = SV.ADDEDBY;
                    //_SQ.SURVEYID = _SQ.HRSECURITYSURVEYID;
                    _SQ.SURVEYTYPE = _SQ.SURVEYTYPE;
                    _SQ.LOGINCNT = SV.LoginCNT;
                    _SQ.HRSURVEYHEADER = SV.HRSURVEY_HEADER;
                    _SQ.HRSURVEYHEADERHINDI = SV.HRSURVEY_HEADER_HINDI;
                    _SQ.HRSURVEYHEADERKANNADA = SV.HRSURVEY_HEADER_KANNADA;
                    _SQ.HRSURVEYHEADERGUJARATI = SV.HRSURVEY_HEADER_GUJARATI;
                    _SQ.HRSURVEYHEADERJAPANESE = SV.HRSURVEY_HEADER_JAPANESE;
                    _SQ.ISREMARKSMANDATORY = Convert.ToInt16(SV.ISREMARKSMANDATORY);

                    bool vInMAIL = SV.SURVEYRESULTMAIL;
                    long vOutMAIL = Convert.ToInt64(vInMAIL);
                    _SQ.SURVEYRESULTMAIL = vOutMAIL;

                    bool vInSTATUS = SV.FEEDBACKSTATUS;
                    short vOutSTATUS = Convert.ToInt16(vInSTATUS);
                    _SQ.FEEDBACKSTATUS = vOutSTATUS;

                    _SQ.PASSINGPERCENTAGE = SV.PASSINGPERCENTAGE;
                    string fnIds = "";
                    if (SV.FUNCTIONAL_DESIGNATION != null)
                    {
                        foreach (var val in SV.FUNCTIONAL_DESIGNATION)
                        {
                            fnIds += val.ToString() + ",";
                        }
                        ;
                    }
                    _SQ.FUNCTIONAL_DESIGNATION = fnIds;

                    string LocationIds = "";
                    if (SV.LOCATION != null)
                    {
                        foreach (var val in SV.LOCATION)
                        {
                            LocationIds += val.ToString() + ",";
                        }
                        ;
                    }

                    _SQ.LOCATION = LocationIds;

                    string operationIds = "";
                    if (SV.OPERATION != null)
                    {
                        foreach (var val in SV.OPERATION)
                        {
                            operationIds += val.ToString() + ",";
                        }
                        ;
                    }
                    _SQ.OPERATION = operationIds;

                    string divisionIds = "";
                    if (SV.DIVISION != null)
                    {
                        foreach (var val in SV.DIVISION)
                        {
                            divisionIds += val.ToString() + ",";
                        }
                        ;
                    }
                    _SQ.DIVISION = divisionIds;
                    //_ServeyDBContext.HRSECURITYSURVEY.Add(_survey);
                    _ServeyDBContext.Entry(_SQ).State = EntityState.Modified;
                    _ServeyDBContext.SaveChanges();
                    //_ServeyDBContext.Entry(_SQ).Property(r => r.RETEST).IsModified = true;
                    //_ServeyDBContext.SaveChanges();

                    //#region Deactive Old Question
                    //List<HRSECURITYQUSLIST> oldQuesList = _ServeyDBContext.HRSECURITYQUSLIST.Where(h => h.HRSECURITYSURVEYID == id).ToList();
                    //if(oldQuesList.Count > 0)
                    //{
                    //    foreach(HRSECURITYQUSLIST _Qobj in oldQuesList)
                    //    {
                    //        _Qobj.ACTIVE = 0;
                    //        _Qobj.DATELSTMOD = DateTime.Now;
                    //        _Qobj.MODBY = SV.ADDEDBY;
                    //        _ServeyDBContext.Entry(_Qobj).State = EntityState.Modified;
                    //    }
                    //}
                    //#endregion

                    //new code start
                    #region update question and answer start

                    if (SV.Questionlist != null)
                    {
                        foreach (var item in SV.Questionlist)
                        {
                            HRSECURITYQUSLIST HRQUESTION = new HRSECURITYQUSLIST();
                            HRQUESTION = _ServeyDBContext.HRSECURITYQUSLIST.Where(r => r.HRSECURITYQUSLISTID == item.QusId && r.HRSECURITYSURVEYID == _SQ.HRSECURITYSURVEYID).FirstOrDefault();
                            if (HRQUESTION != null)
                            {
                                HRQUESTION.ACTIVE = 1;
                                HRQUESTION.QUSDESCRIPTION = item.QUSDESCRIPTION;
                                HRQUESTION.QUSDESCRIPTIONHINDI = item.QUSDESCRIPTIONHINDI;
                                HRQUESTION.QUSDESCRIPTIONKANNADA = item.QUSDESCRIPTIONKANNADA;
                                HRQUESTION.QUSDESCRIPTIONGUJARATI = item.QUSDESCRIPTIONGUJARATI;
                                HRQUESTION.QUSDESCRIPTIONJAPANESE = item.QUSDESCRIPTIONJAPANESE;
                                HRQUESTION.DATELSTMOD = DateTime.Now;
                                HRQUESTION.MODBY = SV.ADDEDBY;
                                HRQUESTION.OPTIONTYPE = Convert.ToInt16(item.OPTIONTYPE);
                                HRQUESTION.SUBQUESTIONPARENTID = item.SUBQUESTIONPARENTID;
                                HRQUESTION.ISSUBQUESTION = item.ISSUBQUESTION;
                                HRQUESTION.TRIGGEROPTIONIDS = item.TRIGGEROPTIONIDS;
                                HRQUESTION.HRSECURITYSURVEYID = id;
                                _ServeyDBContext.Entry(HRQUESTION).State = EntityState.Modified;
                                _ServeyDBContext.SaveChanges();
                            }

                            foreach (var ansObj in item.MappingList)
                            {
                                HRSECURITYANSLIST HRANSWER = new HRSECURITYANSLIST();
                                HRANSWER = _ServeyDBContext.HRSECURITYANSLIST.Where(r => r.HRSECURITYANSLISTID == ansObj.AnsId && r.HRSECURITYQUSLISTID == HRQUESTION.HRSECURITYQUSLISTID).FirstOrDefault();
                                if (HRANSWER != null)
                                {
                                    HRANSWER.ANSDESCRIPTION = ansObj.ANSDESCRIPTION;
                                    HRANSWER.ANSDESCRIPTIONHINDI = ansObj.ANSDESCRIPTIONHINDI;
                                    HRANSWER.ANSDESCRIPTIONKANNADA = ansObj.ANSDESCRIPTIONKANNADA;
                                    HRANSWER.ANSDESCRIPTIONGUJARATI = ansObj.ANSDESCRIPTIONGUJARATI;
                                    HRANSWER.ANSDESCRIPTIONJAPANESE = ansObj.ANSDESCRIPTIONJAPANESE;
                                    HRANSWER.ISCORRECT = ansObj.ISCORRECT;
                                    HRANSWER.ACTIVE = ansObj.Status;
                                    HRANSWER.DATELSTMOD = DateTime.Now;
                                    HRANSWER.MODBY = SV.ADDEDBY;
                                    HRANSWER.ISFAMILYDEC_REQ = (ansObj.ISFAMILYDECREQ ? 1 : 0);
                                    _ServeyDBContext.Entry(HRANSWER).State = EntityState.Modified;
                                    _ServeyDBContext.SaveChanges();
                                }
                            }
                        }
                    }
                    #endregion update question and asnwer end

                    //Deactive the question and answer which where excluded in retest
                    var questionDetails = _ServeyDBContext.HRSECURITYQUSLIST.Where(r => r.HRSECURITYSURVEYID == id).Select(r => r.HRSECURITYQUSLISTID).ToList();
                    var activeQuestionIds = SV.Questionlist.Select(q => q.QusId).ToList();
                    foreach (var questionId in questionDetails)
                    {
                        if (!activeQuestionIds.Contains(questionId))
                        {
                            HRSECURITYQUSLIST HRQUESTION = _ServeyDBContext.HRSECURITYQUSLIST.FirstOrDefault(r => r.HRSECURITYQUSLISTID == questionId);

                            if (HRQUESTION != null)
                            {
                                HRQUESTION.ACTIVE = 0;
                                _ServeyDBContext.Entry(HRQUESTION).State = EntityState.Modified;
                                _ServeyDBContext.SaveChanges();

                                var answersToDeactivate = _ServeyDBContext.HRSECURITYANSLIST.Where(r => r.HRSECURITYQUSLISTID == HRQUESTION.HRSECURITYQUSLISTID).ToList();

                                foreach (var HRANSWER in answersToDeactivate)
                                {
                                    HRANSWER.ACTIVE = 0;
                                    _ServeyDBContext.Entry(HRANSWER).State = EntityState.Modified;
                                    _ServeyDBContext.SaveChanges();
                                }
                            }
                        }
                    }


                    //new code end

                    //old code start
                    #region Add Question and Answer
                    //foreach (var item in SV.Questionlist)
                    //{
                    //    //HRSECURITYQUSLIST oldQuesObj = _ServeyDBContext.HRSECURITYQUSLIST.Where(h => h.HRSECURITYSURVEYID == id && h.HRSECURITYQUSLISTID == item.QusId).FirstOrDefault();
                    //    //if (oldQuesObj != null)
                    //    //{
                    //    //    oldQuesObj.ACTIVE = 1;
                    //    //    oldQuesObj.DATELSTMOD = DateTime.Now;
                    //    //    oldQuesObj.MODBY = SV.ADDEDBY;
                    //    //    _ServeyDBContext.Entry(oldQuesObj).State = EntityState.Modified;
                    //    //}
                    //    //else
                    //    //{
                    //    HRSECURITYQUSLIST HRQUESTION = new HRSECURITYQUSLIST();
                    //    if (HRQUESTION.HRSECURITYQUSLISTID == 0)
                    //    {
                    //        HRQUESTION = new HRSECURITYQUSLIST();
                    //        if (_ServeyDBContext.HRSECURITYQUSLIST.ToList().Count == 0)
                    //        {
                    //            HRQUESTION.HRSECURITYQUSLISTID = 1;
                    //        }
                    //        else
                    //        {
                    //            HRQUESTION.HRSECURITYQUSLISTID = _ServeyDBContext.HRSECURITYQUSLIST.Max(r => r.HRSECURITYQUSLISTID) + 1;
                    //        }
                    //    }
                    //    HRQUESTION.ACTIVE = 1;
                    //    HRQUESTION.QUSDESCRIPTION = item.QUSDESCRIPTION;
                    //    HRQUESTION.QUSDESCRIPTIONHINDI = item.QUSDESCRIPTIONHINDI;
                    //    HRQUESTION.QUSDESCRIPTIONKANNADA = item.QUSDESCRIPTIONKANNADA;
                    //    HRQUESTION.QUSDESCRIPTIONGUJARATI = item.QUSDESCRIPTIONGUJARATI;
                    //    HRQUESTION.QUSDESCRIPTIONJAPANESE = item.QUSDESCRIPTIONJAPANESE;
                    //    HRQUESTION.HRSECURITYSURVEYID = _survey.HRSECURITYSURVEYID;

                    //    HRQUESTION.OPTIONTYPE = item.OPTIONTYPE;
                    //    HRQUESTION.ISSUBQUESTION = item.ISSUBQUESTION;
                    //    HRQUESTION.SUBQUESTIONPARENTID = item.SUBQUESTIONPARENTID > 0 ? HRQUESTION.HRSECURITYQUSLISTID - 1 : item.SUBQUESTIONPARENTID;
                    //    //HRQUESTION.TRIGGEROPTIONIDS = item.TRIGGEROPTIONIDS;

                    //    HRQUESTION.DATEADDED = DateTime.Now;
                    //    HRQUESTION.ADDEDBY = SV.ADDEDBY;
                    //    _ServeyDBContext.HRSECURITYQUSLIST.Add(HRQUESTION);
                    //    _ServeyDBContext.SaveChanges();

                    //    // List to store new matching answer IDs
                    //    //List<long> matchingAnswerIds = new List<long>();

                    //    //var previousTriggerOptionIds = item.TRIGGEROPTIONIDS;

                    //    foreach (var ansObj in item.MappingList)
                    //    {
                    //        HRSECURITYANSLIST HRANSWER = new HRSECURITYANSLIST();
                    //        if (HRANSWER.HRSECURITYANSLISTID == 0)
                    //        {
                    //            HRANSWER = new HRSECURITYANSLIST();
                    //            if (_ServeyDBContext.HRSECURITYANSLIST.ToList().Count == 0)
                    //            {
                    //                HRANSWER.HRSECURITYANSLISTID = 1;
                    //            }
                    //            else
                    //            {
                    //                HRANSWER.HRSECURITYANSLISTID = _ServeyDBContext.HRSECURITYANSLIST.Max(r => r.HRSECURITYANSLISTID) + 1;
                    //            }
                    //        }
                    //        HRANSWER.HRSECURITYQUSLISTID = HRQUESTION.HRSECURITYQUSLISTID;
                    //        HRANSWER.ANSDESCRIPTION = ansObj.ANSDESCRIPTION;
                    //        HRANSWER.ANSDESCRIPTIONHINDI = ansObj.ANSDESCRIPTIONHINDI;
                    //        HRANSWER.ANSDESCRIPTIONKANNADA = ansObj.ANSDESCRIPTIONKANNADA;
                    //        HRANSWER.ANSDESCRIPTIONGUJARATI = ansObj.ANSDESCRIPTIONGUJARATI;
                    //        HRANSWER.ANSDESCRIPTIONJAPANESE = ansObj.ANSDESCRIPTIONJAPANESE;
                    //        HRANSWER.ISCORRECT = ansObj.ISCORRECT;
                    //        HRANSWER.ACTIVE = ansObj.Status;
                    //        HRANSWER.DATEADDED = DateTime.Now;
                    //        HRANSWER.ADDEDBY = SV.ADDEDBY;
                    //        _ServeyDBContext.HRSECURITYANSLIST.Add(HRANSWER);
                    //        _ServeyDBContext.SaveChanges();



                    //    }
                    //}
                    #endregion
                    //old code end
                }
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }
        public IEnumerable<ADFUNCTIONALDESIGNATION> Bind_ADFunctionalDesignation()
        {
            IEnumerable<ADFUNCTIONALDESIGNATION> iList;
            iList = (from data in _ServeyDBContext.ADFUNCTIONALDESIGNATION.Where(x => x.ACTIVE == 1).ToList()
                     select new ADFUNCTIONALDESIGNATION
                     {
                         ADFUNCTIONALDESIGNATIONID = data.ADFUNCTIONALDESIGNATIONID,
                         DESCRIP = data.DESCRIP,
                     })
                     //------------------START-[Code Added by Aumento on 10-Apr-2024] :: SR68760 -------------------------
                     .Union(
                from designation in _ServeyDBContext.ADDESIGNATION.Where(x => x.ADDESIGNATIONID == 16 || x.ADDESIGNATIONID == 33).ToList()
                select new ADFUNCTIONALDESIGNATION
                {
                    ADFUNCTIONALDESIGNATIONID = designation.ADDESIGNATIONID,
                    DESCRIP = designation.DESCRIP
                });
            //------------------END-[Code Added by Aumento on 10-Apr-2024] :: SR68760 -------------------------

            return iList;
        }
        public IEnumerable<SYSITE> Bind_SYSite()
        {
            IEnumerable<SYSITE> iList;
            iList = (from data in _ServeyDBContext.SYSITE.Where(x => x.ACTIVE == 1).ToList()
                     select new SYSITE
                     {
                         SYSITEID = data.SYSITEID,
                         DESCRIP = data.DESCRIP,
                     });
            return iList;
        }
        public short EditSurvey(SurveyViewModel SV, int? id)
        {
            short retVal = 0;

            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = DateTime.Now.Date;
            //if (!string.IsNullOrEmpty(SV.HRSURVEYSTARTDATE_STRING))
            //{
            //    startDate = ParseDate(SV.HRSURVEYSTARTDATE_STRING);
            //    //startDate = DateTime.ParseExact(SV.HRSURVEYSTARTDATE_STRING, "dd-MMM-yyyy", null);
            //}

            //if (!string.IsNullOrEmpty(SV.HRSURVEYENDDATE_STRING))
            //{
            //    endDate = ParseDate(SV.HRSURVEYENDDATE_STRING);
            //    //endDate = DateTime.ParseExact(SV.HRSURVEYENDDATE_STRING, "dd-MMM-yyyy", null);
            //}

            startDate = DateTime.ParseExact(SV.HRSURVEYSTARTDATE_STRING, "dd-MMM-yyyy", null);
            endDate = DateTime.ParseExact(SV.HRSURVEYENDDATE_STRING, "dd-MMM-yyyy", null);


            try
            {
                HRSECURITYSURVEY _SQ = new HRSECURITYSURVEY();
                _SQ = _ServeyDBContext.HRSECURITYSURVEY.Where(r => r.HRSECURITYSURVEYID == id).FirstOrDefault();
                if (_SQ != null)
                {
                    #region Insert into HrSecuritySurvey
                    _SQ.HRSURVEYDESC = SV.HRSURVEYDESC;
                    _SQ.HRSURVEYDESCHINDI = SV.HRSURVEYDESCHINDI;
                    _SQ.HRSURVEYDESCKANNADA = SV.HRSURVEYDESCKANNADA;
                    _SQ.HRSURVEYDESCGUJARATI = SV.HRSURVEYDESCGUJARATI;
                    _SQ.HRSURVEYDESCJAPANESE = SV.HRSURVEYDESCJAPANESE;
                    _SQ.HRSURVEYSTARTDATE = startDate;  // SV.HRSURVEYSTARTDATE;
                    _SQ.HRSURVEYENDDATE = endDate;      // SV.HRSURVEYENDDATE;
                    _SQ.DATEADDED = DateTime.Now;
                    _SQ.HRSURVEYTITAL = SV.HRSURVEYTITAL;
                    _SQ.ACTIVE = Convert.ToInt16(SV.ACTIVE);
                    _SQ.RETEST = 0;
                    _SQ.ADDEDBY = SV.ADDEDBY;
                    _SQ.HRSURVEYHEADER = SV.HRSURVEY_HEADER;
                    _SQ.HRSURVEYHEADERHINDI = SV.HRSURVEY_HEADER_HINDI;
                    _SQ.HRSURVEYHEADERKANNADA = SV.HRSURVEY_HEADER_KANNADA;
                    _SQ.HRSURVEYHEADERGUJARATI = SV.HRSURVEY_HEADER_GUJARATI;
                    _SQ.HRSURVEYHEADERJAPANESE = SV.HRSURVEY_HEADER_JAPANESE;
                    _SQ.ISREMARKSMANDATORY = Convert.ToInt16(SV.ISREMARKSMANDATORY);
                    _SQ.SURVEYTYPE = SV.SurveyType;
                    _SQ.LOGINCNT = SV.LoginCNT;
                    _SQ.ISHCGSURVEY = (SV.ISHCGSURVEY == true ? 1 : 0);
                    bool vInMAIL = SV.SURVEYRESULTMAIL;
                    long vOutMAIL = Convert.ToInt64(vInMAIL);
                    _SQ.SURVEYRESULTMAIL = vOutMAIL;

                    bool vInSTATUS = SV.FEEDBACKSTATUS;
                    short vOutSTATUS = Convert.ToInt16(vInSTATUS);
                    _SQ.FEEDBACKSTATUS = vOutSTATUS;

                    //_SQ.SURVEYID = _SQ.SURVEYID;
                    string fnIds = "";
                    if (SV.FUNCTIONAL_DESIGNATION != null)
                    {
                        foreach (var val in SV.FUNCTIONAL_DESIGNATION)
                        {
                            fnIds += val.ToString() + ",";
                        }
                        ;
                    }
                    _SQ.FUNCTIONAL_DESIGNATION = fnIds;

                    string LocationIds = "";
                    if (SV.LOCATION != null)
                    {
                        foreach (var val in SV.LOCATION)
                        {
                            LocationIds += val.ToString() + ",";
                        }
                        ;
                    }

                    _SQ.LOCATION = LocationIds;

                    string operationIds = "";
                    if (SV.OPERATION != null)
                    {
                        foreach (var val in SV.OPERATION)
                        {
                            operationIds += val.ToString() + ",";
                        }
                        ;
                    }
                    _SQ.OPERATION = operationIds;

                    string divisionIds = "";
                    if (SV.DIVISION != null)
                    {
                        foreach (var val in SV.DIVISION)
                        {
                            divisionIds += val.ToString() + ",";
                        }
                        ;
                    }
                    _SQ.DIVISION = divisionIds;
                    _SQ.HRSURVEYMODULETYPE = SV.HRSURVEYMODULETYPE; //Survey enhancement

                    _ServeyDBContext.Entry(_SQ).State = EntityState.Modified;
                    _ServeyDBContext.SaveChanges();
                    #endregion

                    #region EDIT QUESTION AND ANSWER

                    if (SV.Questionlist != null)
                    {
                        foreach (var item in SV.Questionlist)
                        {
                            HRSECURITYQUSLIST HRQUESTION = new HRSECURITYQUSLIST();
                            HRQUESTION = _ServeyDBContext.HRSECURITYQUSLIST.Where(r => r.HRSECURITYQUSLISTID == item.QusId && r.HRSECURITYSURVEYID == _SQ.HRSECURITYSURVEYID).FirstOrDefault();
                            if (HRQUESTION != null)
                            {
                                HRQUESTION.ACTIVE = 1;
                                HRQUESTION.QUSDESCRIPTION = item.QUSDESCRIPTION;
                                HRQUESTION.QUSDESCRIPTIONHINDI = item.QUSDESCRIPTIONHINDI;
                                HRQUESTION.QUSDESCRIPTIONKANNADA = item.QUSDESCRIPTIONKANNADA;
                                HRQUESTION.QUSDESCRIPTIONGUJARATI = item.QUSDESCRIPTIONGUJARATI;
                                HRQUESTION.QUSDESCRIPTIONJAPANESE = item.QUSDESCRIPTIONJAPANESE;
                                HRQUESTION.OPTIONTYPE = Convert.ToInt16(item.OPTIONTYPE);
                                HRQUESTION.HRSECURITYSURVEYID = _SQ.HRSECURITYSURVEYID;
                                HRQUESTION.DATELSTMOD = DateTime.Now;
                                HRQUESTION.MODBY = SV.ADDEDBY;
                                _ServeyDBContext.Entry(HRQUESTION).State = EntityState.Modified;
                                _ServeyDBContext.SaveChanges();
                            }
                            else
                            {
                                HRQUESTION = new HRSECURITYQUSLIST();
                                if (_ServeyDBContext.HRSECURITYQUSLIST.ToList().Count == 0)
                                {
                                    HRQUESTION.HRSECURITYQUSLISTID = 1;
                                }
                                else
                                {
                                    HRQUESTION.HRSECURITYQUSLISTID = _ServeyDBContext.HRSECURITYQUSLIST.Max(r => r.HRSECURITYQUSLISTID) + 1;
                                }
                                HRQUESTION.ACTIVE = 1;
                                HRQUESTION.QUSDESCRIPTION = item.QUSDESCRIPTION;
                                HRQUESTION.QUSDESCRIPTIONHINDI = item.QUSDESCRIPTIONHINDI;
                                HRQUESTION.QUSDESCRIPTIONKANNADA = item.QUSDESCRIPTIONKANNADA;
                                HRQUESTION.QUSDESCRIPTIONGUJARATI = item.QUSDESCRIPTIONGUJARATI;
                                HRQUESTION.QUSDESCRIPTIONJAPANESE = item.QUSDESCRIPTIONJAPANESE;
                                HRQUESTION.OPTIONTYPE = Convert.ToInt16(item.OPTIONTYPE);
                                HRQUESTION.HRSECURITYSURVEYID = _SQ.HRSECURITYSURVEYID;
                                HRQUESTION.DATEADDED = DateTime.Now;
                                HRQUESTION.ADDEDBY = SV.ADDEDBY; _ServeyDBContext.HRSECURITYQUSLIST.Add(HRQUESTION);
                                _ServeyDBContext.SaveChanges();
                            }

                            foreach (var ansObj in item.MappingList)
                            {
                                HRSECURITYANSLIST HRANSWER = new HRSECURITYANSLIST();
                                HRANSWER = _ServeyDBContext.HRSECURITYANSLIST.Where(r => r.HRSECURITYANSLISTID == ansObj.AnsId && r.HRSECURITYQUSLISTID == HRQUESTION.HRSECURITYQUSLISTID).FirstOrDefault();
                                if (HRANSWER != null)
                                {
                                    HRANSWER.ANSDESCRIPTION = ansObj.ANSDESCRIPTION;
                                    HRANSWER.ANSDESCRIPTIONHINDI = ansObj.ANSDESCRIPTIONHINDI;
                                    HRANSWER.ANSDESCRIPTIONKANNADA = ansObj.ANSDESCRIPTIONKANNADA;
                                    HRANSWER.ANSDESCRIPTIONGUJARATI = ansObj.ANSDESCRIPTIONGUJARATI;
                                    HRANSWER.ANSDESCRIPTIONJAPANESE = ansObj.ANSDESCRIPTIONJAPANESE;
                                    HRANSWER.ISCORRECT = ansObj.ISCORRECT;
                                    HRANSWER.ACTIVE = ansObj.Status;
                                    HRANSWER.DATELSTMOD = DateTime.Now;
                                    HRANSWER.MODBY = SV.ADDEDBY;
                                    HRANSWER.ISFAMILYDEC_REQ = (ansObj.ISFAMILYDECREQ ? 1 : 0);
                                    _ServeyDBContext.Entry(HRANSWER).State = EntityState.Modified;
                                    _ServeyDBContext.SaveChanges();
                                }
                                else
                                {
                                    HRANSWER = new HRSECURITYANSLIST();
                                    if (_ServeyDBContext.HRSECURITYANSLIST.ToList().Count == 0)
                                    {
                                        HRANSWER.HRSECURITYANSLISTID = 1;
                                    }
                                    else
                                    {
                                        HRANSWER.HRSECURITYANSLISTID = _ServeyDBContext.HRSECURITYANSLIST.Max(r => r.HRSECURITYANSLISTID) + 1;
                                    }
                                    HRANSWER.HRSECURITYQUSLISTID = HRQUESTION.HRSECURITYQUSLISTID;
                                    HRANSWER.ANSDESCRIPTION = ansObj.ANSDESCRIPTION;
                                    HRANSWER.ANSDESCRIPTIONHINDI = ansObj.ANSDESCRIPTIONHINDI;
                                    HRANSWER.ANSDESCRIPTIONKANNADA = ansObj.ANSDESCRIPTIONKANNADA;
                                    HRANSWER.ANSDESCRIPTIONGUJARATI = ansObj.ANSDESCRIPTIONGUJARATI;
                                    HRANSWER.ANSDESCRIPTIONJAPANESE = ansObj.ANSDESCRIPTIONJAPANESE;
                                    HRANSWER.ISCORRECT = ansObj.ISCORRECT;
                                    HRANSWER.ACTIVE = ansObj.Status;
                                    HRANSWER.ISFAMILYDEC_REQ = (ansObj.ISFAMILYDECREQ ? 1 : 0);
                                    HRANSWER.DATEADDED = DateTime.Now;
                                    HRANSWER.ADDEDBY = SV.ADDEDBY;
                                    _ServeyDBContext.HRSECURITYANSLIST.Add(HRANSWER);
                                    _ServeyDBContext.SaveChanges();
                                }
                            }
                        }
                    }
                }

                #endregion

                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = -1;
                //transaction.Rollback();
            }

            return retVal;
        }
        public QuestionViewModel GetAllAnswer(int? id)
        {
            QuestionViewModel QVM = new QuestionViewModel();
            QVM = (from data in _ServeyDBContext.HRSECURITYANSLIST
                   where data.HRSECURITYQUSLISTID == id
                   select new QuestionViewModel
                   {
                       MappingList = (from _AnsList in _ServeyDBContext.HRSECURITYANSLIST
                                      where _AnsList.HRSECURITYQUSLISTID == data.HRSECURITYQUSLISTID
                                      select new AnswerViewModel
                                      {
                                          AnsId = _AnsList.HRSECURITYANSLISTID,
                                          ANSDESCRIPTION = _AnsList.ANSDESCRIPTION,
                                          ANSDESCRIPTIONHINDI = _AnsList.ANSDESCRIPTIONHINDI,
                                          ANSDESCRIPTIONKANNADA = _AnsList.ANSDESCRIPTIONKANNADA,
                                          ANSDESCRIPTIONGUJARATI = _AnsList.ANSDESCRIPTIONGUJARATI,
                                          ANSDESCRIPTIONJAPANESE = _AnsList.ANSDESCRIPTIONJAPANESE,
                                      }).ToList(),
                   }).FirstOrDefault();
            var Question = _ServeyDBContext.HRSECURITYQUSLIST.Where(r => r.HRSECURITYQUSLISTID == id).Select(r => r.QUSDESCRIPTION).ToList();
            if (Question != null && Question.Count > 0)
            {
                QVM.QUSDESCRIPTION = Question[0];
            }

            return QVM;
        }
        public IEnumerable<QuestionViewModel> UserDashboard()
        {
            IEnumerable<QuestionViewModel> iList;
            iList = (from data in _ServeyDBContext.HRSECURITYQUSLIST.ToList()
                     select new QuestionViewModel
                     {
                         QusId = data.HRSECURITYQUSLISTID,
                         QUSDESCRIPTION = data.QUSDESCRIPTION,
                         // SurveyName = data.HRSECURITYSURVEY.HRSURVEYDESC,
                         MappingList = (from _AnsList in _ServeyDBContext.HRSECURITYANSLIST
                                        where _AnsList.HRSECURITYQUSLISTID == data.HRSECURITYQUSLISTID
                                        select new AnswerViewModel
                                        {
                                            AnsId = _AnsList.HRSECURITYANSLISTID,
                                            ANSDESCRIPTION = _AnsList.ANSDESCRIPTION,
                                            ISCORRECT = _AnsList.ISCORRECT,
                                            Status = _AnsList.ACTIVE,
                                        }).ToList(),
                     });
            return iList;
        }
        //----Start--- Added by Aumento on 29-05-2024 :: SR71845
        public IEnumerable<ADDESIGNATION> Bind_ADDesignation()
        {
            IEnumerable<ADDESIGNATION> iList;
            iList = (from data in _ServeyDBContext.ADDESIGNATION.Where(x => x.ACTIVE == 1).ToList()
                     orderby data.DESCRIP
                     select new ADDESIGNATION
                     {
                         ADDESIGNATIONID = data.ADDESIGNATIONID,
                         DESCRIP = data.DESCRIP,
                     });
            return iList;
        }
        //----End--- Added by Aumento on 29-05-2024 :: SR71845

        //***********Added by Vineet For Family Declaration Check**************
        public SurveyViewModel CheckFamilyDeclarion(SurveyViewModel oSur)
        {
            SurveyViewModel AVM = new SurveyViewModel();
            HRSECURITYSURVEY SAM = new HRSECURITYSURVEY();
            //var data = (from _data in _ServeyDBContext.HRSECURITYQUSLIST.Where(m => m.HRSECURITYSURVEYID == Surveyid)
            //            join _Ans in _ServeyDBContext.HRSECURITYANSLIST on _data.HRSECURITYQUSLISTID equals _Ans.HRSECURITYQUSLISTID
            //            where _Ans.ACTIVE == 1 && _Ans.ISFAMILYDEC_REQ == 1
            //            select _Ans
            //          ).ToList()
            return AVM;
        }
        //*********************************************************************


        public IEnumerable<OperationViewModel> BindOperation(int typeId)
        {
            var iList = (from data in _ServeyDBContext.ADORGLEVEL
                         where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID && data.ADORGLEVELTYPEID == typeId
                         select new OperationViewModel
                         {
                             OPERATIONID = data.ADORGLEVELID,
                             OPERATION = data.LEVELDESCRIP
                         });
            return iList.ToList();
        }

        public IEnumerable<DivisionViewModel> BindDivision(long typeId, long op_Id)
        {
            var iList = (from data in _ServeyDBContext.ADORGLEVEL
                         where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID
                         && data.ADORGLEVELTYPEID == typeId && data.PARENTLEVELID == op_Id
                         select new DivisionViewModel
                         {
                             ADDIVISIONID = data.ADORGLEVELID,
                             DESCRIP = data.LEVELDESCRIP
                         });
            return iList.ToList();
        }

        public QuestionViewModel AddQuestionDetails(QuestionViewModel QVM)
        {
            //long retVal = 0;
            try
            {
                var _dbobj = _ServeyDBContext.HRSECURITYQUSLIST
                    .FirstOrDefault(r => r.HRSECURITYQUSLISTID == QVM.QusId && r.HRSECURITYSURVEYID == QVM.HRSECURITYSURVEYID);
                if (_dbobj == null)
                {
                    _dbobj = new HRSECURITYQUSLIST
                    {
                        ACTIVE = 1,
                        ADDEDBY = QVM.ADDEDBY,
                        DATEADDED = DateTime.Now,
                        HRSECURITYQUSLISTID = _ServeyDBContext.HRSECURITYQUSLIST.Max(r => r.HRSECURITYQUSLISTID) + 1,
                        QUSDESCRIPTION = QVM.QUSDESCRIPTION,
                        QUSDESCRIPTIONHINDI = QVM.QUSDESCRIPTIONHINDI,
                        QUSDESCRIPTIONGUJARATI = QVM.QUSDESCRIPTIONGUJARATI,
                        QUSDESCRIPTIONJAPANESE = QVM.QUSDESCRIPTIONJAPANESE,
                        QUSDESCRIPTIONKANNADA = QVM.QUSDESCRIPTIONKANNADA,
                        HRSECURITYSURVEYID = QVM.HRSECURITYSURVEYID,
                        OPTIONTYPE = QVM.OPTIONTYPE,
                        ISSUBQUESTION = QVM.ISSUBQUESTION,
                        SUBQUESTIONPARENTID = QVM.SUBQUESTIONPARENTID,
                        TRIGGEROPTIONIDS = QVM.TRIGGEROPTIONIDS
                    };

                    _ServeyDBContext.HRSECURITYQUSLIST.Add(_dbobj);
                }
                else
                {
                    _dbobj.QUSDESCRIPTION = QVM.QUSDESCRIPTION;
                    _dbobj.QUSDESCRIPTIONHINDI = QVM.QUSDESCRIPTIONHINDI;
                    _dbobj.QUSDESCRIPTIONGUJARATI = QVM.QUSDESCRIPTIONGUJARATI;
                    _dbobj.QUSDESCRIPTIONJAPANESE = QVM.QUSDESCRIPTIONJAPANESE;
                    _dbobj.QUSDESCRIPTIONKANNADA = QVM.QUSDESCRIPTIONKANNADA;
                    _dbobj.OPTIONTYPE = QVM.OPTIONTYPE;
                    _dbobj.ISSUBQUESTION = QVM.ISSUBQUESTION;
                    _dbobj.SUBQUESTIONPARENTID = QVM.SUBQUESTIONPARENTID;
                    _dbobj.TRIGGEROPTIONIDS = QVM.TRIGGEROPTIONIDS;
                    _dbobj.DATELSTMOD = DateTime.Now;
                    _dbobj.MODBY = QVM.ADDEDBY;
                    _ServeyDBContext.Entry(_dbobj).State = EntityState.Modified;
                }
                _ServeyDBContext.SaveChanges();
                QVM.QusId = _dbobj.HRSECURITYQUSLISTID;

                foreach (var X in QVM.MappingList)
                {
                    var HRANSWER = _ServeyDBContext.HRSECURITYANSLIST.FirstOrDefault(r => r.HRSECURITYANSLISTID == X.AnsId && r.HRSECURITYQUSLISTID == _dbobj.HRSECURITYQUSLISTID);

                    if (HRANSWER == null)
                    {
                        HRANSWER = new HRSECURITYANSLIST
                        {
                            HRSECURITYANSLISTID = _ServeyDBContext.HRSECURITYANSLIST.Max(r => r.HRSECURITYANSLISTID) + 1,
                            ANSDESCRIPTION = X.ANSDESCRIPTION,
                            ANSDESCRIPTIONGUJARATI = X.ANSDESCRIPTIONGUJARATI,
                            ANSDESCRIPTIONHINDI = X.ANSDESCRIPTIONHINDI,
                            ANSDESCRIPTIONKANNADA = X.ANSDESCRIPTIONKANNADA,
                            ANSDESCRIPTIONJAPANESE = X.ANSDESCRIPTIONJAPANESE,
                            ISFAMILYDEC_REQ = (X.ISFAMILYDECREQ ? 1 : 0),
                            ACTIVE = X.Status,
                            ISCORRECT = X.ISCORRECT,
                            HRSECURITYQUSLISTID = _dbobj.HRSECURITYQUSLISTID,
                            DATEADDED = DateTime.Now,
                            ADDEDBY = QVM.ADDEDBY
                        };
                        _ServeyDBContext.HRSECURITYANSLIST.Add(HRANSWER);
                    }
                    else
                    {
                        HRANSWER.ANSDESCRIPTION = X.ANSDESCRIPTION;
                        HRANSWER.ANSDESCRIPTIONGUJARATI = X.ANSDESCRIPTIONGUJARATI;
                        HRANSWER.ANSDESCRIPTIONHINDI = X.ANSDESCRIPTIONHINDI;
                        HRANSWER.ANSDESCRIPTIONKANNADA = X.ANSDESCRIPTIONKANNADA;
                        HRANSWER.ANSDESCRIPTIONJAPANESE = X.ANSDESCRIPTIONJAPANESE;
                        HRANSWER.ISFAMILYDEC_REQ = (X.ISFAMILYDECREQ ? 1 : 0);
                        HRANSWER.ACTIVE = X.Status;
                        HRANSWER.ISCORRECT = X.ISCORRECT;
                        HRANSWER.DATELSTMOD = DateTime.Now;
                        HRANSWER.MODBY = QVM.ADDEDBY;
                        _ServeyDBContext.Entry(HRANSWER).State = EntityState.Modified;
                    }
                    _ServeyDBContext.SaveChanges();
                    X.AnsId = HRANSWER.HRSECURITYANSLISTID;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return QVM;
        }

        //sa CR6927
        //public long RemoveQuestion(long questionID)
        //{
        //    long retVal = 0;
        //    try
        //    {
        //        var _dbobj = _ServeyDBContext.HRSECURITYQUSLIST
        //            .FirstOrDefault(r => r.HRSECURITYQUSLISTID == questionID);
        //        if (_dbobj != null)
        //        {
        //            _ServeyDBContext.HRSECURITYQUSLIST.Remove(_dbobj);
        //            _ServeyDBContext.SaveChanges();
        //            retVal = 1;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = 0;
        //    }
        //    return retVal;
        //}


        //hard delete from ans table
        public long RemoveQuestion(long questionID)
        {
            long retVal = 0;

            using var transaction = _ServeyDBContext.Database.BeginTransaction();
            try
            {
                // Load the question
                var question = _ServeyDBContext.HRSECURITYQUSLIST
                    .FirstOrDefault(r => r.HRSECURITYQUSLISTID == questionID);

                if (question != null)
                {
                    // Delete related rows from HRSECURITYANSLIST table

                    var related = _ServeyDBContext.HRSECURITYANSLIST
                        .Where(x => x.HRSECURITYQUSLISTID == questionID)
                        .ToList();

                    if (related.Count > 0)
                        _ServeyDBContext.HRSECURITYANSLIST.RemoveRange(related);

                    // Delete the question
                    _ServeyDBContext.HRSECURITYQUSLIST.Remove(question);

                    _ServeyDBContext.SaveChanges();
                    transaction.Commit();
                    retVal = 1;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                retVal = 0;
                // Log exception as needed
            }
            return retVal;
        }


        //Soft delete ans rows from table 
        //public long RemoveQuestion(long questionID)
        //{
        //    long retVal = 0;

        //    using var transaction = _ServeyDBContext.Database.BeginTransaction();
        //    try
        //    {
        //        var question = _ServeyDBContext.HRSECURITYQUSLIST
        //            .FirstOrDefault(r => r.HRSECURITYQUSLISTID == questionID);

        //        if (question != null)
        //        {
        //            // Remove the question
        //            _ServeyDBContext.HRSECURITYQUSLIST.Remove(question);

        //            // Update related rows in another table (example: HRSECURITYQUSSTATUS)        
        //            var relatedRows = _ServeyDBContext.HRSECURITYANSLIST
        //                .Where(x => x.HRSECURITYQUSLISTID == questionID)   // FK to the question
        //                .ToList();

        //            foreach (var row in relatedRows)
        //            {
        //                row.ACTIVE = 0;
        //                // Optionally set audit fields if you have them:
        //                // row.ModifiedOn = DateTime.UtcNow;
        //                // row.ModifiedBy = userId;
        //            }

        //            // Persist both changes atomically
        //            _ServeyDBContext.SaveChanges();
        //            transaction.Commit();
        //            retVal = 1;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction.Rollback();
        //        retVal = 0;
        //        // Consider logging ex
        //    }
        //    return retVal;
        //}


        public List<PR_Div_Dep_SecViewModel> GetEmpListForSurvey()
        {
            var employeeIdList = _ServeyDBContext.HRSECURITYSURVEY.Select(x => x.ADDEDBY).Distinct().ToList();

            var obj = _ServeyDBContext.ADEMPLOYEE
                                .Where(emp => employeeIdList.Contains(emp.ADEMPCODE))
                                .Select(data => new PR_Div_Dep_SecViewModel
                                {
                                    Text = data.FIRSTNAME + " " + data.LASTNAME,
                                    Value = (long)(data.ADEMPCODE)
                                })
                                .OrderBy(m => m.Text)
                                .ToList();
            return obj;
        }
        public IEnumerable<SurveyViewModel> GetSurveyList(SearchSurveyViewModel model)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(model.Startdate))
            {
                ReqDateFrom = DateTime.ParseExact(model.Startdate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(model.ENDDATE))
            {
                ReqDateTo = DateTime.ParseExact(model.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }
            IEnumerable<SurveyViewModel> iList;
            //iList = (from data in _ServeyDBContext.HRSECURITYSURVEY.OrderBy(r => r.HRSECURITYSURVEYID).Where(r => r.ACTIVE == 1).ToList() //Survey enhancement
            iList = (from data in _ServeyDBContext.HRSECURITYSURVEY.OrderByDescending(r => r.HRSECURITYSURVEYID).ToList() //Survey enhancement
                     where (!string.IsNullOrEmpty(model.Startdate) ? (data.DATEADDED >= ReqDateFrom) : true)
                       && (!string.IsNullOrEmpty(model.ENDDATE) ? (data.DATEADDED <= ReqDateTo) : true)
                       && (model.EmpID == 0 ? true : data.ADDEDBY == model.EmpID)
                     select new SurveyViewModel
                     {
                         HRSECURITYSURVEYID = data.HRSECURITYSURVEYID,
                         HRSURVEYDESC = data.HRSURVEYDESC,
                         HRSURVEYDESCHINDI = data.HRSURVEYDESCHINDI,
                         HRSURVEYDESCKANNADA = data.HRSURVEYDESCKANNADA,
                         HRSURVEYDESCGUJARATI = data.HRSURVEYDESCGUJARATI,
                         HRSURVEYDESCJAPANESE = data.HRSURVEYDESCJAPANESE,
                         //ACTIVE = data.ACTIVE == 0 ? false : true,
                         ACTIVE = data.ACTIVE, //Survey enhancement
                         HRSURVEYSTARTDATE = data.HRSURVEYSTARTDATE,
                         HRSURVEYENDDATE = data.HRSURVEYENDDATE,
                         RETEST = data.RETEST
                     });
            return iList;
        }
        //ea CR6927

        public long DuplicateSurvey(long oldSurveyId)
        {
            try
            {
                var oldSurveyDetails = _ServeyDBContext.HRSECURITYSURVEY.FirstOrDefault(s => s.HRSECURITYSURVEYID == oldSurveyId);

                //Now we will create a duplicate of this survey
                if (oldSurveyDetails != null)
                {
                    HRSECURITYSURVEY _newSurvey = new HRSECURITYSURVEY();
                    if (_newSurvey.HRSECURITYSURVEYID == 0)
                    {
                        _newSurvey.HRSECURITYSURVEYID = _ServeyDBContext.HRSECURITYSURVEY.Max(r => r.HRSECURITYSURVEYID) + 1;
                    }

                    #region Duplicate of HRSECURITYSURVEY table Start

                    _newSurvey.HRSURVEYDESC = oldSurveyDetails.HRSURVEYDESC;
                    _newSurvey.HRSURVEYDESCHINDI = oldSurveyDetails.HRSURVEYDESCHINDI;
                    _newSurvey.HRSURVEYDESCKANNADA = oldSurveyDetails.HRSURVEYDESCKANNADA;
                    _newSurvey.HRSURVEYDESCGUJARATI = oldSurveyDetails.HRSURVEYDESCGUJARATI;
                    _newSurvey.HRSURVEYDESCJAPANESE = oldSurveyDetails.HRSURVEYDESCJAPANESE;
                    _newSurvey.HRSURVEYSTARTDATE = oldSurveyDetails.HRSURVEYSTARTDATE;
                    _newSurvey.HRSURVEYENDDATE = oldSurveyDetails.HRSURVEYENDDATE;
                    _newSurvey.DATEADDED = DateTime.Now;
                    _newSurvey.HRSURVEYTITAL = oldSurveyDetails.HRSURVEYTITAL;
                    _newSurvey.ACTIVE = oldSurveyDetails.ACTIVE;
                    oldSurveyDetails.RETEST = 1;
                    _newSurvey.RETEST = 0;
                    _newSurvey.ADDEDBY = oldSurveyDetails.ADDEDBY;
                    _newSurvey.SURVEYID = oldSurveyDetails.HRSECURITYSURVEYID;
                    _newSurvey.SURVEYTYPE = oldSurveyDetails.SURVEYTYPE;
                    _newSurvey.LOGINCNT = oldSurveyDetails.LOGINCNT;
                    _newSurvey.HRSURVEYHEADER = oldSurveyDetails.HRSURVEYHEADER;
                    _newSurvey.HRSURVEYHEADERHINDI = oldSurveyDetails.HRSURVEYHEADERHINDI;
                    _newSurvey.HRSURVEYHEADERKANNADA = oldSurveyDetails.HRSURVEYHEADERKANNADA;
                    _newSurvey.HRSURVEYHEADERGUJARATI = oldSurveyDetails.HRSURVEYHEADERGUJARATI;
                    _newSurvey.HRSURVEYHEADERJAPANESE = oldSurveyDetails.HRSURVEYHEADERJAPANESE;
                    _newSurvey.ISREMARKSMANDATORY = oldSurveyDetails.ISREMARKSMANDATORY;
                    _newSurvey.SURVEYRESULTMAIL = oldSurveyDetails.SURVEYRESULTMAIL;
                    _newSurvey.FEEDBACKSTATUS = oldSurveyDetails.FEEDBACKSTATUS;
                    _newSurvey.ISHCGSURVEY = oldSurveyDetails.ISHCGSURVEY;
                    //_newSurvey.PASSINGPERCENTAGE = oldSurveyDetails.PASSINGPERCENTAGE;
                    _newSurvey.FUNCTIONAL_DESIGNATION = oldSurveyDetails.FUNCTIONAL_DESIGNATION;
                    _newSurvey.LOCATION = oldSurveyDetails.LOCATION;
                    _newSurvey.OPERATION = oldSurveyDetails.OPERATION;
                    _newSurvey.DIVISION = oldSurveyDetails.DIVISION;

                    _ServeyDBContext.HRSECURITYSURVEY.Add(_newSurvey);
                    _ServeyDBContext.SaveChanges();
                    _ServeyDBContext.Entry(oldSurveyDetails).Property(r => r.RETEST).IsModified = true;
                    _ServeyDBContext.SaveChanges();

                    #endregion Duplicate of HRSECURITYSURVEY table end


                    #region duplicate of question and answer Table Name - hrsecurityquslist,hrsecurityanslist 
                    var oldParentQuestionInsertedId = new List<long?>();
                    //First insert the sub question details
                    var oldSubQuestions = _ServeyDBContext.HRSECURITYQUSLIST.Where(q => q.HRSECURITYSURVEYID == oldSurveyId && q.ISSUBQUESTION == 1).ToList();

                    foreach (var oldQuestion in oldSubQuestions)
                    {

                        var newQuestion = new HRSECURITYQUSLIST
                        {
                            HRSECURITYQUSLISTID = _ServeyDBContext.HRSECURITYQUSLIST.Max(r => r.HRSECURITYQUSLISTID) + 1,
                            ACTIVE = 1,
                            QUSDESCRIPTION = oldQuestion.QUSDESCRIPTION,
                            QUSDESCRIPTIONHINDI = oldQuestion.QUSDESCRIPTIONHINDI,
                            QUSDESCRIPTIONKANNADA = oldQuestion.QUSDESCRIPTIONKANNADA,
                            QUSDESCRIPTIONGUJARATI = oldQuestion.QUSDESCRIPTIONGUJARATI,
                            QUSDESCRIPTIONJAPANESE = oldQuestion.QUSDESCRIPTIONJAPANESE,
                            HRSECURITYSURVEYID = _newSurvey.HRSECURITYSURVEYID,
                            OPTIONTYPE = oldQuestion.OPTIONTYPE,
                            ISSUBQUESTION = oldQuestion.ISSUBQUESTION,
                            SUBQUESTIONPARENTID = oldQuestion.SUBQUESTIONPARENTID,
                            TRIGGEROPTIONIDS = oldQuestion.TRIGGEROPTIONIDS,
                            DATEADDED = DateTime.Now,
                            ADDEDBY = oldQuestion.ADDEDBY
                        };

                        // Add the new question to the context and save
                        _ServeyDBContext.HRSECURITYQUSLIST.Add(newQuestion);
                        _ServeyDBContext.SaveChanges();

                        var subQuestionParentId = oldQuestion.SUBQUESTIONPARENTID;
                        oldParentQuestionInsertedId.Add(subQuestionParentId);
                        var oldTriggerOptionIds = string.IsNullOrEmpty(oldQuestion.TRIGGEROPTIONIDS)
                        ? new List<long>()
                        : oldQuestion.TRIGGEROPTIONIDS
                            .Split(',')
                            .Select(long.Parse)
                            .ToList();

                        var newTriggerOptionIds = new List<long>();

                        var oldAnswers = _ServeyDBContext.HRSECURITYANSLIST.Where(a => a.HRSECURITYQUSLISTID == oldQuestion.HRSECURITYQUSLISTID).ToList();

                        foreach (var oldAnswer in oldAnswers)
                        {
                            var newAnswer = new HRSECURITYANSLIST
                            {
                                HRSECURITYANSLISTID = _ServeyDBContext.HRSECURITYANSLIST.Max(r => r.HRSECURITYANSLISTID) + 1,
                                HRSECURITYQUSLISTID = newQuestion.HRSECURITYQUSLISTID,
                                ANSDESCRIPTION = oldAnswer.ANSDESCRIPTION,
                                ANSDESCRIPTIONHINDI = oldAnswer.ANSDESCRIPTIONHINDI,
                                ANSDESCRIPTIONKANNADA = oldAnswer.ANSDESCRIPTIONKANNADA,
                                ANSDESCRIPTIONGUJARATI = oldAnswer.ANSDESCRIPTIONGUJARATI,
                                ANSDESCRIPTIONJAPANESE = oldAnswer.ANSDESCRIPTIONJAPANESE,
                                ISCORRECT = oldAnswer.ISCORRECT,
                                ACTIVE = oldAnswer.ACTIVE,
                                DATEADDED = DateTime.Now,
                                ADDEDBY = oldAnswer.ADDEDBY,
                                ISFAMILYDEC_REQ = oldAnswer.ISFAMILYDEC_REQ,
                            };
                            _ServeyDBContext.HRSECURITYANSLIST.Add(newAnswer);
                            _ServeyDBContext.SaveChanges();
                        }

                        var oldSubQuestionsParent = _ServeyDBContext.HRSECURITYQUSLIST.Where(q => q.HRSECURITYSURVEYID == oldSurveyId && q.HRSECURITYQUSLISTID == subQuestionParentId).FirstOrDefault();
                        if (oldSubQuestionsParent != null)
                        {

                            var newSubQuestionParent = new HRSECURITYQUSLIST
                            {
                                HRSECURITYQUSLISTID = _ServeyDBContext.HRSECURITYQUSLIST.Max(r => r.HRSECURITYQUSLISTID) + 1,
                                ACTIVE = 1,
                                QUSDESCRIPTION = oldSubQuestionsParent.QUSDESCRIPTION,
                                QUSDESCRIPTIONHINDI = oldSubQuestionsParent.QUSDESCRIPTIONHINDI,
                                QUSDESCRIPTIONKANNADA = oldSubQuestionsParent.QUSDESCRIPTIONKANNADA,
                                QUSDESCRIPTIONGUJARATI = oldSubQuestionsParent.QUSDESCRIPTIONGUJARATI,
                                QUSDESCRIPTIONJAPANESE = oldSubQuestionsParent.QUSDESCRIPTIONJAPANESE,
                                HRSECURITYSURVEYID = _newSurvey.HRSECURITYSURVEYID,
                                OPTIONTYPE = oldSubQuestionsParent.OPTIONTYPE,
                                ISSUBQUESTION = oldSubQuestionsParent.ISSUBQUESTION,
                                SUBQUESTIONPARENTID = oldSubQuestionsParent.SUBQUESTIONPARENTID,
                                TRIGGEROPTIONIDS = oldSubQuestionsParent.TRIGGEROPTIONIDS,
                                DATEADDED = DateTime.Now,
                                ADDEDBY = oldSubQuestionsParent.ADDEDBY
                            };

                            // Add the new sub question parent to the context and save
                            _ServeyDBContext.HRSECURITYQUSLIST.Add(newSubQuestionParent);
                            _ServeyDBContext.SaveChanges();

                            var oldSubParentAnswers = _ServeyDBContext.HRSECURITYANSLIST.Where(a => a.HRSECURITYQUSLISTID == oldSubQuestionsParent.HRSECURITYQUSLISTID).ToList();

                            foreach (var oldSubParentAnswer in oldSubParentAnswers)
                            {
                                var newSubParentAnswer = new HRSECURITYANSLIST
                                {
                                    HRSECURITYANSLISTID = _ServeyDBContext.HRSECURITYANSLIST.Max(r => r.HRSECURITYANSLISTID) + 1,
                                    HRSECURITYQUSLISTID = newSubQuestionParent.HRSECURITYQUSLISTID,
                                    ANSDESCRIPTION = oldSubParentAnswer.ANSDESCRIPTION,
                                    ANSDESCRIPTIONHINDI = oldSubParentAnswer.ANSDESCRIPTIONHINDI,
                                    ANSDESCRIPTIONKANNADA = oldSubParentAnswer.ANSDESCRIPTIONKANNADA,
                                    ANSDESCRIPTIONGUJARATI = oldSubParentAnswer.ANSDESCRIPTIONGUJARATI,
                                    ANSDESCRIPTIONJAPANESE = oldSubParentAnswer.ANSDESCRIPTIONJAPANESE,
                                    ISCORRECT = oldSubParentAnswer.ISCORRECT,
                                    ACTIVE = oldSubParentAnswer.ACTIVE,
                                    DATEADDED = DateTime.Now,
                                    ADDEDBY = oldSubParentAnswer.ADDEDBY,
                                    ISFAMILYDEC_REQ = oldSubParentAnswer.ISFAMILYDEC_REQ,
                                };
                                _ServeyDBContext.HRSECURITYANSLIST.Add(newSubParentAnswer);
                                _ServeyDBContext.SaveChanges();

                                if (oldTriggerOptionIds.Contains(oldSubParentAnswer.HRSECURITYANSLISTID))
                                {
                                    newTriggerOptionIds.Add(newSubParentAnswer.HRSECURITYANSLISTID);
                                }
                            }

                            if (newTriggerOptionIds.Any())
                            {
                                newQuestion.TRIGGEROPTIONIDS = string.Join(",", newTriggerOptionIds);
                                newQuestion.SUBQUESTIONPARENTID = newSubQuestionParent.HRSECURITYQUSLISTID;
                                _ServeyDBContext.SaveChanges();  // Save the updated subquestion TRIGGEROPTIONIDS
                            }
                        }
                    }

                    //second step insert the parent question which are not inserted in previous step
                    var oldSurveyParentQuestions = _ServeyDBContext.HRSECURITYQUSLIST.Where(q => q.HRSECURITYSURVEYID == oldSurveyId && q.ISSUBQUESTION == 0 && !oldParentQuestionInsertedId.Contains(q.HRSECURITYQUSLISTID)).ToList();

                    foreach (var oldSurveyParentQuestion in oldSurveyParentQuestions)
                    {

                        var newSurveyParentQuestion = new HRSECURITYQUSLIST
                        {
                            HRSECURITYQUSLISTID = _ServeyDBContext.HRSECURITYQUSLIST.Max(r => r.HRSECURITYQUSLISTID) + 1,
                            ACTIVE = 1,
                            QUSDESCRIPTION = oldSurveyParentQuestion.QUSDESCRIPTION,
                            QUSDESCRIPTIONHINDI = oldSurveyParentQuestion.QUSDESCRIPTIONHINDI,
                            QUSDESCRIPTIONKANNADA = oldSurveyParentQuestion.QUSDESCRIPTIONKANNADA,
                            QUSDESCRIPTIONGUJARATI = oldSurveyParentQuestion.QUSDESCRIPTIONGUJARATI,
                            QUSDESCRIPTIONJAPANESE = oldSurveyParentQuestion.QUSDESCRIPTIONJAPANESE,
                            HRSECURITYSURVEYID = _newSurvey.HRSECURITYSURVEYID,
                            OPTIONTYPE = oldSurveyParentQuestion.OPTIONTYPE,
                            ISSUBQUESTION = oldSurveyParentQuestion.ISSUBQUESTION,
                            SUBQUESTIONPARENTID = oldSurveyParentQuestion.SUBQUESTIONPARENTID,
                            TRIGGEROPTIONIDS = oldSurveyParentQuestion.TRIGGEROPTIONIDS,
                            DATEADDED = DateTime.Now,
                            ADDEDBY = oldSurveyParentQuestion.ADDEDBY
                        };

                        // Add the new question to the context and save
                        _ServeyDBContext.HRSECURITYQUSLIST.Add(newSurveyParentQuestion);
                        _ServeyDBContext.SaveChanges();

                        var oldSurveyParentAnswers = _ServeyDBContext.HRSECURITYANSLIST.Where(a => a.HRSECURITYQUSLISTID == oldSurveyParentQuestion.HRSECURITYQUSLISTID).ToList();

                        foreach (var oldSubAnswer in oldSurveyParentAnswers)
                        {
                            var newSurveyParentAnswer = new HRSECURITYANSLIST
                            {
                                HRSECURITYANSLISTID = _ServeyDBContext.HRSECURITYANSLIST.Max(r => r.HRSECURITYANSLISTID) + 1,
                                HRSECURITYQUSLISTID = newSurveyParentQuestion.HRSECURITYQUSLISTID,
                                ANSDESCRIPTION = oldSubAnswer.ANSDESCRIPTION,
                                ANSDESCRIPTIONHINDI = oldSubAnswer.ANSDESCRIPTIONHINDI,
                                ANSDESCRIPTIONKANNADA = oldSubAnswer.ANSDESCRIPTIONKANNADA,
                                ANSDESCRIPTIONGUJARATI = oldSubAnswer.ANSDESCRIPTIONGUJARATI,
                                ANSDESCRIPTIONJAPANESE = oldSubAnswer.ANSDESCRIPTIONJAPANESE,
                                ISCORRECT = oldSubAnswer.ISCORRECT,
                                ACTIVE = oldSubAnswer.ACTIVE,
                                DATEADDED = DateTime.Now,
                                ADDEDBY = oldSubAnswer.ADDEDBY,
                                ISFAMILYDEC_REQ = oldSubAnswer.ISFAMILYDEC_REQ,
                            };
                            _ServeyDBContext.HRSECURITYANSLIST.Add(newSurveyParentAnswer);
                            _ServeyDBContext.SaveChanges();
                        }

                    }

                    #endregion

                    return _newSurvey.HRSECURITYSURVEYID;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

    }
}
