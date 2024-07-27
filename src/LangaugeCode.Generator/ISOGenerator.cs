﻿using System.Net.Http;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Panlingo.LangaugeCode.Core;
using Panlingo.LangaugeCode.Core.Models;
using Panlingo.LanguageCode.Core.Models;

namespace Panlingo.LangaugeCode.Generator
{
    /// <summary>
    /// # ISO Home: 
    /// https://www.iso.org/iso-639-language-code
    /// 
    /// # ISO 639-2:
    /// https://www.loc.gov/standards/iso639-2/langhome.html
    /// How to get code dataset: find the link "Code list for downloading" -> "Character encoding in UTF-8"
    /// https://www.loc.gov/standards/iso639-2/ISO-639-2_utf-8.txt
    ///
    /// # ISO 639-1 vs ISO 639-2
    /// https://www.loc.gov/standards/iso639-2/php/code_changes.php
    ///
    /// # ISO 639-3
    /// https://iso639-3.sil.org/code_tables/download_tables#639-3%20Code%20Set
    /// 
    /// https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3.tab
    /// https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3_Name_Index.tab
    /// https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3_Retirements.tab
    /// https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3-macrolanguages.tab
    /// 
    /// </summary>
    [Generator]
    public class ISOGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var className = $"{nameof(ISOGenerator)}Resources";
            var sourceBuilder = new StringBuilder();
            var extractor = new ISOExtractor(new HttpClient());

            A();
            B();
            C();

            var code = $@"
// <auto-generated/>
using System;
using System.Collections.Generic;
using Panlingo.LangaugeCode.Core;
using Panlingo.LangaugeCode.Core.Models;

namespace {nameof(Panlingo)}.{nameof(LangaugeCode)}
{{
    public static class {className}
    {{
        public static readonly IEnumerable<{nameof(SetTwoLanguageDescriptor)}> {nameof(SetTwoLanguageDescriptor)}List;
        public static readonly IEnumerable<{nameof(SetThreeLanguageDescriptor)}> {nameof(SetThreeLanguageDescriptor)}List;
        public static readonly IEnumerable<{nameof(SetTwoLanguageDeprecationDescriptor)}> {nameof(SetTwoLanguageDeprecationDescriptor)}List;

        static {className}()
        {{
            {sourceBuilder}
        }}
    }}
}}
            ";

            context.AddSource($"{className}.g.cs", SourceText.From(code, Encoding.UTF8));

            // Methods
            void A()
            {
                var descriptors = extractor.ExtractLangaugeCodesSetTwoAsync().GetResult();

                sourceBuilder.AppendLine($@"
                {nameof(SetTwoLanguageDescriptor)}List = new List<{nameof(SetTwoLanguageDescriptor)}>() 
                {{
                ");

                foreach (var descriptor in descriptors)
                {
                    sourceBuilder.AppendLine($@"
                    new {nameof(SetTwoLanguageDescriptor)}
                    {{
                        {nameof(SetTwoLanguageDescriptor.CodeAlpha3Bibliographic)} = ""{descriptor.CodeAlpha3Bibliographic.ToLiteral()}"",
                        {nameof(SetTwoLanguageDescriptor.CodeAlpha3Terminologic)} = ""{descriptor.CodeAlpha3Terminologic.ToLiteral()}"",
                        {nameof(SetTwoLanguageDescriptor.CodeAlpha2)} = ""{descriptor.CodeAlpha2.ToLiteral()}"",
                        {nameof(SetTwoLanguageDescriptor.EnglishName)} = ""{descriptor.EnglishName.ToLiteral()}"",
                        {nameof(SetTwoLanguageDescriptor.FrenchName)} = ""{descriptor.FrenchName.ToLiteral()}"",
                    }},
                    ");
                }

                sourceBuilder.AppendLine($@"
                }};
                ");
            }

            void B()
            {
                var descriptors = extractor.ExtractLangaugeCodesSetThreeAsync().GetResult();

                sourceBuilder.AppendLine($@"
                {nameof(SetThreeLanguageDescriptor)}List = new List<{nameof(SetThreeLanguageDescriptor)}>() 
                {{
                ");

                foreach (var descriptor in descriptors)
                {
                    sourceBuilder.AppendLine($@"
                    new {nameof(SetThreeLanguageDescriptor)}
                    {{
                        {nameof(SetThreeLanguageDescriptor.Id)} = ""{descriptor.Id.ToLiteral()}"",
                        {nameof(SetThreeLanguageDescriptor.Part2b)} = ""{descriptor.Part2b.ToLiteral()}"",
                        {nameof(SetThreeLanguageDescriptor.Part2t)} = ""{descriptor.Part2t.ToLiteral()}"",
                        {nameof(SetThreeLanguageDescriptor.Part1)} = ""{descriptor.Part1.ToLiteral()}"",
                        {nameof(SetThreeLanguageDescriptor.Scope)} = ""{descriptor.Scope.ToLiteral()}"",
                        {nameof(SetThreeLanguageDescriptor.LanguageType)} = ""{descriptor.LanguageType.ToLiteral()}"",
                        {nameof(SetThreeLanguageDescriptor.RefName)} = ""{descriptor.RefName.ToLiteral()}"",
                        {nameof(SetThreeLanguageDescriptor.Comment)} = ""{descriptor.Comment.ToLiteral()}"",
                    }},
                    ");
                }

                sourceBuilder.AppendLine($@"
                }};
                ");
            }

            void C()
            {
                var descriptors = extractor.ExtractLanguageCodeDeprecationsSetTwoAsync().GetResult();

                sourceBuilder.AppendLine($@"
                {nameof(SetTwoLanguageDeprecationDescriptor)}List = new List<{nameof(SetTwoLanguageDeprecationDescriptor)}>() 
                {{
                ");

                foreach (var descriptor in descriptors)
                {
                    sourceBuilder.AppendLine($@"
                    new {nameof(SetTwoLanguageDeprecationDescriptor)}
                    {{
                        {nameof(SetTwoLanguageDeprecationDescriptor.CodeAlpha2)} = ""{descriptor.CodeAlpha2.ToLiteral()}"",
                        {nameof(SetTwoLanguageDeprecationDescriptor.CodeAlpha2Deprecated)} = ""{descriptor.CodeAlpha2Deprecated.ToLiteral()}"",
                        {nameof(SetTwoLanguageDeprecationDescriptor.CodeAlpha3)} = ""{descriptor.CodeAlpha3.ToLiteral()}"",
                        {nameof(SetTwoLanguageDeprecationDescriptor.CodeAlpha3Deprecated)} = ""{descriptor.CodeAlpha3Deprecated.ToLiteral()}"",
                        {nameof(SetTwoLanguageDeprecationDescriptor.EnglishName)} = ""{descriptor.EnglishName.ToLiteral()}"",
                        {nameof(SetTwoLanguageDeprecationDescriptor.CategoryOfChange)} = ""{descriptor.CategoryOfChange.ToLiteral()}"",
                        {nameof(SetTwoLanguageDeprecationDescriptor.Comment)} = ""{descriptor.Comment.ToLiteral()}"",
                    }},
                    ");
                }

                sourceBuilder.AppendLine($@"
                }};
                ");
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }


    }
}
