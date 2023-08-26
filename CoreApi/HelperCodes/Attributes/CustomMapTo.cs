using System;

namespace CoreApi.HelperCodes.Attributes
{
    [AttributeUsage((AttributeTargets.Property), Inherited = false, AllowMultiple = true)]
    public class CustomMapTo : Attribute
    {
        public CustomMapTo(string matchingName)
        {
            MatchingNameValue = matchingName;
        }

        public virtual string MatchingName => MatchingNameValue;

        protected string MatchingNameValue { get; set; }
    }
}
