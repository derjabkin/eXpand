using System;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.ModelDifference.Security;

namespace MasterDetailExpand.Module
{
	public enum PermissionBehaviour
	{
		None,
		Model
	}

	public class Updater : ModuleUpdater
	{
		public Updater(IObjectSpace objectSpace, Version currentDBVersion)
			: base(objectSpace, currentDBVersion)
		{
		}
	}
}

