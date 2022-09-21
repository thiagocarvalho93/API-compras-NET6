using ApiDotnet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiDotnet.Infra.Data.Maps
{
    public class PersonMap : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Pessoa");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("IdPessoa")
                .UseIdentityColumn();

            builder.Property(c => c.Name)
                .HasColumnName("NmPessoa");

            builder.Property(c => c.Document)
                .HasColumnName("NrCpf");

            builder.Property(c => c.Phone)
                .HasColumnName("NrCelular");

            builder.HasMany(c => c.Purchases)
                .WithOne(p => p.Person)
                .HasForeignKey(c => c.PersonId);
        }
    }
}