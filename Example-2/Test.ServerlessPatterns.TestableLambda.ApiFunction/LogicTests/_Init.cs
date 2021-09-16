using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LambdaSharp.Logging;
using ServerlessPatterns.TestableLambda.ApiFunction;
using ServerlessPatterns.TestableLambda.ApiFunction.DataModel.Records;

namespace Test.ServerlessPatterns.TestableLambda.ApiFunction.LogicTests {
    public class _Init {

        //--- Types ---
        public class TestProvider : IDependencyProvider {

            //--- Class Properties ---
            public static DateTimeOffset DefaultUtcNow { get; } = new DateTimeOffset(2021, 09, 16, 12, 15, 32, 0, TimeSpan.Zero);

            //--- Properties ---
            public DateTimeOffset UtcNow { get; set; } = DefaultUtcNow;
            public int IdCounter { get; set; } = 0;
            public List<PostRecord> Records { get; } = new List<PostRecord>();

            //--- Methods ---
            public string CreatePostId() {
                var id = ++IdCounter;
                return id.ToString();
            }

            public Task<bool> CreatePostRecordAsync(PostRecord record, CancellationToken cancellationToken = default) {
                Records.Add(record);
                return Task.FromResult(true);
            }

            public Task<PostRecord> GetPostRecordAsync(string postId, CancellationToken cancellationToken = default)
                => Task.FromResult(Records.FirstOrDefault(record => record.Id == postId));

            public Task<IEnumerable<PostRecord>> ListPostRecordsAsync(int limit, CancellationToken cancellationToken = default)
                => Task.FromResult((IEnumerable<PostRecord>)Records.Take(limit).ToList());

            public void Log(LambdaLogLevel level, Exception exception, string format, params object[] arguments) { }
        }

        //--- Properties ---
        protected TestProvider Provider { get; set; } = new TestProvider();
    }
}
