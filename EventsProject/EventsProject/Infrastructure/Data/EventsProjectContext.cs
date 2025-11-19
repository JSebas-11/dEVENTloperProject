using EventsProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Infrastructure.Data;

public partial class EventsProjectContext : DbContext {
    //--------------------------INITIALIZATIION--------------------------
    public EventsProjectContext() { }
    public EventsProjectContext(DbContextOptions<EventsProjectContext> options)
        : base(options)
    { }

    //--------------------------MAPPED TABLES--------------------------
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<EventInfo> EventInfos { get; set; }
    public virtual DbSet<EventState> EventStates { get; set; }
    public virtual DbSet<NotificationInfo> NotificationInfos { get; set; }
    public virtual DbSet<NotificationState> NotificationStates { get; set; }
    public virtual DbSet<UserAccount> UserAccounts { get; set; }
    public virtual DbSet<UserEvent> UserEvents { get; set; }
    public virtual DbSet<UsersReg> UsersRegs { get; set; }

    public string GetStringConnection() => Database.GetDbConnection().ConnectionString;
    //--------------------------MAPPING--------------------------
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Category>(entity => {
            entity.HasKey(e => e.CatId).HasName("PK__Category__6A1C8AFAD51C9DFF");
            entity.ToTable("Category");
            entity.HasIndex(e => e.CatName, "UQ__Category__9C61AB266AA0A8B0").IsUnique();
            entity.Property(e => e.CatDescription)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.CatName)
                .HasMaxLength(16)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EventInfo>(entity => {
            entity.HasKey(e => e.EventId).HasName("PK__EventInf__7944C81008F35113");
            entity.ToTable("EventInfo");
            entity.Property(e => e.Artist)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.EventCity)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.EventDescription)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.EventImg).IsUnicode(false);
            entity.Property(e => e.EventPlace)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.InitialTime).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(32)
                .IsUnicode(false);

            entity.HasOne(d => d.Cat).WithMany(p => p.EventInfos)
                .HasForeignKey(d => d.CatId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_EventInfo_Category");
            entity.HasOne(d => d.CreatedBy).WithMany(p => p.EventInfos)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_EventInfo_UserAccount");
            entity.HasOne(d => d.EventState).WithMany(p => p.EventInfos)
                .HasForeignKey(d => d.EventStateId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_EventInfo_EventState");
        });

        modelBuilder.Entity<EventState>(entity => {
            entity.HasKey(e => e.EventStateId).HasName("PK__EventSta__D242BA045AE66F28");
            entity.ToTable("EventState");
            entity.HasIndex(e => e.StateName, "UQ__EventSta__5547631535D5211E").IsUnique();
            entity.Property(e => e.StateName)
                .HasMaxLength(16)
                .IsUnicode(false);
        });

        modelBuilder.Entity<NotificationInfo>(entity => {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12F7447E9A");
            entity.ToTable("NotificationInfo");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.NotMessage)
                .HasMaxLength(128)
                .IsUnicode(false);

            entity.HasOne(d => d.NotState).WithMany(p => p.NotificationInfos)
                .HasForeignKey(d => d.NotStateId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_NotificationInfo_NotificationState");
            entity.HasOne(d => d.User).WithMany(p => p.NotificationInfos)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_NotificationInfo_UserAccount");
        });

        modelBuilder.Entity<NotificationState>(entity => {
            entity.HasKey(e => e.NotStateId).HasName("PK__Notifica__AC8FE614DCAF73A2");
            entity.ToTable("NotificationState");
            entity.HasIndex(e => e.StateName, "UQ__Notifica__55476315D321FA47").IsUnique();
            entity.Property(e => e.StateName)
                .HasMaxLength(16)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserAccount>(entity => {
            entity.HasKey(e => e.UserId).HasName("PK__UserAcco__1788CC4CC758E92B");
            entity.ToTable("UserAccount", tb =>
            {
                tb.HasTrigger("UsersReg_AD");
                tb.HasTrigger("UsersReg_AI");
            });

            entity.HasIndex(e => e.Dni, "UQ__UserAcco__C030857569E22320").IsUnique();
            entity.Property(e => e.Dni)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.HashPassword)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.UserEmail)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UserImg).IsUnicode(false);
        });

        modelBuilder.Entity<UserEvent>(entity => {
            entity.HasKey(e => e.UserEventId).HasName("PK__UserEven__C59E16B9C2ABCBCF");
            entity.ToTable("UserEvent");
            entity.Property(e => e.EnrolledAt).HasColumnType("datetime");

            entity.HasOne(d => d.Event).WithMany(p => p.UserEvents)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK_UserEvent_EventInfo");
            entity.HasOne(d => d.User).WithMany(p => p.UserEvents)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserEvent_UserAccount");
        });

        modelBuilder.Entity<UsersReg>(entity => {
            entity.HasKey(e => e.UsersRegId).HasName("PK__UsersReg__93A07378819DDA40");
            entity.ToTable("UsersReg");
            entity.HasIndex(e => e.Dni, "UQ__UsersReg__C03085759F373732").IsUnique();
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deletedAt");
            entity.Property(e => e.Dni)
                .HasMaxLength(12)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
