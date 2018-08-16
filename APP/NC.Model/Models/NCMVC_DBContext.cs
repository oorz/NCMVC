using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NC.Model.Models
{
    public partial class NCMVC_DBContext : DbContext
    {
        public NCMVC_DBContext()
        {
        }

        public NCMVC_DBContext(DbContextOptions<NCMVC_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<NcDictbase> NcDictbase { get; set; }
        public virtual DbSet<NcDictcache> NcDictcache { get; set; }
        public virtual DbSet<NcManager> NcManager { get; set; }
        public virtual DbSet<NcManagerDept> NcManagerDept { get; set; }
        public virtual DbSet<NcManagerLog> NcManagerLog { get; set; }
        public virtual DbSet<NcManagerRole> NcManagerRole { get; set; }
        public virtual DbSet<NcManagerRoleValue> NcManagerRoleValue { get; set; }
        public virtual DbSet<NcNavigation> NcNavigation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NcDictbase>(entity =>
            {
                entity.ToTable("nc_dictbase");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CodeValue)
                    .HasColumnName("Code_Value")
                    .HasMaxLength(100);

                entity.Property(e => e.KeyCode)
                    .IsRequired()
                    .HasColumnName("Key_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.KeyType)
                    .IsRequired()
                    .HasColumnName("Key_Type")
                    .HasMaxLength(20);

                entity.Property(e => e.KeyValue)
                    .IsRequired()
                    .HasColumnName("Key_Value")
                    .HasMaxLength(50);

                entity.Property(e => e.Ostatus).HasColumnName("OStatus");

                entity.Property(e => e.SortId).HasColumnName("Sort_ID");
            });

            modelBuilder.Entity<NcDictcache>(entity =>
            {
                entity.HasKey(e => e.CacheId);

                entity.ToTable("nc_dictcache");

                entity.Property(e => e.CacheId).HasColumnName("Cache_ID");

                entity.Property(e => e.CacheDesc)
                    .IsRequired()
                    .HasColumnName("Cache_Desc")
                    .HasMaxLength(200);

                entity.Property(e => e.CacheExp)
                    .HasColumnName("Cache_Exp")
                    .HasMaxLength(50);

                entity.Property(e => e.CacheKey)
                    .HasColumnName("Cache_Key")
                    .HasMaxLength(50);

                entity.Property(e => e.ClassLayer).HasColumnName("Class_Layer");

                entity.Property(e => e.ClassList)
                    .HasColumnName("Class_List")
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedName)
                    .HasColumnName("Created_Name")
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("Created_Time")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Depend).HasMaxLength(50);

                entity.Property(e => e.Ostatus).HasColumnName("OStatus");

                entity.Property(e => e.ParentId).HasColumnName("Parent_ID");

                entity.Property(e => e.SortId).HasColumnName("Sort_ID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedName)
                    .HasColumnName("Updated_Name")
                    .HasMaxLength(20);

                entity.Property(e => e.UpdatedTime)
                    .HasColumnName("Updated_Time")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<NcManager>(entity =>
            {
                entity.ToTable("nc_manager");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AddTime)
                    .HasColumnName("add_time")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Avatar)
                    .HasColumnName("avatar")
                    .HasMaxLength(255);

                entity.Property(e => e.DeptId).HasColumnName("dept_id");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(30)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.IsAudit)
                    .HasColumnName("is_audit")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsLock)
                    .HasColumnName("is_lock")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(100);

                entity.Property(e => e.RealName)
                    .HasColumnName("real_name")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.RoleType)
                    .HasColumnName("role_type")
                    .HasDefaultValueSql("((2))");

                entity.Property(e => e.Salt)
                    .HasColumnName("salt")
                    .HasMaxLength(20);

                entity.Property(e => e.Telephone)
                    .HasColumnName("telephone")
                    .HasMaxLength(30)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.UserName)
                    .HasColumnName("user_name")
                    .HasMaxLength(100);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.NcManager)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_nc_MANAG_REFERENCE_nc_MANAG");
            });

            modelBuilder.Entity<NcManagerDept>(entity =>
            {
                entity.HasKey(e => e.DeptId);

                entity.ToTable("nc_manager_dept");

                entity.Property(e => e.DeptId).HasColumnName("Dept_ID");

                entity.Property(e => e.ClassLayer).HasColumnName("Class_Layer");

                entity.Property(e => e.ClassList)
                    .HasColumnName("Class_List")
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedId).HasColumnName("Created_ID");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("Created_Time")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeptDesc)
                    .IsRequired()
                    .HasColumnName("Dept_Desc")
                    .HasMaxLength(200);

                entity.Property(e => e.DeptName)
                    .IsRequired()
                    .HasColumnName("Dept_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.IsAudit).HasDefaultValueSql("((0))");

                entity.Property(e => e.Ostatus).HasColumnName("OStatus");

                entity.Property(e => e.ParentId).HasColumnName("Parent_ID");

                entity.Property(e => e.SortId).HasColumnName("Sort_ID");

                entity.Property(e => e.UpdatedId).HasColumnName("Updated_ID");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnName("Updated_Time")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<NcManagerLog>(entity =>
            {
                entity.ToTable("nc_manager_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionType)
                    .HasColumnName("action_type")
                    .HasMaxLength(100);

                entity.Property(e => e.AddTime)
                    .HasColumnName("add_time")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Remark)
                    .HasColumnName("remark")
                    .HasMaxLength(255);

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserIp)
                    .HasColumnName("user_ip")
                    .HasMaxLength(30);

                entity.Property(e => e.UserName)
                    .HasColumnName("user_name")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<NcManagerRole>(entity =>
            {
                entity.ToTable("nc_manager_role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsSys)
                    .HasColumnName("is_sys")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RoleName)
                    .HasColumnName("role_name")
                    .HasMaxLength(100);

                entity.Property(e => e.RoleType).HasColumnName("role_type");
            });

            modelBuilder.Entity<NcManagerRoleValue>(entity =>
            {
                entity.ToTable("nc_manager_role_value");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionType)
                    .HasColumnName("action_type")
                    .HasMaxLength(50);

                entity.Property(e => e.NavName)
                    .HasColumnName("nav_name")
                    .HasMaxLength(100);

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.NcManagerRoleValue)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_nc_MANAG_REFERENCE_nc_MANAG_ROLE_VALUE");
            });

            modelBuilder.Entity<NcNavigation>(entity =>
            {
                entity.ToTable("nc_navigation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionType)
                    .HasColumnName("action_type")
                    .HasMaxLength(500)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ChannelId)
                    .HasColumnName("channel_id")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IconUrl)
                    .HasColumnName("icon_url")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.IsLock)
                    .HasColumnName("is_lock")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsSys)
                    .HasColumnName("is_sys")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.LinkUrl)
                    .HasColumnName("link_url")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.NavType)
                    .HasColumnName("nav_type")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ParentId)
                    .HasColumnName("parent_id")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Remark)
                    .HasColumnName("remark")
                    .HasMaxLength(500)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.SortId)
                    .HasColumnName("sort_id")
                    .HasDefaultValueSql("((99))");

                entity.Property(e => e.SubTitle)
                    .HasColumnName("sub_title")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("('')");
            });
        }
    }
}
