﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adorable.Application.Services;
using Adorable.Application.Services.Dto;
using Adorable.Auditing;
using Adorable.Authorization;
using Adorable.AutoMapper;
using Adorable.Domain.Repositories;
using Adorable.Extensions;
using Adorable.TestBase.SampleApplication.People.Dto;

namespace Adorable.TestBase.SampleApplication.People
{
    public class PersonAppService : ApplicationService, IPersonAppService
    {
        private readonly IRepository<Person> _personRepository;

        public PersonAppService(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }

        [DisableAuditing]
        public ListResultOutput<PersonDto> GetPeople(GetPeopleInput input)
        {
            var query = _personRepository.GetAll();

            if (!input.NameFilter.IsNullOrEmpty())
            {
                query = query.Where(p => p.Name.Contains(input.NameFilter));
            }

            var people = query.ToList();

            return new ListResultOutput<PersonDto>(people.MapTo<List<PersonDto>>());
        }

        public async Task CreatePersonAsync(CreatePersonInput input)
        {
            await _personRepository.InsertAsync(input.MapTo<Person>());
        }

        [AbpAuthorize("CanDeletePerson")]
        public async Task DeletePerson(EntityRequestInput input)
        {
            await _personRepository.DeleteAsync(input.Id);
        }

        public string TestPrimitiveMethod(int a, string b, EntityRequestInput c)
        {
            return a + "#" + b + "#" + c.Id;
        }
    }
}
