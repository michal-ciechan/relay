﻿using System;
using System.Linq;
using System.Threading.Tasks;
using HttpMultipartParser;
using System.IO;
using System.Text.Json;
using GraphQL.SystemTextJson;

namespace GraphQL.Relay.Http
{
    public static class Deserializer
    {
        public static async Task<RelayRequest> Deserialize(Stream body, string contentType)
        {
            RelayRequest queries;

            switch (contentType)
            {
                case "multipart/form-data":
                    queries = DeserializeFormData(body);
                    break;
                case "application/json":
                    var stream = new StreamReader(body);
                    queries = DeserializeJson(await stream.ReadToEndAsync());
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown media type: {contentType}. Cannot deserialize the Http request");
            }

            return queries;
        }


        private static RelayRequest DeserializeJson(string stringContent)
        {
            if (stringContent[0] == '[')
                return new RelayRequest(
                    JsonSerializer.Deserialize<RelayQuery[]>(stringContent),
                    isBatched: true
                );

            if (stringContent[0] == '{')
                return new RelayRequest() {
                    JsonSerializer.Deserialize<RelayQuery>(stringContent)
                };

            throw new Exception("Unrecognized request json. GraphQL queries requests should be a single object, or an array of objects");
        }

        private static RelayRequest DeserializeFormData(Stream body)
        {
            var form = MultipartFormDataParser.Parse(body);

            var req = new RelayRequest
            {
                Files = form.Files.Select(f => new HttpFile {
                    ContentDisposition = f.ContentDisposition,
                    ContentType = f.ContentType,
                    Data = f.Data,
                    FileName = f.FileName,
                    Name = f.Name
                })
            };

            req.Add(new RelayQuery {
                Query = form.Parameters.Find(p => p.Name == "query").Data,
                Variables = form.Parameters.Find(p => p.Name == "variables").Data.ToInputs(),
            });

            return req;
        }
    }
}
