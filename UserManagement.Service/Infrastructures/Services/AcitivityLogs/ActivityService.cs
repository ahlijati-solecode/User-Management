using AutoMapper;
using Shared.Infrastructures.Repositories;
using Shared.Models.Dtos.Core;
using Shared.Models.Filters;
using User_Management.Infrastructures.Utils.Report;

namespace User_Management.Infrastructures.Services.AcitivityLogs
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;
        private readonly IActivityReportProvider _reportProvider;

        public ActivityService(
            IActivityRepository activityRepository,
            IMapper mapper,
            IActivityReportProvider reportProvider)
        {
            _activityRepository = activityRepository;
            _mapper = mapper;
            _reportProvider = reportProvider;
        }
        public async Task<byte[]?> ExportAsync(PagedDto page, ActivityFilter filter)
        {
            page.Page = 1;
            page.Size = int.MaxValue;
            var activities = await _activityRepository.GetPaged(_mapper.Map<PageFilter>(page), filter);
            return await _reportProvider.GenerateReportAsync(activities.Items);
        }
    }
}
