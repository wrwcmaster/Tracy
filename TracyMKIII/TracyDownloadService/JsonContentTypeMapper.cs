using System.ServiceModel.Channels;

namespace TracyDownloadService
{
    class JsonContentTypeMapper : WebContentTypeMapper
    {
        public override WebContentFormat
                   GetMessageFormatForContentType(string contentType)
        {
            if (contentType.ToLower() == "text/plain" || contentType == "text/javascript")
            {
                return WebContentFormat.Json;
            }
            else
            {
                return WebContentFormat.Default;
            }
        }
    }
}