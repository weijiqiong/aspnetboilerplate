﻿using System;
using Adorable.Dependency;
using Adorable.Runtime.Caching;
using Adorable.Runtime.Caching.Configuration;
using Adorable.Runtime.Caching.Memory;
using Shouldly;
using Xunit;

namespace Adorable.Tests.Runtime.Caching.Memory
{
    public class MemoryCacheManager_Tests : TestBaseWithLocalIocManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly ITypedCache<string, MyCacheItem> _cache;

        public MemoryCacheManager_Tests()
        {
            LocalIocManager.Register<ICachingConfiguration, CachingConfiguration>();
            LocalIocManager.Register<ICacheManager, AbpMemoryCacheManager>();
            LocalIocManager.Register<MyClientPropertyInjects>(DependencyLifeStyle.Transient);
            _cacheManager = LocalIocManager.Resolve<ICacheManager>();

            var defaultSlidingExpireTime = TimeSpan.FromHours(24);
            LocalIocManager.Resolve<ICachingConfiguration>().ConfigureAll(cache =>
            {
                cache.DefaultSlidingExpireTime = defaultSlidingExpireTime;
            });

            _cache = _cacheManager.GetCache<string, MyCacheItem>("MyCacheItems");
            _cache.DefaultSlidingExpireTime.ShouldBe(defaultSlidingExpireTime);
        }

        [Fact]
        public void Simple_Get_Set_Test()
        {
            _cache.GetOrDefault("A").ShouldBe(null);

            _cache.Set("A", new MyCacheItem { Value = 42 });

            _cache.GetOrDefault("A").ShouldNotBe(null);
            _cache.GetOrDefault("A").Value.ShouldBe(42);

            _cache.Get("B", () => new MyCacheItem { Value = 43 }).Value.ShouldBe(43);
            _cache.Get("B", () => new MyCacheItem { Value = 44 }).Value.ShouldBe(43); //Does not call factory, so value is not changed
        }

        [Fact]
        public void Property_Injected_CacheManager_Should_Work()
        {
            LocalIocManager.Using<MyClientPropertyInjects>(client =>
            {
                client.SetGetValue(42).ShouldBe(42); //Not in cache, getting from factory
            });

            LocalIocManager.Using<MyClientPropertyInjects>(client =>
            {
                client.SetGetValue(43).ShouldBe(42); //Retrieving from the cache
            });
        }

        [Serializable]
        public class MyCacheItem
        {
            public int Value { get; set; }
        }

        public class MyClientPropertyInjects
        {
            public ICacheManager CacheManager { get; set; }

            public int SetGetValue(int value)
            {
                return CacheManager
                    .GetCache("MyClientPropertyInjectsCache")
                    .Get("A", () =>
                    {
                        return value;
                    });
            }
        }
    }
}
