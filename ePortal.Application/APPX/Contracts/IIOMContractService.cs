using ePortal.ViewModels.APPX.IOM;

namespace ePortal.Application.APPX.Contracts
{
    public interface IIOMContractService
    {
        string RemoveSpecialChar(string str);

        IOMContractOperationResult Insert_IOMDetail(IOMRequestFormSaveModel model);
        IOMContractOperationResult UPDATEIOMDETAIL(IOMRequestFormSaveModel model);
        IOMContractOperationResult IOMApprovalFormSave(IOMApprovalFormSave model);
        IOMContractOperationResult SubmitFinalDocumentSave(SubmitFinalDocumentSave model);
        IOMContractOperationResult UserAcknowledgementSave(UserAcknowledgementSave model);
        IOMContractOperationResult UserCommunicationSave(UserCommunicationSave model);
        IOMContractOperationResult ContractRenewalSave(ContractRenewalSave model);
        IOMContractOperationResult CloseContractSave(CloseContractSave model);
        IOMContractOperationResult ContractAmendmentSave(ContractAmendmentSave model);
        IOMContractOperationResult VerificationRequestFormSave(VerificationRequestFormSave model);

    }
}
