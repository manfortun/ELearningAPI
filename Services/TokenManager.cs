namespace eLearningApi.Services;

public class TokenManager
{
    private static HashSet<string> _activeTokens;

    static TokenManager()
    {
        _activeTokens = new HashSet<string>();
    }

    public bool IsTokenActive(string token)
    {
        return _activeTokens.Contains(token);
    }

    public bool ActivateToken(string token)
    {
        return _activeTokens.Add(token);
    }

    public bool DeactivateToken(string token)
    {
        return _activeTokens.Remove(token);
    }
}
