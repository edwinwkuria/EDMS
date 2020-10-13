using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EDMS.Models
{
    public class Organization
    {
    }

    public class Department
    {
        [Key]
        public int departmentId { get; set; }
        public string departmentName { get; set; }
    }
    public class Division
    {
        [Key]
        public int divisionId { get; set; }
        public string divisionName { get; set; }
        [ForeignKey("Department")]
        public int departmentId { get; set; }
        public virtual Department Department { get; set; }

    }
    public class Category
    {
        [Key]
        public int categoryId { get; set; }
        public string  categoryName { get; set; }

        [ForeignKey("Division")]
        public int divisionId { get; set; }
        public virtual Division Division { get; set; }

        [ForeignKey("Department")]
        public int departmentId { get; set; }
        public virtual Department Department { get; set; }

    }
    public class Document
    {
        public int documentId { get; set; }
        public string documentName { get; set; }
        public string documentPath { get; set; }
        public string documentType { get; set; }
        public int documentSize { get; set; }
        [ForeignKey("Department")]
        public int departmentId { get; set; }
        public virtual Department Department { get; set; }
        [ForeignKey("Division")]
        public int divisionid { get; set; }
        public virtual Division Division { get; set; }
        [ForeignKey("Category")]
        public int categoryId { get; set; }
        public virtual Category Category { get; set; }
        public DateTime documentCreate { get; set; }
        public DateTime documentModify { get; set; }

    }
}