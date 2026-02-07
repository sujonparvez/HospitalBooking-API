namespace HospitalBooking.Infrastructure.Persistence.Configurations;

using HospitalBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        
        builder.HasIndex(x => x.Email).IsUnique(); 

        builder.HasIndex(x => x.Email).IsUnique(); 
    }
}

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.User)
               .WithOne() // 1:0..1 relationship effectively, but here defined as 1:1 from Doctor side
               .HasForeignKey<Doctor>(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
               .WithMany(x => x.Doctors)
               .HasForeignKey(x => x.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Doctor)
               .WithMany(x => x.Appointments)
               .HasForeignKey(x => x.DoctorId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

        builder.HasOne(x => x.Patient)
               .WithMany()
               .HasForeignKey(x => x.PatientId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
