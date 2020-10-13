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
using EDMS.Helpers;

namespace EDMS.Controllers
{
    [Route("api/Divisions/{action}", Name = "DivisionsApi")]
    public class DivisionsController : ApiController
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        [HttpGet]
        public async Task<HttpResponseMessage> Get(DataSourceLoadOptions loadOptions) {
            var divisions = _context.Divisions.Select(i => new {
                i.divisionId,
                i.divisionName,
                i.departmentId
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "divisionId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Request.CreateResponse(await DataSourceLoader.LoadAsync(divisions, loadOptions));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(FormDataCollection form) {
            var model = new Division();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var department = await _context.Departments.FirstOrDefaultAsync(item => item.departmentId == model.departmentId);
            string rootPath = ConfigurationManager.AppSettings["storageRoot"];
            string divisionPath = rootPath + @"\" + department.departmentName + @"\" + model.divisionName;

            var createDirectory = DirectoryOperations.createDirectory(divisionPath);

            if (createDirectory)
            {
                var result = _context.Divisions.Add(model);
                await _context.SaveChangesAsync();

                return Request.CreateResponse(HttpStatusCode.Created, result.divisionId);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));
            }
            
        }

        [HttpPut]
        public async Task<HttpResponseMessage> Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = await _context.Divisions.FirstOrDefaultAsync(item => item.divisionId == key);
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
            var model = await _context.Divisions.FirstOrDefaultAsync(item => item.divisionId == key);

            _context.Divisions.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<HttpResponseMessage> DepartmentsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Departments
                         orderby i.departmentName
                         select new {
                             Value = i.departmentId,
                             Text = i.departmentName
                         };
            return Request.CreateResponse(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(Division model, IDictionary values) {
            string DIVISION_ID = nameof(Division.divisionId);
            string DIVISION_NAME = nameof(Division.divisionName);
            string DEPARTMENT_ID = nameof(Division.departmentId);

            if(values.Contains(DIVISION_ID)) {
                model.divisionId = Convert.ToInt32(values[DIVISION_ID]);
            }

            if(values.Contains(DIVISION_NAME)) {
                model.divisionName = Convert.ToString(values[DIVISION_NAME]);
            }

            if(values.Contains(DEPARTMENT_ID)) {
                model.departmentId = Convert.ToInt32(values[DEPARTMENT_ID]);
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