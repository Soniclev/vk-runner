using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vk_runner
{
    internal interface IAccessTokenStorage
    {
        bool IsAccessTokenExists();
        string ReadAccessToken();
        void WriteAccessToken(string accessToken);
    }
}
