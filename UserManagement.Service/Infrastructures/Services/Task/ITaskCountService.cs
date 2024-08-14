using User_Management.Infrastructures.Repositories.Tasks.Aggregate;

namespace User_Management.Infrastructures.Services.Tasks
{
    public interface ITaskCountService
    {
        Task<int> CountAsync();
    }
    public class TaskCountService : ITaskCountService
    {
        private readonly ITaskCountRepository _taskCountRepository;

        public TaskCountService(ITaskCountRepository taskCountRepository)
        {
            _taskCountRepository = taskCountRepository;
        }
        public async Task<int> CountAsync()
        {
            var count = await _taskCountRepository.CountAsync();
            return count;
        }
    }
}
