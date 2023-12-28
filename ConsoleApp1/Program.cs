using NetSDKCS;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IntPtr LoginID = IntPtr.Zero;

            try
            {
                bool success = NETClient.Init(null, IntPtr.Zero, null);
            }
            catch (Exception _)
            {
                Process.GetCurrentProcess().Kill();
            }

            if (IntPtr.Zero == LoginID)
            {
                NET_DEVICEINFO_Ex deviceInfo_ex = new NET_DEVICEINFO_Ex();
                LoginID = NETClient.LoginWithHighLevelSecurity(
                    "192.168.1.108", 37777, "admin", "123456admin",
                    EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref deviceInfo_ex);

                if (IntPtr.Zero == LoginID)
                {
                    Console.WriteLine(NETClient.GetLastError());
                    Console.ReadLine();
                    return;
                }

                /* DeviceConfig */

                NET_USER_MANAGE_INFO_NEW info = NET_USER_MANAGE_INFO_NEW.New();

                bool success = NETClient.QueryUserInfoNew(LoginID, ref info, 500);

                if (!success) goto END;

                Console.WriteLine($"\nGROUPS: {info.dwGroupNum}\n");
                
                for (int i = 0; i < info.dwGroupNum; i++)
                {
                    Console.WriteLine($"Name: {info.groupList[i].name} : {info.groupList[i].dwID}");
                    Console.WriteLine($"Rights:\n");
                    for (int j = 0; j < info.groupList[i].dwRightNum; j++)
                    {
                        Console.Write($"{info.groupList[i].rights[j]} ");
                    }
                    Console.WriteLine();
                }
                
                Console.WriteLine();

                Console.WriteLine($"RIGHTS: {info.dwRightNum}");

                for (int i = 0; i < info.dwRightNum; i++)
                {
                    Console.WriteLine($"Name: {info.rightList[i].name} : {info.rightList[i].dwID}");
                    Console.WriteLine($"Memo: {info.rightList[i].memo}");
                }

                Console.WriteLine();
                Console.WriteLine($"USERS: {info.dwUserNum}");

                for (int i = 0; i < info.dwUserNum; i++)
                {
                    var user = info.userList[i];
                    Console.WriteLine($"Name   : {user.name} : {user.dwID}");
                    Console.WriteLine($"GroupId: {user.dwGroupID}");
                    Console.WriteLine($"Rights : {user.dwRightNum}");
                    for (int j = 0; j < user.dwRightNum; j++)
                    {
                        var right = user.rights[j];
                        Console.Write($"{right} ");
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                }

            END:
                {
                    Console.WriteLine(NETClient.GetLastError());
                    NETClient.Cleanup();
                    Console.ReadLine();
                }
            }
        }
    }
}
