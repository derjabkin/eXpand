using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;

namespace Xpand.ExpressApp.NH
{
    public class NHDataView : XafDataView
    {
        private IList<CriteriaOperator> propertyNames;

        public NHDataView(IObjectSpace objectSpace, Type objectType, IList<DevExpress.ExpressApp.Utils.DataViewExpression> expressions, DevExpress.Data.Filtering.CriteriaOperator criteria, IList<DevExpress.Xpo.SortProperty> sorting)
            : base(objectSpace, objectType, expressions, criteria, sorting)
        {
        }

        
        protected override void InitObjects()
        {
            if (objects == null)
            {
                objectsDictionary = new Dictionary<object, XafDataViewRecord>();

                NHObjectSpace os = (NHObjectSpace)objectSpace;
                objects = new List<XafDataViewRecord>();
                propertyNames = expressions.Select(dve => dve.Expression).ToArray();
           
                
                var persistentObjects = os.GetObjects(objectType, propertyNames, criteria, sorting, topReturnedObjectsCount);
                foreach (var obj in persistentObjects)
                {
                    var record = new NHDataViewRecord(this, obj);
                    objectsDictionary.Add(obj, record);
                    AddDataViewRecordToObjects(record);
                }
            }
        }

        internal int GetPropertyIndex(string propertyName)
        {
            var expression = expressions.FirstOrDefault(e => e.Name == propertyName);

            if (expression != null)
                return expressions.IndexOf(expression);
            else
                return -1;
        }
    }
}
