using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Infrastructure.Repositories
{
    public class ChroniclesRepository
    {
        private EPortalDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ISessionService _sessionService;
        public ChroniclesRepository(EPortalDBContext dbContext, IConfiguration configuration, ISessionService sessionService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _sessionService = sessionService;
        }
        public async Task<List<ChroniclesViewModel>> GetChroniclesData()
        {
            var query = _dbContext.T_HR_CHRONICLES
                .Where(data => data.HRC_STATUS == "A" && (data.HRC_VALID_FROM== null || data.HRC_VALID_TILL==null || (data.HRC_VALID_FROM <= DateTime.Now && data.HRC_VALID_TILL >= DateTime.Now)));

            var result =  query
                .OrderByDescending(data => data.HRC_CREATED_DATE)
                .Select(data => new ChroniclesViewModel
                {
                    HRC_TITLE = data.HRC_TITLE,
                    HRC_DESCRIPTION = data.HRC_DESCRIPTION,
                    HRC_DOC_NAME = data.HRC_DOC_NAME,
                    HRC_DOC_TYPE = data.HRC_DOC_TYPE,
                })
                .ToList();

            return result;
        }
        public async Task<List<ChroniclesViewModel>> GetChronicleVolumeData()
        {
            try
            {             
                    var _data = _dbContext.T_HR_CHRONICLES
                                           .AsEnumerable()
                                           .Select(data => new ChroniclesViewModel
                                           {
                                               HRC_ROWID = data.HRC_ROWID,
                                               HRC_TITLE = data.HRC_TITLE,
                                               HRC_DESCRIPTION = data.HRC_DESCRIPTION,
                                               HRC_VALID_FROM = data.HRC_VALID_FROM != null ? data.HRC_VALID_FROM.Value.ToString("dd-MM-yyyy") : "",
                                               HRC_VALID_TILL = data.HRC_VALID_TILL != null ? data.HRC_VALID_TILL.Value.ToString("dd-MM-yyyy") : "",
                                               HRC_CREATED_BY = data.HRC_CREATED_BY,
                                               HRC_STATUS = data.HRC_STATUS,
                                               HRC_DOC_NAME = data.HRC_DOC_NAME
                                           })
                                           .ToList();
                    return _data;                              
            }
            catch (Exception ex)
            {
                return new List<ChroniclesViewModel>();
            }
        }

        public async Task<List<ChroniclesViewModel>> GetChronicleVolumeDataById(int? id)
        {
            if (!id.HasValue)
                return null;

            try
            {
                var _data = _dbContext.T_HR_CHRONICLES
                                          .Where(x => x.HRC_ROWID == id.Value)
                                          .AsEnumerable()
                                          .Select(data => new ChroniclesViewModel
                                          {
                                              HRC_ROWID = data.HRC_ROWID,
                                              HRC_TITLE = data.HRC_TITLE,
                                              HRC_DESCRIPTION = data.HRC_DESCRIPTION,
                                              HRC_VALID_FROM = data.HRC_VALID_FROM != null ? data.HRC_VALID_FROM.Value.ToString("yyyy-MM-dd") : null,
                                              HRC_VALID_TILL = data.HRC_VALID_TILL != null ? data.HRC_VALID_TILL.Value.ToString("yyyy-MM-dd") : null,
                                              HRC_CREATED_BY = data.HRC_CREATED_BY,
                                              HRC_STATUS = data.HRC_STATUS,
                                          })
                                          .ToList();

                return _data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }      
        public async Task<Message> AddChroniclesData(ChroniclesViewAddModel obj)
        {
            try
            {
                if (obj.HRC_ROWID.HasValue)
                {
                    var existingEntity = _dbContext.T_HR_CHRONICLES.Where(x => x.HRC_ROWID == obj.HRC_ROWID).SingleOrDefault();
                    if (existingEntity != null)
                    {
                        existingEntity.HRC_TITLE = obj.HRC_TITLE;
                        existingEntity.HRC_DESCRIPTION = obj.HRC_DESCRIPTION;
                        existingEntity.HRC_VALID_FROM = obj.HRC_VALID_FROM;
                        existingEntity.HRC_VALID_TILL = obj.HRC_VALID_TILL;
                        existingEntity.HRC_STATUS = obj.HRC_STATUS;
                        existingEntity.HRC_DOC_TYPE = GetDocumentType(obj.HRC_DOC_NAME);
                        existingEntity.HRC_DOC_NAME = await SaveFileAsync(obj.HRC_DOC_NAME);
                        existingEntity.HRC_UPDATED_BY = Convert.ToString(_sessionService.Get<string>("userID"));
                        existingEntity.HRC_UPDATED_DATE = DateTime.Now;

                        _dbContext.T_HR_CHRONICLES.Update(existingEntity);
                        await _dbContext.SaveChangesAsync();
                        return new Message { type = "success", text = "Chronicles data updated successfully!" };
                    }
                }
                var chroniclesEntity = new T_HR_CHRONICLES
                {
                    HRC_TITLE = obj.HRC_TITLE,
                    HRC_DESCRIPTION = obj.HRC_DESCRIPTION,
                    HRC_VALID_FROM = obj.HRC_VALID_FROM,
                    HRC_VALID_TILL = obj.HRC_VALID_TILL,
                    HRC_STATUS = obj.HRC_STATUS,
                    HRC_DOC_TYPE = GetDocumentType(obj.HRC_DOC_NAME),
                    HRC_CREATED_BY = Convert.ToString(_sessionService.Get<string>("userID")),
                    HRC_CREATED_DATE = DateTime.Now,
                    HRC_DOC_NAME = await SaveFileAsync(obj.HRC_DOC_NAME)
                };

                _dbContext.T_HR_CHRONICLES.Add(chroniclesEntity);
                await _dbContext.SaveChangesAsync();
                return new Message { type = "success", text = "Chronicles data saved successfully!" };
            }
            catch (Exception ex)
            {
                return new Message { type = "error", text = $"Error: {ex.Message}" };
            }
        }

        private string GetDocumentType(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var videoExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".mkv", ".webm" };

            if (imageExtensions.Contains(extension)) return "IMAGE";
            if (videoExtensions.Contains(extension)) return "VIDEO";

            return "UNKNOWN";
        }
        private async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var basePath = _configuration["GeneralSettings:Get_FileUpload_Path"];
            var uploadsFolder = Path.Combine(basePath, "HR_CHRONICLES");

            var originalFileName = Path.GetFileName(file.FileName);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var uniqueFileName = $"{timestamp}_{originalFileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return uniqueFileName;
        }
    }
}
