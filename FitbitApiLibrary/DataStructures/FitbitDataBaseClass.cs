using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FitbitApiLibrary.DataStructures
{
	public abstract class FitbitDataBaseClass
	{
        public PropertyInfo[] GetAllProperties()
        {
            List<PropertyInfo> result = new List<PropertyInfo>();
            Type t = this.GetType();
            result.AddRange(t.GetProperties());
            return result.ToArray();
        }
    }
}
