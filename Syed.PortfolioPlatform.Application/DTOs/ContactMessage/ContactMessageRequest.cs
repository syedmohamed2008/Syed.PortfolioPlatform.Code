using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syed.PortfolioPlatform.Application.DTOs.ContactMessage
{
    public class ContactMessageRequest
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }
    }
}
