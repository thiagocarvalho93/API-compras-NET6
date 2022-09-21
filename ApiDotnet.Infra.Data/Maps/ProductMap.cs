using System;
using ApiDotnet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiDotnet.Infra.Data.Maps
{
    public class ProductMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Produto");
            builder.HasKey(p => p.Id);

            builder.Property(c => c.Id)
                .HasColumnName("IdProduto")
                .UseIdentityColumn();

            builder.Property(p => p.Name)
                .HasColumnName("NmProduto");

            builder.Property(p => p.Price)
                .HasColumnName("VlVenda");

            builder.Property(p => p.CodErp)
                .HasColumnName("CdErp");

            builder.Property(p => p.Name)
                .HasColumnName("NmProduto");

            builder.HasMany(x => x.Purchases)
                .WithOne(p => p.Product)
                .HasForeignKey(x => x.ProductId);
        }
    }
}