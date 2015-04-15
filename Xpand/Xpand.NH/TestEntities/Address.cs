using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TestEntities
{
    [DataContract]
    public class Address
    {

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string City { get; set; }
        
        [DataMember]
        public string ZipCode { get; set; }
    }
}
