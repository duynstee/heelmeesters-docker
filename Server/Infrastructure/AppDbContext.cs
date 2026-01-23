namespace HeelmeestersAPI.Infrastructure;

using Microsoft.EntityFrameworkCore;
using HeelmeestersAPI.Features.Shared.Auth.Models;
using HeelmeestersAPI.Features.Shared.Appointments;
using HeelmeestersAPI.Features.Shared.MedicalRecords;
using HeelmeestersAPI.Features.HuisartsPortal.Patient.Models;
using HeelmeestersAPI.Features.HuisartsPortal.Referral.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Auth / users
    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<HospitalStaff> HospitalStaff => Set<HospitalStaff>();
    public DbSet<GeneralPractitioner> GeneralPractitioners => Set<GeneralPractitioner>();
    public DbSet<Admin> Admins => Set<Admin>();

    // Appointments domain
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<AppointmentHospitalStaff> AppointmentHospitalStaff => Set<AppointmentHospitalStaff>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();

    
    public DbSet<GeneralPractitionerHasPatient> GeneralPractitionerHasPatients => Set<GeneralPractitionerHasPatient>();
    public DbSet<Referral> Referrals => Set<Referral>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");

            e.Property(x => x.MailAdress)
                .HasColumnName("mail_adress")
                .HasMaxLength(50)
                .IsRequired();

            e.Property(x => x.Password)
                .HasColumnName("password")
                .HasMaxLength(50)
                .IsRequired();
        });

        modelBuilder.Entity<Patient>(e =>
        {
            e.ToTable("patients");
            e.HasKey(x => x.PatientNumber);

            e.Property(x => x.PatientNumber).HasColumnName("patient_number");
            e.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(254).IsRequired();
            e.Property(x => x.Prefix).HasColumnName("prefix").HasMaxLength(50);
            e.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(254).IsRequired();
            e.Property(x => x.DateOfBirth).HasColumnName("date_of_birth").IsRequired();

            e.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        });

        modelBuilder.Entity<HospitalStaff>(e =>
        {
            e.ToTable("hospital_staff");
            e.HasKey(x => x.EmployeeNumber);

            e.Property(x => x.EmployeeNumber).HasColumnName("employee_number");
            e.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(254).IsRequired();
            e.Property(x => x.Prefix).HasColumnName("prefix").HasMaxLength(50);
            e.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(254).IsRequired();
            e.Property(x => x.DateOfBirth).HasColumnName("date_of_birth").IsRequired();

            e.Property(x => x.Specialization).HasColumnName("specialization").HasMaxLength(100).IsRequired();

            e.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        });

        modelBuilder.Entity<GeneralPractitioner>(e =>
        {
            e.ToTable("general_practitioners");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(254).IsRequired();
            e.Property(x => x.Prefix).HasColumnName("prefix").HasMaxLength(50);
            e.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(254).IsRequired();
            e.Property(x => x.DateOfBirth).HasColumnName("date_of_birth").IsRequired();

            e.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        });

        modelBuilder.Entity<Treatment>(e =>
        {
            e.ToTable("treatments");
            e.HasKey(x => x.CareCode);

            e.Property(x => x.CareCode).HasColumnName("care_code").HasMaxLength(5);
            e.Property(x => x.Description).HasColumnName("description").HasMaxLength(254);
        });

        modelBuilder.Entity<Room>(e =>
        {
            e.ToTable("rooms");
            e.HasKey(x => x.RoomCode);

            e.Property(x => x.RoomCode).HasColumnName("room_code").HasMaxLength(10);
        });

        modelBuilder.Entity<Appointment>(e =>
        {
            e.ToTable("appointments");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.StartTime).HasColumnName("start_time");
            e.Property(x => x.EndTime).HasColumnName("end_time");
            e.Property(x => x.PatientNumber).HasColumnName("patient_number");
            e.Property(x => x.CareCode).HasColumnName("care_code").HasMaxLength(5);
            e.Property(x => x.RoomCode).HasColumnName("room_code").HasMaxLength(10);

            // EmployeeNumber staat niet in appointments tabel (join table), dus niet mappen naar kolom.
            e.Ignore(x => x.EmployeeNumber);
        });


        modelBuilder.Entity<AppointmentHospitalStaff>(e =>
        {
            e.ToTable("appointment_has_hospital_staff");
            e.HasKey(x => new { x.AppointmentId, x.EmployeeNumber });

            e.Property(x => x.AppointmentId).HasColumnName("appointment_id");
            e.Property(x => x.EmployeeNumber).HasColumnName("employee_number");
        });

        modelBuilder.Entity<Admin>(e =>
        {
            e.ToTable("admins");
            e.HasKey(x => x.UserId);

            e.Property(x => x.UserId)
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<MedicalRecord>(e =>
        {
            e.ToTable("medical_record");
            e.HasKey(x => x.LineNumber);

            e.Property(x => x.LineNumber).HasColumnName("line_number");
            e.Property(x => x.PatientNumber).HasColumnName("patient_number");
            e.Property(x => x.CreatedOn).HasColumnName("created_on");
            e.Property(x => x.Description)
                .HasColumnName("description")
                .HasColumnType("text")
                .IsRequired();
            e.Property(x => x.File).HasColumnName("file");

            e.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientNumber)
                .HasConstraintName("FK_medical_record_patients");
        });

        modelBuilder.Entity<GeneralPractitionerHasPatient>(e =>
        {
            e.ToTable("general_practitioner_has_patient");
            e.HasKey(x => new { x.GeneralPractitionerId, x.PatientNumber });

            e.Property(x => x.GeneralPractitionerId).HasColumnName("general_practitioner_id");
            e.Property(x => x.PatientNumber).HasColumnName("patient_number");
        });

        modelBuilder.Entity<Referral>(e =>
        {
            e.ToTable("referrals");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
            e.Property(x => x.GeneralPractitionerId).HasColumnName("general_practitioner_id");
            e.Property(x => x.PatientNumber).HasColumnName("patient_number");
            e.Property(x => x.CareCode).HasColumnName("care_code");
            e.Property(x => x.IsUsed).HasColumnName("is_used");
            e.Property(x => x.UsedOn).HasColumnName("used_on");
        });
    }
}
