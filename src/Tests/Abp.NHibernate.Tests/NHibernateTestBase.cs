﻿using System;
using System.Data;
using System.Data.SQLite;
using Adorable.Collections;
using Adorable.Modules;
using Adorable.TestBase;
using Castle.MicroKernel.Registration;
using NHibernate;

namespace Adorable.NHibernate.Tests
{
    public class NHibernateTestBase : AbpIntegratedTestBase
    {
        private SQLiteConnection _connection;

        protected override void PreInitialize()
        {
            _connection = new SQLiteConnection("data source=:memory:");
            _connection.Open();

            LocalIocManager.IocContainer.Register(
                Component.For<IDbConnection>().UsingFactoryMethod(() => _connection).LifestyleSingleton()
                );
        }

        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            modules.Add<NHibernateTestModule>();
        }

        public void UsingSession(Action<ISession> action)
        {
            using (var session = LocalIocManager.Resolve<ISessionFactory>().OpenSession(_connection))
            {
                using (var transaction = session.BeginTransaction())
                {
                    action(session);
                    session.Flush();
                    transaction.Commit();
                }
            }
        }

        public T UsingSession<T>(Func<ISession, T> func)
        {
            T result;

            using (var session = LocalIocManager.Resolve<ISessionFactory>().OpenSession(_connection))
            {
                using (var transaction = session.BeginTransaction())
                {
                    result = func(session);
                    session.Flush();
                    transaction.Commit();
                }
            }

            return result;
        }

        public override void Dispose()
        {
            _connection.Dispose();
            base.Dispose();
        }
    }
}