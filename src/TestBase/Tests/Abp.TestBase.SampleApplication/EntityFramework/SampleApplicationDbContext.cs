﻿using System.Data.Common;
using System.Data.Entity;
using Adorable.EntityFramework;
using Adorable.TestBase.SampleApplication.ContacLists;
using Adorable.TestBase.SampleApplication.People;

namespace Adorable.TestBase.SampleApplication.EntityFramework
{
    public class SampleApplicationDbContext : AbpDbContext
    {
        public virtual IDbSet<ContactList> ContactLists { get; set; }

        public virtual IDbSet<Person> People { get; set; }

        public virtual IDbSet<Message> Messages { get; set; }

        public SampleApplicationDbContext()
        {

        }

        public SampleApplicationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public SampleApplicationDbContext(DbConnection connection)
            : base(connection, true)
        {

        }
    }
}
