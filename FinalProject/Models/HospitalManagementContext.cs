using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Models;

public partial class HospitalManagementContext : DbContext
{
    public HospitalManagementContext()
    {
    }

    public HospitalManagementContext(DbContextOptions<HospitalManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<ApprovalAction> ApprovalActions { get; set; }

    public virtual DbSet<AuditDetail> AuditDetails { get; set; }

    public virtual DbSet<AuditSession> AuditSessions { get; set; }

    public virtual DbSet<CancellationLog> CancellationLogs { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<GoodsReceiptDetail> GoodsReceiptDetails { get; set; }

    public virtual DbSet<GoodsReceiptNote> GoodsReceiptNotes { get; set; }

    public virtual DbSet<HospitalAdmissionRequest> HospitalAdmissionRequests { get; set; }

    public virtual DbSet<HospitalManager> HospitalManagers { get; set; }

    public virtual DbSet<InsuranceInfo> InsuranceInfos { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<LabOrder> LabOrders { get; set; }

    public virtual DbSet<LabOrderDetail> LabOrderDetails { get; set; }

    public virtual DbSet<LabTest> LabTests { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<NotificationLog> NotificationLogs { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Pharmacist> Pharmacists { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<PrescriptionDetail> PrescriptionDetails { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    public virtual DbSet<Receptionist> Receptionists { get; set; }

    public virtual DbSet<ReportExportLog> ReportExportLogs { get; set; }

    public virtual DbSet<RevenueReport> RevenueReports { get; set; }

    public virtual DbSet<RevenueReportItem> RevenueReportItems { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<ServicePackagePriceHistory> ServicePackagePriceHistories { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<StaffSchedule> StaffSchedules { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionDetail> TransactionDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var cs = Environment.GetEnvironmentVariable("HOSPITAL_DB_CONNECTION");
        if (!string.IsNullOrWhiteSpace(cs))
        {
            optionsBuilder.UseSqlServer(cs);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__appointm__A50828FC96999947");

            entity.ToTable("appointment");

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.AppointmentDate)
                .HasColumnType("datetime")
                .HasColumnName("appointment_date");
            entity.Property(e => e.CancelReason).HasColumnName("cancel_reason");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.MedicalRecordId).HasColumnName("medical_record_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.QueueNumber).HasColumnName("queue_number");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.ReceptionistId).HasColumnName("receptionist_id");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__appointme__docto__4E53A1AA");

            entity.HasOne(d => d.MedicalRecord).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.MedicalRecordId)
                .HasConstraintName("FK__appointme__medic__4F47C5E3");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__appointme__patie__503BEA1C");

            entity.HasOne(d => d.Receptionist).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ReceptionistId)
                .HasConstraintName("FK__appointme__recep__51300E55");
        });

        modelBuilder.Entity<ApprovalAction>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("PK__approval__74EFC21745396C7A");

            entity.ToTable("approval_action");

            entity.Property(e => e.ActionId).HasColumnName("action_id");
            entity.Property(e => e.ActedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("acted_at");
            entity.Property(e => e.Action)
                .HasMaxLength(10)
                .HasColumnName("action");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Reason).HasColumnName("reason");

            entity.HasOne(d => d.Manager).WithMany(p => p.ApprovalActions)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__approval___manag__5224328E");

            entity.HasOne(d => d.Order).WithMany(p => p.ApprovalActions)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__approval___order__531856C7");
        });

        modelBuilder.Entity<AuditDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__audit_de__38E9A22428080A18");

            entity.ToTable("audit_detail");

            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.BookQuantity).HasColumnName("book_quantity");
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.IsResolved)
                .HasDefaultValue(false)
                .HasColumnName("is_resolved");
            entity.Property(e => e.PhysicalCount).HasColumnName("physical_count");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.Variance)
                .HasComputedColumnSql("([physical_count]-[book_quantity])", true)
                .HasColumnName("variance");
            entity.Property(e => e.VarianceReason).HasColumnName("variance_reason");

            entity.HasOne(d => d.Inventory).WithMany(p => p.AuditDetails)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__audit_det__inven__540C7B00");

            entity.HasOne(d => d.Session).WithMany(p => p.AuditDetails)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__audit_det__sessi__55009F39");
        });

        modelBuilder.Entity<AuditSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__audit_se__69B13FDC195D688F");

            entity.ToTable("audit_session");

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("datetime")
                .HasColumnName("completed_at");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PharmacistId).HasColumnName("pharmacist_id");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("started_at");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValue("IN_PROGRESS")
                .HasColumnName("status");

            entity.HasOne(d => d.Pharmacist).WithMany(p => p.AuditSessions)
                .HasForeignKey(d => d.PharmacistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__audit_ses__pharm__55F4C372");
        });

        modelBuilder.Entity<CancellationLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__cancella__9E2397E0B25C5358");

            entity.ToTable("cancellation_log");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.CancelledAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("cancelled_at");
            entity.Property(e => e.CancelledBy).HasColumnName("cancelled_by");
            entity.Property(e => e.Reason).HasColumnName("reason");

            entity.HasOne(d => d.Appointment).WithMany(p => p.CancellationLogs)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__cancellat__appoi__56E8E7AB");

            entity.HasOne(d => d.CancelledByNavigation).WithMany(p => p.CancellationLogs)
                .HasForeignKey(d => d.CancelledBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__cancellat__cance__57DD0BE4");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__doctor__F3993564FD91F26A");

            entity.ToTable("doctor");

            entity.HasIndex(e => e.UserId, "UQ__doctor__B9BE370EBD79327D").IsUnique();

            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Department)
                .HasMaxLength(150)
                .HasColumnName("department");
            entity.Property(e => e.LicenseNumber)
                .HasMaxLength(100)
                .HasColumnName("license_number");
            entity.Property(e => e.SpecialtyId).HasColumnName("specialty_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.YearsExperience)
                .HasDefaultValue(0)
                .HasColumnName("years_experience");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__doctor__specialt__58D1301D");

            entity.HasOne(d => d.User).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__doctor__user_id__59C55456");
        });

        modelBuilder.Entity<GoodsReceiptDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__goods_re__38E9A2245CF8339A");

            entity.ToTable("goods_receipt_detail");

            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.BatchNumber)
                .HasMaxLength(100)
                .HasColumnName("batch_number");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.GrnId).HasColumnName("grn_id");
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.ManufactureDate).HasColumnName("manufacture_date");
            entity.Property(e => e.MedicineId).HasColumnName("medicine_id");
            entity.Property(e => e.PurchaseOrderDetailId).HasColumnName("purchase_order_detail_id");
            entity.Property(e => e.QuantityReceived).HasColumnName("quantity_received");
            entity.Property(e => e.UnitCost)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("unit_cost");

            entity.HasOne(d => d.Grn).WithMany(p => p.GoodsReceiptDetails)
                .HasForeignKey(d => d.GrnId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_rec__grn_i__5AB9788F");

            entity.HasOne(d => d.Inventory).WithMany(p => p.GoodsReceiptDetails)
                .HasForeignKey(d => d.InventoryId)
                .HasConstraintName("FK__goods_rec__inven__5BAD9CC8");

            entity.HasOne(d => d.Medicine).WithMany(p => p.GoodsReceiptDetails)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_rec__medic__5CA1C101");

            entity.HasOne(d => d.PurchaseOrderDetail).WithMany(p => p.GoodsReceiptDetails)
                .HasForeignKey(d => d.PurchaseOrderDetailId)
                .HasConstraintName("FK__goods_rec__purch__5D95E53A");
        });

        modelBuilder.Entity<GoodsReceiptNote>(entity =>
        {
            entity.HasKey(e => e.GrnId).HasName("PK__goods_re__39D8A22AE1FDC495");

            entity.ToTable("goods_receipt_note");

            entity.Property(e => e.GrnId).HasColumnName("grn_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PharmacistId).HasColumnName("pharmacist_id");
            entity.Property(e => e.ReceivedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("received_at");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValue("DRAFT")
                .HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.GoodsReceiptNotes)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_rec__order__5E8A0973");

            entity.HasOne(d => d.Pharmacist).WithMany(p => p.GoodsReceiptNotes)
                .HasForeignKey(d => d.PharmacistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__goods_rec__pharm__5F7E2DAC");
        });

        modelBuilder.Entity<HospitalAdmissionRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__hospital__18D3B90FCF8BBE75");

            entity.ToTable("hospital_admission_request");

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.AdmissionDate)
                .HasColumnType("datetime")
                .HasColumnName("admission_date");
            entity.Property(e => e.AdmissionReason).HasColumnName("admission_reason");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DischargeDate)
                .HasColumnType("datetime")
                .HasColumnName("discharge_date");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.RequestedDate).HasColumnName("requested_date");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Appointment).WithMany(p => p.HospitalAdmissionRequests)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK__hospital___appoi__607251E5");

            entity.HasOne(d => d.Doctor).WithMany(p => p.HospitalAdmissionRequests)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__hospital___docto__6166761E");

            entity.HasOne(d => d.Patient).WithMany(p => p.HospitalAdmissionRequests)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__hospital___patie__625A9A57");

            entity.HasOne(d => d.Room).WithMany(p => p.HospitalAdmissionRequests)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__hospital___room___634EBE90");
        });

        modelBuilder.Entity<HospitalManager>(entity =>
        {
            entity.HasKey(e => e.ManagerId).HasName("PK__hospital__5A6073FCB617A56D");

            entity.ToTable("hospital_manager");

            entity.HasIndex(e => e.UserId, "UQ__hospital__B9BE370ED3602DFC").IsUnique();

            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.HospitalManager)
                .HasForeignKey<HospitalManager>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__hospital___user___6442E2C9");
        });

        modelBuilder.Entity<InsuranceInfo>(entity =>
        {
            entity.HasKey(e => e.InsuranceId).HasName("PK__insuranc__58B60F450AC21A20");

            entity.ToTable("insurance_info");

            entity.Property(e => e.InsuranceId).HasColumnName("insurance_id");
            entity.Property(e => e.CoverageDetails).HasColumnName("coverage_details");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PolicyNumber)
                .HasMaxLength(100)
                .HasColumnName("policy_number");
            entity.Property(e => e.ProviderName)
                .HasMaxLength(200)
                .HasColumnName("provider_name");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");
            entity.Property(e => e.ValidTo).HasColumnName("valid_to");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__inventor__B59ACC49BE745470");

            entity.ToTable("inventory");

            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.BatchNumber)
                .HasMaxLength(100)
                .HasColumnName("batch_number");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_updated");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
            entity.Property(e => e.ManufactureDate).HasColumnName("manufacture_date");
            entity.Property(e => e.MedicineId).HasColumnName("medicine_id");
            entity.Property(e => e.SellingPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("selling_price");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
            entity.Property(e => e.UnitCost)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("unit_cost");

            entity.HasOne(d => d.Medicine).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__inventory__medic__65370702");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__invoice__F58DFD491153F0EF");

            entity.ToTable("invoice");

            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("discount");
            entity.Property(e => e.FinalAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("final_amount");
            entity.Property(e => e.IssuedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("issued_at");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.ReceptionistId).HasColumnName("receptionist_id");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValue("UNPAID")
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK__invoice__appoint__662B2B3B");

            entity.HasOne(d => d.Patient).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__invoice__patient__671F4F74");

            entity.HasOne(d => d.Receptionist).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.ReceptionistId)
                .HasConstraintName("FK__invoice__recepti__681373AD");
        });

        modelBuilder.Entity<LabOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__lab_orde__4659622993CBFA1C");

            entity.ToTable("lab_order");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ClinicalNotes).HasColumnName("clinical_notes");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.OrderedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ordered_at");
            entity.Property(e => e.Priority)
                .HasMaxLength(10)
                .HasDefaultValue("ROUTINE")
                .HasColumnName("priority");
            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValue("REQUESTED")
                .HasColumnName("status");

            entity.HasOne(d => d.Doctor).WithMany(p => p.LabOrders)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lab_order__docto__690797E6");

            entity.HasOne(d => d.Record).WithMany(p => p.LabOrders)
                .HasForeignKey(d => d.RecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lab_order__recor__69FBBC1F");
        });

        modelBuilder.Entity<LabOrderDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__lab_orde__38E9A22435EED27D");

            entity.ToTable("lab_order_detail");

            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.IsAbnormal)
                .HasDefaultValue(false)
                .HasColumnName("is_abnormal");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ReferenceRange)
                .HasMaxLength(100)
                .HasColumnName("reference_range");
            entity.Property(e => e.Result).HasColumnName("result");
            entity.Property(e => e.ResultUnit)
                .HasMaxLength(50)
                .HasColumnName("result_unit");
            entity.Property(e => e.ResultedAt)
                .HasColumnType("datetime")
                .HasColumnName("resulted_at");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.VerifiedBy).HasColumnName("verified_by");

            entity.HasOne(d => d.Order).WithMany(p => p.LabOrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lab_order__order__6AEFE058");

            entity.HasOne(d => d.Test).WithMany(p => p.LabOrderDetails)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lab_order__test___6BE40491");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.LabOrderDetails)
                .HasForeignKey(d => d.VerifiedBy)
                .HasConstraintName("FK__lab_order__verif__6CD828CA");
        });

        modelBuilder.Entity<LabTest>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("PK__lab_test__F3FF1C026D710617");

            entity.ToTable("lab_test");

            entity.HasIndex(e => e.TestCode, "UQ__lab_test__040975AB16CFBA83").IsUnique();

            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.SampleType)
                .HasMaxLength(100)
                .HasColumnName("sample_type");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.TestCode)
                .HasMaxLength(50)
                .HasColumnName("test_code");
            entity.Property(e => e.TestName)
                .HasMaxLength(200)
                .HasColumnName("test_name");
            entity.Property(e => e.TurnaroundTime)
                .HasMaxLength(50)
                .HasColumnName("turnaround_time");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("unit_price");
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__medical___BFCFB4DD20210779");

            entity.ToTable("medical_record");

            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.ChiefComplaint).HasColumnName("chief_complaint");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Diagnosis).HasColumnName("diagnosis");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("OPEN")
                .HasColumnName("status");
            entity.Property(e => e.Treatment).HasColumnName("treatment");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.VisitDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("visit_date");

            entity.HasOne(d => d.Doctor).WithMany(p => p.MedicalRecords)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__medical_r__docto__6DCC4D03");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalRecords)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__medical_r__patie__6EC0713C");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.MedicineId).HasName("PK__medicine__E7148EBB33FA645F");

            entity.ToTable("medicine");

            entity.HasIndex(e => e.MedicineCode, "UQ__medicine__7CAD9BFB3CE269EE").IsUnique();

            entity.Property(e => e.MedicineId).HasColumnName("medicine_id");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DosageForm)
                .HasMaxLength(100)
                .HasColumnName("dosage_form");
            entity.Property(e => e.GenericName)
                .HasMaxLength(200)
                .HasColumnName("generic_name");
            entity.Property(e => e.MedicineCode)
                .HasMaxLength(50)
                .HasColumnName("medicine_code");
            entity.Property(e => e.MedicineName)
                .HasMaxLength(200)
                .HasColumnName("medicine_name");
            entity.Property(e => e.RequiresPrescription)
                .HasDefaultValue(true)
                .HasColumnName("requires_prescription");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");
        });

        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842F20F22CF8");

            entity.ToTable("notification_log");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.Channel)
                .HasMaxLength(10)
                .HasDefaultValue("EMAIL")
                .HasColumnName("channel");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.RetryCount)
                .HasDefaultValue(0)
                .HasColumnName("retry_count");
            entity.Property(e => e.SentAt)
                .HasColumnType("datetime")
                .HasColumnName("sent_at");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(30)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Appointment).WithMany(p => p.NotificationLogs)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK__notificat__appoi__6FB49575");

            entity.HasOne(d => d.Patient).WithMany(p => p.NotificationLogs)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__notificat__patie__70A8B9AE");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__notificat__user___719CDDE7");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__patient__4D5CE476D7BBAC8F");

            entity.ToTable("patient");

            entity.HasIndex(e => e.UserId, "uix_patient_user_id")
                .IsUnique()
                .HasFilter("([user_id] IS NOT NULL)");

            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Allergies).HasColumnName("allergies");
            entity.Property(e => e.BloodType)
                .HasMaxLength(5)
                .HasColumnName("blood_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.InsuranceId).HasColumnName("insurance_id");
            entity.Property(e => e.NationalId)
                .HasMaxLength(50)
                .HasColumnName("national_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Insurance).WithMany(p => p.Patients)
                .HasForeignKey(d => d.InsuranceId)
                .HasConstraintName("FK__patient__insuran__72910220");

            entity.HasOne(d => d.User).WithOne(p => p.Patient)
                .HasForeignKey<Patient>(d => d.UserId)
                .HasConstraintName("FK__patient__user_id__73852659");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__payment__ED1FC9EA43A42001");

            entity.ToTable("payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.AmountPaid)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount_paid");
            entity.Property(e => e.ChangeAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("change_amount");
            entity.Property(e => e.GatewayResponse).HasColumnName("gateway_response");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.PaidAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("paid_at");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(15)
                .HasColumnName("payment_method");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.TransactionRef)
                .HasMaxLength(200)
                .HasColumnName("transaction_ref");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__payment__invoice__74794A92");
        });

        modelBuilder.Entity<Pharmacist>(entity =>
        {
            entity.HasKey(e => e.PharmacistId).HasName("PK__pharmaci__A62BABECF3969595");

            entity.ToTable("pharmacist");

            entity.HasIndex(e => e.UserId, "UQ__pharmaci__B9BE370E3B5ECB71").IsUnique();

            entity.Property(e => e.PharmacistId).HasColumnName("pharmacist_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Pharmacist)
                .HasForeignKey<Pharmacist>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pharmacis__user___756D6ECB");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId).HasName("PK__prescrip__3EE444F86D14FC24");

            entity.ToTable("prescription");

            entity.Property(e => e.PrescriptionId).HasColumnName("prescription_id");
            entity.Property(e => e.DiagnosisNote).HasColumnName("diagnosis_note");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.IssuedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("issued_at");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.Status)
                .HasMaxLength(25)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__prescript__docto__76619304");

            entity.HasOne(d => d.Record).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.RecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__prescript__recor__7755B73D");
        });

        modelBuilder.Entity<PrescriptionDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__prescrip__38E9A2241E111CD9");

            entity.ToTable("prescription_detail");

            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.Dosage)
                .HasMaxLength(100)
                .HasColumnName("dosage");
            entity.Property(e => e.Duration)
                .HasMaxLength(100)
                .HasColumnName("duration");
            entity.Property(e => e.EveningDose)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("evening_dose");
            entity.Property(e => e.Frequency)
                .HasMaxLength(100)
                .HasColumnName("frequency");
            entity.Property(e => e.Instructions).HasColumnName("instructions");
            entity.Property(e => e.MedicineId).HasColumnName("medicine_id");
            entity.Property(e => e.MorningDose)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("morning_dose");
            entity.Property(e => e.NoonDose)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("noon_dose");
            entity.Property(e => e.PrescriptionId).HasColumnName("prescription_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Medicine).WithMany(p => p.PrescriptionDetails)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__prescript__medic__7849DB76");

            entity.HasOne(d => d.Prescription).WithMany(p => p.PrescriptionDetails)
                .HasForeignKey(d => d.PrescriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__prescript__presc__793DFFAF");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__purchase__46596229C9C5EC02");

            entity.ToTable("purchase_order");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ExpectedDate).HasColumnName("expected_date");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.RequestedBy).HasColumnName("requested_by");
            entity.Property(e => e.Status)
                .HasMaxLength(25)
                .HasDefaultValue("PENDING_APPROVAL")
                .HasColumnName("status");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TotalAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.RequestedByNavigation).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.RequestedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchase___reque__7A3223E8");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchase___suppl__7B264821");
        });

        modelBuilder.Entity<PurchaseOrderDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__purchase__38E9A224030F79A2");

            entity.ToTable("purchase_order_detail");

            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.MedicineId).HasColumnName("medicine_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.QuantityOrdered).HasColumnName("quantity_ordered");
            entity.Property(e => e.QuantityReceived)
                .HasDefaultValue(0)
                .HasColumnName("quantity_received");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Medicine).WithMany(p => p.PurchaseOrderDetails)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchase___medic__7C1A6C5A");

            entity.HasOne(d => d.Order).WithMany(p => p.PurchaseOrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchase___order__7D0E9093");
        });

        modelBuilder.Entity<Receptionist>(entity =>
        {
            entity.HasKey(e => e.ReceptionistId).HasName("PK__receptio__4DC4999AB30C671B");

            entity.ToTable("receptionist");

            entity.HasIndex(e => e.UserId, "UQ__receptio__B9BE370EEECCD9F7").IsUnique();

            entity.Property(e => e.ReceptionistId).HasColumnName("receptionist_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Receptionist)
                .HasForeignKey<Receptionist>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__reception__user___7E02B4CC");
        });

        modelBuilder.Entity<ReportExportLog>(entity =>
        {
            entity.HasKey(e => e.ExportId).HasName("PK__report_e__323057CFF9FBA855");

            entity.ToTable("report_export_log");

            entity.Property(e => e.ExportId).HasColumnName("export_id");
            entity.Property(e => e.ExportedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("exported_at");
            entity.Property(e => e.ExportedBy).HasColumnName("exported_by");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FileSizeKb).HasColumnName("file_size_kb");
            entity.Property(e => e.Format)
                .HasMaxLength(10)
                .HasColumnName("format");
            entity.Property(e => e.ReportId).HasColumnName("report_id");

            entity.HasOne(d => d.ExportedByNavigation).WithMany(p => p.ReportExportLogs)
                .HasForeignKey(d => d.ExportedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__report_ex__expor__7EF6D905");

            entity.HasOne(d => d.Report).WithMany(p => p.ReportExportLogs)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__report_ex__repor__7FEAFD3E");
        });

        modelBuilder.Entity<RevenueReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__revenue___779B7C5858DBAA83");

            entity.ToTable("revenue_report");

            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.FromDate).HasColumnName("from_date");
            entity.Property(e => e.GeneratedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("generated_at");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.NetProfit)
                .HasComputedColumnSql("([total_revenue]-[total_expense])", true)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("net_profit");
            entity.Property(e => e.ReportName)
                .HasMaxLength(200)
                .HasColumnName("report_name");
            entity.Property(e => e.ReportType)
                .HasMaxLength(15)
                .HasDefaultValue("MONTHLY")
                .HasColumnName("report_type");
            entity.Property(e => e.ToDate).HasColumnName("to_date");
            entity.Property(e => e.TotalExpense)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(14, 2)")
                .HasColumnName("total_expense");
            entity.Property(e => e.TotalRevenue)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(14, 2)")
                .HasColumnName("total_revenue");

            entity.HasOne(d => d.Manager).WithMany(p => p.RevenueReports)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__revenue_r__manag__00DF2177");
        });

        modelBuilder.Entity<RevenueReportItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__revenue___52020FDDDF6D131C");

            entity.ToTable("revenue_report_item");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.Department)
                .HasMaxLength(150)
                .HasColumnName("department");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Percentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("percentage");
            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.ServiceCount)
                .HasDefaultValue(0)
                .HasColumnName("service_count");

            entity.HasOne(d => d.Report).WithMany(p => p.RevenueReportItems)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__revenue_r__repor__01D345B0");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__room__19675A8AF5B76100");

            entity.ToTable("room");

            entity.HasIndex(e => e.RoomNumber, "UQ__room__FE22F61B9ADD5E0F").IsUnique();

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.AvailableBeds).HasColumnName("available_beds");
            entity.Property(e => e.Floor)
                .HasMaxLength(20)
                .HasColumnName("floor");
            entity.Property(e => e.GenderAllowed)
                .HasMaxLength(10)
                .HasDefaultValue("ANY")
                .HasColumnName("gender_allowed");
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(50)
                .HasColumnName("room_number");
            entity.Property(e => e.RoomType)
                .HasMaxLength(100)
                .HasColumnName("room_type");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValue("AVAILABLE")
                .HasColumnName("status");
            entity.Property(e => e.TotalBeds).HasColumnName("total_beds");
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__service___63846AE80CC1B290");

            entity.ToTable("service_package");

            entity.HasIndex(e => e.PackageName, "UQ__service___671434CA930EA1D1").IsUnique();

            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.PackageName)
                .HasMaxLength(200)
                .HasColumnName("package_name");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("unit_price");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ServicePackages)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__service_p__creat__02C769E9");
        });

        modelBuilder.Entity<ServicePackagePriceHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__service___096AA2E9D0C1EAB9");

            entity.ToTable("service_package_price_history");

            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.NewPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("new_price");
            entity.Property(e => e.OldPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("old_price");
            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.Reason).HasColumnName("reason");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.ServicePackagePriceHistories)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__service_p__chang__03BB8E22");

            entity.HasOne(d => d.Package).WithMany(p => p.ServicePackagePriceHistories)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__service_p__packa__04AFB25B");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.SpecialtyId).HasName("PK__specialt__B90D8D12CF4F5E84");

            entity.ToTable("specialty");

            entity.Property(e => e.SpecialtyId).HasColumnName("specialty_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.SpecialtyName)
                .HasMaxLength(150)
                .HasColumnName("specialty_name");
        });

        modelBuilder.Entity<StaffSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__staff_sc__C46A8A6F8775A74B");

            entity.ToTable("staff_schedule");

            entity.HasIndex(e => new { e.UserId, e.ScheduleDate, e.ShiftType }, "uq_user_date_shift").IsUnique();

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.Department)
                .HasMaxLength(150)
                .HasColumnName("department");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.ScheduleDate).HasColumnName("schedule_date");
            entity.Property(e => e.ShiftType)
                .HasMaxLength(15)
                .HasColumnName("shift_type");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValue("SCHEDULED")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Manager).WithMany(p => p.StaffSchedules)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__staff_sch__manag__05A3D694");

            entity.HasOne(d => d.User).WithMany(p => p.StaffSchedules)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__staff_sch__user___0697FACD");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__supplier__6EE594E8C9759204");

            entity.ToTable("supplier");

            entity.HasIndex(e => e.SupplierCode, "UQ__supplier__A82CE469D822059D").IsUnique();

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(150)
                .HasColumnName("contact_person");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.SupplierCode)
                .HasMaxLength(50)
                .HasColumnName("supplier_code");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(200)
                .HasColumnName("supplier_name");
            entity.Property(e => e.TaxCode)
                .HasMaxLength(50)
                .HasColumnName("tax_code");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__transact__85C600AF1536EF30");

            entity.ToTable("transaction");

            entity.HasIndex(e => e.PrescriptionId, "UQ__transact__3EE444F9F3D18DBF").IsUnique();

            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.ExecutedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("executed_at");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PharmacistId).HasColumnName("pharmacist_id");
            entity.Property(e => e.PrescriptionId).HasColumnName("prescription_id");
            entity.Property(e => e.Status)
                .HasMaxLength(25)
                .HasDefaultValue("DISPENSED")
                .HasColumnName("status");

            entity.HasOne(d => d.Pharmacist).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PharmacistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__transacti__pharm__078C1F06");

            entity.HasOne(d => d.Prescription).WithOne(p => p.Transaction)
                .HasForeignKey<Transaction>(d => d.PrescriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__transacti__presc__0880433F");
        });

        modelBuilder.Entity<TransactionDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__transact__38E9A224E6994338");

            entity.ToTable("transaction_detail");

            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.PrescriptionDetailId).HasColumnName("prescription_detail_id");
            entity.Property(e => e.QuantityDispensed).HasColumnName("quantity_dispensed");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            entity.HasOne(d => d.Inventory).WithMany(p => p.TransactionDetails)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__transacti__inven__09746778");

            entity.HasOne(d => d.PrescriptionDetail).WithMany(p => p.TransactionDetails)
                .HasForeignKey(d => d.PrescriptionDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__transacti__presc__0A688BB1");

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionDetails)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__transacti__trans__0B5CAFEA");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__user__B9BE370FC828F5E7");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "UQ__user__AB6E6164BC4358E8").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__user__F3DBC572156A4C9A").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .HasColumnName("full_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
