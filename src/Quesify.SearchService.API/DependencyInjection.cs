using Confluent.Kafka;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Quesify.SearchService.API.Data;
using Quesify.SearchService.API.IntegrationEvents.EventHandlers;
using Quesify.SharedKernel.AspNetCore.Swagger.Filters;
using Quesify.SharedKernel.EventBus;
using Quesify.SharedKernel.Guids;
using Quesify.SharedKernel.Json;
using Quesify.SharedKernel.TimeProviders;
using Quesify.SharedKernel.Utilities.Guards;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(
     this IServiceCollection services,
     IConfiguration configuration,
     ConfigureHostBuilder hostBuilder)
    {
        Guard.Against.Null(services, nameof(services));
        Guard.Against.Null(configuration, nameof(configuration));
        Guard.Against.Null(hostBuilder, nameof(hostBuilder));

        services
           .AddControllers()
           .AddJsonOptions(options =>
           {
               var enumConverter = new JsonStringEnumConverter();
               options.JsonSerializerOptions.Converters.Add(enumConverter);
           })
           .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

        services.AddFluentValidationAutoValidation(options => { options.DisableDataAnnotationsValidation = true; });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Quesify.SearchService.API", Version = "v1" });
            options.AddSecurity();
            options.AddLowercaseDocumentFilter();
        });

        services.AddEnableRequestBuffering();

        services.AddRequestTime();

        hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
            .WriteTo.Seq(context.Configuration.GetValue<string>("Seq:ServerUrl")!)
            .WriteTo.Console()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
            .AddCustomEnriches(services);
        });

        services.AddPushSerilogProperties();

        services.AddCustomHttpLogging(o => { });

        services.AddCustomAuthentication(
            configuration.GetValue<string>("AccessTokenOptions:Audience")!,
            configuration.GetValue<string>("AccessTokenOptions:Issuer")!,
            configuration.GetValue<string>("AccessTokenOptions:SecurityKey")!);

        services.AddHttpContextAccessor();
        services.AddHttpContextCurrentPrincipalAccessor();

        services.AddCustomExceptionHandler();

        services.AddGuid(options => { options.UseSequentialGuidGenerator(services); });

        services.AddTimeProvider(options => { options.UseUtcDateTime(services); });

        services.AddJsonSerializer(options => { options.UseNewtonsoft(services, settings => { }); });

        services.AddSecurity(options => { });

        services.AddEventBus(options =>
        {
            options.ProjectName = "Quesify";
            options.ServiceName = "SearchService";
            options.UseKafka(services, kafkaOptions =>
            {
                kafkaOptions.ProducerConfig.BootstrapServers = "localhost:9092"; //TODO: Read from appsettings.json
                kafkaOptions.ConsumerConfig.BootstrapServers = "localhost:9092";
                kafkaOptions.ConsumerConfig.GroupId = "Quesify.SearchService.Group";
                kafkaOptions.ConsumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                kafkaOptions.ConsumerConfig.ClientId = "Quesify.SearchService";
                kafkaOptions.ConsumerConfig.AllowAutoCreateTopics = true;
                kafkaOptions.AdminClientConfig.AllowAutoCreateTopics = true;
                kafkaOptions.AdminClientConfig.BootstrapServers = "localhost:9092";
            });
        });

        services.AddScoped<IElasticClientFactory, ElasticClientFactory>();

        services.AddTransient<QuestionCreatedIntegrationEventHandler>();
        services.AddTransient<QuestionVotedIntegrationEventHandler>();
        services.AddTransient<AnswerVotedIntegrationEventHandler>();
        services.AddTransient<UserCreatedIntegrationEventHandler>();
        services.AddTransient<UserUpdatedIntegrationEventHandler>();

        return services;
    }
}