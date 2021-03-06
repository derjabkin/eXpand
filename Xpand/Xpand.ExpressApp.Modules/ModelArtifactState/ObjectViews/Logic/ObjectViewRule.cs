﻿using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic {
    public class ObjectViewRule : LogicRule, IObjectViewRule {
        public ObjectViewRule(IContextObjectViewRule objectViewRule)
            : base(objectViewRule) {
            ObjectView = objectViewRule.ObjectView;
        }

        public IModelObjectView ObjectView { get; set; }
    }
}
