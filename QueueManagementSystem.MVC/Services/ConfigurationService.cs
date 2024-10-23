using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.MVC.Data;
using QueueManagementSystem.MVC.Models;

namespace QueueManagementSystem.MVC.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IDbContextFactory<QueueManagementSystemContext> _dbFactory;
        public const string QueueToRoomWithAvailableUsersOnly = "QueueToRoomWithAvailableUsersOnly";
        public const string IsPoolingEnabled = "IsPoolingEnabled";
        public const string RemovePatientsDuration = "RemovePatientsFromQueueAfterHrs";
        public const string RemovePatientsTime = "RemovePatientsFromQueueAtSpecificTime";
        public const string TicketCallRepetitions = "TicketCallRepetitions";

        public ConfigurationService(IDbContextFactory<QueueManagementSystemContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        private async Task<Configuration?> GetConfigurationAsync(string configName)
        {
            using var context = _dbFactory.CreateDbContext();
            return await context.Configurations
                .FirstOrDefaultAsync(c => c.ConfigurationName == configName);
        }

        public async Task<string> GetStringConfigurationAsync(string configName, string defaultValue = "")
        {
            var config = await GetConfigurationAsync(configName);
            return config?.StringValue ?? defaultValue;
        }

        public async Task<int> GetIntConfigurationAsync(string configName, int defaultValue = 0)
        {
            var config = await GetConfigurationAsync(configName);
            return config?.IntValue ?? defaultValue;
        }

        public async Task<bool> GetBoolConfigurationAsync(string configName, bool defaultValue = false)
        {
            var config = await GetConfigurationAsync(configName);
            return config?.BoolValue ?? defaultValue;
        }

        public async Task SaveConfigurationAsync(string configName, string value)
        {
            using var context = _dbFactory.CreateDbContext();
            var config = await context.Configurations
                .FirstOrDefaultAsync(c => c.ConfigurationName == configName);

            if (config == null)
            {
                config = new Configuration
                {
                    ConfigurationName = configName,
                    StringValue = value,
                    ValueType = Configuration.ConfigurationType.String
                };
                context.Configurations.Add(config);
            }
            else
            {
                config.StringValue = value;
                config.ValueType = Configuration.ConfigurationType.String;
            }

            await context.SaveChangesAsync();
        }

        public async Task SaveConfigurationAsync(string configName, int value)
        {
            using var context = _dbFactory.CreateDbContext();
            var config = await context.Configurations
                .FirstOrDefaultAsync(c => c.ConfigurationName == configName);

            if (config == null)
            {
                config = new Configuration
                {
                    ConfigurationName = configName,
                    IntValue = value,
                    ValueType = Configuration.ConfigurationType.Integer
                };
                context.Configurations.Add(config);
            }
            else
            {
                config.IntValue = value;
                config.ValueType = Configuration.ConfigurationType.Integer;
            }

            await context.SaveChangesAsync();
        }

        public async Task SaveConfigurationAsync(string configName, bool value)
        {
            using var context = _dbFactory.CreateDbContext();
            var config = await context.Configurations
                .FirstOrDefaultAsync(c => c.ConfigurationName == configName);

            if (config == null)
            {
                config = new Configuration
                {
                    ConfigurationName = configName,
                    BoolValue = value,
                    ValueType = Configuration.ConfigurationType.Boolean
                };
                context.Configurations.Add(config);
            }
            else
            {
                config.BoolValue = value;
                config.ValueType = Configuration.ConfigurationType.Boolean;
            }

            await context.SaveChangesAsync();
        }
    }
}
