using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IScreenSaverService
    {
        List<ScreenSaverViewModel> GetScreenSaverMasterList(ScreenViewModel SVM);
        Int16 SaveProcessScreenSaver_Trn(ScreenSaverViewModel AVM);
        ScreenSaverViewModel GetScreenSaverDetails(long id);
        FileViewModel GetFileForDownload(Int64 id, string type);
        IEnumerable<ScreenViewModel> BindScreenSaver();
        short UpdateScreenSaver_Trn(ScreenSaverViewModel mst);
        List<ScreenSaverViewModel> ScreenSaver(ScreenViewModel SVM);
    }
}
