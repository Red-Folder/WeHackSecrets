namespace WeHackSecrets.Services.Actions
{
    public interface ISecretsList
    {
        string GetTargetSecret(string secretKey);
    }
}