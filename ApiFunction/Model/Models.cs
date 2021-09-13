using System;

namespace ServerlessPatterns.TestableLambda.ApiFunction.Model {

    public sealed class PreviewPostRequest {

        //--- Properties ---
        public string Markdown { get; set; }
    }

    public sealed class PreviewPostResponse {

        //--- Properties ---
        public string Html { get; set; }
    }

    public sealed class CreatePostRequest {

        //--- Properties ---
        public string Markdown { get; set; }
    }
    public sealed class CreatePostResponse {

        //--- Properties ---
        public string Id { get; set; }
    }

    public sealed class ViewPostResponse {

        //--- Properties ---
        public string Id { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string Html { get; set; }
    }
}