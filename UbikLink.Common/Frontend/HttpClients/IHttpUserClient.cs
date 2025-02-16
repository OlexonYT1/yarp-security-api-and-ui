namespace UbikLink.Common.Frontend.HttpClients
{ 
    public interface IHttpUserClient
    {
        Task<HttpResponseMessage> GetUserInfoAsync(string token);
    }
}
