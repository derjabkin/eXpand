#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
namespace Xpand.ExpressApp.NH
{
    public class CriteriaToNHExpressionConverter : ICriteriaToExpressionConverter
    {
        private void CriteriaToEFExpressionConverterInternal_OnFunctionOperator(Object sender, CriteriaToExpressionConverterOnCriteriaArgs e)
        {
            if (e.Criteria is FunctionOperator)
            {
                Object value = null;
                if (CriteriaToNHConverterHelper.ConvertCustomFunctionToValue((FunctionOperator)e.Criteria, out value))
                {
                    e.Result = Expression.Constant(value);
                    e.Handled = true;
                }
            }
        }
        public Expression Convert(ParameterExpression thisExpression, CriteriaOperator criteria)
        {
            Expression result = null;
            CriteriaToEFExpressionConverterInternal converter = new CriteriaToEFExpressionConverterInternal(thisExpression);
            converter.OnFunctionOperator += CriteriaToEFExpressionConverterInternal_OnFunctionOperator;
            try
            {
                result = converter.Process(criteria);
            }
            finally
            {
                converter.OnFunctionOperator -= CriteriaToEFExpressionConverterInternal_OnFunctionOperator;
            }
            return result;
        }

    }
}
