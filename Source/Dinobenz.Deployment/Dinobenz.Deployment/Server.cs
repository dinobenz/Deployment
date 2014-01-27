
namespace Dinobenz.Deployment
{
    public class Server : BaseObject
    {
        public string ClientName { get; set; }

        public Server() : base() { }
        public Server(string projectName, string path) : base(projectName, path) { }
        public Server(string projectName, string path, string clientName) : base(projectName, path)
        {
            this.ClientName = clientName;
        }
    }
}
