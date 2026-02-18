using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.DomainClasses.Enums;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ePortal.Application.Services
{
    public class LostAndFoundService : ILostAndFoundService
    {
        private readonly LostAndFoundRepository _lostAndFoundRepository;
        private readonly CommonRepository _commonRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly string _uploadPath;
        private readonly IAppConfigurationService _env;

        public LostAndFoundService(
            LostAndFoundRepository lostAndFoundRepository,
            CommonRepository commonRepository,
            IConfiguration configuration,
            IAppConfigurationService env,
            
        IEmailNotificationService emailNotificationService)
        {
            _lostAndFoundRepository = lostAndFoundRepository;
            _commonRepository = commonRepository;
            _configuration = configuration;
            _emailNotificationService = emailNotificationService;
            _env = env;

            _uploadPath = _env.GetGeneralSettings().Get_FileUpload_Path + "LostAndFound";
            // Ensure upload directory exists
            //if (!Directory.Exists(_uploadPath))
            //{
            //    Directory.CreateDirectory(_uploadPath);
            //}
        }

        public LostAndFoundViewModel GetById(long id)
        {
            var entity = _lostAndFoundRepository.GetById(id);
            return MapToViewModel(entity);
        }
        public LostAndFoundViewModel GetDetailById(long id)
        {
            var entity = _lostAndFoundRepository.GetDetailById(id);
            return MapToViewModel(entity);
        }

        public long Create(LostAndFoundViewModel model, string uploadedBy, string uploadedByEmpCode)
        {
            var entity = MapToEntity(model);
            entity.Status = 0; // Set to Pending for security review
            entity.CreatedBy = uploadedByEmpCode;
            entity.CreatedDate = DateTime.Now;
            entity.IsActive = 1;

            // Save photo if provided
            if (model.ItemPhoto != null && model.ItemPhoto.Any())
            {
                var paths = new List<string>();
                foreach (var photo in model.ItemPhoto)
                {
                    var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                    var path = SaveItemPhoto(photo, fileName);
                    paths.Add(path);
                }

                // Store as JSON
                entity.ItemPhotoPath = JsonSerializer.Serialize(paths);
            }

            return _lostAndFoundRepository.Create(entity);
        }

        public bool Update(LostAndFoundViewModel model, string modifiedBy)
        {
            var existingEntity = _lostAndFoundRepository.GetById(model.Id);
            if (existingEntity == null)
                return false;

            // Update fields
            existingEntity.ItemName = model.ProductName;
            existingEntity.ItemType = Convert.ToString(model.ItemType);
            existingEntity.IdentificationMark = model.MarkDescription;
            existingEntity.MaterialType = model.MaterialType;
            existingEntity.AdditionalDetails = model.AdditionalDetails;
            existingEntity.LocationId = model.LocationFound;
            existingEntity.Date = model.DateFound;
            existingEntity.ModifiedBy = modifiedBy;
            existingEntity.ModifiedDate = DateTime.Now;

            if (existingEntity.Status == 2)
            {
                existingEntity.IsResubmit = 0;
                existingEntity.Status = 0;
            }

            // Handle photo update
            if (model.ItemPhoto != null && model.ItemPhoto.Any())
            {
                // Delete old photo(s) if exists
                if (!string.IsNullOrEmpty(existingEntity.ItemPhotoPath))
                {
                    DeleteItemPhoto(existingEntity.ItemPhotoPath);
                }

                // Save new photos and join paths with comma
                var savedFiles = new List<string>();
                foreach (var photo in model.ItemPhoto)
                {
                    if (photo != null && photo.Length > 0)
                    {
                        var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                        var savedFile = SaveItemPhoto(photo, fileName);
                        if (!string.IsNullOrEmpty(savedFile))
                        {
                            savedFiles.Add(savedFile);
                        }
                    }
                }

                // Store all file paths as a comma-separated string
                existingEntity.ItemPhotoPath = string.Join(",", savedFiles);
            }

            return _lostAndFoundRepository.Update(existingEntity);
        }

        public bool Delete(long id, string deletedBy)
        {
            var entity = _lostAndFoundRepository.GetById(id);
            if (entity == null)
                return false;

            entity.IsActive = 0;
            entity.ModifiedBy = deletedBy;
            entity.ModifiedDate = DateTime.Now;

            return _lostAndFoundRepository.Update(entity);
        }

        public string SaveItemPhoto(IFormFile photo, string fileName)
        {
            if (photo == null || photo.Length == 0)
                return string.Empty;

            var filePath = Path.Combine(_uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                photo.CopyTo(stream);
            }

            return fileName;
        }

        public bool DeleteItemPhoto(string photoPath)
        {
            if (string.IsNullOrEmpty(photoPath))
                return true;

            var fullPath = Path.Combine(_uploadPath, photoPath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

        private LostAndFound MapToEntity(LostAndFoundViewModel viewModel)
        {
            return new LostAndFound
            {
                Id = viewModel.Id,
                ItemType = viewModel.ItemType.ToString(),
                ItemName = viewModel.ProductName,
                IdentificationMark = viewModel.MarkDescription,
                MaterialType = viewModel.MaterialType,
                AdditionalDetails = viewModel.AdditionalDetails,
                LocationId = viewModel.LocationFound,
                Date = viewModel.DateFound,
                ItemPhotoPath = viewModel.ItemPhotoPath,

                ClaimedBy = viewModel.ClaimedBy,
                DateClaimed = viewModel.DateClaimed,
                VerificationDetails = viewModel.VerificationDetails,
                IsActive = (short)(viewModel.IsActive == true ? 1 : 0),
                CreatedBy = viewModel.CreatedBy,
                CreatedDate = viewModel.CreatedDate,
                ModifiedBy = viewModel.ModifiedBy,
                ModifiedDate = viewModel.ModifiedDate,
            };
        }

        private LostAndFoundViewModel MapToViewModel(LostAndFound entity)
        {
            if (entity == null)
                return null;

            return new LostAndFoundViewModel
            {
                Id = entity.Id,
                ItemType = Enum.Parse<LostAndFoundItemType>(entity.ItemType),
                ProductName = entity.ItemName,
                MarkDescription = entity.IdentificationMark,
                MaterialType = entity.MaterialType,
                AdditionalDetails = entity.AdditionalDetails,
                LocationFound = entity.LocationId,
                DateFound = entity.Date,
                ItemPhotoPath = entity.ItemPhotoPath,
                Status = entity.Status,
                ClaimedByName = _lostAndFoundRepository.GetEmployeeNameByCodeAsync(entity.CreatedBy).Result,
                ReportedBy = _lostAndFoundRepository.GetEmployeeNameByCodeAsync(entity.CreatedBy).Result,
                ModifiedByName = _lostAndFoundRepository.GetEmployeeNameByCodeAsync(entity.ModifiedBy).Result,
                ClaimedBy = entity.ClaimedBy,
                ClaimedByEmpCode = entity.ClaimedByEmpCode,
                DateClaimed = entity.DateClaimed,
                VerificationDetails = entity.VerificationDetails,
                //Remark = entity.Remark,
                IsActive = entity.IsActive == 1 ? true : false,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                ModifiedBy = entity.ModifiedBy,
                ModifiedDate = entity.ModifiedDate,
                location = entity.Location?.DESCRIP,
                IsResubmit = entity.IsResubmit,
            };
        }

        public List<SelectListItmesVm> GetLocation()
        {
            return _lostAndFoundRepository.GetSiteList();
        }

        public List<LostAndFoundViewModel> GetAll()
        {
            var entities = _lostAndFoundRepository.GetAll();
            return entities.Select(MapToViewModel).ToList();
        }

        public List<LostAndFoundViewModel> GetAllActive()
        {
            var entities = _lostAndFoundRepository.GetAllActive();
            return entities.Select(MapToViewModel).ToList();
        }

        public List<LostAndFoundViewModel> GetByUserLocation(long userSiteId)
        {
            var entities = _lostAndFoundRepository.GetByUserLocation(userSiteId);
            return entities.Select(MapToViewModel).ToList();
        }

        public List<LostAndFoundViewModel> GetActiveByUserLocation(long userSiteId)
        {
            var entities = _lostAndFoundRepository.GetActiveByUserLocation(userSiteId);
            return entities.Select(MapToViewModel).ToList();
        }

        public string GetUserLocationName(long userSiteId)
        {
            var siteList = _lostAndFoundRepository.GetSiteList();
            var userSite = siteList.FirstOrDefault(x => x.Value == userSiteId);
            return userSite?.Text ?? "Unknown Location";
        }

        public List<SecurityLocationMapViewModel> GetAllMapping()
        {
            var result = _lostAndFoundRepository.GetAllMappingSP().ToList();
            return result;
        }

        public SecurityLocationMapViewModel? GetByIdMapping(long id)
        {
            var entity = _lostAndFoundRepository.GetByIdMappingSP(id);
            return entity == null ? null : MapToVm(entity);
        }

        public long CreateMapping(SecurityLocationMapViewModel model, string createdBy)
        {
            var entity = new SECURITY_LOCATION_MAP
            {
                SiteId = model.SiteId,
                EmpCode = model.EmpCode,
                IsActive = 1,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };
            return _lostAndFoundRepository.CreateMappingSP(entity);
        }

        public bool UpdateMapping(SecurityLocationMapViewModel model, string modifiedBy)
        {
            var entity = _lostAndFoundRepository.GetByIdMappingSP(model.Id);
            if (entity == null) return false;
            entity.SiteId = model.SiteId;
            entity.EmpCode = model.EmpCode;
            entity.IsActive = model.IsActive;
            entity.ModifiedBy = modifiedBy;
            entity.ModifiedDate = DateTime.Now;
            return _lostAndFoundRepository.UpdateMappingSP(entity);
        }

        public List<SelectListItmesVm> GetSiteList()
        {

            return _lostAndFoundRepository.GetSiteList();
        }

        public List<SelectListItmesVm> GetSecurityEmpList(string term)
        {
            return _lostAndFoundRepository.SearchEmployees(term);
        }

        private static SecurityLocationMapViewModel MapToVm(SECURITY_LOCATION_MAP e)
        {
            return new SecurityLocationMapViewModel
            {
                Id = e.Id,
                SiteId = e.SiteId,
                EmpCode = e.EmpCode,
                IsActive = e.IsActive,
                CreatedBy = e.CreatedBy,
                CreatedDate = e.CreatedDate,
                ModifiedBy = e.ModifiedBy,
                ModifiedDate = e.ModifiedDate

            };
        }
        public List<SelectListItmesVm> GetAllEmployeeMapping()
        {
            return _lostAndFoundRepository.GetAllEmployee();
        }
        public bool IsUserMappedAsSecurityPersonnel(long empCode)
        {
            return _lostAndFoundRepository.IsUserMappedAsSecurityPersonnel(empCode);
        }
  

        // Status management methods for security personnel
        public bool ApproveItem(long id, string securityRemarks, string processedBy, string processedByEmpCode)
        {
            var entity = _lostAndFoundRepository.GetById(id);
            if (entity == null) return false;

            entity.Status = ePortal.DomainClasses.LostAndFoundStatus.Approved;
            entity.ModifiedBy = processedByEmpCode;
            entity.ModifiedDate = DateTime.Now;

            var success = _lostAndFoundRepository.Update(entity);

            // Send email notification
            if (success && !string.IsNullOrEmpty(entity.CreatedBy))
            {
                AddLostAndFoundHistory(new LostAndFoundHistory
                {

                    ADDEDBY = Convert.ToInt64(processedByEmpCode),
                    UPDATEBY = Convert.ToInt64(processedByEmpCode),
                    ADDEDDATE = DateTime.Now,
                    ADEMPCODE = Convert.ToInt64(processedByEmpCode),
                    CHANGEDDATE = DateTime.Now,
                    LOST_AND_FOUND_ID = id,
                    LAF_STATUS = (short)ePortal.DomainClasses.LostAndFoundStatus.Approved,
                    REMARKS = securityRemarks
                });
                var userEmail = GetUserEmailByEmpCode(entity.CreatedBy);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var viewModel = MapToViewModel(entity);
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            _emailNotificationService.SendItemApprovedNotification(viewModel, userEmail, entity.CreatedBy);
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }
            }

            return success;
        }

        public bool SendBackItem(long id, string securityRemarks, string processedBy, string processedByEmpCode)
        {
            var entity = _lostAndFoundRepository.GetById(id);
            if (entity == null) return false;

            entity.Status = ePortal.DomainClasses.LostAndFoundStatus.SendBack;
            //entity.Status = 0;
            /*entity.Remark = securityRemarks;*/ // Use existing Remark field           
            //entity.IsActive = 1;
            entity.IsResubmit = 1;
            entity.ModifiedBy = processedByEmpCode;
            entity.ModifiedDate = DateTime.Now;

            var success = _lostAndFoundRepository.Update(entity);

            // Send email notification
            if (success && !string.IsNullOrEmpty(entity.CreatedBy))
            {
                AddLostAndFoundHistory(new LostAndFoundHistory
                {
                    
                    ADDEDBY = Convert.ToInt64(processedByEmpCode),
                    UPDATEBY = Convert.ToInt64(processedByEmpCode),
                    ADDEDDATE = DateTime.Now,
                    ADEMPCODE = Convert.ToInt64(processedByEmpCode),
                    CHANGEDDATE = DateTime.Now,
                    LOST_AND_FOUND_ID = id,
                    LAF_STATUS = (short)ePortal.DomainClasses.LostAndFoundStatus.SendBack,
                    REMARKS = securityRemarks
                });


                var userEmail = GetUserEmailByEmpCode(entity.CreatedBy);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var viewModel = MapToViewModel(entity);
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            _emailNotificationService.SendItemSentBackNotification(viewModel, userEmail, entity.CreatedBy);
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }
            }

            return success;
        }
        public bool RejectItem(long id)
        {
            var entity = _lostAndFoundRepository.GetById(id);
            if (entity == null) return false;

            entity.Status = ePortal.DomainClasses.LostAndFoundStatus.Rejected;
            entity.IsActive = 0;
            entity.ModifiedDate = DateTime.Now;

            var success = _lostAndFoundRepository.Update(entity);

            if (success && !string.IsNullOrEmpty(entity.CreatedBy))
            {
                var userEmail = GetUserEmailByEmpCode(entity.CreatedBy);
                var viewModel = MapToViewModel(entity);
                AddLostAndFoundHistory(new LostAndFoundHistory
                {
                    ADDEDDATE = DateTime.Now,
                    CHANGEDDATE = DateTime.Now,
                    LOST_AND_FOUND_ID = id,
                    LAF_STATUS = (short)ePortal.DomainClasses.LostAndFoundStatus.Rejected,
                    REMARKS = $"Dear {viewModel.ReportedBy} San,Your request id {entity.Id} raised for Lost item has been auto cancelled as no such item found or reported to Security Desk.",
                });

                if (!string.IsNullOrEmpty(userEmail))
                {
                    _emailNotificationService.SendItemRejectedNotification(viewModel, userEmail, entity.CreatedBy);
                }
            }

            return success;
        }

        public bool CloseItem(long id, string claimedBy, string claimedByEmpCode, string verificationDetails, string processedBy, string processedByEmpCode, string companyName, long? empType)
        {
            var entity = _lostAndFoundRepository.GetById(id);
            if (entity == null) return false;

            entity.Status = ePortal.DomainClasses.LostAndFoundStatus.Closed;
            entity.ClaimedBy = claimedBy;
            entity.ClaimedByEmpCode = claimedByEmpCode;
            entity.DateClaimed = DateTime.Now;
            entity.VerificationDetails = verificationDetails;
            entity.ModifiedBy = processedByEmpCode;
            entity.ModifiedDate = DateTime.Now;
            entity.CompanyName = companyName ?? string.Empty;
            entity.EmployeeType = empType == (int)ePortal.DomainClasses.LostAndFoundEmployeeType.HMSIASSOCIATE ?
                                             (int?)ePortal.DomainClasses.LostAndFoundEmployeeType.HMSIASSOCIATE
                                           : (int?)ePortal.DomainClasses.LostAndFoundEmployeeType.OTHER;

            var success = _lostAndFoundRepository.Update(entity);

            // Send email notification
            if (success && !string.IsNullOrEmpty(entity.CreatedBy))
            {
                AddLostAndFoundHistory(new LostAndFoundHistory
                {

                    ADDEDBY = Convert.ToInt64(processedByEmpCode),
                    UPDATEBY = Convert.ToInt64(processedByEmpCode),
                    ADDEDDATE = DateTime.Now,
                    ADEMPCODE = Convert.ToInt64(processedByEmpCode),
                    CHANGEDDATE = DateTime.Now,
                    LOST_AND_FOUND_ID = id,
                    LAF_STATUS = (short)ePortal.DomainClasses.LostAndFoundStatus.Closed,
                    REMARKS = "The item has been claimed and the request is now closed.",
                });
                var userEmail = GetUserEmailByEmpCode(entity.CreatedBy);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var viewModel = MapToViewModel(entity);
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            _emailNotificationService.SendItemClaimedNotification(viewModel, userEmail, entity.CreatedBy);
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }
            }

            return success;
        }

        public List<LostAndFoundViewModel> GetPendingItems(long userSiteId)
        {
            var entities = _lostAndFoundRepository.GetPendingItems(userSiteId);
            return entities.Select(MapToViewModel).ToList();
        }

        public List<LostAndFoundViewModel> GetAllItemsForSecurity(long userSiteId)
        {
            var entities = _lostAndFoundRepository.GetAllItemsForSecurity(userSiteId);
            return entities.Select(MapToViewModel).ToList();
        }
     

        public string GetUserEmailByEmpCode(string empCode)
        {
            return _lostAndFoundRepository.GetUserEmailByEmpCode(empCode);
        }

        public List<LostAndFoundViewModel> GetRequestByUserId(string userId)
        {
            var entities = _lostAndFoundRepository.GetRequestByUserId(userId);
            return entities.Select(MapToViewModel).ToList();
        }

        public bool CancelRequestById(long id)
        {
            return _lostAndFoundRepository.CancelRequestById(id);
        }

        public (string FullName, string DepartmentName, string Designation) GetEmployeeDetailsByEmpCode(long empCode)
        {
            return _lostAndFoundRepository.GetEmployeeDetailsByEmpCode(empCode);
        }

        public List<LostAndFound> GetPendingItemsOlderThanWorkingDays(int workingDays)
        {
            return _lostAndFoundRepository.GetPendingItemsOlderThanWorkingDays(workingDays);
        }
        public List<LostAndFoundViewModel> GetLostAndFoundForExprotExcel(LostAndFoundExportModel model,long userSiteId)
        {
            var entities =  _lostAndFoundRepository.GetLostAndFoundForExprotExcel(model, userSiteId);
            return entities.Select(MapToViewModel).ToList();
        }

        public string LostAndFoundExcelHtml(List<LostAndFoundViewModel> lostAndFoundList)
        {
            string str = "";
            if (lostAndFoundList != null && lostAndFoundList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>Sr No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Item Type</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Product Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Location</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Status</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Reported By</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Created Date</th>");
                stringBuilder.Append("</tr>");

                int srNo = 1;
                foreach (var item in lostAndFoundList)
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + (item.ItemType) + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + (item.ProductName ?? "") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + (item.location ?? "") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + item.DateFound.ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ePortal.DomainClasses.LostAndFoundStatus.GetStatusText(item.Status) ?? "" + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + (item.ReportedBy ?? "") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + item.CreatedDate.ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }


    
        public void AddLostAndFoundHistory(LostAndFoundHistory history)
        {
            _lostAndFoundRepository.AddLostAndFoundHistory(history);
        }
        //public List<LostAndFoundHistoryViewModel> GetHistoryByItemId(long id)
        //{
        //    var data = _lostAndFoundRepository
        //        .GetHitoryByItemid(id);

        //    var result = data.Select(x => new LostAndFoundHistoryViewModel
        //    {
        //        ID = x.ID,
        //        LOST_AND_FOUND_ID = x.LOST_AND_FOUND_ID,
        //        LAF_STATUS = x.LAF_STATUS,
        //        REMARKS = x.REMARKS,
        //        ADEMPCODE = x.ADEMPCODE,
        //        CHANGEDDATE = x.CHANGEDDATE,
        //        ADDEDBY = _lostAndFoundRepository.GetEmployeeNameByCodeAsync(Convert.ToString(x.ADDEDBY)).Result,
        //        ADDEDDATE = x.ADDEDDATE,
        //        UPDATEBY = _lostAndFoundRepository.GetEmployeeNameByCodeAsync(Convert.ToString(x.UPDATEBY)).Result,
        //        UPDATEDATE = x.UPDATEDATE
        //    }).ToList();
        //    return result;
        //}

        public List<LostAndFoundHistoryViewModel> GetHistoryByItemId(long id)
        {
            var histories = _lostAndFoundRepository.GetHitoryByItemid(id);

            if (histories == null || histories.Count == 0)
                return new List<LostAndFoundHistoryViewModel>();

            var employeeCodes = histories
                .Where(x => x.UPDATEBY.HasValue)
                .Select(x => Convert.ToString(x.UPDATEBY))
                .Distinct()
                .ToList();

            var employeeNames = _lostAndFoundRepository.GetEmployeeNamesByCodes(employeeCodes);

            var viewModels = histories.Select(x => new LostAndFoundHistoryViewModel
            {
                ID = x.ID,
                LOST_AND_FOUND_ID = x.LOST_AND_FOUND_ID,
                LAF_STATUS = ePortal.DomainClasses.LostAndFoundStatus.GetStatusText(x.LAF_STATUS),
                REMARKS = x.REMARKS,
                ADEMPCODE = x.ADEMPCODE,
                CHANGEDDATE = x.CHANGEDDATE,
                ADDEDBY = x.ADDEDBY,
                ADDEDDATE = x.ADDEDDATE,
                UPDATEBY = x.UPDATEBY.HasValue && employeeNames.TryGetValue(x.UPDATEBY.Value.ToString(), out var name)
                ? $"{x.UPDATEBY.Value} - {name}" 
                : x.UPDATEBY.HasValue
                ? x.UPDATEBY.Value.ToString()
                : string.Empty,   

                UPDATEDATE = x.UPDATEDATE
            }).ToList();

            return viewModels;
        }

        public List<long> GetSecurityMappedLocations(long empCode)
        {
            return _lostAndFoundRepository.GetSecurityMappedLocations(empCode);
        }
    }
}
