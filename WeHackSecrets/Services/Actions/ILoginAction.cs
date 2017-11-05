using System.Threading.Tasks;

namespace WeHackSecrets.Services.Actions
{
    public interface ILoginAction
    {
        Task LoginAsync(string user, string password);
        bool Successful { get; }
    }
}