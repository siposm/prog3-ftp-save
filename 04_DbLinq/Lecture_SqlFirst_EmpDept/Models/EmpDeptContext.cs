using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Lecture_SqlFirst_EmpDept.Models
{
    public partial class EmpDeptContext : DbContext
    {
        public EmpDeptContext()
        {
        }

        public EmpDeptContext(DbContextOptions<EmpDeptContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dept> Dept { get; set; }
        public virtual DbSet<Emp> Emp { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.
                    UseLazyLoadingProxies().
                    UseSqlServer(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|\EmpDept.mdf; Integrated Security = True; MultipleActiveResultSets = true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dept>(entity =>
            {
                entity.HasKey(e => e.Deptno)
                    .HasName("DEPT_PRIMARY_KEY");

                entity.ToTable("DEPT");

                entity.Property(e => e.Deptno)
                    .HasColumnName("DEPTNO")
                    .HasColumnType("numeric(2, 0)");

                entity.Property(e => e.Dname)
                    .HasColumnName("DNAME")
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.Loc)
                    .HasColumnName("LOC")
                    .HasMaxLength(13)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Emp>(entity =>
            {
                entity.HasKey(e => e.Empno)
                    .HasName("EMP_PRIMARY_KEY");

                entity.ToTable("EMP");

                entity.Property(e => e.Empno)
                    .HasColumnName("EMPNO")
                    .HasColumnType("numeric(4, 0)");

                entity.Property(e => e.Comm)
                    .HasColumnName("COMM")
                    .HasColumnType("numeric(7, 2)");

                entity.Property(e => e.Deptno)
                    .HasColumnName("DEPTNO")
                    .HasColumnType("numeric(2, 0)");

                entity.Property(e => e.Ename)
                    .HasColumnName("ENAME")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Hiredate)
                    .HasColumnName("HIREDATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Job)
                    .HasColumnName("JOB")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.Mgr)
                    .HasColumnName("MGR")
                    .HasColumnType("numeric(4, 0)");

                entity.Property(e => e.Sal)
                    .HasColumnName("SAL")
                    .HasColumnType("numeric(7, 2)");

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
