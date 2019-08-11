using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using EmployeeDataAccess;

namespace WebAPIDemo.Controllers
{
    [EnableCorsAttribute("*", "*", "*")]
    public class EmployeeController : ApiController
    {
        [BasicAuthenticationAttributes]
        public HttpResponseMessage Get(string gender="All")
        {
            string username = Thread.CurrentPrincipal.Identity.Name;

            using (WEBAPIEntities entities = new WEBAPIEntities())
            {
                switch (username.ToLower())
                {
                    case "male":
                        return Request.CreateResponse(HttpStatusCode.OK, 
                            entities.Employees.Where(e => e.Gender.ToLower() == "male").ToList());
                    case "female":
                        return Request.CreateResponse(HttpStatusCode.OK,
                            entities.Employees.Where(e => e.Gender.ToLower() == "female").ToList());
                    default:
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
        }

        public HttpResponseMessage Get(int id)
        {
            using (WEBAPIEntities entities = new WEBAPIEntities())
            {
                var entity = entities.Employees.FirstOrDefault(e => e.ID == id);

                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Employee with id =" + id.ToString() + "not found");
                }
            }
        }

        public HttpResponseMessage Post([FromBody]Employee employee)
        {
            try
            {
                using (WEBAPIEntities entities = new WEBAPIEntities())
                {
                    entities.Employees.Add(employee);
                    entities.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, employee);
                    message.Headers.Location = new Uri(Request.RequestUri + employee.ID.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (WEBAPIEntities entities = new WEBAPIEntities())
                {
                    var entity = entities.Employees.FirstOrDefault(e => e.ID == id);

                    if (entity == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Employee with id =" + id.ToString() + "not found");
                    }
                    else
                    {
                        entities.Employees.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(int id, [FromBody]Employee employee)
        {
            try
            {
                using (WEBAPIEntities entities = new WEBAPIEntities())
                {
                    var entity = entities.Employees.FirstOrDefault(e => e.ID == id);

                    if (entity == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Employee with id =" + id.ToString() + "not found");
                    }
                    else
                    {
                        entity.FirstName = employee.FirstName;
                        entity.LastName = employee.LastName;
                        entity.Gender = employee.Gender;
                        entity.Salary = employee.Salary;

                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
