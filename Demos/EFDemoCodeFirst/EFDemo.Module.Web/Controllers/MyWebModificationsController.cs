using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web.SystemModule;
using EFDemo.Module.Data;

namespace EFDemo.Module.Web.Controllers {
	public partial class MyWebModificationsController : WebModificationsController {
        protected override void SaveAndClose(SimpleActionExecuteEventArgs args) {
            View view = View;
            base.SaveAndClose(args);
            if(!view.IsDisposed && (view is DetailView) && (((DetailView)view).ObjectTypeInfo.Type == typeof(Contact))) {
                view.Close();
            }
        }
        public MyWebModificationsController() {
			InitializeComponent();
			RegisterActions(components);
		}
	}
}
