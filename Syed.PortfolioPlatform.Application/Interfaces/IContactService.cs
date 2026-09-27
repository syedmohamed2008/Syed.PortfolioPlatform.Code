using Syed.PortfolioPlatform.Application.DTOs.ContactMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syed.PortfolioPlatform.Application.Interfaces
{
    public interface IContactService
    {
        Task SaveMessageAsync(ContactMessageRequest request);
    }
}
