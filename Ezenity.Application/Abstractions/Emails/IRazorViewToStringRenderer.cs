namespace Ezenity.Application.Abstractions.Emails;

public interface IRazorViewToStringRenderer
{
    Task<string> RenderViewtoStringAsync<TModel>(string viewName, TModel model);
}
