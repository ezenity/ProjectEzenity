using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html, string from = null);
    }
}
