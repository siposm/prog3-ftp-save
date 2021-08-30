using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lecture_SqlFirst_EmpDept.Models2
{
    [Table("DEPT")]
    public partial class Dept
    {
        public Dept()
        {
            Emp = new HashSet<Emp>();
        }

        [Key]
        [Column("DEPTNO", TypeName = "numeric(2, 0)")]
        public decimal Deptno { get; set; }
        [Column("DNAME")]
        [StringLength(14)]
        public string Dname { get; set; }
        [Column("LOC")]
        [StringLength(13)]
        public string Loc { get; set; }

        [InverseProperty("DeptnoNavigation")]
        public virtual ICollection<Emp> Emp { get; set; }
    }
}
