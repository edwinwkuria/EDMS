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
    [Route("api/Categories/{action}", Name = "CategoriesApi")]
    public class CategoriesController : ApiController
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        [HttpGet]
        public async Task<HttpResponseMessage> Get(DataSourceLoadOptions loadOptions) {
            var categories = _context.Categories.Select(i => new {
                i.categoryId,
                i.categoryName,
                i.divisionId,
                i.departmentId
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "categoryId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Request.CreateResponse(await DataSourceLoader.LoadAsync(categories, loadOptions));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(FormDataCollection form) {
            var model = new Category();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            
            var department = await _context.Departments.FirstOrDefaultAsync(item => item.departmentId == model.departmentId);
            var division = await _context.Divisions.FirstOrDefaultAsync(item => item.divisionId == model.divisionId);
            string rootPath = ConfigurationManager.AppSettings["storageRoot"];
            string categoryPath = rootPath + @"\" + department.departmentName + @"\" + division.divisionName + @"\" + model.categoryName;

            var createDirectory = DirectoryOperations.createDirectory(categoryPath);
            if (createDirectory)
            {
                var result = _context.Categories.Add(model);
                await _context.SaveChangesAsync();

                return Request.CreateResponse(HttpStatusCode.Created, result.categoryId);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));
            }

        }

        [HttpPut]
        public async Task<HttpResponseMessage> Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = await _context.Categories.FirstOrDefaultAsync(item => item.categoryId == key);
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
            var model = await _context.Categories.FirstOrDefaultAsync(item => item.categoryId == key);

            _context.Categories.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<HttpResponseMessage> DivisionsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Divisions
                         orderby i.divisionName
                         select new {
                             Value = i.divisionId,
                             Text = i.divisionName
                         };
            return Request.CreateResponse(await DataSourceLoader.LoadAsync(lookup, loadOptions));
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

        private void PopulateModel(Category model, IDictionary values) {
            string CATEGORY_ID = nameof(Category.categoryId);
            string CATEGORYNAME = nameof(Category.categoryName);
            string DIVISION_ID = nameof(Category.divisionId);
            string DEPARTMENT_ID = nameof(Category.departmentId);

            if(values.Contains(CATEGORY_ID)) {
                model.categoryId = Convert.ToInt32(values[CATEGORY_ID]);
            }

            if(values.Contains(CATEGORYNAME)) {
                model.categoryName = Convert.ToString(values[CATEGORYNAME]);
            }

            if(values.Contains(DIVISION_ID)) {
                model.divisionId = Convert.ToInt32(values[DIVISION_ID]);
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