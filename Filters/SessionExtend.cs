using System.Text.Json;

namespace Travel_Reimbursement.Filters
{
    public static class SessionExtend
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key,JsonSerializer.Serialize(value));
        }
        public static T ? GetObjectFromJson<T>(this ISession session,string key)
        {
            var value=session.GetString(key);
            return value==null? default(T):JsonSerializer.Deserialize<T>(value);

        }
    }
}