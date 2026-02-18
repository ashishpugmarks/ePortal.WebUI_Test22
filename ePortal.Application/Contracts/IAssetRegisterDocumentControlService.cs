using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IAssetRegisterDocumentControlService
    {
        AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIList();
        AssetRegistrationSYKIViewModel GetSYKIData(int SYKIID);

        //Tuple<short, List<AssetRegisterDocumentControlViewModel>> InsertUpdateAssRegDocControlDTL(AssetRegisterDocumentControlViewModel model);//KD
        AssetRegisterDocumentControlViewModel InsertUpdateAssRegDocControlDTL(AssetRegisterDocumentControlViewModel model);

        List<AssetRegisterDocumentControlViewModel> GetAssetRegisterDocumentControlDetail();

        AssetRegisterDocumentControlViewModel GetAssetRegisterDocumentControlDTLbySYKI(long? SYKIID);

        AssetRegisterDocumentControlViewModel UpdateAssRegDocControlDTL(AssetRegisterDocumentControlViewModel model);


    }
}
