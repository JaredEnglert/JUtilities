using System.Collections.Generic;

namespace Utilitarian.Settings.Test.Unit.Mocks
{
    public class MockConnectionStringProvider : IConnectionStringProvider
    {
        public Dictionary<string, string> ConnectionStrings { get; set; }

        public MockConnectionStringProvider()
        {
            ConnectionStrings = new Dictionary<string, string>();
        }

        public MockConnectionStringProvider(Dictionary<string, string> connectionStrings)
        {
            ConnectionStrings = connectionStrings;
        }

        public string Get(string key)
        {
            return ConnectionStrings[key];
        }

        public void ForceUpdate()
        {
        }
    }
}
