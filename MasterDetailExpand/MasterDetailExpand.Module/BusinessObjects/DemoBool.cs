namespace MasterDetailExpand.Module.BusinessObjects
{
	using DevExpress.ExpressApp.ConditionalAppearance;
	using DevExpress.ExpressApp.Editors;
	using DevExpress.Persistent.Base;
	using DevExpress.Persistent.BaseImpl;
	using DevExpress.Xpo;
	using DevExpress.ExpressApp.Model;

	[NavigationItem]
	[ModelDefault("DefaultListViewMasterDetailMode", "ListViewAndDetailView")]
	[Appearance("rule1", TargetItems = "Bool2", Criteria = "Not IsNullOrEmpty([Name])", BackColor = "255, 0, 0")]
	[Appearance("rule2", TargetItems = "Bool1", Criteria = "Not IsNullOrEmpty([Name])", Visibility = ViewItemVisibility.Hide)]
	public class DemoBool : BaseObject
	{
		#region Fields

		private bool bool1;

		private bool bool2;
		
		private string name;

		#endregion

		#region Constructors and Destructors

		public DemoBool(Session session)
			: base(session)
		{
		}

		#endregion

		#region Public Properties
		
		[Index(1)]
		[VisibleInListView(false)]
		public bool Bool1
		{
			get { return this.bool1; }
			set { this.SetPropertyValue("Bool1", ref this.bool1, value); }
		}

		[Index(2)]
		[VisibleInListView(false)]
		public bool Bool2
		{
			get { return this.bool2; }
			set { this.SetPropertyValue("Bool2", ref this.bool2, value); }
		}

		[Index(0)]
		public string Name
		{
			get { return this.name; }
			set { this.SetPropertyValue("Name", ref this.name, value); }
		}

        [Association, Aggregated]
        public XPCollection<DemoBoolDetail> Details
        {
            get
            {
                return GetCollection<DemoBoolDetail>("Details");
            }
        }
		#endregion
	}
}