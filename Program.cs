using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SeansOwinRESTfulService
{
    public class Program
    {
        public static int UserCounter = 0;
        public static Dictionary<int, User> Users = new Dictionary<int, User>();

        static void Main()
        {
            Users.Add(++UserCounter, new User(UserCounter, "Sean", "London"));
            Users.Add(++UserCounter, new User(UserCounter, "Emmy", "Helsinki"));
            Users.Add(++UserCounter, new User(UserCounter, "Cosmo", "New York"));
            
            string baseAddress = "http://127.0.0.1:8080/";

            // Start OWIN host 
            using (WebApp.Start(url: baseAddress))
            {
                Console.WriteLine("Service Listening at " + baseAddress);
                System.Threading.Thread.Sleep(-1);
            }
        }
    }

    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.EnableCors();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }

        public User(int Id, string Name, string City)
        {
            this.Id = Id;
            this.Name = Name;
            this.City = City;
        }
    }

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UsersController : ApiController
    {
        // GET api/Users 
        public IEnumerable Get()
        {
            return Program.Users;
        }

        // GET api/Users/5 
        public User Get(int id)
        {
            try
            {
                return Program.Users[id];
            }
            catch (Exception e)
            {
                return new User(-1, "", "");
            }
        }

        // POST api/Users 
        public void Post([FromBody] User value)
        {
            Program.Users.Add(++Program.UserCounter, new User(Program.UserCounter, value.Name, value.City));
        }

        // PUT api/Users/5 
        public void Put(int id, [FromBody] User value)
        {
            Program.Users[id].Name = value.Name;
            Program.Users[id].City = value.City;
        }

        // DELETE api/Users/5 
        public void Delete(int id)
        {
            Program.Users.Remove(id);
        }
    }
}
