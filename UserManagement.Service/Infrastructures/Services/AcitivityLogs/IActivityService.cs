using Shared.Models.Dtos.Core;
using Shared.Models.Filters;

namespace User_Management.Infrastructures.Services.AcitivityLogs
{
    public interface IActivityService
    {
        Task<byte[]?> ExportAsync(PagedDto page, ActivityFilter filter);
    }
}
