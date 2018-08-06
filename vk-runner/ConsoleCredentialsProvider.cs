using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vk_runner
{
    class ConsoleCredentialsProvider : ICredentialsProvider
    {
        public ulong GetAppId()
        {
            string appId = null;

            while (true)
            {
                Console.WriteLine("Enter your application id:");
                Console.Write("> ");
                appId = Console.ReadLine();

                if (appId?.ToCharArray().All(c => char.IsDigit(c)) == true)
                {
                    break;
                }
                Console.WriteLine("The application id is invalid, try again");
            }

            return ulong.Parse(appId);
        }

        public string GetLogin()
        {
            string login = null;

            while (true)
            {
                Console.WriteLine("Enter your phone number or email:");
                Console.Write("> ");
                login = Console.ReadLine();

                if (!string.IsNullOrEmpty(login))
                {
                    break;
                }
                Console.WriteLine("The phone number or email is invalid, try again");
            }

            return login;
        }

        public string GetPassword()
        {
            string password = null;

            while (true)
            {
                Console.WriteLine("Enter your password:");
                Console.Write("> ");
                password = Console.ReadLine();

                if (!string.IsNullOrEmpty(password))
                {
                    break;
                }
                Console.WriteLine("The password is invalid, try again");
            }

            return password;
        }

        public string GetTwoFactorCode()
        {
            string appId = null;

            while (true)
            {
                Console.WriteLine("Enter code from SMS or VK administrators message for completing the two factor authorize:");
                Console.Write("> ");
                appId = Console.ReadLine();

                if (appId?.ToCharArray().All(c => char.IsDigit(c)) == true)
                {
                    break;
                }
                Console.WriteLine("The verification code is invalid, try again");
            }

            return appId;
        }
    }
}
