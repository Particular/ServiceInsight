namespace ServiceInsight.Tests.ConversationsData
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using RestSharp;

    class PayLoad : IRestResponse
    {
        public PayLoad(string content)
        {
            Content = content;
        }

        public IRestRequest Request { get; set; }

        public string ContentType { get; set; }

        public long ContentLength { get; set; }

        public string ContentEncoding { get; set; }

        public string Content { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public byte[] RawBytes { get; set; }

        public Uri ResponseUri { get; set; }

        public string Server { get; set; }

        public IList<RestResponseCookie> Cookies { get; }

        public IList<Parameter> Headers { get; }

        public ResponseStatus ResponseStatus { get; set; }

        public string ErrorMessage { get; set; }

        public Exception ErrorException { get; set; }
    }
}