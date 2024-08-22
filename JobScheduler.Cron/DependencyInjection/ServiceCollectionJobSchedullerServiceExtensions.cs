﻿using JobScheduler.Cron.Configurations;
using JobScheduler.Cron.Hosting;
using JobScheduler.Cron.JobExecuter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobScheduler.Cron.DependencyInjection;

/// <summary>
/// Extension methods for adding job scheduling services to the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionJobSchedulerServiceExtensions
{
    /// <summary>
    /// Adds the job scheduler services, excluding the hosted service, to the specified 
    /// <see cref="IServiceCollection"/>. This includes the <see cref="TimeProvider"/> and 
    /// <see cref="IJobExecuter"/>. The method can be called multiple times to register different 
    /// job configurations. Note that the job executor does not handle exceptions; it is the 
    /// responsibility of the caller to ensure that the job configuration is valid and that any 
    /// necessary error handling is implemented within the job itself.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="jobConfiguration">The configuration for the scheduled job.</param>
    /// <returns>The original <see cref="IServiceCollection"/> with the job scheduler services added.</returns>
    public static IServiceCollection AddJobScheduler(this IServiceCollection services,
        JobConfiguration jobConfiguration)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IJobExecuter, JobExecuter.JobExecuter>();
        return services.AddSingleton(jobConfiguration);
    }

    /// <summary>
    /// Adds the job scheduler services, including the hosted service, to the specified 
    /// <see cref="IServiceCollection"/>. This includes the <see cref="TimeProvider"/>, 
    /// <see cref="IJobExecuter"/>, and <see cref="Microsoft.Extensions.Hosting.IHostedService"/> 
    /// (implemented by <see cref="JobExecuterBackgroundService"/>) for executing scheduled jobs. 
    /// The method can be called multiple times to register different job configurations. 
    /// Note that the application will follow the patterns of <see cref="IHostedService"/>, and the 
    /// <see cref="CancellationToken"/> provided by the hosted service will be propagated to the jobs. 
    /// The job executor does not handle exceptions; it is the responsibility of the caller to ensure 
    /// that the job configuration is valid and that any necessary error handling is implemented within 
    /// the job itself.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="jobConfiguration">The configuration for the scheduled job.</param>
    /// <returns>The original <see cref="IServiceCollection"/> with the job scheduler services added.</returns>
    public static IServiceCollection AddHostedJobScheduler(this IServiceCollection services,
        JobConfiguration jobConfiguration) => services
            .AddJobScheduler(jobConfiguration)
            .AddHostedService<JobExecuterBackgroundService>();
}