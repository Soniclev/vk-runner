using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vk_runner
{
    internal class AccessTokenFileStorage : IAccessTokenStorage
    {
        private const string AccessTokenPath = @"..\..\token.txt";

        public bool IsAccessTokenExists()
        {
            return File.Exists(AccessTokenPath) && !String.IsNullOrWhiteSpace(ReadAccessToken());
        }

        public string ReadAccessToken()
        {
            return File.ReadAllText(AccessTokenPath);
        }

        public void WriteAccessToken(string accessToken)
        {
            File.WriteAllText(AccessTokenPath, accessToken);
        }
    }
}
