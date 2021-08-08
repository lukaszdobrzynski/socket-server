using Newtonsoft.Json;

namespace Common
{
    public class JsonProtocol : Protocol
    {
        protected override T Decode<T>(byte[] bytes)
        {
           var str = System.Text.Encoding.UTF8.GetString(bytes);
           return JsonConvert.DeserializeObject<T>(str);
        }

        protected override byte[] EncodeBody<T>(T message)
        {
            var serialized = JsonConvert.SerializeObject(message);
            return System.Text.Encoding.UTF8.GetBytes(serialized);
        }
    }
}