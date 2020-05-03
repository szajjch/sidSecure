using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.DirectoryServices;
using System.Net;

namespace sidSecure
{
    class Program
    {
        public static SecurityIdentifier GetSID()
        {
            return new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid;
        }
        static void Main(string[] args)
        {
            Console.Title = "sidSecure - SID Secure System by goldblack";
            string SID = GetSID().ToString();
            string list = new WebClient()
            {
                Proxy = ((IWebProxy)null)
            }.DownloadString("https://pastebin.com/raw/YK5XWGyv");
            if (list.Contains(SID))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Successfully authenticated. SID: (" + SID + ")");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Authentication failure. SID: (" + SID + ")");
            }
            Console.ReadKey(true);
        }
    }
}