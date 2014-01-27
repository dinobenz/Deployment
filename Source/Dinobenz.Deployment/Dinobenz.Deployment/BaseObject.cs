
namespace Dinobenz.Deployment
{
    public class BaseObject
    {
        public string ProjectName { get; set; }
        public string Path { get; set; }

        public BaseObject() : this(string.Empty, string.Empty) { }
        public BaseObject(string projectName, string path)
        {
            this.ProjectName = projectName;
            this.Path = path;
        }
    }
}
