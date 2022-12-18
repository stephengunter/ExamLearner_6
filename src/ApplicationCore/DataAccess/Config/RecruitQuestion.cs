using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationCore.DataAccess.Config;
public class RecruitQuestionConfiguration : IEntityTypeConfiguration<RecruitQuestion>
{
	public void Configure(EntityTypeBuilder<RecruitQuestion> builder)
	{
		builder.HasKey(item => new { item.RecruitId, item.QuestionId });

		builder.HasOne<Recruit>(item => item.Recruit)
			.WithMany(item => item.RecruitQuestions)
			.HasForeignKey(item => item.RecruitId);


		builder.HasOne<Question>(item => item.Question)
			.WithMany(item => item.RecruitQuestions)
			.HasForeignKey(item => item.QuestionId);
	}
}
