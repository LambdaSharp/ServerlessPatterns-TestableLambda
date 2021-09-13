using System;
using LambdaSharp.DynamoDB.Native;
using ServerlessPatterns.TestableLambda.ApiFunction.DataModel.Records;

namespace ServerlessPatterns.TestableLambda.ApiFunction.DataAccess {

    public static class DataModel {

        //--- Constants ---
        private const string POST_RECORD_PK_FORMAT = "POST={0}";
        private const string POST_SK_FORMAT = "INFO";

        //--- Extension Methods ---
        public static DynamoPrimaryKey<PostRecord> GetPrimaryKey(this PostRecord record)
            => GetPrimaryKeyForPostRecord(record.Id);

        //--- Methods ---
        public static DynamoPrimaryKey<PostRecord> GetPrimaryKeyForPostRecord(string postId)
            => new DynamoPrimaryKey<PostRecord>(POST_RECORD_PK_FORMAT, POST_SK_FORMAT, postId);
    }
}