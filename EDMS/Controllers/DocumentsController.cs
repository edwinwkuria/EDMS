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

namespace EDMS.Controllers
{
    [Route("api/Documents/{action}", Name = "DocumentsApi")]
    public class DocumentsController : ApiController
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        [HttpGet]
        public async Task<HttpResponseMessage> Get(DataSourceLoadOptions loadOptions) {
            var documents = _context.Documents.Select(i => new {
                i.documentId,
                i.documentName,
                i.documentPath,
                i.documentType,
                i.documentSize,
                i.departmentId,
                i.divisionid,
                i.categoryId,
                i.documentCreate,
                i.documentModify
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "documentId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Request.CreateResponse(await DataSourceLoader.LoadAsync(documents, loadOptions));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(FormDataCollection form) {
            var model = new Document();
            /*
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));*/
            var files = form.Get("file");
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }

            //var result = _context.Documents.Add(model);
            //await _context.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.Created/*,result.documentId*/);
        }

        [HttpPut]
        public async Task<HttpResponseMessage> Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = await _context.Documents.FirstOrDefaultAsync(item => item.documentId == key);
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
            var model = await _context.Documents.FirstOrDefaultAsync(item => item.documentId == key);

            _context.Documents.Remove(model);
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
        public async Task<HttpResponseMessage> CategoriesLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Categories
                         orderby i.categoryName
                         select new {
                             Value = i.categoryId,
                             Text = i.categoryName
                         };
            return Request.CreateResponse(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(Document model, IDictionary values) {
            string DOCUMENT_ID = nameof(Document.documentId);
            string DOCUMENT_NAME = nameof(Document.documentName);
            string DOCUMENT_PATH = nameof(Document.documentPath);
            string DOCUMENT_TYPE = nameof(Document.documentType);
            string DOCUMENT_SIZE = nameof(Document.documentSize);
            string DEPARTMENT_ID = nameof(Document.departmentId);
            string DIVISIONID = nameof(Document.divisionid);
            string CATEGORY_ID = nameof(Document.categoryId);
            string DOCUMENT_CREATE = nameof(Document.documentCreate);
            string DOCUMENT_MODIFY = nameof(Document.documentModify);

            if(values.Contains(DOCUMENT_ID)) {
                model.documentId = Convert.ToInt32(values[DOCUMENT_ID]);
            }

            if(values.Contains(DOCUMENT_NAME)) {
                model.documentName = Convert.ToString(values[DOCUMENT_NAME]);
            }

            if(values.Contains(DOCUMENT_PATH)) {
                model.documentPath = Convert.ToString(values[DOCUMENT_PATH]);
            }

            if(values.Contains(DOCUMENT_TYPE)) {
                model.documentType = Convert.ToString(values[DOCUMENT_TYPE]);
            }

            if(values.Contains(DOCUMENT_SIZE)) {
                model.documentSize = Convert.ToInt32(values[DOCUMENT_SIZE]);
            }

            if(values.Contains(DEPARTMENT_ID)) {
                model.departmentId = Convert.ToInt32(values[DEPARTMENT_ID]);
            }

            if(values.Contains(DIVISIONID)) {
                model.divisionid = Convert.ToInt32(values[DIVISIONID]);
            }

            if(values.Contains(CATEGORY_ID)) {
                model.categoryId = Convert.ToInt32(values[CATEGORY_ID]);
            }

            if(values.Contains(DOCUMENT_CREATE)) {
                model.documentCreate = Convert.ToDateTime(values[DOCUMENT_CREATE]);
            }

            if(values.Contains(DOCUMENT_MODIFY)) {
                model.documentModify = Convert.ToDateTime(values[DOCUMENT_MODIFY]);
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