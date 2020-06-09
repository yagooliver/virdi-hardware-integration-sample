using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptureFingerTextVirdi.Models
{
    public class User : EntityBase
    {
        public string UserName { get; set; }

        public string UserId { get; set; }

        public string Rfid { get; set; }

        public byte[] FingerText { get; set; }

        public string Name { get; set; }

        public string Document { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public int Nfp { get; set; }

        public byte[] Bfp1 { get; set; }
        public byte[] Bfp2 { get; set; }
        public byte[] Bfp3 { get; set; }
        public byte[] Bfp4 { get; set; }
    }
}
