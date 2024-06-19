using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var keyVaultUrl = builder.Configuration["AzureKeyVault:VaultUrl"];
var secretName = builder.Configuration["AzureKeyVault:SecretName"];
var managedIdentityClientId = "7d265de3-0ac1-4e81-a054-4f328c42a93c";

// Create a SecretClient using DefaultAzureCredential to retrieve secrets from Key Vault
var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    ManagedIdentityClientId = managedIdentityClientId
}));

// Retrieve the secret value from Key Vault
KeyVaultSecret secret = secretClient.GetSecret(secretName);
builder.Configuration["ConnectionStrings:DefaultConnection"] = secret.Value;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
