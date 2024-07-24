using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Core.Interfaces
{
  public interface IEmailTemplateResolver
  {
    string GetTemplatePath(string templateName);
  }
}
