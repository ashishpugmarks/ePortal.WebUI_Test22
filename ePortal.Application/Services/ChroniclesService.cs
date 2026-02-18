using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;
using Microsoft.Extensions.Logging;

namespace ePortal.Application.Services
{
    public class ChroniclesService : IChroniclesService
    {
        private readonly ChroniclesRepository _chroniclesRepo;
        private readonly ILogger<ChroniclesService> _logger;

        public ChroniclesService(ChroniclesRepository chroniclesRepo, ILogger<ChroniclesService> logger)
        {
            _chroniclesRepo = chroniclesRepo;
            _logger = logger;
        }

        public async Task<List<ChroniclesViewModel>> GetChroniclesData()
        {
            return await _chroniclesRepo.GetChroniclesData();
        }

        public async Task<List<ChroniclesViewModel>> GetChronicleVolumeDataById(int? id)
        {
            return await _chroniclesRepo.GetChronicleVolumeDataById(id);
        }
        public async Task<List<ChroniclesViewModel>> GetChronicleVolumeData()
        {
            return await _chroniclesRepo.GetChronicleVolumeData();
        }
        public async Task<Message> AddChroniclesData(ChroniclesViewAddModel obj)
        {
            return await _chroniclesRepo.AddChroniclesData(obj);
        }
      
    }
}
