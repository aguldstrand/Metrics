using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CollectorLogImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var logDir = @"..\..\..\Collector\logs";
            var logFiles = Directory.EnumerateFiles(logDir, "*.txt*");

            logFiles.AsParallel()
                .WithDegreeOfParallelism(1)
                .SelectMany(GetLogEntries)
                .GroupBy(logEntry => logEntry.Session)
                .OrderBy(sg => sg.Key)
                .ForAll(sg =>
                {
                    Console.WriteLine("======== {0} ========", sg.Key);

                    foreach (var se in sg.OrderBy(le => le.TimeStamp))
                    {
                        string referer = (string)se.Data["referer"];
                        if (referer != null)
                        {
                            referer = string.IsNullOrEmpty(referer) ? "?" : referer;

                            Console.WriteLine("Page view, referer: {0}", referer);
                            continue;
                        }

                        Console.WriteLine("Page action, action: {0}", se.Data["action"]);
                    }
                });


            /*
            logFiles.AsParallel()
                .WithDegreeOfParallelism(1)
                .SelectMany(GetLogEntries)
                .GroupBy(logEntry => logEntry.Session)
                .ForAll(StoreSessionInDatabase);
             * */

        }

        private static IEnumerable<LogEntry> GetLogEntries(string filePath)
        {
            using (var reader = File.OpenText(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var logEntry = JsonConvert.DeserializeObject<LogEntry>(line, new JsonSerializerSettings
                    {
                        DefaultValueHandling= DefaultValueHandling.Ignore,
                    });

                    yield return logEntry;
                }
            }
        }

        private static void StoreSessionInDatabase(IEnumerable<LogEntry> session)
        {
            Console.WriteLine("connecting");
            var client = new Neo4jClient.GraphClient(new Uri("http://localhost:7474/db/data"));
            client.Connect();

            var graph = (Neo4jClient.IRawGraphClient)client;

            // Sort entries in ascending order
            foreach (var logEntry in session.OrderBy(logEntry => logEntry.TimeStamp))
            {
                // Find last persisted log entry in session
                var lastNode = client.Cypher.Start(new { root = client.RootNode })
                    .Match("path = root-[:session]->session-[:next*0..]->last")
                    .Where("session.s = {session_id}")
                    .WithParam("session_id", logEntry.Session)
                    .Return<Neo4jClient.Node<LogEntry>>("last")
                    .OrderBy("last.t DESC")
                    .Limit(1)
                    .Results
                    .FirstOrDefault();

                Neo4jClient.NodeReference origin = client.RootNode.Id;
                var relationType = "session";

                // is session started?
                if (lastNode != null)
                {
                    // Store log as first node in session
                    origin = lastNode.Reference;
                    relationType = "next";

                    // Check that curent log entry is newer
                    if (lastNode.Data.TimeStamp.CompareTo(logEntry.TimeStamp) >= 0)
                    {
                        // Skip log entry
                        continue;
                    }
                }

                // Store log entry in database
                var createQuery = client.Cypher.Start("origin", origin)
                    .CreateUnique("origin-[:" + relationType + "]->(step {t:{t}, s:{s}, d:{d}})")
                    .WithParam("relation_type", relationType)
                    .WithParam("t", logEntry.TimeStamp)
                    .WithParam("s", logEntry.Session)
                    .WithParam("d", logEntry.Data);

                Console.WriteLine(createQuery.Query.QueryText);
                Console.WriteLine(JsonConvert.SerializeObject(logEntry));

                try
                {
                    createQuery.ExecuteWithoutResults();
                    Console.WriteLine("created");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine("done");
        }
    }

    class LogEntry
    {
        [JsonProperty(PropertyName = "t")]
        public string TimeStamp { get; set; }

        [JsonProperty(PropertyName = "s")]
        public string Session { get; set; }

        [JsonProperty(PropertyName = "d")]
        public JObject Data { get; set; }
    }
}