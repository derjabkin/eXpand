using DevExpress.Persistent.BaseImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace MasterDetailExpand.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ModelDefault("DefaultListViewMasterDetailMode", "ListViewAndDetailView")]
    public class DemoBoolDetail : BaseObject
    {
        public DemoBoolDetail(DevExpress.Xpo.Session session)
            : base(session)
        {
            
        }

        private DemoBool parent;
        [Association]
        public DemoBool Parent
        {
            get { return parent; }
            set { SetPropertyValue("Parent", ref parent, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { SetPropertyValue("Description", ref description, value); }
        }
    }
}
