using System;
using ePortal.BusinessLibraries.Contracts;
using ePortal.Repositories;
using ePortal.ViewModels;
using System.Collections.Generic;
using ePortal.DomainClasses;
using ePortal.Application.Contracts;

namespace ePortal.Application.Services
{

    public class AssetRegisterDocumentControlService : IAssetRegisterDocumentControlService
    {
        AssetRegisterDocumentControlRepository _objAssetRegisterDocumentControlRepositry;
        public AssetRegisterDocumentControlService()
        {
            _objAssetRegisterDocumentControlRepositry = new AssetRegisterDocumentControlRepository();
        }

        public AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIList()
        {
            var SYKIList = _objAssetRegisterDocumentControlRepositry.GetAssetRegistrationSYKIList();
            return SYKIList;
        }
        public AssetRegistrationSYKIViewModel GetSYKIData(int SYKIID)
        {
            var SYKIList = _objAssetRegisterDocumentControlRepositry.GetSYKIData(SYKIID);
            return SYKIList;
        }

        //public Tuple<short, List<AssetRegisterDocumentControlViewModel>> InsertUpdateAssRegDocControlDTL(AssetRegisterDocumentControlViewModel model)
        //{
        //    var PeriodSettingdata = _objAssetRegisterDocumentControlRepositry.InsertUpdateAssRegDocControlDTL(model);
        //    return PeriodSettingdata;
        //}
        public AssetRegisterDocumentControlViewModel InsertUpdateAssRegDocControlDTL(AssetRegisterDocumentControlViewModel model)
        {
            var PeriodSettingdata = _objAssetRegisterDocumentControlRepositry.InsertUpdateAssRegDocControlDTL(model);
            return PeriodSettingdata;

        }

        public List<AssetRegisterDocumentControlViewModel> GetAssetRegisterDocumentControlDetail()
        {
            var objresult = _objAssetRegisterDocumentControlRepositry.GetAssetRegisterDocumentControlDetail();
            return objresult;
        }

        public AssetRegisterDocumentControlViewModel GetAssetRegisterDocumentControlDTLbySYKI(long? SYKIID)
        {
            var objresult = _objAssetRegisterDocumentControlRepositry.GetAssetRegisterDocumentControlDTLbySYKI(SYKIID);
            return objresult;
        }

        public AssetRegisterDocumentControlViewModel UpdateAssRegDocControlDTL(AssetRegisterDocumentControlViewModel model)
        {
            var PeriodSettingdata = _objAssetRegisterDocumentControlRepositry.UpdateAssRegDocControlDTL(model);
            return PeriodSettingdata;

        }


    }
}
