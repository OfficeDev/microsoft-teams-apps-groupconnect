// <copyright file="Startup.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect
{
    extern alias BetaLib;

    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using global::Azure.Identity;
    using global::Azure.Security.KeyVault.Secrets;
    using global::Azure.Storage.Blobs;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.DIConnect.Authentication;
    using Microsoft.Teams.Apps.DIConnect.Authentication.AuthenticationHelper;
    using Microsoft.Teams.Apps.DIConnect.Bot;
    using Microsoft.Teams.Apps.DIConnect.Common.Extensions;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.ExportData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.FeedbackData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.SentNotificationData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.TeamData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping;
    using Microsoft.Teams.Apps.DIConnect.Common.Services;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.AdaptiveCard;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.CommonBot;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.DataQueue;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.ExportQueue;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.PrepareToSendQueue;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.Teams;
    using Microsoft.Teams.Apps.DIConnect.Controllers;
    using Microsoft.Teams.Apps.DIConnect.DraftNotificationPreview;
    using Microsoft.Teams.Apps.DIConnect.Helpers;
    using Microsoft.Teams.Apps.DIConnect.Localization;
    using Microsoft.Teams.Apps.DIConnect.Models;
    using Beta = BetaLib::Microsoft.Graph;

    /// <summary>
    /// Register services in DI container, and setup middle-wares in the pipeline.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Gets the IConfiguration instance.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration instance.</param>
#pragma warning disable SA1201 // Declare property before initializing in constructor.
        public Startup(IConfiguration configuration)
#pragma warning restore SA1201 // Declare property before initializing in constructor.
        {
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var useKeyVault = this.Configuration.GetValue<bool>("UseKeyVault");

            if (useKeyVault)
            {
                this.GetKeyVaultByManagedServiceIdentity();
            }
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">IServiceCollection instance.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add all options set from configuration values.
            services.AddOptions<AuthenticationOptions>()
                .Configure<IConfiguration>((authenticationOptions, configuration) =>
                {
                    Startup.FillAuthenticationOptionsProperties(authenticationOptions, configuration);
                });

            services.AddOptions<BotOptions>()
                .Configure<IConfiguration>((botOptions, configuration) =>
                {
                    botOptions.MicrosoftAppId = configuration.GetValue<string>("MicrosoftAppId");
                    botOptions.MicrosoftAppPassword = configuration.GetValue<string>("MicrosoftAppPassword");
                    botOptions.AppBaseUri = configuration.GetValue<string>("AppBaseUri");
                    botOptions.AdminTeamId = ParseTeamIdExtension.GetTeamIdFromDeepLink(configuration.GetValue<string>("AdminTeamLink"));
                    botOptions.ManifestId = configuration.GetValue<string>("ManifestId");
                });

            services.AddOptions<BotFilterMiddlewareOptions>()
                .Configure<IConfiguration>((botFilterMiddlewareOptions, configuration) =>
                {
                    botFilterMiddlewareOptions.DisableTenantFilter =
                        configuration.GetValue<bool>("DisableTenantFilter", false);
                    botFilterMiddlewareOptions.AllowedTenants =
                        configuration.GetValue<string>("AllowedTenants")?.ToString().Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                        ?.Select(p => p.Trim()).ToArray();
                });

            services.AddOptions<RepositoryOptions>()
                .Configure<IConfiguration>((repositoryOptions, configuration) =>
                {
                    repositoryOptions.StorageAccountConnectionString =
                        configuration.GetValue<string>("StorageAccountConnectionString");

                    // Setting this to true because the main application should ensure that all
                    // tables exist.
                    repositoryOptions.EnsureTableExists = true;
                });

            services.AddOptions<MessageQueueOptions>()
                .Configure<IConfiguration>((messageQueueOptions, configuration) =>
                {
                    messageQueueOptions.ServiceBusConnection =
                        configuration.GetValue<string>("ServiceBusConnection");
                });

            services.AddOptions<DataQueueMessageOptions>()
                .Configure<IConfiguration>((dataQueueMessageOptions, configuration) =>
                {
                    dataQueueMessageOptions.ForceCompleteMessageDelayInSeconds =
                        configuration.GetValue<double>("ForceCompleteMessageDelayInSeconds", 86400);
                });
            services.AddOptions<UserAppOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    options.ProactivelyInstallUserApp =
                        configuration.GetValue<bool>("ProactivelyInstallUserApp", true);

                    options.UserAppExternalId =
                        configuration.GetValue<string>("UserAppExternalId", "148a66bb-e83d-425a-927d-09f4299a9274");
                });

            services.AddOptions<QnAMakerSettings>()
               .Configure<IConfiguration>((options, configuration) =>
               {
                   options.ScoreThreshold =
                       configuration.GetValue<double>("ScoreThreshold", 0.5);
               });

            services.AddOptions();

            // Add localization services.
            services.AddLocalizationSettings(this.Configuration);

            // Add authentication services.
            AuthenticationOptions authenticationOptionsParameter = new AuthenticationOptions();
            Startup.FillAuthenticationOptionsProperties(authenticationOptionsParameter, this.Configuration);
            services.AddAuthentication(this.Configuration, authenticationOptionsParameter);
            services.AddControllersWithViews();

            // Setup SPA static files.
            // In production, the React files will be served from this directory.
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            // Add blob client.
            services.AddSingleton(sp => new BlobContainerClient(
                sp.GetService<IConfiguration>().GetValue<string>("StorageAccountConnectionString"),
                Common.Constants.BlobContainerName));

            // The bot needs an HttpClient to download and upload files.
            services.AddHttpClient();

            services.AddSingleton<BotFrameworkHttpAdapter>();

            // Add bot services.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();
            services.AddTransient<DIConnectBotFilterMiddleware>();
            services.AddSingleton<DIConnectBotAdapter>();
            services.AddTransient<AdminTeamNotifier>();
            services.AddTransient<TeamsDataCapture>();
            services.AddTransient<TeamsFileUpload>();
            services.AddTransient<KnowledgeBaseResponse>();
            services.AddTransient<NotificationCardHelper>();
            services.AddTransient<IBot, DIConnectBot>();

            // Add repositories.
            services.AddSingleton<TeamDataRepository>();
            services.AddSingleton<EmployeeResourceGroupRepository>();
            services.AddSingleton<UserDataRepository>();
            services.AddSingleton<SentNotificationDataRepository>();
            services.AddSingleton<NotificationDataRepository>();
            services.AddSingleton<ExportDataRepository>();
            services.AddSingleton<AppConfigRepository>();
            services.AddSingleton<FeedbackDataRepository>();
            services.AddSingleton<TeamUserPairUpMappingRepository>();

            // Add service bus message queues.
            services.AddSingleton<PrepareToSendQueue>();
            services.AddSingleton<DataQueue>();
            services.AddSingleton<ExportQueue>();

            // Add draft notification preview services.
            services.AddTransient<DraftNotificationPreviewService>();

            // Add Microsoft graph services.
            services.AddScoped<IAuthenticationProvider, GraphTokenProvider>();
            services.AddScoped<IGraphServiceClient, GraphServiceClient>();
            services.AddScoped<Beta.IGraphServiceClient, Beta.GraphServiceClient>();
            services.AddScoped<IGraphServiceFactory, GraphServiceFactory>();
            services.AddScoped<IGroupMembersService>(sp => sp.GetRequiredService<IGraphServiceFactory>().GetGroupMembersService());
            services.AddScoped<IGroupsService>(sp => sp.GetRequiredService<IGraphServiceFactory>().GetGroupsService());
            services.AddScoped<IAppCatalogService>(sp => sp.GetRequiredService<IGraphServiceFactory>().GetAppCatalogService());
            IQnAMakerClient qnaMakerClient = new QnAMakerClient(new ApiKeyServiceClientCredentials(this.Configuration["QnAMakerSubscriptionKey"])) { Endpoint = this.Configuration["QnAMakerApiEndpointUrl"] };
            string endpointKey = Task.Run(() => qnaMakerClient.EndpointKeys.GetKeysAsync()).Result.PrimaryEndpointKey;

            services.AddSingleton<IQnAService>((provider) => new QnAService(
                provider.GetRequiredService<AppConfigRepository>(),
                provider.GetRequiredService<IOptionsMonitor<QnAMakerSettings>>(),
                new QnAMakerRuntimeClient(new EndpointKeyServiceClientCredentials(endpointKey)) { RuntimeEndpoint = this.Configuration["QnAMakerHostUrl"] }));
            services.AddScoped<IGroupMembersService>(sp => sp.GetRequiredService<IGraphServiceFactory>().GetGroupMembersService());

            // Add Application Insights telemetry.
            services.AddApplicationInsightsTelemetry();

            // Add miscellaneous dependencies.
            services.AddTransient<TableRowKeyGenerator>();
            services.AddTransient<AdaptiveCardCreator>();
            services.AddSingleton<IAppSettingsService, AppSettingsService>();
            services.AddSingleton<ITeamMembersService, TeamMembersService>();
            services.AddSingleton<IMemberValidationHelper, MemberValidationHelper>();

            // Add helper class.
            services.AddSingleton<UserTeamMappingsHelper>();
            services.AddSingleton<CardHelper>();

            services.Configure<MvcOptions>(options =>
            {
                options.EnableEndpointRouting = false;
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IApplicationBuilder instance, which is a class that provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">IHostingEnvironment instance, which provides information about the web hosting environment an application is running in.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(applicationBuilder => this.HandleGlobalException(applicationBuilder));
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseMvc();
            app.UseAuthorization();
            app.UseRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }

        /// <summary>
        /// Fills the AuthenticationOptions's properties with the correct values from the configuration.
        /// </summary>
        /// <param name="authenticationOptions">The AuthenticationOptions whose properties will be filled.</param>
        /// <param name="configuration">The configuration.</param>
        private static void FillAuthenticationOptionsProperties(AuthenticationOptions authenticationOptions, IConfiguration configuration)
        {
            // NOTE: This AzureAd:Instance configuration setting does not need to be
            // overridden by any deployment specific value. It can stay the default value
            // that is set in the project's configuration.
            authenticationOptions.AzureAdInstance = configuration.GetValue<string>("AzureAd:Instance");
            authenticationOptions.AzureAdTenantId = configuration.GetValue<string>("AzureAd:TenantId");
            authenticationOptions.AzureAdClientId = configuration.GetValue<string>("AzureAd:ClientId");
            authenticationOptions.AzureAdApplicationIdUri = configuration.GetValue<string>("AzureAd:ApplicationIdUri");

            // NOTE: This AzureAd:ValidIssuers configuration setting does not need to be
            // overridden by any deployment specific value. It can stay the default value
            // that is set in the project's configuration.
            authenticationOptions.AzureAdValidIssuers = configuration.GetValue<string>("AzureAd:ValidIssuers");
            authenticationOptions.AdminTeamId = ParseTeamIdExtension.GetTeamIdFromDeepLink(configuration.GetValue<string>("AdminTeamLink"));
        }

        /// <summary>
        /// Handle exceptions happened in the HTTP process pipe-line.
        /// </summary>
        /// <param name="applicationBuilder">IApplicationBuilder instance, which is a class that provides the mechanisms to configure an application's request pipeline.</param>
        private void HandleGlobalException(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var loggerFactory = applicationBuilder.ApplicationServices.GetService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger(nameof(Startup));
                    logger.LogError($"{contextFeature.Error}");

                    await context.Response.WriteAsync(new
                    {
                        context.Response.StatusCode,
                        Message = "Internal Server Error.",
                    }.ToString());
                }
            });
        }

        /// <summary>
        /// Get KeyVault secrets and set app-settings values.
        /// </summary>
        private void GetKeyVaultByManagedServiceIdentity()
        {
            // Create a new secret client using the default credential from Azure.Identity using environment variables.
            var client = new SecretClient(
                vaultUri: new Uri($"{this.Configuration["KeyVault:BaseUrl"]}/"),
                credential: new DefaultAzureCredential());

            this.Configuration["MicrosoftAppPassword"] = client.GetSecret("MicrosoftAppPassword").Value.Value;
            this.Configuration["StorageAccountConnectionString"] = client.GetSecret("StorageAccountConnectionString--SecretKey").Value.Value;
            this.Configuration["ServiceBusConnection"] = client.GetSecret("ServiceBusConnection--SecretKey").Value.Value;
            this.Configuration["QnAMakerSubscriptionKey"] = client.GetSecret("QnAMakerSubscriptionKey").Value.Value;
        }
    }
}