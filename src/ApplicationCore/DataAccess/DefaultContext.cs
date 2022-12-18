using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
using ApplicationCore.Models.Data;

namespace ApplicationCore.DataAccess;
public class DefaultContext : IdentityDbContext<User>
{
	public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
	{
	}
	public DefaultContext(string connectionString) : base(new DbContextOptionsBuilder<DefaultContext>().UseSqlServer(connectionString).Options)
	{

	}
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
	public DbSet<UploadFile> UploadFiles => Set<UploadFile>();
	public DbSet<Category> Categories => Set<Category>();
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<OAuth> OAuth => Set<OAuth>();
	public DbSet<Exam> Exams => Set<Exam>();
	public DbSet<ExamPart> ExamParts => Set<ExamPart>();
	public DbSet<ExamQuestion> ExamQuestions => Set<ExamQuestion>();
	public DbSet<Question> Questions => Set<Question>();
	public DbSet<Option> Options => Set<Option>();
	public DbSet<Resolve> Resolves => Set<Resolve>();
	public DbSet<Subject> Subjects => Set<Subject>();


	public DbSet<Term> Terms => Set<Term>();
	public DbSet<Note> Notes => Set<Note>();
	public DbSet<TermQuestion> TermQuestions => Set<TermQuestion>();
	public DbSet<Recruit> Recruits => Set<Recruit>();
	public DbSet<RecruitQuestion> RecruitQuestions => Set<RecruitQuestion>();

	public DbSet<ReviewRecord> ReviewRecords => Set<ReviewRecord>();

	#region Data
	public DbSet<ExamSettings> ExamSettings { get; set; }
	public DbSet<NoteParams> NoteParams { get; set; }
	public DbSet<SubjectQuestions> SubjectQuestions { get; set; }
	public DbSet<YearRecruit> YearRecruits { get; set; }
	public DbSet<NoteCategories> NoteCategories { get; set; }
	public DbSet<TermNotes> TermNotes { get; set; }

	#endregion

	public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

}
