using Vermaat.Crm.Specflow;

namespace RR.Dynamics365.SpecFlow.Helpers
{
    internal class ObjectConverter : IObjectConverter
    {
        public object ToCrmObject(string entityName, string attributeName, string value, ICrmTestingContext context, ConvertedObjectType objectType)
        {
            return Vermaat.Crm.Specflow.ObjectConverter.ToCrmObject(entityName, attributeName, value, context, objectType);
        }
    }
}
