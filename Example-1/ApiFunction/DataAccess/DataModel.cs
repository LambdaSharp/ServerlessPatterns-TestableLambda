using System;
using LambdaSharp.DynamoDB.Native;
using ServerlessPatterns.TestableLambda.ApiFunction.DataModel.Records;

namespace ServerlessPatterns.TestableLambda.ApiFunction.DataAccess {

    public static class DataModel {

        //--- Constants ---
        private const string POST_RECORD_PK_FORMAT = "POST";
        private const string POST_SK_FORMAT = "ID={0}";

        //--- Extension Methods ---
        public static DynamoPrimaryKey<PostRecord> GetPrimaryKey(this PostRecord record)
            => GetPrimaryKeyForPostRecord(record.Id);

        //--- Methods ---
        public static DynamoPrimaryKey<PostRecord> GetPrimaryKeyForPostRecord(string postId)
            => new DynamoPrimaryKey<PostRecord>(POST_RECORD_PK_FORMAT, POST_SK_FORMAT, postId);

        public static IDynamoQueryClause<PostRecord> SelectPostRecords()
            => DynamoQuery.SelectPK<PostRecord>(POST_RECORD_PK_FORMAT)
                .WhereSKBeginsWith(string.Format(POST_SK_FORMAT, ""));
    }
}