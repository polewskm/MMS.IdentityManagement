using Newtonsoft.Json;

namespace MMS.IdentityManagement.Api.Test
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

    }
}