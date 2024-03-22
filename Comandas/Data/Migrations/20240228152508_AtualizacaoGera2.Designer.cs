﻿// <auto-generated />
using System;
using Comandas.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Comandas.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240228152508_AtualizacaoGera2")]
    partial class AtualizacaoGera2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Comandas.Data.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Comandas.Data.Models.Caixa", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal?>("Abertura")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<DateTime>("DataDeAbertura")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DataDeFechamento")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Estado")
                        .HasColumnType("bit");

                    b.Property<decimal?>("Total")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal?>("TotalDeEntrada")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal?>("TotalDeSaida")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<string>("UsuarioId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Caixas");
                });

            modelBuilder.Entity("Comandas.Data.Models.Categoria", b =>
                {
                    b.Property<Guid>("CategoriaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Descricao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("CategoriaId");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Categorias");
                });

            modelBuilder.Entity("Comandas.Data.Models.FechamentoCaixa", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CaixaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdMetodo")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NomeMetodo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ValorApurado")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal>("ValorInformado")
                        .HasColumnType("decimal(5, 2)");

                    b.HasKey("Id");

                    b.HasIndex("CaixaId");

                    b.ToTable("fechamentoCaixas");
                });

            modelBuilder.Entity("Comandas.Data.Models.FormaDePagamento", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MetodoDePagamentoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NomeDoMetodo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Valor")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<Guid>("VendaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("VendaId");

                    b.ToTable("FormaDePagamento");
                });

            modelBuilder.Entity("Comandas.Data.Models.MetodoDePagamento", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("IdDoUsuario")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Padrao")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("MetodosDePagamento");
                });

            modelBuilder.Entity("Comandas.Data.Models.Produto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("CategoriaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Codigo")
                        .HasColumnType("int");

                    b.Property<Guid?>("ID_categoria")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsControled")
                        .HasColumnType("bit");

                    b.Property<double?>("MargemLucro")
                        .HasColumnType("float");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NomeDaCategoria")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PrecoAutomatico")
                        .HasColumnType("bit");

                    b.Property<int?>("Quantidade")
                        .HasColumnType("int");

                    b.Property<decimal?>("Valor")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal?>("ValorDeCusto")
                        .HasColumnType("decimal(5, 2)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("CategoriaId");

                    b.ToTable("Produtos");
                });

            modelBuilder.Entity("Comandas.Data.Models.ProdutoVendido", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal?>("Desconto")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<Guid>("IdDoProduto")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumeroDaVenda")
                        .HasColumnType("int");

                    b.Property<int?>("Quantidade")
                        .HasColumnType("int");

                    b.Property<Guid?>("VendaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("VendaId");

                    b.ToTable("ProdutosVendidos");
                });

            modelBuilder.Entity("Comandas.Data.Models.Transacao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CaixaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("MetodoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MetodoNome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Tipo")
                        .HasColumnType("bit");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserNome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Valor")
                        .HasColumnType("decimal(5, 2)");

                    b.HasKey("Id");

                    b.HasIndex("CaixaId");

                    b.ToTable("transacoes");
                });

            modelBuilder.Entity("Comandas.Data.Models.Venda", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CaixaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DataDaVenda")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("Descontos")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<string>("IdUsuario")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NomeDoUsuario")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Numero")
                        .HasColumnType("int");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(5, 2)");

                    b.HasKey("Id");

                    b.HasIndex("CaixaId");

                    b.ToTable("Vendas");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Comandas.Data.Models.Categoria", b =>
                {
                    b.HasOne("Comandas.Data.ApplicationUser", "UserId")
                        .WithMany("Categorias")
                        .HasForeignKey("ApplicationUserId");

                    b.Navigation("UserId");
                });

            modelBuilder.Entity("Comandas.Data.Models.FechamentoCaixa", b =>
                {
                    b.HasOne("Comandas.Data.Models.Caixa", "Caixa")
                        .WithMany("fhecamentos")
                        .HasForeignKey("CaixaId");

                    b.Navigation("Caixa");
                });

            modelBuilder.Entity("Comandas.Data.Models.FormaDePagamento", b =>
                {
                    b.HasOne("Comandas.Data.Models.Venda", "Venda")
                        .WithMany("formasDePagamentosDaVenda")
                        .HasForeignKey("VendaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Venda");
                });

            modelBuilder.Entity("Comandas.Data.Models.Produto", b =>
                {
                    b.HasOne("Comandas.Data.ApplicationUser", "UserId")
                        .WithMany("Produtos")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("Comandas.Data.Models.Categoria", "Categoria")
                        .WithMany("Produtos")
                        .HasForeignKey("CategoriaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Categoria");

                    b.Navigation("UserId");
                });

            modelBuilder.Entity("Comandas.Data.Models.ProdutoVendido", b =>
                {
                    b.HasOne("Comandas.Data.Models.Venda", "Venda")
                        .WithMany("produtoVendidos")
                        .HasForeignKey("VendaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Venda");
                });

            modelBuilder.Entity("Comandas.Data.Models.Transacao", b =>
                {
                    b.HasOne("Comandas.Data.Models.Caixa", "Caixa")
                        .WithMany("transacoes")
                        .HasForeignKey("CaixaId");

                    b.Navigation("Caixa");
                });

            modelBuilder.Entity("Comandas.Data.Models.Venda", b =>
                {
                    b.HasOne("Comandas.Data.Models.Caixa", "Caixa")
                        .WithMany("Vendas")
                        .HasForeignKey("CaixaId");

                    b.Navigation("Caixa");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Comandas.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Comandas.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Comandas.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Comandas.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Comandas.Data.ApplicationUser", b =>
                {
                    b.Navigation("Categorias");

                    b.Navigation("Produtos");
                });

            modelBuilder.Entity("Comandas.Data.Models.Caixa", b =>
                {
                    b.Navigation("Vendas");

                    b.Navigation("fhecamentos");

                    b.Navigation("transacoes");
                });

            modelBuilder.Entity("Comandas.Data.Models.Categoria", b =>
                {
                    b.Navigation("Produtos");
                });

            modelBuilder.Entity("Comandas.Data.Models.Venda", b =>
                {
                    b.Navigation("formasDePagamentosDaVenda");

                    b.Navigation("produtoVendidos");
                });
#pragma warning restore 612, 618
        }
    }
}
