﻿using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace SystemTester.Module.Win.FunctionalTests.PropertyEditors.StringLookupPropertyEditor {
    [XpandNavigationItem("PropertyEditors/StringLookup/Default", "StringLookupPropertyEditorObject_ListView")]

    [XpandNavigationItem("PropertyEditors/StringLookup/Mask", "StringLookupPropertyEditorObject_Mask_ListView")]
    [CloneView(CloneViewType.DetailView, "StringLookupPropertyEditorObject_Mask_DetailView")]
    [CloneView(CloneViewType.ListView, "StringLookupPropertyEditorObject_Mask_ListView", DetailView = "StringLookupPropertyEditorObject_Mask_DetailView")]
    public class StringLookupPropertyEditorObject:BaseObject {
        public StringLookupPropertyEditorObject(Session session) : base(session){
        }
        
        public string Phone { get; set; }
    }
}
