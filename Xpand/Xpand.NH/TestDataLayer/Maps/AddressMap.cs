using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestEntities;

namespace TestDataLayer.Maps
{
    public class AddressMap : ClassMap<Address>
    {
        public AddressMap()
        {
            Id(x => x.Id).GeneratedBy.Guid().UnsavedValue(Guid.Empty);
            Map(x => x.City);
            Map(x => x.Street);
            Map(x => x.ZipCode);
            Not.LazyLoad();
        }
    }
}
