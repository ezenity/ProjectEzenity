namespace Ezenity.Application.Abstractions.Emails;

public interface IRazorViewRenderer
{
Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
}
