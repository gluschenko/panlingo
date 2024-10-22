﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Panlingo.LanguageIdentification.FastText.SourceGenerator
{
    [Generator]
    public class FastTextGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var modelDataProvider = context.AnalyzerConfigOptionsProvider
                .Select((options, cancellationToken) => FetchModelDataAsync().Result);

            context.RegisterSourceOutput(modelDataProvider, (sourceProductionContext, modelDataString) =>
            {
                if (!string.IsNullOrEmpty(modelDataString))
                {
                    var className = $"{nameof(FastText)}ResourceProvider";
                    var sourceBuilder = new StringBuilder();

                    sourceBuilder.AppendLine($@"
                    byte[] bytes = Base64Decode(""{modelDataString}"");
                    DefaultModel = bytes;
                    ");

                    var code = $@"
    // <auto-generated/>
    using System;
    using System.Collections.Generic;

    namespace {nameof(Panlingo)}.{nameof(LanguageIdentification)}.{nameof(FastText)}
    {{
        public static class {className}
        {{
            public static readonly byte[] DefaultModel;

            static {className}()
            {{
                {sourceBuilder}
            }}

            static byte[] Base64Decode(string base64EncodedData) 
            {{
                var result = System.Convert.FromBase64String(base64EncodedData);
                return result;
            }}
        }}
    }}
                    ";

                    sourceProductionContext.AddSource($"{className}.g.cs", SourceText.From(code, Encoding.UTF8));
                }
            });
        }

        private static async Task<string> FetchModelDataAsync()
        {
            var modelUrl = "https://dl.fbaipublicfiles.com/fasttext/supervised-models/lid.176.ftz";
            using (var httpClient = new HttpClient())
            {
                var modelData = await httpClient.GetByteArrayAsync(modelUrl);
                return Base64Encode(modelData);
            }
        }

        public static string Base64Encode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }
}
