using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SsoAuth;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services
    .AddAuthentication(option =>
    {
        option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = "okta";//GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(option =>
    {
        option.LoginPath = "/Login";
        option.Events = new CookieAuthenticationEvents()
        {
            OnSigningIn = async context =>
            {
                var identity = context.Principal.Identity as ClaimsIdentity;

                var email = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (!string.IsNullOrEmpty(email))
                {
                    var userService = context.HttpContext.RequestServices.GetService<IUserService>();
                    var user = userService.GetUserByEmail(email);

                    if (user != null)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
                    }
                }
                await Task.CompletedTask;
            }
        };
    })
    .AddOpenIdConnect("google", option =>
    {
        option.Authority = "https://accounts.google.com";
        option.ClientId = "445005812065-tkfirfjcd290b2ih44jcm0c3jkrrpmv9.apps.googleusercontent.com";
        option.ClientSecret = "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";
        option.CallbackPath = "/auth";
        option.Scope.Add("email");
    })
    .AddOpenIdConnect("okta", option =>
    {
        option.Authority = "https://dev-26993886.okta.com/oauth2/default";
        option.ClientId = "0oa8kyd4r5UhcAKza5d7";
        option.ClientSecret = "G6KhKimKQYJy9ioIFj_EuF8c4sCqzteoTxAwkF8s";
        option.CallbackPath = "/okta-auth";
        option.ResponseType = OpenIdConnectResponseType.Code;
        option.Scope.Add("email");
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
