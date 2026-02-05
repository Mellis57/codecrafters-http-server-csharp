using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_http_server.src.Constants
{
    public static class ResponseHeaders
    {
        // Message body information
        public const string ContentType = "Content-Type:";
        public const string ContentLength = "Content-Length:";
        public const string ContentEncoding = "Content-Encoding:";
        public const string ContentLanguage = "Content-Language:";
        public const string ContentLocation = "Content-Location:";
        public const string ContentDisposition = "Content-Disposition:";
        public const string ContentRange = "Content-Range:";

        // Authentication
        public const string WWWAuthenticate = "WWW-Authenticate:";
        public const string ProxyAuthenticate = "Proxy-Authenticate:";

        // Caching
        public const string Age = "Age:";
        public const string CacheControl = "Cache-Control:";
        public const string Expires = "Expires:";

        // Conditionals
        public const string ETag = "ETag:";
        public const string LastModified = "Last-Modified:";
        public const string Vary = "Vary:";

        // Connection management
        public const string Connection = "Connection:";
        public const string KeepAlive = "Keep-Alive:";

        // Cookies
        public const string SetCookie = "Set-Cookie:";

        // CORS
        public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin:";
        public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials:";
        public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers:";
        public const string AccessControlAllowMethods = "Access-Control-Allow-Methods:";
        public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers:";
        public const string AccessControlMaxAge = "Access-Control-Max-Age:";

        // Redirects
        public const string Location = "Location:";
        public const string Refresh = "Refresh:";

        // Response context
        public const string Allow = "Allow:";
        public const string Server = "Server:";

        // Transfer coding
        public const string TransferEncoding = "Transfer-Encoding:";
        public const string Trailer = "Trailer:";

        // Security
        public const string StrictTransportSecurity = "Strict-Transport-Security:";
        public const string ContentSecurityPolicy = "Content-Security-Policy:";
        public const string XContentTypeOptions = "X-Content-Type-Options:";
        public const string XFrameOptions = "X-Frame-Options:";
        public const string XXSSProtection = "X-XSS-Protection:";
        public const string ReferrerPolicy = "Referrer-Policy:";
        public const string PermissionsPolicy = "Permissions-Policy:";
        public const string CrossOriginEmbedderPolicy = "Cross-Origin-Embedder-Policy:";
        public const string CrossOriginOpenerPolicy = "Cross-Origin-Opener-Policy:";
        public const string CrossOriginResourcePolicy = "Cross-Origin-Resource-Policy:";

        // Other
        public const string Date = "Date:";
        public const string Link = "Link:";
        public const string RetryAfter = "Retry-After:";
        public const string AcceptRanges = "Accept-Ranges:";
        public const string AltSvc = "Alt-Svc:";
        public const string ServerTiming = "Server-Timing:";
    }

    public static class RequestHeaders
    {
        // Request context
        public const string Host = "Host:";
        public const string UserAgent = "User-Agent:";
        public const string From = "From:";
        public const string Referer = "Referer:";

        // Content negotiation
        public const string Accept = "Accept:";
        public const string AcceptEncoding = "Accept-Encoding:";
        public const string AcceptLanguage = "Accept-Language:";

        // Authentication
        public const string Authorization = "Authorization:";
        public const string ProxyAuthorization = "Proxy-Authorization:";

        // Caching
        public const string CacheControl = "Cache-Control:";

        // Conditionals
        public const string IfMatch = "If-Match:";
        public const string IfNoneMatch = "If-None-Match:";
        public const string IfModifiedSince = "If-Modified-Since:";
        public const string IfUnmodifiedSince = "If-Unmodified-Since:";
        public const string IfRange = "If-Range:";

        // Connection management
        public const string Connection = "Connection:";
        public const string KeepAlive = "Keep-Alive:";

        // Cookies
        public const string Cookie = "Cookie:";

        // CORS
        public const string Origin = "Origin:";
        public const string AccessControlRequestHeaders = "Access-Control-Request-Headers:";
        public const string AccessControlRequestMethod = "Access-Control-Request-Method:";

        // Message body information
        public const string ContentType = "Content-Type:";
        public const string ContentLength = "Content-Length:";
        public const string ContentEncoding = "Content-Encoding:";

        // Range requests
        public const string Range = "Range:";

        // Controls
        public const string Expect = "Expect:";
        public const string MaxForwards = "Max-Forwards:";

        // Transfer coding
        public const string TE = "TE:";
        public const string Trailer = "Trailer:";
        public const string TransferEncoding = "Transfer-Encoding:";

        // Proxies
        public const string Forwarded = "Forwarded:";
        public const string Via = "Via:";
        public const string XForwardedFor = "X-Forwarded-For:";
        public const string XForwardedHost = "X-Forwarded-Host:";
        public const string XForwardedProto = "X-Forwarded-Proto:";

        // Security
        public const string UpgradeInsecureRequests = "Upgrade-Insecure-Requests:";

        // Other
        public const string Date = "Date:";
        public const string Upgrade = "Upgrade:";
    }
}
