using System;
using System.Net.Sockets;
using UCSAPICOMLib;
using UCBioBSPCOMLib;

namespace TestVirdi
{
    class Program
    {
        private static string _mac;
        private static TcpListener _tcpListener;
        private static TcpClient _tcpClient;

        // UCSAPI
        public static UCSAPI ucsAPI;

        private static IServerUserData serveruserData;
        private static ITerminalUserData terminalUserData;
        private static IServerAuthentication serverAuthentication;
        private static IAccessLogData accessLogData;
        private static ITerminalOption terminalOption;

        // UCBioBSP
        public static UCBioBSP ucBioBSP;
        public static IFPData fpData;
        private static ITemplateInfo templateInfo;
        public static IDevice device;
        public static IExtraction extraction;
        public static IFastSearch fastSearch;
        public static IMatching matching;        
        private static string szTextEnrolledFIR;

        private static bool _isConnected;


        static void Main(string[] args)
        {
            try
            {
                ucsAPI = new UCSAPIClass();

                serveruserData = ucsAPI.ServerUserData as IServerUserData;
                terminalUserData = ucsAPI.TerminalUserData as ITerminalUserData;
                accessLogData = ucsAPI.AccessLogData as IAccessLogData;
                serverAuthentication = ucsAPI.ServerAuthentication as IServerAuthentication;
                terminalOption = ucsAPI.TerminalOption as ITerminalOption;
                // create UCBioBSP Instance
                ucBioBSP = new UCBioBSPClass();
                fpData = ucBioBSP.FPData as IFPData;
                device = ucBioBSP.Device as IDevice;
                extraction = ucBioBSP.Extraction as IExtraction;
                fastSearch = ucBioBSP.FastSearch as IFastSearch;
                matching = ucBioBSP.Matching as IMatching;

                ucsAPI.EventTerminalConnected += UCSCOMObj_EventTerminalConnected;

                ucsAPI.EventRealTimeAccessLog += UcsAPI_EventRealTimeAccessLog;
                ucsAPI.EventOpenDoor += ucsAPI_EventOpenDoor;
                ucsAPI.EventFirmwareVersion += (id, terminalId, version) =>
                {
                    Console.WriteLine("*EVENTFIRMAREVERSION*");
                    Console.WriteLine("+    ERROR CODE: {0}", ucsAPI.ErrorCode);
                };

                ucsAPI.ServerStart(255, 9840);

                ucsAPI.EventVerifyCard += (id, mode, level, rfid) =>
                {
                    Console.WriteLine("<--EventVerifyCard");
                    Console.WriteLine($"   +ErrorCode:{ucsAPI.ErrorCode}");
                    Console.WriteLine($"   +TerminalID:{id}");
                    Console.WriteLine($"   +AuthMode:{mode}");
                    Console.WriteLine($"   +Antipassback Level:{level}");
                    Console.WriteLine($"   +TextRFID:{rfid}");
                };

                ucsAPI.EventTerminalStatus += (id, terminalId, status, doorStatus, coverStatus) =>
                {
                    Console.WriteLine("<--EventTerminal Status");
                    Console.WriteLine($"   +ClientID:{0}", id);
                    Console.WriteLine($"   +TerminalID:{0}", terminalId);
                    Console.WriteLine($"   +Terminal Status:{status}");
                    Console.WriteLine($"   +Door Status:{doorStatus}");
                    Console.WriteLine($"   +Cover Status:{coverStatus}");
                    Console.WriteLine($"   +Error Code: {ucsAPI.ErrorCode}");
                    Console.WriteLine($"    +MAC: {ucsAPI.TerminalMacAddr[1]}");
                };

                ucBioBSP.OnCaptureEvent += UcBioBSP_OnCaptureEvent;

                Console.ReadKey();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }

        private static void UcBioBSP_OnCaptureEvent(int quality)
        {
            Console.WriteLine("Teste = " + quality);
        }

        static void AddUser()
        {

        }

        public static void UcsAPI_EventRealTimeAccessLog(int TerminalID)
        {
            Console.WriteLine("<--EventRealTimeAccessLog");
            Console.WriteLine($"   +TerminalID:{TerminalID}");
            Console.WriteLine($"   +ErrorCode:{ucsAPI.ErrorCode}");
            Console.WriteLine($"   +TerminalID:{TerminalID}");
            Console.WriteLine($"   +UserID:{accessLogData.UserID}");
            Console.WriteLine($"   +DataTime:{accessLogData.DateTime}");
            Console.WriteLine($"   +AuthMode:{accessLogData.AuthMode}");
            Console.WriteLine($"   +AuthType:{accessLogData.AuthType}");
            Console.WriteLine($"   +IsAuthorized:{accessLogData.IsAuthorized}");
            Console.WriteLine($"   +Device:{accessLogData.DeviceID}");
            Console.WriteLine($"   +Result:{accessLogData.AuthResult}");
            Console.WriteLine($"   +RFID:{accessLogData.RFID}");
            Console.WriteLine($"   +PictureDataLength:{accessLogData.PictureDataLength}");
            Console.WriteLine($"   +Progress:{accessLogData.CurrentIndex}/{accessLogData.TotalNumber}");
            Console.WriteLine($"   +FingerImageData:{accessLogData.FingerImageData}");
            Console.WriteLine($"   +FingerImageFormat:{accessLogData.FingerImageFormat}");
            Console.WriteLine($"   +FingerImageLength:{accessLogData.FingerImageLength}");
        }

        public static void UCSCOMObj_EventTerminalConnected(int TerminalID, string TerminalIP)
        {
            var errorCode = ucsAPI.ErrorCode;

            Console.WriteLine("<--EventTerminalConnected");
            Console.WriteLine($"   +TerminalID:{TerminalID}");
            Console.WriteLine($"   +TerminalIP:{TerminalIP}");
            Console.WriteLine($"   +ErrorCode:{errorCode}");
            Console.WriteLine($"   +Extraction: {extraction.ErrorDescription}");
        }

        public static void ucsAPI_EventOpenDoor(int ClientID, int TerminalID)
        {
            Console.WriteLine("<--EventOpenDoor");
            Console.WriteLine($"   +ClientId:{ClientID}");
            Console.WriteLine($"   +ClientID:{ClientID}");
            Console.WriteLine($"   +TerminalID:{TerminalID}");
            Console.WriteLine($"   +ErrorCode: {ucsAPI.ErrorCode}");
        }
        public static void CapturaEvento(int quality)
        {
            Console.WriteLine($"Evento capturado: {quality}");
        }

        public static void GetUserCount(int ClientID, int TerminalID, int AdminCount, int UserCount)
        {
            Console.WriteLine($"<--EventGetUserCount");
            Console.WriteLine($"   +ErrorCode: {ucsAPI.ErrorCode}");
            Console.WriteLine($"   +CID, TID : {ClientID}, {TerminalID}");
            Console.WriteLine($"   +Admin, User : {AdminCount}, {UserCount}");
        }
    }
}
