using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using LambdaSharp;
using LambdaSharp.ApiGateway;
using LambdaSharp.Logging;
using ServerlessPatterns.TestableLambda.ApiFunction.DataAccess;
using ServerlessPatterns.TestableLambda.ApiFunction.DataModel.Records;
using ServerlessPatterns.TestableLambda.ApiFunction.Model;

namespace ServerlessPatterns.TestableLambda.ApiFunction {

    public sealed class Function : ALambdaApiGatewayFunction, IDependencyProvider {

        //--- Constructors ---
        public Function() : base(new LambdaSharp.Serialization.LambdaSystemTextJsonSerializer()) { }

        //--- Properties ---
        private DataAccessClient DataAccessClient { get; set; }
        private Logic Logic { get; set; }


        //--- Methods ---
        public override async Task InitializeAsync(LambdaConfig config) {

            // read configuration settings
            var tableName = config.ReadDynamoDBTableName("DataTable");

            // initialize clients
            DataAccessClient = new DataAccessClient(tableName);
            Logic = new Logic(this);
        }

        public async Task<PreviewPostResponse> PreviewPostAsync(PreviewPostRequest request)

            // respond with converted markdown into HTML
            => new PreviewPostResponse {
                Html = Logic.ConvertMarkdownToHtml(request.Markdown)
            };

        public async Task<CreatePostResponse> CreatePostAsync(CreatePostRequest request) {
            var postRecord = await Logic.CreatePostAsync(request.Markdown);
            if(postRecord == null) {

                // an internal error occurred that prevented the record from being created
                throw Abort(new APIGatewayProxyResponse {
                    StatusCode = 500,
                    Body = "Unable to create post.",
                    Headers = {
                        ["Content-Type"] = "plain/text"
                    }
                });
            }

            // respond with new post record id
            return new CreatePostResponse {
                Id = postRecord.Id
            };
        }

        public async Task<ViewPostResponse> ViewPostAsync(string postId) {


            // attempt to fetch post record
            var postRecordAndHtml = await Logic.ViewPostAsync(postId);
            if(postRecordAndHtml.Record == null) {
                throw AbortNotFound("could not find post");
            }

            // respond with rendered post record
            return new ViewPostResponse {
                Id = postRecordAndHtml.Record.Id,
                DateTime = postRecordAndHtml.Record.DateTime,
                Html = postRecordAndHtml.Html
            };
        }

        public async Task<ListPostsResponse> ListPostsAsync(int? limit) {

            // fetch list of all post records
            var idAndDateTimeTuples = await Logic.ListPostsAsync(limit ?? 10);
            return new ListPostsResponse {
                Posts = idAndDateTimeTuples.Select(tuple => new ListPostsResponse.Entry {
                    Id = tuple.Id,
                    DateTime = tuple.DateTime
                }).ToList()
            };
        }

        //--- IDependencyProvider Members ---
        DateTimeOffset IDependencyProvider.UtcNow => DateTimeOffset.UtcNow;
        string IDependencyProvider.CreatePostId() => Guid.NewGuid().ToString();

        Task<bool> IDependencyProvider.CreatePostRecordAsync(PostRecord record, CancellationToken cancellationToken)
            => DataAccessClient.CreatePostRecordAsync(record, cancellationToken);

        Task<PostRecord> IDependencyProvider.GetPostRecordAsync(string postId, CancellationToken cancellationToken)
            => DataAccessClient.GetPostRecordAsync(postId, cancellationToken);

        Task<IEnumerable<PostRecord>> IDependencyProvider.ListPostRecordsAsync(int limit, CancellationToken cancellationToken)
            => DataAccessClient.ListPostRecordsAsync(limit, cancellationToken);

        void IDependencyProvider.Log(LambdaLogLevel level, Exception exception, string format, params object[] arguments)
            => Logger.Log(level, exception, format, arguments);
    }
}
