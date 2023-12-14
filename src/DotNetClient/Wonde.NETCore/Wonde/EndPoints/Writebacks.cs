using System.Collections.Generic;

namespace Wonde.EndPoints

{
    public class Writebacks : BootstrapEndpoint
    {

        private string id;

        public Writebacks(string token, string id = "") : base(token, "writebacks/")
        {
            _token = token;
            if (id.Trim().Length > 0)
            {
                Uri = Uri + id + "/";
                this.id = id;
            }

        }

        public new object get(string id, string[] includes = null, Dictionary<string, string> parameters = null)
        {
            return base.get(id, includes, parameters);
        }

        public ResultIterator search(string[] includes = null, Dictionary<string, string> parameters = null)
        {
            ExtendedUri = "";
            return all(includes, parameters);
        }
    }
}