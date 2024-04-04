using Blazored.Toast;
using Comandas.Components;
using Comandas.Components.Account;
using Comandas.Data;
using Comandas.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<IProdutoServices, ProdutoServices>();
builder.Services.AddScoped<ICategoriaServices, CategoriaServices>();
builder.Services.AddScoped<IVendaServices, VendaServices>();
builder.Services.AddScoped<IMetodoDePagamentoServices, MetodoDePagamentoServices>();
builder.Services.AddScoped<IProdutoVendidoServices, ProdutoVendidoServices>();
builder.Services.AddScoped<IFormaDePagamentoServices, FormaDePagamentoServices>();
builder.Services.AddScoped<ICaixaServices, CaixaServices>();
builder.Services.AddScoped<ITransacaoServices, TransacaoServices>();
builder.Services.AddScoped<IUsuarioServices, UsuarioServices>();
builder.Services.AddScoped<IClienteServices, ClienteServices>();
builder.Services.AddScoped<IEmAbertoServices, EmAbertoServices>();





builder.Services.AddBlazoredToast();


builder.Services.AddMudServices();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("EasyAdmin"));
    options.AddPolicy("DonoPolicy", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("CaixaPolicy", policy =>
        policy.RequireRole("caixa"));
    options.AddPolicy("VendasPolicy", policy =>
        policy.RequireRole("vendas"));
    options.AddPolicy("CategoriasPolicy", policy =>
        policy.RequireRole("categorias"));
    options.AddPolicy("ConfigPolicy", policy =>
        policy.RequireRole("Configuracao"));
    options.AddPolicy("ProdutosPolicy", policy =>
        policy.RequireRole("produtos"));
    options.AddPolicy("UpdateCadProdPolicy", policy =>
       policy.RequireRole("cadastro/alteracao"));
    options.AddPolicy("ReabrirCaixaPolicy", policy =>
      policy.RequireRole("reabrirCaixa"));
    options.AddPolicy("CancelarVendaPolicy", policy =>
      policy.RequireRole("cacelarVenda"));
    options.AddPolicy("DashboardPolicy", policy =>
      policy.RequireRole("dashboard"));
});


builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();


var cultureInfo = new CultureInfo("pt-BR");
cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

}
else
{

    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseAntiforgery();
app.MapControllerRoute("default", "{conroller=Home}/{action=Index}/{id?}");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



// Add additional endpoints required by the Identity /Account Razor components.

app.MapAdditionalIdentityEndpoints();

app.Run();
