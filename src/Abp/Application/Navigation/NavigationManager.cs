﻿using System.Collections.Generic;
using Adorable.Configuration.Startup;
using Adorable.Dependency;
using Adorable.Localization;

namespace Adorable.Application.Navigation
{
    internal class NavigationManager : INavigationManager, ISingletonDependency
    {
        public IDictionary<string, MenuDefinition> Menus { get; private set; }

        public MenuDefinition MainMenu
        {
            get { return Menus["MainMenu"]; }
        }

        private readonly IIocResolver _iocResolver;
        private readonly INavigationConfiguration _configuration;

        public NavigationManager(IIocResolver iocResolver, INavigationConfiguration configuration)
        {
            _iocResolver = iocResolver;
            _configuration = configuration;

            Menus = new Dictionary<string, MenuDefinition>
                    {
                        {"MainMenu", new MenuDefinition("MainMenu", new FixedLocalizableString("Main menu"))} //TODO: Localization for "Main menu"
                    };
        }

        public void Initialize()
        {
            var context = new NavigationProviderContext(this);

            foreach (var providerType in _configuration.Providers)
            {
                var provider = (NavigationProvider)_iocResolver.Resolve(providerType);
                provider.SetNavigation(context);
            }
        }
    }
}