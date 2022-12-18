using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationCore.DataAccess.Config;
public class TermQuestionConfiguration : IEntityTypeConfiguration<TermQuestion>
{
	public void Configure(EntityTypeBuilder<TermQuestion> builder)
	{
		builder.HasKey(item => new { item.TermId, item.QuestionId });
	}
}
