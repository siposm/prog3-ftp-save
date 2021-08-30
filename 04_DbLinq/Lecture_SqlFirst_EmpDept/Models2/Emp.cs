using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lecture_SqlFirst_EmpDept.Models2
{
    [Table("EMP")]
    public partial class Emp
    {
        public Emp()
        {
            InverseMgrNavigation = new HashSet<Emp>();
        }

        [Key]
        [Column("EMPNO", TypeName = "numeric(4, 0)")]
        public decimal Empno { get; set; }
        [Column("ENAME")]
        [StringLength(10)]
        public string Ename { get; set; }
        [Column("JOB")]
        [StringLength(9)]
        public string Job { get; set; }
        [Column("MGR", TypeName = "numeric(4, 0)")]
        public decimal? Mgr { get; set; }
        [Column("HIREDATE", TypeName = "datetime")]
        public DateTime? Hiredate { get; set; }
        [Column("SAL", TypeName = "numeric(7, 2)")]
        public decimal? Sal { get; set; }
        [Column("COMM", TypeName = "numeric(7, 2)")]
        public decimal? Comm { get; set; }
        [Column("DEPTNO", TypeName = "numeric(2, 0)")]
        public decimal Deptno { get; set; }

        [ForeignKey(nameof(Deptno))]
        [InverseProperty(nameof(Dept.Emp))]
        public virtual Dept DeptnoNavigation { get; set; }
        [ForeignKey(nameof(Mgr))]
        [InverseProperty(nameof(Emp.InverseMgrNavigation))]
        public virtual Emp MgrNavigation { get; set; }
        [InverseProperty(nameof(Emp.MgrNavigation))]
        public virtual ICollection<Emp> InverseMgrNavigation { get; set; }
    }
}
