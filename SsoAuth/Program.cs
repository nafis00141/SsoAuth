using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services
    .AddAuthentication(option =>
    {
        option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(option =>
    {
        option.LoginPath = "/Login";
        option.Events = new CookieAuthenticationEvents()
        {
            OnSigningIn = async context =>
            {
                var identity = context.Principal.Identity as ClaimsIdentity;

                if (identity.Claims.Any(c => (c.Type == ClaimTypes.NameIdentifier && c.Value == "nafis0014@gmail.com") || (c.Type == ClaimTypes.Email && c.Value == "nafis0014@gmail.com")))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                }
                else
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
                }

                await Task.CompletedTask;
            }
        };
    })
    .AddGoogle(option =>
    {
        option.ClientId = "445005812065-tkfirfjcd290b2ih44jcm0c3jkrrpmv9.apps.googleusercontent.com";
        option.ClientSecret = "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";
        option.CallbackPath = "/auth";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
