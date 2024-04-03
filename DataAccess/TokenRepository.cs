using eLearningApi.Models;

namespace eLearningApi.DataAccess;

public class TokenRepository : BaseRepository<Token>
{
    public TokenRepository(AppDbContext context) : base(context) { }

    public virtual Token? GetToken(string token)
    {
        return Get(t => t.Value == token)?.FirstOrDefault();
    }
}
