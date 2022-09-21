using ApiDotnet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiDotnet.Infra.Data.Maps
{
    public class PurchaseMap : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Compra");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("IdCompra")
                .UseIdentityColumn();

            builder.Property(c => c.Date)
                .HasColumnType("date")
                .HasColumnName("DtCompra");

            builder.Property(c => c.ProductId)
                .HasColumnName("IdProduto");

            builder.Property(c => c.PersonId)
                            .HasColumnName("IdPessoa");

            builder.HasOne(x => x.Person)
                .WithMany(p => p.Purchases);

            builder.HasOne(x => x.Product)
                .WithMany(p => p.Purchases);
        }
    }
}