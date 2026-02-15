using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Application.Abstractions.Emails;

public interface IEmailTemplateResolver
{
string GetTemplatePath(string templateName);
}
