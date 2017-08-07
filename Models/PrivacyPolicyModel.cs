using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxonPartners.Models
{
    [Serializable]
    public class PrivacyPolicyModel
    {
        public string OrgName { get; set; }
        public string Address { get; set; }
        public string ProjectName { get; set; }
        public bool IsToUExist { get; set; }
        public string LinkToToU { get; set; }
        public bool HasUserRegistration { get; set; }
        public bool HasContactForm { get; set; }
        public string LinkToContactForm { get; set; }
        public bool HasCookies { get; set; }
        public bool UseCameras { get; set; }
        public bool SendMarketingEmails { get; set; }
        public bool ShareDataTo3dParties { get; set; }
        public bool CanMakePayments { get; set; }
        public bool IsExternalLinks { get; set; }
        public string Email { get; set; }
    }
}
