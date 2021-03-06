﻿using Adorable.MongoDb.Configuration;

namespace Adorable.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP MongoDb module.
    /// </summary>
    public static class AbpMongoDbConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP MongoDb module.
        /// </summary>
        public static IAbpMongoDbModuleConfiguration AbpMongoDb(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Adorable.MongoDb", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpMongoDbModuleConfiguration>());
        }
    }
}