using Quartz;

namespace Yamato.Utilities
{
    public static class QuartzServicesUtilities
    {
        public static void StartCronJob<TJob>(IScheduler scheduler, string cron)
            where TJob : IJob
        {
            var jobName = typeof(TJob).FullName;

            var job = JobBuilder.Create<TJob>()
                .WithIdentity(jobName)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobName}.trigger")
                .WithSchedule(CronScheduleBuilder.CronSchedule(cron))
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}