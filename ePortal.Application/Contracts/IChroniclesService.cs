using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IChroniclesService
    {
        Task<List<ChroniclesViewModel>> GetChroniclesData();
        Task<List<ChroniclesViewModel>> GetChronicleVolumeData();
        Task<List<ChroniclesViewModel>> GetChronicleVolumeDataById(int? id);
        Task<Message> AddChroniclesData(ChroniclesViewAddModel obj);
    }
}
