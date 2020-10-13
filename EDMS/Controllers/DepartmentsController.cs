using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using EDMS.Models;
using System.Configuration;
using System.IO;
using EDMS.Helpers;

namespace EDMS.Controllers
{
    [Route("api/Departments/{action}", Name = "DepartmentsApi")]
    public class DepartmentsController : ApiController
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        [HttpGet]
        public async Task<HttpResponseMessage> Get(DataSourceLoadOptions loadOptions) {
            var departments = _context.Departments.Select(i => new {
                i.departmentId,
                i.departmentName
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "departmentId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Request.CreateResponse(await DataSourceLoader.LoadAsync(departments, loadOptions));
        }

        /**
         * Method Functions: Create new Department entry in Database, Create new Directory in the File Structure
         */
        [HttpPost]
        public async Task<HttpResponseMessage> Post(FormDataCollection form) {

            var model = new Department();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            string rootPath = ConfigurationManager.AppSettings["storageRoot"];
            string departmentPath = rootPath + @"\" + model.departmentName;

            var createDirectory = DirectoryOperations.createDirectory(departmentPath);
            if (createDirectory)
            {
                var result = _context.Departments.Add(model);
                await _context.SaveChangesAsync();
                return Request.CreateResponse(HttpStatusCode.Created, result.departmentId);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));
            }
        }

        [HttpPut]
        public async Task<HttpResponseMessage> Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = await _context.Departments.FirstOrDefaultAsync(item => item.departmentId == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "Object not found");

            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        public async Task Delete(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = await _context.Departments.FirstOrDefaultAsync(item => item.departmentId == key);

            _context.Departments.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(Department model, IDictionary values) {
            string DEPARTMENT_ID = nameof(Department.departmentId);
            string DEPARTMENT_NAME = nameof(Department.departmentName);

            if(values.Contains(DEPARTMENT_ID)) {
                model.departmentId = Convert.ToInt32(values[DEPARTMENT_ID]);
            }

            if(values.Contains(DEPARTMENT_NAME)) {
                model.departmentName = Convert.ToString(values[DEPARTMENT_NAME]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}