namespace WeHackSecrets.Services.Actions
{
    public interface IAntiForgeryAction
    {
        string GetToken(string relativePath);
    }
}