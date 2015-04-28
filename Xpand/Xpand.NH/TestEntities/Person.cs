using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TestEntities
{
    [DefaultClassOptions]
    [DataContract(IsReference = true)]
    public class Person
    {
        private List<PhoneNumber> phoneNumbers;

        public Person()
        {
            Address = new Address();
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime BirthDate { get; set; }

        public string NonpersistentProperty { get; set; }

        [DataMember]
        [DataSourceCriteria("FirstName<>'Sergej'")]
        public Person Manager { get; set; }

        [DataMember]
        public IList<PhoneNumber> PhoneNumbers
        {
            get
            {
                if (phoneNumbers == null)
                    phoneNumbers = new List<PhoneNumber>();

                return phoneNumbers;
            }
        }

        private Address address;
        [DataMember, ExpandObjectMembers(ExpandObjectMembers.Always)]
        public Address Address
        {
            get { return address; }
            set
            {
                if (value != null)
                    address = value;
            }
        }
    }
}
