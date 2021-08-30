using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Lecture_SqlFirst_EmpDept.Models2
{
    public partial class EmpDeptAnnotationsContext : DbContext
    {
        public EmpDeptAnnotationsContext()
        {
        }

        public EmpDeptAnnotationsContext(DbContextOptions<EmpDeptAnnotationsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dept> Dept { get; set; }
        public virtual DbSet<Emp> Emp { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|\\EmpDept.mdf; Integrated Security = True; MultipleActiveResultSets = true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dept>(entity =>
            {
                entity.HasKey(e => e.Deptno)
                    .HasName("DEPT_PRIMARY_KEY");

                entity.Property(e => e.Dname).IsUnicode(false);

                entity.Property(e => e.Loc).IsUnicode(false);
            });

            modelBuilder.Entity<Emp>(entity =>
            {
                entity.HasKey(e => e.Empno)
                    .HasName("EMP_PRIMARY_KEY");

                entity.Property(e => e.Ename).IsUnicode(false);

                entity.Property(e => e.Job).IsUnicode(false);

                entity.HasOne(d => d.DeptnoNavigation)
                    .WithMany(p => p.Emp)
                    .HasForeignKey(d => d.Deptno)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EMP_FOREIGN_KEY");

                entity.HasOne(d => d.MgrNavigation)
                    .WithMany(p => p.InverseMgrNavigation)
                    .HasForeignKey(d => d.Mgr)
                    .HasConstraintName("EMP_BOSS_KEY");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
