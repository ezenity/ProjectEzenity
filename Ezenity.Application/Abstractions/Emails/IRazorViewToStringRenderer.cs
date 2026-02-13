namespace Ezenity.Core.Interfaces
{
    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewtoStringAsync<TModel>(string viewName, TModel model);
    }
}
