using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using LambdaSharp.DynamoDB.Native;
using ServerlessPatterns.TestableLambda.ApiFunction.DataModel.Records;

namespace ServerlessPatterns.TestableLambda.ApiFunction.DataAccess {

    public class DataAccessClient {

        //--- Constructors ---
        public DataAccessClient(string tableName, IAmazonDynamoDB dynamoClient = null)
            => Table = new DynamoTable(tableName, dynamoClient);

        //--- Properties ---
        private IDynamoTable Table { get; }

        //--- Methods ---
        public Task<bool> CreatePostRecordAsync(PostRecord post, CancellationToken cancellationToken = default) {

            // specify DynamoDB PutItem operation
            return Table.PutItem(post.GetPrimaryKey(), post)

                // post record cannot yet exist
                .WithCondition(record => DynamoCondition.DoesNotExist(record))

                // execute PutItem operation
                .ExecuteAsync(cancellationToken);
        }

        public Task<PostRecord> GetPostRecordAsync(string postId, CancellationToken cancellationToken = default)
            => Table.GetItemAsync(DataModel.GetPrimaryKeyForPostRecord(postId), consistentRead: true, cancellationToken);

        public Task<IEnumerable<PostRecord>> ListPostRecordsAsync(int limit, CancellationToken cancellationToken = default)
            => Table.Query(DataModel.SelectPostRecords(), limit: limit, scanIndexForward: false)
                .Get(record => record.Id)
                .Get(record => record.DateTime)
                .ExecuteAsync(cancellationToken);
    }
}