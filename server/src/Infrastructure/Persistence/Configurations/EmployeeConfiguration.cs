using HrManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrManager.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.PasswordHash)
            .IsRequired();

        builder.Property(e => e.Role)
            .IsRequired();

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.DocumentNumber).IsUnique();

        builder.HasOne(e => e.Manager)
            .WithMany(e => e.TeamMembers)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(e => e.Phones, pb =>
        {
            pb.ToTable("EmployeePhones");
            pb.WithOwner().HasForeignKey("EmployeeId");

            pb.Property<int>("Id");
            pb.HasKey("Id");

            pb.Property(p => p.Label)
                .IsRequired()
                .HasMaxLength(50);

            pb.Property(p => p.Number)
                .IsRequired()
                .HasMaxLength(25);
        });
    }
}
