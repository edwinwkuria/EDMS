using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EDMS.Models;
using EDMS.Helpers;

namespace EDMS.Controllers
{
    public class FileUploadController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FileUpload
        public async Task<ActionResult> Index()
        {
            var documents = db.Documents.Include(d => d.Category).Include(d => d.Department).Include(d => d.Division);
            return View(await documents.ToListAsync());
        }

        // GET: FileUpload/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: FileUpload/Create
        public ActionResult Create()
        {
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName");
            ViewBag.departmentId = new SelectList(db.Departments, "departmentId", "departmentName");
            ViewBag.divisionid = new SelectList(db.Divisions, "divisionId", "divisionName");
            return View();
        }

        // POST: FileUpload/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FormCollection form)
        {
            string filePath = @"D:\EDMS\IT\Development\Back End";
            List<HttpPostedFileBase> files = new List<HttpPostedFileBase>() { Request.Files["file"] };
            foreach (var file in files)
            {
                Document document = new Document() { };
                document.documentName = file.FileName;
                document.documentPath = filePath;
                document.documentType = file.ContentType;
                document.documentSize = file.ContentLength;
                document.departmentId = 1;
                document.divisionid = 2;
                document.categoryId = 3;
                document.documentCreate = DateTime.Now;
                document.documentModify = DateTime.Now;
                
                if(FileOperations.saveFileToServer(file, filePath))
                {
                    db.Documents.Add(document);
                    await db.SaveChangesAsync();
                    //return RedirectToAction("Index");
                    return View("~/Views/Documents/Index.cshtml");
                }
            }
                

            /*ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", document.categoryId);
            ViewBag.departmentId = new SelectList(db.Departments, "departmentId", "departmentName", document.departmentId);
            ViewBag.divisionid = new SelectList(db.Divisions, "divisionId", "divisionName", document.divisionid);*/
            return View("~/Views/Home/Index.cshtml");
        }

        // GET: FileUpload/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", document.categoryId);
            ViewBag.departmentId = new SelectList(db.Departments, "departmentId", "departmentName", document.departmentId);
            ViewBag.divisionid = new SelectList(db.Divisions, "divisionId", "divisionName", document.divisionid);
            return View(document);
        }

        // POST: FileUpload/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "documentId,documentName,documentPath,documentType,documentSize,departmentId,divisionid,categoryId,documentCreate,documentModify")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", document.categoryId);
            ViewBag.departmentId = new SelectList(db.Departments, "departmentId", "departmentName", document.departmentId);
            ViewBag.divisionid = new SelectList(db.Divisions, "divisionId", "divisionName", document.divisionid);
            return View(document);
        }

        // GET: FileUpload/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: FileUpload/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Document document = await db.Documents.FindAsync(id);
            db.Documents.Remove(document);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
