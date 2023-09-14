namespace Gleeman.JwtGenerator.Configuration;

public static class ServiceConfigurationExtension
{
    public static IServiceCollection AddJwtGenerator(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TokenSetting>(configuration.GetSection(nameof(TokenSetting)));
        services.AddJwtBearerService(configuration);
        services.AddServiceContainer();
        return services;
    }

    private static void AddJwtBearerService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(scheme =>
        {
            scheme.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = configuration.GetValue<bool>("TokenSetting:SaveToken");
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = configuration.GetValue<bool>("TokenSetting:ValidateIssuer"),
                ValidateAudience = configuration.GetValue<bool>("TokenSetting:ValidateAudience"),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = configuration.GetValue<bool>("TokenSetting:ValidateLifetime"),
                ValidIssuer = configuration.GetValue<bool>("TokenSetting:ValidateIssuer") == true ? configuration.GetValue<string>("TokenSetting:Isser") : null,
                ValidAudience = configuration.GetValue<bool>("TokenSetting:ValidateAudience") == true ? configuration.GetValue<string>("TokenSetting:Audience") : null,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("TokenSetting:SigningKey")!)),
                ClockSkew = TimeSpan.Zero,
            };
        });

    }

    private static void AddServiceContainer(this IServiceCollection services)
    {
        services.AddScoped<ITokenGenerator, TokenGenerator>();
    }


}
