namespace SearchService.Interfaces
{
    public interface IMessageHandlersService
    {
        public Task<string> HandleMessage(string message);
    }
}
