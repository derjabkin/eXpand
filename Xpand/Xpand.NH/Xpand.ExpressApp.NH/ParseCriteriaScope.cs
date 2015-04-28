using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH
{
    public class ParseCriteriaScope : IDisposable
    {

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        ~ParseCriteriaScope()
        {
            Dispose(false);
        }

        #endregion

	
    }
}
