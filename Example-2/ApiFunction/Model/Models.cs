using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServerlessPatterns.TestableLambda.ApiFunction.Model {

    public sealed class PreviewPostRequest {

        //--- Properties ---

        [Required]
        public string Markdown { get; set; }
    }

    public sealed class PreviewPostResponse {

        //--- Properties ---
        public string Html { get; set; }
    }

    public sealed class CreatePostRequest {

        //--- Properties ---

        [Required]
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

    public sealed class ListPostsResponse {

        //--- Types ---
        public sealed class Entry {

            //--- Properties ---
            public string Id { get; set; }
            public DateTimeOffset DateTime { get; set; }
        }

        //--- Properties ---
        public List<Entry> Posts { get; set; }
    }
}