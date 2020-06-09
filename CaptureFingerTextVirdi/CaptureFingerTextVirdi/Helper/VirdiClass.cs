using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using CaptureFingerTextVirdi.Context;
using CaptureFingerTextVirdi.Models;
using UCBioBSPCOMLib;
using UCSAPICOMLib;

namespace CaptureFingerTextVirdi.Helper
{
    public class VirdiClass
    {
        public IList<int> Users = new List<int>();
        public IList<int> UsersSaved = new List<int>();
        public bool WasConnectedOnce;

        private readonly UCSAPI _ucsApi;
        private readonly ITerminalUserData _terminalUserData;
        private readonly UCBioBSP _ucBioBsp = new UCBioBSPClass();
        private IDevice _device;
        private IExtraction _extraction;
        private string _szTextEnrolledFir;
        private readonly IFPData _fpData;

        public VirdiClass()
        {
            Console.WriteLine(@"Starting Virdi constructor");
            try
            {
                _ucsApi = new UCSAPIClass();
                _terminalUserData = _ucsApi.TerminalUserData as ITerminalUserData;
                _device = _ucBioBsp.Device as IDevice;
                _extraction = _ucBioBsp.Extraction as IExtraction;
                _fpData = _ucBioBsp.FPData as IFPData;

                IniciaEventosVirdi();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"VirdiClass constructor error " + e.Message);
            }
        }

        public int StartConnection(int port = 9840, int maxClient = 255)
        {
            var result = -1;

            if (_ucsApi == null)
            {
                Console.WriteLine(@"UCSAPI IS NULL");
                return result;
            }

            try
            {
                Console.WriteLine(@"Starting virdi....");

                _ucsApi?.ServerStart(maxClient, port);

                Thread.Sleep(3000);

                result = _ucsApi.ErrorCode;

                if (result != 0)
                    throw new Exception(@"Error on connection with VIRDI - StartConnection(port,maxclient) - VirdiClass");

                Console.WriteLine(@"Connection complete");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        public bool GetUserData(int clientId, int terminalId, int userId)
        {
            _terminalUserData.GetUserDataFromTerminal(clientId, terminalId, userId);
            
            return _terminalUserData.ErrorCode == 0;
        }

        public void OpenDevice(string template)
        {
            _device.Open(0xff);

            try
            {
                if (_ucBioBsp.ErrorCode == 0)
                {
                    if (template != null)
                        _szTextEnrolledFir = template;

                    _extraction.Enroll("1004", _szTextEnrolledFir);

                    if (_ucBioBsp.ErrorCode == 0)
                        _szTextEnrolledFir = _extraction.TextFIR;
                }
                _device.Close(0xff);
            }
            catch (Exception e)
            {
                _device.Close(0xff);
            }
        }

        private int contador = 0;

        public void ImportUsers(List<User> users)
        {
            if (users.Any())
            {
                try
                {
                    var userImported = new UserImported
                    {
                        Id = Guid.NewGuid()
                    };

                    _fpData.ClearFPData();
                    var fingerClasses = new List<FingerClass>();
                    foreach (var user in users)
                    {
                        userImported.Document = user.Document;
                        userImported.Email = user.Email;
                        userImported.PhoneNumber = user.PhoneNumber;
                        userImported.Name = user.Name;
                        userImported.UserName = user.UserName;
                        userImported.UserId = user.UserId;

                        fingerClasses.Add(new FingerClass
                        {
                            IndexFinger = user.Nfp,
                            BiFinger1 = user.Bfp1, //_fpData.get_FPSampleData(user.Nfp,0) as byte[],
                            BiFinger2 = user.Bfp2, //_fpData.get_FPSampleData(user.Nfp, 1) as byte[],
                            BiFinger3 = user.Bfp3,
                            BiFinger4 = user.Bfp4,
                            FingerText = user.FingerText
                        });
                    }

                    Console.WriteLine($@"Starting {userImported.Name} importation");

                    for (var i = 0; i < fingerClasses.Count; i++)
                    {
                        _fpData.Import(i == 0 ? 1 : 0, fingerClasses[i].IndexFinger, 1, 4, fingerClasses[i].GetFirstSize(),
                            fingerClasses[i].BiFinger1, fingerClasses[i].BiFinger2);
                        //_fpData.Import(0, fingerClasses[i].IndexFinger, 1, 4, fingerClasses[i].GetSecondSize(),
                        //    fingerClasses[i].BiFinger3, fingerClasses[i].BiFinger4);
                    }

                    var fingers = _fpData.TextFIR;

                    userImported.FingerText = Encoding.Default.GetBytes(fingers);
                    //OpenDevice(fingers);

                    using (var db = new CaptureFingerTextContext())
                    {
                        db.UserImported.Add(userImported);
                        db.SaveChanges();
                    }

                    Console.WriteLine($@"User {userImported.Name} was imported");
                }
                catch(DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine(
                            $@"Entity of type {eve.Entry.Entity.GetType().Name} in state {
                                    eve.Entry.State
                                } has the following validation errors:");
                        foreach (var ve in eve.ValidationErrors)
                            Console.WriteLine($@"- Property: {ve.PropertyName}, Error: {ve.ErrorMessage}");
                    }
                    throw e;
                }
            }
        }

        public void IniciaEventosVirdi()
        {
            _ucsApi.EventTerminalConnected += EventTerminalConnected;
            _ucsApi.EventTerminalDisconnected += EventTerminalDisconnected;
            _ucsApi.EventGetUserData += EventGetUserData;
            _ucsApi.EventGetUserInfoList += EventGetUserInfoList;
        }

        private void EventGetUserInfoList(int clientId, int terminalId)
        {
            Users.Add(_terminalUserData.UserID);

            if(_terminalUserData.CurrentIndex == _terminalUserData.TotalNumber)
                Console.WriteLine($@"LAST USER OF {_terminalUserData.TotalNumber} WAS IMPORTED");
        }

        private void EventGetUserData(int clientid, int terminalid)
        {
            Console.WriteLine(@"+GetUserInfoList Event");
            var user = new User
            {
                UserId = _terminalUserData.UserID.ToString(),
                UserName = _terminalUserData.UserName,
                Id = Guid.NewGuid()
            };
            Console.WriteLine($@"    +Saving {user.UserName} on database");
            try
            {
                for (var i = 0; i < _terminalUserData.CardNumber; i++)
                {
                    user.Rfid = _terminalUserData.RFID[i];
                }

                long nFpDataCount = _terminalUserData.TotalFingerCount;
                
                for (var i = 0; i < nFpDataCount; i++)
                {
                    var nFingerId = _terminalUserData.FingerID[i];
                    long nFpDataSize1 = _terminalUserData.FPSampleDataLength[nFingerId, 0];
                    var biFpData1 = _terminalUserData.FPSampleData[nFingerId, 0] as byte[];
                    long nFpDataSize2 = _terminalUserData.FPSampleDataLength[nFingerId, 1];
                    var biFpData2 = _terminalUserData.FPSampleData[nFingerId, 1] as byte[];

                    _fpData.Import(i == 0 ? 1 : 0, nFingerId, 1, 4, (int)(nFpDataSize1 + nFpDataSize2), biFpData1,
                        biFpData2);
                }
                if (_terminalUserData.TotalFingerCount > 0 && _fpData.FIRLength > 0)
                    user.FingerText = Encoding.Default.GetBytes(_fpData.TextFIR);

                using (var db = new CaptureFingerTextContext())
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }

                Console.WriteLine(@"     +User was save successfull!");
                Console.WriteLine(@"********************");

                UsersSaved.Add(Convert.ToInt32(user.UserId));
            }
            catch (Exception e)
            {
                Console.WriteLine($@"     +Error on saving {user.UserName} - Error: {e.Message}");
                Console.WriteLine(@"********************");
            }
        }

        private void EventTerminalDisconnected(int terminalid)
        {
            Console.WriteLine($@"Virdi - Event: EventTerminalDisconnected - TerminalId: {terminalid}");
        }

        private void EventTerminalConnected(int terminalId, string terminalIp)
        {
            Console.WriteLine($@"Virdi IP: {terminalIp} - Event: EventTerminalConnected - TerminalId: {terminalId}");

            Console.WriteLine(@"+ Getting all users from terminal.....");
            if (!WasConnectedOnce)
            {
                _terminalUserData.GetUserInfoListFromTerminal(1, terminalId);
                WasConnectedOnce = true;
            }
            else
            {
                foreach (var item in Users.Where(x => !UsersSaved.Contains(x)))
                    _terminalUserData.GetUserDataFromTerminal(1, terminalId, item);
            }
        }
    }

    internal class FingerClass
    {
        public int IndexFinger { get; set; }
        public byte[] BiFinger1 { get; set; }
        public byte[] BiFinger2 { get; set; }
        public byte[] BiFinger3 { get; set; }
        public byte[] BiFinger4 { get; set; }
        public byte[] FingerText { get; set; }

        public int GetFirstSize()
        {
            return BiFinger1.Length + BiFinger2.Length;
        }

        public int GetSecondSize()
        {
            return BiFinger3.Length + BiFinger4.Length;
        }
    }
}
