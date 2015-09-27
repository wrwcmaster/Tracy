using System;
using System.ServiceModel.Channels;

namespace TracyDownloadService
{
    class JsonContentTypeMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            Console.WriteLine(contentType);
            if (contentType.ToLower().Contains("text/plain") || contentType.ToLower().Contains("application/json"))
            {
                return WebContentFormat.Json;
            }
            else
            {
                return WebContentFormat.Raw;
            }
        }
    }
}