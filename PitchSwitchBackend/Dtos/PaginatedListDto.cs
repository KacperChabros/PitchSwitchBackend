
namespace PitchSwitchBackend.Dtos
{
    public class PaginatedListDto<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
