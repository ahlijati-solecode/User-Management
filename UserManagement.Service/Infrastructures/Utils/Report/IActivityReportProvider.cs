using Shared.Models.Entities;

namespace User_Management.Infrastructures.Utils.Report
{
    public interface IActivityReportProvider
    {
        Task<byte[]?> GenerateReportAsync(IEnumerable<LgActivity> items);
    }
}
