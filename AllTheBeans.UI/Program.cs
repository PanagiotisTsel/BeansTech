using AllTheBeans.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddSingleton<IBeanCatalog, JsonBeanCatalog>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var contentRoot = env.ContentRootPath;

    var jsonPath = Path.Combine(contentRoot, "..", "AllTheBeans 1 (1).json");
    //check if the file is missing
    return new JsonBeanCatalog(jsonPath);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();

