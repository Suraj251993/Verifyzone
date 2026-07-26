using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OrgCheck.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Absconddetail> Absconddetails { get; set; }

    public virtual DbSet<Absconddocumentdatum> Absconddocumentdata { get; set; }

    public virtual DbSet<Autoapprovalconfig> Autoapprovalconfigs { get; set; }

    public virtual DbSet<Autoapprovalexclusion> Autoapprovalexclusions { get; set; }

    public virtual DbSet<Bulkupload> Bulkuploads { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Companycredit> Companycredits { get; set; }

    public virtual DbSet<Companyquestion> Companyquestions { get; set; }

    public virtual DbSet<Companywallet> Companywallets { get; set; }

    public virtual DbSet<Companywallettransaction> Companywallettransactions { get; set; }

    public virtual DbSet<Consentauditlog> Consentauditlogs { get; set; }

    public virtual DbSet<Consentrequest> Consentrequests { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Customercredit> Customercredits { get; set; }

    public virtual DbSet<Customeremailsetting> Customeremailsettings { get; set; }

    public virtual DbSet<Customersetting> Customersettings { get; set; }

    public virtual DbSet<Customerwallet> Customerwallets { get; set; }

    public virtual DbSet<Customerwallettransaction> Customerwallettransactions { get; set; }

    public virtual DbSet<Downloadreport> Downloadreports { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Employeeapproval> Employeeapprovals { get; set; }

    public virtual DbSet<Employeequestionaire> Employeequestionaires { get; set; }

    public virtual DbSet<Employeesearch> Employeesearches { get; set; }

    public virtual DbSet<Empverificationrequest> Empverificationrequests { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Invalidemployee> Invalidemployees { get; set; }

    public virtual DbSet<Invalidemployeequestionaire> Invalidemployeequestionaires { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<LookupConsentstatus> LookupConsentstatuses { get; set; }

    public virtual DbSet<LookupCustomertype> LookupCustomertypes { get; set; }

    public virtual DbSet<LookupDiscrepancytype> LookupDiscrepancytypes { get; set; }

    public virtual DbSet<LookupEmpverificationResponse> LookupEmpverificationResponses { get; set; }

    public virtual DbSet<LookupStuverificationResponse> LookupStuverificationResponses { get; set; }

    public virtual DbSet<LookupTransactiontype> LookupTransactiontypes { get; set; }

    public virtual DbSet<LookupUsertype> LookupUsertypes { get; set; }

    public virtual DbSet<Questionaire> Questionaires { get; set; }

    public virtual DbSet<Reportdownload> Reportdownloads { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Studentapproval> Studentapprovals { get; set; }

    public virtual DbSet<Studentrequest> Studentrequests { get; set; }

    public virtual DbSet<Studentsearch> Studentsearches { get; set; }

    public virtual DbSet<Tempemployee> Tempemployees { get; set; }

    public virtual DbSet<Tempemployeequestionaire> Tempemployeequestionaires { get; set; }

    public virtual DbSet<Tempstudent> Tempstudents { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=postgres;Username=postgres;Password=postgres123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_catalog", "adminpack");

        modelBuilder.Entity<Absconddetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("absconddetails_pkey");

            entity.ToTable("absconddetails", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Discrepancetype).HasColumnName("discrepancetype");
            entity.Property(e => e.Emailid).HasColumnName("emailid");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Fathername).HasColumnName("fathername");
            entity.Property(e => e.Isprocessed)
                .HasDefaultValue(false)
                .HasColumnName("isprocessed");
            entity.Property(e => e.Linkedinurl).HasColumnName("linkedinurl");
            entity.Property(e => e.Mobileno).HasColumnName("mobileno");
            entity.Property(e => e.Resume).HasColumnName("resume");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Uannumber).HasColumnName("uannumber");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Absconddetails)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("abs_login");

            entity.HasOne(d => d.DiscrepancetypeNavigation).WithMany(p => p.Absconddetails)
                .HasForeignKey(d => d.Discrepancetype)
                .HasConstraintName("abs_discrepancytype");

            entity.HasOne(d => d.Employee).WithMany(p => p.Absconddetails)
                .HasForeignKey(d => d.Employeeid)
                .HasConstraintName("abs_employee");
        });

        modelBuilder.Entity<Absconddocumentdatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("absconddocumentdata_pkey");

            entity.ToTable("absconddocumentdata", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Abscondid).HasColumnName("abscondid");
            entity.Property(e => e.Extracttext).HasColumnName("extracttext");

            entity.HasOne(d => d.Abscond).WithMany(p => p.Absconddocumentdata)
                .HasForeignKey(d => d.Abscondid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("data_abscond");
        });

        modelBuilder.Entity<Autoapprovalconfig>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("autoapprovalconfig_pkey");

            entity.ToTable("autoapprovalconfig", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Enddate).HasColumnName("enddate");
            entity.Property(e => e.Startdate).HasColumnName("startdate");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");
            entity.Property(e => e.Updateddate).HasColumnName("updateddate");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.AutoapprovalconfigCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("aaconfig_login2");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.AutoapprovalconfigUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .HasConstraintName("aaconfig_login3");
        });

        modelBuilder.Entity<Autoapprovalexclusion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("autoapprovalexclusion_pkey");

            entity.ToTable("autoapprovalexclusion", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Autoapprovalexclusions)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("aae_login");

            entity.HasOne(d => d.Customer).WithMany(p => p.Autoapprovalexclusions)
                .HasForeignKey(d => d.Customerid)
                .HasConstraintName("aae_customer");

            entity.HasOne(d => d.Employee).WithMany(p => p.Autoapprovalexclusions)
                .HasForeignKey(d => d.Employeeid)
                .HasConstraintName("aae_employee");
        });

        modelBuilder.Entity<Bulkupload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bulkupload_pkey");

            entity.ToTable("bulkupload", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Filename)
                .IsRequired()
                .HasColumnName("filename");
            entity.Property(e => e.Uploadedby).HasColumnName("uploadedby");
            entity.Property(e => e.Uploadeddate).HasColumnName("uploadeddate");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("companies_pkey");

            entity.ToTable("companies", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .IsRequired()
                .HasColumnName("address");
            entity.Property(e => e.Charges)
                .HasDefaultValue(50.0)
                .HasColumnName("charges");
            entity.Property(e => e.Contactname)
                .IsRequired()
                .HasColumnName("contactname");
            entity.Property(e => e.Contactnumber)
                .IsRequired()
                .HasColumnName("contactnumber");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Educharges)
                .HasDefaultValue(50.0)
                .HasColumnName("educharges");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasColumnName("email");
            entity.Property(e => e.GstNumber).HasColumnName("gst_number");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Modifieddate).HasColumnName("modifieddate");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.PanNumber).HasColumnName("pan_number");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.TanNumber).HasColumnName("tan_number");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.CompanyCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("companies_logins");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.CompanyModifiedbyNavigations)
                .HasForeignKey(d => d.Modifiedby)
                .HasConstraintName("companies_logins2");
        });

        modelBuilder.Entity<Companycredit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("companycredits_pkey");

            entity.ToTable("companycredits", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Companyid).HasColumnName("companyid");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Credit).HasColumnName("credit");
            entity.Property(e => e.Referenceno)
                .IsRequired()
                .HasColumnName("referenceno");
            entity.Property(e => e.Remarks)
                .IsRequired()
                .HasColumnName("remarks");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Transactiontype)
                .IsRequired()
                .HasColumnName("transactiontype");

            entity.HasOne(d => d.Company).WithMany(p => p.Companycredits)
                .HasForeignKey(d => d.Companyid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("credits_company");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Companycredits)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("credits_login");
        });

        modelBuilder.Entity<Companyquestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("companyquestions_pkey");

            entity.ToTable("companyquestions", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Companyid).HasColumnName("companyid");
            entity.Property(e => e.Questionid).HasColumnName("questionid");

            entity.HasOne(d => d.Company).WithMany(p => p.Companyquestions)
                .HasForeignKey(d => d.Companyid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cq_company");

            entity.HasOne(d => d.Question).WithMany(p => p.Companyquestions)
                .HasForeignKey(d => d.Questionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cq_questions");
        });

        modelBuilder.Entity<Companywallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("companywallet_pkey");

            entity.ToTable("companywallet", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Companyid).HasColumnName("companyid");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Totalcredit).HasColumnName("totalcredit");

            entity.HasOne(d => d.Company).WithMany(p => p.Companywallets)
                .HasForeignKey(d => d.Companyid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("wallet_company");
        });

        modelBuilder.Entity<Companywallettransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("companywallettransaction_pkey");

            entity.ToTable("companywallettransaction", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Companyid).HasColumnName("companyid");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Credits).HasColumnName("credits");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Transactiontype).HasColumnName("transactiontype");

            entity.HasOne(d => d.Company).WithMany(p => p.Companywallettransactions)
                .HasForeignKey(d => d.Companyid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cowt_company");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Companywallettransactions)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cowt_login");

            entity.HasOne(d => d.TransactiontypeNavigation).WithMany(p => p.Companywallettransactions)
                .HasForeignKey(d => d.Transactiontype)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cowt_transactiontype");
        });

        modelBuilder.Entity<LookupConsentstatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lookup_consentstatus_pkey");

            entity.ToTable("lookup_consentstatus", "orgcheck");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
        });

        modelBuilder.Entity<Consentrequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("consentrequests_pkey");

            entity.ToTable("consentrequests", "orgcheck");

            entity.HasIndex(e => e.Consentrequestid, "consentrequests_consentrequestid_key").IsUnique();

            entity.HasIndex(e => e.Token, "consentrequests_token_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Browser).HasColumnName("browser");
            entity.Property(e => e.Consentdate).HasColumnName("consentdate");
            entity.Property(e => e.Consentdocumentversion).HasColumnName("consentdocumentversion");
            entity.Property(e => e.Consentrequestid)
                .IsRequired()
                .HasColumnName("consentrequestid");
            entity.Property(e => e.Consentsource)
                .HasDefaultValue("EmailLink")
                .HasColumnName("consentsource");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("now()")
                .HasColumnName("createddate");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Device).HasColumnName("device");
            entity.Property(e => e.Employeecode).HasColumnName("employeecode");
            entity.Property(e => e.Employeeemail)
                .IsRequired()
                .HasColumnName("employeeemail");
            entity.Property(e => e.Employeefirstname)
                .IsRequired()
                .HasColumnName("employeefirstname");
            entity.Property(e => e.Employeelastname)
                .IsRequired()
                .HasColumnName("employeelastname");
            entity.Property(e => e.Ipaddress).HasColumnName("ipaddress");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Modifieddate).HasColumnName("modifieddate");
            entity.Property(e => e.Optionalemail).HasColumnName("optionalemail");
            entity.Property(e => e.Statusid)
                .HasDefaultValue(1)
                .HasColumnName("statusid");
            entity.Property(e => e.Token)
                .IsRequired()
                .HasColumnName("token");
            entity.Property(e => e.Tokenconsumed)
                .HasDefaultValue(false)
                .HasColumnName("tokenconsumed");
            entity.Property(e => e.Tokenexpirydate).HasColumnName("tokenexpirydate");

            entity.HasOne(d => d.Customer).WithMany()
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("consreq_customer");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany()
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("consreq_createdby");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany()
                .HasForeignKey(d => d.Modifiedby)
                .HasConstraintName("consreq_modifiedby");

            entity.HasOne(d => d.Status).WithMany(p => p.Consentrequests)
                .HasForeignKey(d => d.Statusid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("consreq_status");
        });

        modelBuilder.Entity<Consentauditlog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("consentauditlogs_pkey");

            entity.ToTable("consentauditlogs", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Action)
                .IsRequired()
                .HasColumnName("action");
            entity.Property(e => e.Consentrequestid).HasColumnName("consentrequestid");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("now()")
                .HasColumnName("createddate");
            entity.Property(e => e.Ipaddress).HasColumnName("ipaddress");
            entity.Property(e => e.Newstatusid).HasColumnName("newstatusid");
            entity.Property(e => e.Oldstatusid).HasColumnName("oldstatusid");
            entity.Property(e => e.Performedby).HasColumnName("performedby");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.Useragent).HasColumnName("useragent");

            entity.HasOne(d => d.Consentrequest).WithMany(p => p.Consentauditlogs)
                .HasForeignKey(d => d.Consentrequestid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("conslog_consreq");

            entity.HasOne(d => d.PerformedbyNavigation).WithMany()
                .HasForeignKey(d => d.Performedby)
                .HasConstraintName("conslog_performedby");

            entity.HasOne(d => d.OldstatusNavigation).WithMany()
                .HasForeignKey(d => d.Oldstatusid)
                .HasConstraintName("conslog_oldstatus");

            entity.HasOne(d => d.NewstatusNavigation).WithMany()
                .HasForeignKey(d => d.Newstatusid)
                .HasConstraintName("conslog_newstatus");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_pkey");

            entity.ToTable("customer", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Charges)
                .HasDefaultValue(50.0)
                .HasColumnName("charges");
            entity.Property(e => e.ClosedDate).HasColumnName("closed_date");
            entity.Property(e => e.CommencementDate).HasColumnName("commencement_date");
            entity.Property(e => e.Contactname).HasColumnName("contactname");
            entity.Property(e => e.Contactnumber).HasColumnName("contactnumber");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.GstNumber).HasColumnName("gst_number");
            entity.Property(e => e.Industrytype).HasColumnName("industrytype");
            entity.Property(e => e.Isbgv).HasColumnName("isbgv");
            entity.Property(e => e.Iseducation).HasColumnName("iseducation");
            entity.Property(e => e.Isemployment).HasColumnName("isemployment");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Modifieddate).HasColumnName("modifieddate");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.PanNumber).HasColumnName("pan_number");
            entity.Property(e => e.Parentname).HasColumnName("parentname");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.TanNumber).HasColumnName("tan_number");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.CustomerCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_logins");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.CustomerModifiedbyNavigations)
                .HasForeignKey(d => d.Modifiedby)
                .HasConstraintName("customer_logins2");
        });

        modelBuilder.Entity<Customercredit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customercredits_pkey");

            entity.ToTable("customercredits", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Credit).HasColumnName("credit");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Referenceno)
                .IsRequired()
                .HasColumnName("referenceno");
            entity.Property(e => e.Remarks)
                .IsRequired()
                .HasColumnName("remarks");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Transactiontype)
                .IsRequired()
                .HasColumnName("transactiontype");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Customercredits)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("credits_login");

            entity.HasOne(d => d.Customer).WithMany(p => p.Customercredits)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("credits_customer");
        });

        modelBuilder.Entity<Customeremailsetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customeremailsetting_pkey");

            entity.ToTable("customeremailsetting", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createdcustomerid).HasColumnName("createdcustomerid");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Templatecontent).HasColumnName("templatecontent");
            entity.Property(e => e.Templateid).HasColumnName("templateid");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Customeremailsettings)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ces_login");

            entity.HasOne(d => d.Createdcustomer).WithMany(p => p.CustomeremailsettingCreatedcustomers)
                .HasForeignKey(d => d.Createdcustomerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ces_customer2");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomeremailsettingCustomers)
                .HasForeignKey(d => d.Customerid)
                .HasConstraintName("ces_customer");
        });

        modelBuilder.Entity<Customersetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customersetting_pkey");

            entity.ToTable("customersetting", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Pendingcreditthreshold)
                .HasDefaultValue(5)
                .HasColumnName("pendingcreditthreshold");

            entity.HasOne(d => d.Customer).WithMany(p => p.Customersettings)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cs_customer");
        });

        modelBuilder.Entity<Customerwallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customerwallet_pkey");

            entity.ToTable("customerwallet", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Creditpending).HasColumnName("creditpending");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Totalcredit).HasColumnName("totalcredit");

            entity.HasOne(d => d.Customer).WithMany(p => p.Customerwallets)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("wallet_customer");
        });

        modelBuilder.Entity<Customerwallettransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customerwallettransaction_pkey");

            entity.ToTable("customerwallettransaction", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Credits).HasColumnName("credits");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Transactiontype).HasColumnName("transactiontype");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Customerwallettransactions)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cwt_login");

            entity.HasOne(d => d.Customer).WithMany(p => p.Customerwallettransactions)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cwt_customer");

            entity.HasOne(d => d.TransactiontypeNavigation).WithMany(p => p.Customerwallettransactions)
                .HasForeignKey(d => d.Transactiontype)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cwt_transactiontype");
        });

        modelBuilder.Entity<Downloadreport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("downloadreports_pkey");

            entity.ToTable("downloadreports", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Downloadby).HasColumnName("downloadby");
            entity.Property(e => e.Downloaddate).HasColumnName("downloaddate");
            entity.Property(e => e.Employeesearchid).HasColumnName("employeesearchid");

            entity.HasOne(d => d.DownloadbyNavigation).WithMany(p => p.Downloadreports)
                .HasForeignKey(d => d.Downloadby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reports_login");

            entity.HasOne(d => d.Employeesearch).WithMany(p => p.Downloadreports)
                .HasForeignKey(d => d.Employeesearchid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reports_search");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_pkey");

            entity.ToTable("employee", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Designation)
                .IsRequired()
                .HasColumnName("designation");
            entity.Property(e => e.Employeecode)
                .IsRequired()
                .HasColumnName("employeecode");
            entity.Property(e => e.Exittype).HasColumnName("exittype");
            entity.Property(e => e.Fromdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fromdate");
            entity.Property(e => e.Isapproved).HasColumnName("isapproved");
            entity.Property(e => e.Isedit).HasColumnName("isedit");
            entity.Property(e => e.Jobtype).HasColumnName("jobtype");
            entity.Property(e => e.Lastdrawnsalary).HasColumnName("lastdrawnsalary");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Managerdesignation).HasColumnName("managerdesignation");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.Reasonforleaving).HasColumnName("reasonforleaving");
            entity.Property(e => e.Reportingto).HasColumnName("reportingto");
            entity.Property(e => e.Todate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("todate");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_login");

            entity.HasOne(d => d.Customer).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_customer");
        });

        modelBuilder.Entity<Employeeapproval>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employeeapproval_pkey");

            entity.ToTable("employeeapproval", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Approvedby).HasColumnName("approvedby");
            entity.Property(e => e.Approveddate).HasColumnName("approveddate");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Employeesearchid).HasColumnName("employeesearchid");
            entity.Property(e => e.Isedit).HasColumnName("isedit");
            entity.Property(e => e.Requestedby).HasColumnName("requestedby");
            entity.Property(e => e.Requesteddate).HasColumnName("requesteddate");

            entity.HasOne(d => d.ApprovedbyNavigation).WithMany(p => p.EmployeeapprovalApprovedbyNavigations)
                .HasForeignKey(d => d.Approvedby)
                .HasConstraintName("approval_login");

            entity.HasOne(d => d.Employee).WithMany(p => p.Employeeapprovals)
                .HasForeignKey(d => d.Employeeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("approval_employee");

            entity.HasOne(d => d.Employeesearch).WithMany(p => p.Employeeapprovals)
                .HasForeignKey(d => d.Employeesearchid)
                .HasConstraintName("approval_search");

            entity.HasOne(d => d.RequestedbyNavigation).WithMany(p => p.EmployeeapprovalRequestedbyNavigations)
                .HasForeignKey(d => d.Requestedby)
                .HasConstraintName("request_login");
        });

        modelBuilder.Entity<Employeequestionaire>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employeequestionaire_pkey");

            entity.ToTable("employeequestionaire", "orgcheck");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Answer)
                .IsRequired()
                .HasColumnName("answer");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Questionid).HasColumnName("questionid");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.Employeequestionaires)
                .HasForeignKey(d => d.Employeeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("empqn_employee");

            entity.HasOne(d => d.Question).WithMany(p => p.Employeequestionaires)
                .HasForeignKey(d => d.Questionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("empqn_question");
        });

        modelBuilder.Entity<Employeesearch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employeesearch_pkey");

            entity.ToTable("employeesearch", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Clientname).HasColumnName("clientname");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Employeecode)
                .IsRequired()
                .HasColumnName("employeecode");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Finalresult)
                .IsRequired()
                .HasColumnName("finalresult");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.Reportdate).HasColumnName("reportdate");
            entity.Property(e => e.Reportlink).HasColumnName("reportlink");
            entity.Property(e => e.Searchrequestid)
                .IsRequired()
                .HasColumnName("searchrequestid");
            entity.Property(e => e.Searchresult)
                .IsRequired()
                .HasColumnName("searchresult");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Transactionamount).HasColumnName("transactionamount");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Employeesearches)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("search_createdby");

            entity.HasOne(d => d.Customer).WithMany(p => p.Employeesearches)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("search_customer");

            entity.HasOne(d => d.Employee).WithMany(p => p.Employeesearches)
                .HasForeignKey(d => d.Employeeid)
                .HasConstraintName("search_employee");
        });

        modelBuilder.Entity<Empverificationrequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("empverificationrequest_pkey");

            entity.ToTable("empverificationrequest", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(1)
                .HasColumnName("active");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Invalidemployeeid).HasColumnName("invalidemployeeid");
            entity.Property(e => e.Reportname).HasColumnName("reportname");
            entity.Property(e => e.Requestnumber).HasColumnName("requestnumber");
            entity.Property(e => e.Requeststatus).HasColumnName("requeststatus");
            entity.Property(e => e.Tempemployeeid).HasColumnName("tempemployeeid");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Empverificationrequests)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("evrlogin");

            entity.HasOne(d => d.Employee).WithMany(p => p.Empverificationrequests)
                .HasForeignKey(d => d.Employeeid)
                .HasConstraintName("evremp");

            entity.HasOne(d => d.Invalidemployee).WithMany(p => p.Empverificationrequests)
                .HasForeignKey(d => d.Invalidemployeeid)
                .HasConstraintName("evrie");

            entity.HasOne(d => d.Tempemployee).WithMany(p => p.Empverificationrequests)
                .HasForeignKey(d => d.Tempemployeeid)
                .HasConstraintName("evrte");
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("files_pkey");

            entity.ToTable("files", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Filename).HasColumnName("filename");
            entity.Property(e => e.Filesize).HasColumnName("filesize");
            entity.Property(e => e.Invalidrecords).HasColumnName("invalidrecords");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Totalrecords).HasColumnName("totalrecords");
            entity.Property(e => e.Uploadedby).HasColumnName("uploadedby");
            entity.Property(e => e.Uploadeddate).HasColumnName("uploadeddate");
            entity.Property(e => e.Uploadedstatus)
                .HasDefaultValue(1)
                .HasColumnName("uploadedstatus");
            entity.Property(e => e.Validrecords).HasColumnName("validrecords");

            entity.HasOne(d => d.Customer).WithMany(p => p.Files)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("files_customer");

            entity.HasOne(d => d.UploadedbyNavigation).WithMany(p => p.Files)
                .HasForeignKey(d => d.Uploadedby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("files_login");
        });

        modelBuilder.Entity<Invalidemployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invalidemployee_pkey");

            entity.ToTable("invalidemployee", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Designation).HasColumnName("designation");
            entity.Property(e => e.Employeecode)
                .IsRequired()
                .HasColumnName("employeecode");
            entity.Property(e => e.Exittype).HasColumnName("exittype");
            entity.Property(e => e.Fromdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fromdate");
            entity.Property(e => e.Jobtype).HasColumnName("jobtype");
            entity.Property(e => e.Lastdrawnsalary).HasColumnName("lastdrawnsalary");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Managerdesignation).HasColumnName("managerdesignation");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.Reasonforleaving).HasColumnName("reasonforleaving");
            entity.Property(e => e.Reportingto).HasColumnName("reportingto");
            entity.Property(e => e.Todate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("todate");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Invalidemployees)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invalidemployee_login");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invalidemployees)
                .HasForeignKey(d => d.Customerid)
                .HasConstraintName("invalidemployee_customer");
        });

        modelBuilder.Entity<Invalidemployeequestionaire>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invalidemployeequestionaire_pkey");

            entity.ToTable("invalidemployeequestionaire", "orgcheck");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Answer)
                .IsRequired()
                .HasColumnName("answer");
            entity.Property(e => e.Invalidemployeeid).HasColumnName("invalidemployeeid");
            entity.Property(e => e.Questionid).HasColumnName("questionid");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");

            entity.HasOne(d => d.Invalidemployee).WithMany(p => p.Invalidemployeequestionaires)
                .HasForeignKey(d => d.Invalidemployeeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invalid_employeeid");

            entity.HasOne(d => d.Question).WithMany(p => p.Invalidemployeequestionaires)
                .HasForeignKey(d => d.Questionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("empqn_question");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("logins_pkey");

            entity.ToTable("logins", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Contactnumber)
                .IsRequired()
                .HasColumnName("contactnumber");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Customertypeid).HasColumnName("customertypeid");
            entity.Property(e => e.Designation).HasColumnName("designation");
            entity.Property(e => e.Displayname)
                .IsRequired()
                .HasDefaultValueSql("''::text")
                .HasColumnName("displayname");
            entity.Property(e => e.Emailid)
                .IsRequired()
                .HasColumnName("emailid");
            entity.Property(e => e.Function).HasColumnName("function");
            entity.Property(e => e.Loginname)
                .IsRequired()
                .HasColumnName("loginname");
            entity.Property(e => e.Password)
                .IsRequired()
                .HasColumnName("password");
            entity.Property(e => e.Reportingmgrdesignation).HasColumnName("reportingmgrdesignation");
            entity.Property(e => e.Reportingmgrname).HasColumnName("reportingmgrname");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Team).HasColumnName("team");
            entity.Property(e => e.Usertypeid).HasColumnName("usertypeid");

            entity.HasOne(d => d.Customer).WithMany(p => p.Logins)
                .HasForeignKey(d => d.Customerid)
                .HasConstraintName("logins_customer");

            entity.HasOne(d => d.Customertype).WithMany(p => p.Logins)
                .HasForeignKey(d => d.Customertypeid)
                .HasConstraintName("logins_customertype");

            entity.HasOne(d => d.Usertype).WithMany(p => p.Logins)
                .HasForeignKey(d => d.Usertypeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("logins_usertype");
        });

        modelBuilder.Entity<LookupCustomertype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lookup_customertype_pkey");

            entity.ToTable("lookup_customertype", "orgcheck");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<LookupDiscrepancytype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lookup_discrepancytype_pkey");

            entity.ToTable("lookup_discrepancytype", "orgcheck");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<LookupEmpverificationResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lookup_verification_response_pkey");

            entity.ToTable("lookup_empverification_response", "orgcheck");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
        });

        modelBuilder.Entity<LookupStuverificationResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lookup_stuverification_response_pkey");

            entity.ToTable("lookup_stuverification_response", "orgcheck");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
        });

        modelBuilder.Entity<LookupTransactiontype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lookup_transactiontype_pkey");

            entity.ToTable("lookup_transactiontype", "orgcheck");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<LookupUsertype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lookup_usertype_pkey");

            entity.ToTable("lookup_usertype", "orgcheck");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Questionaire>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("questionaire_pkey");

            entity.ToTable("questionaire", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Question)
                .IsRequired()
                .HasColumnName("question");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Reportdownload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reportdownload_pkey");

            entity.ToTable("reportdownload", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Downloadby).HasColumnName("downloadby");
            entity.Property(e => e.Downloaddate).HasColumnName("downloaddate");
            entity.Property(e => e.Employeesearchid).HasColumnName("employeesearchid");

            entity.HasOne(d => d.Employeesearch).WithMany(p => p.Reportdownloads)
                .HasForeignKey(d => d.Employeesearchid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_search");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("student_pkey");

            entity.ToTable("student", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Degreetype).HasColumnName("degreetype");
            entity.Property(e => e.EligibleAttainDegree).HasColumnName("eligible_attain_degree");
            entity.Property(e => e.Isapproved).HasColumnName("isapproved");
            entity.Property(e => e.Majorsubject)
                .IsRequired()
                .HasColumnName("majorsubject");
            entity.Property(e => e.Marksobtained).HasColumnName("marksobtained");
            entity.Property(e => e.Passyear).HasColumnName("passyear");
            entity.Property(e => e.Periodfrom)
                .IsRequired()
                .HasColumnName("periodfrom");
            entity.Property(e => e.Periodto)
                .IsRequired()
                .HasColumnName("periodto");
            entity.Property(e => e.Studentid).HasColumnName("studentid");
            entity.Property(e => e.Studentname)
                .IsRequired()
                .HasColumnName("studentname");
            entity.Property(e => e.Studymode).HasColumnName("studymode");
            entity.Property(e => e.University)
                .IsRequired()
                .HasColumnName("university");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stu_logins");

            entity.HasOne(d => d.Customer).WithMany(p => p.Students)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stu_customer");
        });

        modelBuilder.Entity<Studentapproval>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("studentapproval_pkey");

            entity.ToTable("studentapproval", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Approvedby).HasColumnName("approvedby");
            entity.Property(e => e.Approveddate).HasColumnName("approveddate");
            entity.Property(e => e.Isedit).HasColumnName("isedit");
            entity.Property(e => e.Requestedby).HasColumnName("requestedby");
            entity.Property(e => e.Requesteddate).HasColumnName("requesteddate");
            entity.Property(e => e.Studentid).HasColumnName("studentid");
            entity.Property(e => e.Studentsearchid).HasColumnName("studentsearchid");

            entity.HasOne(d => d.ApprovedbyNavigation).WithMany(p => p.StudentapprovalApprovedbyNavigations)
                .HasForeignKey(d => d.Approvedby)
                .HasConstraintName("studentapproval_login");

            entity.HasOne(d => d.RequestedbyNavigation).WithMany(p => p.StudentapprovalRequestedbyNavigations)
                .HasForeignKey(d => d.Requestedby)
                .HasConstraintName("studentrequest_login");

            entity.HasOne(d => d.Student).WithMany(p => p.Studentapprovals)
                .HasForeignKey(d => d.Studentid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("approval_student");

            entity.HasOne(d => d.Studentsearch).WithMany(p => p.Studentapprovals)
                .HasForeignKey(d => d.Studentsearchid)
                .HasConstraintName("approval_search");
        });

        modelBuilder.Entity<Studentrequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("studentrequest_pkey");

            entity.ToTable("studentrequest", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Raisedby).HasColumnName("raisedby");
            entity.Property(e => e.Raiseddate).HasColumnName("raiseddate");
            entity.Property(e => e.Regno)
                .IsRequired()
                .HasDefaultValueSql("''::text")
                .HasColumnName("regno");
            entity.Property(e => e.Repliedby).HasColumnName("repliedby");
            entity.Property(e => e.Replieddate).HasColumnName("replieddate");
            entity.Property(e => e.Replycomments).HasColumnName("replycomments");
            entity.Property(e => e.Requestcomments)
                .IsRequired()
                .HasDefaultValueSql("''::text")
                .HasColumnName("requestcomments");
            entity.Property(e => e.Responsetype).HasColumnName("responsetype");
            entity.Property(e => e.Searchid).HasColumnName("searchid");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.Studentrequests)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sr_customer");

            entity.HasOne(d => d.RaisedbyNavigation).WithMany(p => p.StudentrequestRaisedbyNavigations)
                .HasForeignKey(d => d.Raisedby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sr_request");

            entity.HasOne(d => d.RepliedbyNavigation).WithMany(p => p.StudentrequestRepliedbyNavigations)
                .HasForeignKey(d => d.Repliedby)
                .HasConstraintName("sr_response");

            entity.HasOne(d => d.ResponsetypeNavigation).WithMany(p => p.Studentrequests)
                .HasForeignKey(d => d.Responsetype)
                .HasConstraintName("sr_responsetype");

            entity.HasOne(d => d.Search).WithMany(p => p.Studentrequests)
                .HasForeignKey(d => d.Searchid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sr_search");
        });

        modelBuilder.Entity<Studentsearch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("studenytsearch_pkey");

            entity.ToTable("studentsearch", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Finalresult)
                .IsRequired()
                .HasColumnName("finalresult");
            entity.Property(e => e.Reportdate).HasColumnName("reportdate");
            entity.Property(e => e.Reportlink).HasColumnName("reportlink");
            entity.Property(e => e.Searchrequestid)
                .IsRequired()
                .HasColumnName("searchrequestid");
            entity.Property(e => e.Searchresult)
                .IsRequired()
                .HasColumnName("searchresult");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Studentid)
                .IsRequired()
                .HasColumnName("studentid");
            entity.Property(e => e.Studentkey).HasColumnName("studentkey");
            entity.Property(e => e.Transactionamount).HasColumnName("transactionamount");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Studentsearches)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stusearch_createdby");

            entity.HasOne(d => d.Customer).WithMany(p => p.Studentsearches)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stusearch_customer");

            entity.HasOne(d => d.StudentkeyNavigation).WithMany(p => p.Studentsearches)
                .HasForeignKey(d => d.Studentkey)
                .HasConstraintName("stusearch_student");
        });

        modelBuilder.Entity<Tempemployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tempemployee_pkey");

            entity.ToTable("tempemployee", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Customername).HasColumnName("customername");
            entity.Property(e => e.Designation).HasColumnName("designation");
            entity.Property(e => e.Employeecode)
                .IsRequired()
                .HasColumnName("employeecode");
            entity.Property(e => e.Exittype).HasColumnName("exittype");
            entity.Property(e => e.Fileid).HasColumnName("fileid");
            entity.Property(e => e.Fromdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fromdate");
            entity.Property(e => e.Jobtype).HasColumnName("jobtype");
            entity.Property(e => e.Lastdrawnsalary).HasColumnName("lastdrawnsalary");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Managerdesignation).HasColumnName("managerdesignation");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.Reasonforleaving).HasColumnName("reasonforleaving");
            entity.Property(e => e.Reportingto).HasColumnName("reportingto");
            entity.Property(e => e.Todate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("todate");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Tempemployees)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tempemployee_login");

            entity.HasOne(d => d.Customer).WithMany(p => p.Tempemployees)
                .HasForeignKey(d => d.Customerid)
                .HasConstraintName("tempemployee_customer");

            entity.HasOne(d => d.File).WithMany(p => p.Tempemployees)
                .HasForeignKey(d => d.Fileid)
                .HasConstraintName("tempemployee_file");
        });

        modelBuilder.Entity<Tempemployeequestionaire>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tempemployeequestionaire_pkey");

            entity.ToTable("tempemployeequestionaire", "orgcheck");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Answer)
                .IsRequired()
                .HasColumnName("answer");
            entity.Property(e => e.Questionid).HasColumnName("questionid");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Tempemployeeid).HasColumnName("tempemployeeid");

            entity.HasOne(d => d.Question).WithMany(p => p.Tempemployeequestionaires)
                .HasForeignKey(d => d.Questionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("empqn_question");

            entity.HasOne(d => d.Tempemployee).WithMany(p => p.Tempemployeequestionaires)
                .HasForeignKey(d => d.Tempemployeeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("temp_employeeid");
        });

        modelBuilder.Entity<Tempstudent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tempstudent_pkey");

            entity.ToTable("tempstudent", "orgcheck");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate).HasColumnName("createddate");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Degreetype).HasColumnName("degreetype");
            entity.Property(e => e.EligibleAttainDegree).HasColumnName("eligible_attain_degree");
            entity.Property(e => e.Fileid).HasColumnName("fileid");
            entity.Property(e => e.Institutionname)
                .IsRequired()
                .HasColumnName("institutionname");
            entity.Property(e => e.Majorsubject)
                .IsRequired()
                .HasColumnName("majorsubject");
            entity.Property(e => e.Marksobtained).HasColumnName("marksobtained");
            entity.Property(e => e.Passyear).HasColumnName("passyear");
            entity.Property(e => e.Periodfrom)
                .IsRequired()
                .HasColumnName("periodfrom");
            entity.Property(e => e.Periodto)
                .IsRequired()
                .HasColumnName("periodto");
            entity.Property(e => e.Studentid).HasColumnName("studentid");
            entity.Property(e => e.Studentname)
                .IsRequired()
                .HasColumnName("studentname");
            entity.Property(e => e.Studymode).HasColumnName("studymode");
            entity.Property(e => e.University).HasColumnName("university");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Tempstudents)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("st_logins");

            entity.HasOne(d => d.File).WithMany(p => p.Tempstudents)
                .HasForeignKey(d => d.Fileid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("st_file");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
