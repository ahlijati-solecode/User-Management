namespace User_Management.Infrastructures.Repositories.Tasks.Aggregate
{
    public interface ITaskCountRepository
    {
        Task<int> CountAsync();
    }
}
