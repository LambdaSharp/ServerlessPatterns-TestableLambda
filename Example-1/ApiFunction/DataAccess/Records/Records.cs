using System;

namespace ServerlessPatterns.TestableLambda.ApiFunction.DataModel.Records {

    public sealed class PostRecord {

        //--- Properties ---
        public string Id { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string Markdown { get; set; }
    }
}
