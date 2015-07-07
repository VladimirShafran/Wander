using System;
using System.Reflection;
using System.Globalization;

namespace Chimera
{
	public static class StringEnum
	{
		public static string GetStringValue(Enum value)
		{
			string output = "";
			Type type = value.GetType();
			
			//Check first in our cached results...
			
			//Look for our 'StringValueAttribute' 
			
			//in the field's custom attributes
			
			FieldInfo fi = type.GetField(value.ToString());
			StringValue[] attrs = fi.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
			if (attrs.Length > 0)
			{
				output = attrs[0].Value;
			}
			
			return output;
		}
		
		public static ChimeraEvent StringToChimeraEvent(string value)
		{
			ChimeraEvent result = ChimeraEvent.Default;
			foreach (ChimeraEvent type in Enum.GetValues(typeof(ChimeraEvent)))
			{
				if (StringEnum.GetStringValue((ChimeraEvent)type) == value)
				{
					result = (ChimeraEvent) type;
					break;
				}
			}
			return result;
		}
	}
	
	public class StringValue : System.Attribute
	{
		private string _value;
		
		public StringValue(string value)
		{
			_value = value;
		}
		
		public string Value
		{
			get { return _value; }
		}
		
	}
}
