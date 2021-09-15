using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
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
        private DataAccessClient DataAccessClient { get; set; }

        //--- Methods ---
        public override async Task InitializeAsync(LambdaConfig config) {

            // read configuration settings
            var tableName = config.ReadDynamoDBTableName("DataTable");

            // initialize clients
            DataAccessClient = new DataAccessClient(tableName);
        }

        public async Task<PreviewPostResponse> PreviewPostAsync(PreviewPostRequest request)

            // respond with converted markdown into HTML
            => new PreviewPostResponse {
                Html = Markdown.ToHtml(request.Markdown)
            };

        public async Task<CreatePostResponse> CreatePostAsync(CreatePostRequest request) {

            // convert request into a new post record
            var postRecord = new PostRecord {
                Id = Guid.NewGuid().ToString(),
                DateTime = DateTimeOffset.UtcNow,
                Markdown = request.Markdown
            };

            // attempt to store post record
            bool created;
            try {
                created = await DataAccessClient.CreatePostRecordAsync(postRecord);
            } catch(Exception e) {
                LogError(e, "failed to create post record");
                created = false;
            }
            if(!created) {

                // an internal error occurred that preventedt the record from being created
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
            var postRecord = await DataAccessClient.GetPostRecordAsync(postId);
            if(postRecord == null) {
                throw AbortNotFound("could not find post");
            }

            // respond with rendered post record
            return new ViewPostResponse {
                Id = postRecord.Id,
                DateTime = postRecord.DateTime,
                Html = Markdown.ToHtml(postRecord.Markdown)
            };
        }

        public async Task<ListPostsResponse> ListPostsAsync(int? limit) {

            // fetch list of all post records
            var postRecords = await DataAccessClient.ListPostRecordsAsync(limit ?? 10);
            return new ListPostsResponse {
                Posts = postRecords.Select(record => new ListPostsResponse.Entry {
                    Id = record.Id,
                    DateTime = record.DateTime
                }).ToList()
            };
        }
    }
}
