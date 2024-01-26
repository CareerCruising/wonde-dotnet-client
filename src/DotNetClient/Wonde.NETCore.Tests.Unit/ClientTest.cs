using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Tests
{
    [TestClass]
    public class ClientTest
    {
        private readonly string APITOKEN = "123";
        private readonly string SCHOOLID = "A1930499544";

        [TestMethod]
        public void testCreateClient()
        {
            var client = new Wonde.Client(APITOKEN);
            var school = client.school(SCHOOLID);
            Assert.IsInstanceOfType(school, typeof(Wonde.EndPoints.Schools), "Object assertion fails.");
        }

        [TestMethod]
        public void tests_schools()
        {
            var client = new Wonde.Client(APITOKEN);
            
            foreach (Dictionary<string, object> row in client.schools.all())
            {
                Assert.IsInstanceOfType(row["name"], typeof(string));
            }
        }
    }
}
