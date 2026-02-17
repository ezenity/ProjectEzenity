namespace Ezenity.Application.Abstractions.Emails;

public interface IEmailTemplateResolver
{
string GetTemplatePath(string templateName);
}
