using Autofac;
using Autofac.Extensions.DependencyInjection;
using MailerRobot.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
	config.Sources.Clear();

	var env = hostingContext.HostingEnvironment;

	config.AddJsonFile("appsettings.json", true, true)
		.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

	config.AddEnvironmentVariables();
});


builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(b => { b.RegisterModule(new MainModule(builder.Configuration)); });


builder.Host.ConfigureServices(services =>
{
	services.Configure<HostOptions>(hostOptions =>
	{
		hostOptions.BackgroundServiceExceptionBehavior =
			BackgroundServiceExceptionBehavior.Ignore;
	});
});



// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(b =>
	{
		b.WithOrigins("*")
		.AllowAnyHeader()
		.AllowAnyMethod();
	});
});

builder.Services.AddAutofac();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();