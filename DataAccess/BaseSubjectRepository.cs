using eLearningApi.Models;

namespace eLearningApi.DataAccess
{
    public class BaseSubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        public BaseSubjectRepository(AppDbContext context) : base(context)
        {
        }
    }
}
