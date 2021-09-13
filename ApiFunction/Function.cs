using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LambdaSharp;
using LambdaSharp.ApiGateway;
using Markdig;
using ServerlessPatterns.TestableLambda.ApiFunction.DataAccess;
using ServerlessPatterns.TestableLambda.ApiFunction.DataModel.Records;
using ServerlessPatterns.TestableLambda.ApiFunction.Model;

namespace ServerlessPatterns.TestableLambda.ApiFunction {

    public sealed class Function : ALambdaApiGatewayFunction {

        //--- Constructors ---
        public Function() : base(new LambdaSharp.Serialization.LambdaSystemTextJsonSerializer()) { }

        //--- Properties ---
        public DataAccessClient DataAccessClient { get; private set; }

        //--- Methods ---
        public override async Task InitializeAsync(LambdaConfig config) {

            // read configuration settings
            var tableName = config.ReadDynamoDBTableName("DataTable");

            // initialize clients
            DataAccessClient = new DataAccessClient(tableName);
        }

        public async Task<PreviewPostResponse> PreviewPostAsync(PreviewPostRequest request)
            => new PreviewPostResponse {
                Html = Markdown.ToHtml(request.Markdown)
            };

        public async Task<CreatePostResponse> CreatePostAsync(CreatePostRequest request) {
            var postRecord = new PostRecord {
                Id = Guid.NewGuid().ToString(),
                DateTime = DateTimeOffset.UtcNow,
                Markdown = request.Markdown
            };
            await DataAccessClient.CreatePostRecordAsync(postRecord);
            return new CreatePostResponse {
                Id = postRecord.Id
            };
        }

        public async Task<ViewPostResponse> ViewPostAsync(string postId) {
            var postRecord = await DataAccessClient.GetPostRecordAsync(postId);
            if(postRecord == null) {
                throw AbortNotFound("could not find post");
            }
            return new ViewPostResponse {
                Id = postRecord.Id,
                DateTime = postRecord.DateTime,
                Html = Markdown.ToHtml(postRecord.Markdown)
            };
        }

        public async Task<ListPostsResponse> ListPostsAsync() {
            var postRecords = await DataAccessClient.ListPostRecordsAsync(limit: 10);
            return new ListPostsResponse {
                Posts = postRecords.Select(record => new ListPostsResponse.Entry {
                    Id = record.Id,
                    DateTime = record.DateTime
                }).ToList()
            };
        }
    }
}
