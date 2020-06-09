using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CaptureFingerTextVirdi.Context;
using CaptureFingerTextVirdi.Models;
using CaptureFingerTextVirdi.Repository;

namespace CaptureFingerTextVirdi
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(@"Capturing finger texts to database");

                var thread = new Thread(() => VirdiRepository.Connect());
                thread.Start();

                Console.WriteLine(@"Waiting......");

                Console.WriteLine(@"Type any key to continue after complete the proccess....");

                Console.ReadLine();

                Console.Clear();
                Console.WriteLine(@"Type 1 for read some user finger print or 2 for get all user datas and 3 for imported users");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Console.WriteLine(@"Type User Id value");
                        var userId = Console.ReadLine();

                        using (var db = new CaptureFingerTextContext())
                        {
                            var users = db.Users.Where(x => x.UserId.Contains(userId)).ToList();
                            foreach (var user in users)
                            {
                                VirdiRepository.OpenDevice(Encoding.Default.GetString(user.FingerText));
                            }
                        }
                        break;
                    case "2":
                        thread = new Thread(() => VirdiRepository.GetUserData());
                        thread.Start();
                        break;
                    case "3":
                        Console.WriteLine(@"Importing users");
                        VirdiRepository.ImportUsers();
                        break;
                    default:
                        break;
                }

                Console.WriteLine(@"Type any key to finish after complete the proccess....");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
