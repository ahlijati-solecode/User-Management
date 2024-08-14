using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Shared.Constants;
using Shared.Infrastructures.Repositories.Core;
using Shared.Models.Responses;
using Shared.Service.Infrastructures.Services.Task;

namespace User_Management.Infrastructures.Repositories.Tasks.Aggregate
{
    public class RestApiConfigurationTaskCountingRepository : BaseRestApiRepository, ITaskCountRepository
    {
        private readonly ITaskService _taskService;

        public RestApiConfigurationTaskCountingRepository(IHttpClientFactory httpClientFactory,
            ILoggerFactory loggerFactory,
            ITaskService taskService, IMemoryCache memoryCache) : base(httpClientFactory, loggerFactory, memoryCache)
        {
            _taskService = taskService;
        }

        public async Task<int> CountAsync()
        {
            try
            {

                var tasks = new List<Task<int>>();
                tasks.Add(GetCount("config/task-list/count"));
                tasks.Add(GetCount("template/task-list/count"));
                tasks.Add(GetCount("customer/task-list/count"));
                tasks.Add(GetCount("email/task-list/count"));
                Task.WaitAll(tasks.ToArray());
                var userCount = await _taskService.GePaged(new Shared.Models.Dtos.Core.PagedDto());
                return tasks.Sum(n => n.Result) + userCount.TotalItems;

            }
            catch (Exception)
            {
                return 0;

            }
        }

        public async Task<int> GetCount(string url)
        {
            try
            {
                var result = await GetAsync(ApiConstants.ConfigurationHttpClient, url);
                var body = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<BaseResponse<int>>(body);
                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return 0;
            }

        }
    }
}
