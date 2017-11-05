namespace WeHackSecrets.Services.Actions
{
    public interface ICreateSecretAction
    {
        void Create(string key, string value);
    }
}