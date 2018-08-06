using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vk_runner
{
    internal interface ICredentialsProvider
    {
        ulong GetAppId();
        string GetLogin();
        string GetPassword();
        string GetTwoFactorCode();
    }
}
