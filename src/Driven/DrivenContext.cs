namespace Driven
{
    public class DrivenContext
    {
        public DrivenContext() { }

        public DrivenContext(IRepository repository)
        {
            Repository = repository;
        }

        public IRepository Repository { get; set; }
        public Message Message { get; set; }
    }
}