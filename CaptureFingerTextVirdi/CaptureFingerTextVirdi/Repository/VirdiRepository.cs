using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using CaptureFingerTextVirdi.Context;
using CaptureFingerTextVirdi.Helper;
using CaptureFingerTextVirdi.Models;
using Dapper;

namespace CaptureFingerTextVirdi.Repository
{
    public static class VirdiRepository
    {
        public static VirdiClass VirdiModel;

        static VirdiRepository()
        {
            try
            {
                VirdiModel = new VirdiClass();
                
            }
            catch(Exception e)
            {
                Console.WriteLine(@"Error(VirdiRepository) - " + e.Message);
            }
        }

        public static void Connect()
        {
            try
            {
                if(VirdiModel == null)
                    throw new Exception("Instance of VirdiClass is null in VirdiRepository");

                VirdiModel.StartConnection();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void OpenDevice(string template)
        {
            VirdiModel?.OpenDevice(template);
        }

        public static void GetUserData()
        {
            try
            {
                Console.WriteLine($@"{(VirdiModel.Users.Any() ? "There are users" : "There are no users")}");
                foreach (var item in VirdiModel.Users)
                {
                    VirdiModel.GetUserData(1,3,item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void ImportUsers()
        {
            try
            {
                var names = GetNames();
                foreach (var name in names)
                {
                    var users = GetUsersByName(name);
                    VirdiModel?.ImportUsers(users);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static List<string> GetNames()
        {
            List<string> names;
            using (var db = new CaptureFingerTextContext())
            {
                names = db.Users.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToList();
            }

            return names;
        }

        private static List<User> GetUsersByName(string name)
        {
            using (var db = new CaptureFingerTextContext())
            {
                var users = db.Users.Where(x => x.Name.Contains(name)).OrderBy(x => x.Name).ToList();
                return users;
            }
        }
    }
}
