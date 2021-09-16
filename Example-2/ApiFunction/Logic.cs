using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LambdaSharp.Logging;
using Markdig;
using ServerlessPatterns.TestableLambda.ApiFunction.DataModel.Records;

namespace ServerlessPatterns.TestableLambda.ApiFunction {

    public interface IDependencyProvider {

        //--- Properties ---
        DateTimeOffset UtcNow { get; }

        //--- Methods ---
        string CreatePostId();

        Task<bool> CreatePostRecordAsync(PostRecord record, CancellationToken cancellationToken = default);
        Task<PostRecord> GetPostRecordAsync(string postId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostRecord>> ListPostRecordsAsync(int limit, CancellationToken cancellationToken = default);
        void Log(LambdaLogLevel level, Exception exception, string format, params object[] arguments);
    }

    public class Logic {

        //--- Constructors ---
        public Logic(IDependencyProvider provider)
            => Provider = provider ?? throw new System.ArgumentNullException(nameof(provider));

        //--- Properties ---
        private IDependencyProvider Provider { get; }

        //--- Methods ---
        public string ConvertMarkdownToHtml(string markdown)
            => Markdown.ToHtml(markdown);

        public async Task<PostRecord> CreatePostAsync(string markdown, CancellationToken cancellationToken = default) {

            // convert request into a new post record
            var postRecord = new PostRecord {
                Id = Provider.CreatePostId(),
                DateTime = Provider.UtcNow,
                Markdown = markdown
            };

            // attempt to store post record
            bool created;
            try {
                created = await Provider.CreatePostRecordAsync(postRecord, cancellationToken);
            } catch(Exception e) {
                Provider.Log(LambdaLogLevel.ERROR, e, "failed to create post record");
                created = false;
            }
            return created
                ? postRecord
                : null;
        }

        public async Task<(PostRecord Record, string Html)> ViewPostAsync(string postId, CancellationToken cancellationToken = default) {

            // attempt to fetch post record
            var postRecord = await Provider.GetPostRecordAsync(postId);
            if(postRecord == null) {
                return (Record: null, Html: null);
            }

            // respond with rendered post record
            return (Record: postRecord, Html: Markdown.ToHtml(postRecord.Markdown));
        }

        public async Task<IEnumerable<(string Id, DateTimeOffset DateTime)>> ListPostsAsync(int limit, CancellationToken cancellationToken = default) {

            // fetch list of all post records
            var postRecords = await Provider.ListPostRecordsAsync(limit);
            return postRecords.Select(record => (Id: record.Id, DateTime: record.DateTime)).ToList();
        }
    }
}