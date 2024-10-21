using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.IssueSolving.Domain.Entities;
using SachkovTech.IssueSolving.Domain.Enums;
using SachkovTech.IssueSolving.Domain.ValueObjects;
using SachkovTech.SharedKernel.ValueObjects.Ids;

namespace SachkovTech.IssueSolving.Infrastructure.Configurations.Write;

public class UserIssueConfiguration : IEntityTypeConfiguration<UserIssue>
{
    public void Configure(EntityTypeBuilder<UserIssue> builder)
    {
        builder.ToTable("user_issues");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                push => push.Value,
                pull => UserIssueId.Create(pull));

        builder.ComplexProperty(u => u.UserId, pb =>
        {
            pb.Property(a => a.Value)
                .IsRequired()
                .HasColumnName("user_id");
        });

        builder.ComplexProperty(u => u.IssueId, pb =>
        {
            pb.Property(a => a.Value)
                .IsRequired()
                .HasColumnName("issue_id");
        });

        builder.ComplexProperty(u => u.ModuleId, pb =>
        {
            pb.Property(a => a.Value)
                .IsRequired()
                .HasColumnName("module_id");
        });

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion(
                push => push.ToString(),
                pull => Enum.Parse<IssueStatus>(pull)
            );

        builder.Property(u => u.StartDateOfExecution);

        builder.Property(u => u.EndDateOfExecution);

        builder.ComplexProperty(u => u.Attempts, pb =>
        {
            pb.Property(a => a.Value)
                .IsRequired()
                .HasColumnName("attempts");
        });

        builder.ComplexProperty(u => u.PullRequestUrl, pb =>
        {
            pb.Property(p => p.Value)
                .IsRequired()
                .HasColumnName("pull_request_url");
        });

    }
}