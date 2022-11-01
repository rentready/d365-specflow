using Vermaat.Crm.Specflow;

namespace RR.Dynamics365.SpecFlow.Helpers
{
    public interface IObjectConverter
    {
        object ToCrmObject(string entityName, string attributeName, string value, ICrmTestingContext context, ConvertedObjectType objectType);
    }
}
