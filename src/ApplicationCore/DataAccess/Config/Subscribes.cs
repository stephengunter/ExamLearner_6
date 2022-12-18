using System.Reflection.Emit;
using System.Reflection.Metadata;
using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationCore.DataAccess.Config;
public class SubscribeConfiguration : IEntityTypeConfiguration<Subscribe>
{
	public void Configure(EntityTypeBuilder<Subscribe> builder)
	{
		builder.HasOne(s => s.Bill)
		  .WithMany(b => b.Subscribes)
		  .OnDelete(DeleteBehavior.ClientCascade);
	}
}
