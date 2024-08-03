using BenchmarkDotNet.Attributes;
using Panlingo.LanguageCode;

namespace UserAgentBetchmarks.Tests
{
    [MemoryDiagnoser(true)]
    public class LanguageCodeTest
    {
        private readonly IEnumerable<string> _langaugeCodes;
        private readonly LanguageCodeResolver _languageCodeResolverA;
        private readonly LanguageCodeResolver _languageCodeResolverB;
        private readonly LanguageCodeResolver _languageCodeResolverC;
        private readonly LanguageCodeResolver _languageCodeResolverD;
        private readonly LanguageCodeResolver _languageCodeResolverE;
        private readonly LanguageCodeResolver _languageCodeResolverF;
        private readonly LanguageCodeResolver _languageCodeResolverG;
        private readonly LanguageCodeResolver _languageCodeResolverH;

        public LanguageCodeTest()
        {
            var codes = "af als am an ar arz as ast av az azb ba bar bcl be bg bh bn bo bpy br bs bxr ca cbk ce ceb ckb co cs cv cy da de diq dsb dty dv el eml en eo es et eu fa fi fr frr fy ga gd gl gn gom gu gv he hi hif hr hsb ht hu hy ia id ie ilo io is it ja jbo jv ka kk km kn ko krc ku kv kw ky la lb lez li lmo lo lrc lt lv mai mg mhr min mk ml mn mr mrj ms mt mwl my myv mzn nah nap nds ne new nl nn no oc or os pa pam pfl pl pms pnb ps pt qu rm ro ru rue sa sah sc scn sco sd sh si sk sl so sq sr su sv sw ta te tg th tk tl tr tt tyv ug uk ur uz vec vep vi vls vo wa war wuu xal xmf yi yo yue zh";
            var codeArray = codes.Split(' ').ToArray();
            var codeList = new List<string>();

            var maxCount = 1000;

            for (var i = 0; i < maxCount; i++)
            {
                codeList.Add(codeArray[i % codeArray.Length]);
            }

            _langaugeCodes = codeList;

            _languageCodeResolverA = new LanguageCodeResolver()
                .ConvertFromIETF()
                .ToLowerAndTrim()
                .ConvertFromDeprecatedCode()
                .ReduceToMacrolanguage()
                .Select(Panlingo.LanguageCode.Models.LanguageCodeEntity.Alpha3);

            _languageCodeResolverB = new LanguageCodeResolver()
                .Select(Panlingo.LanguageCode.Models.LanguageCodeEntity.Alpha3);

            _languageCodeResolverC = new LanguageCodeResolver()
                .Select(Panlingo.LanguageCode.Models.LanguageCodeEntity.Alpha2);

            _languageCodeResolverD = new LanguageCodeResolver()
                .Select(Panlingo.LanguageCode.Models.LanguageCodeEntity.EnglishName);

            _languageCodeResolverE = new LanguageCodeResolver()
                .ReduceToMacrolanguage();

            _languageCodeResolverF = new LanguageCodeResolver()
                .ConvertFromDeprecatedCode();

            _languageCodeResolverG = new LanguageCodeResolver()
                .ToLowerAndTrim();

            _languageCodeResolverH = new LanguageCodeResolver()
                .ConvertFromIETF();
        }

        [Benchmark]
        public void A()
        {
            foreach (var code in _langaugeCodes)
            {
                LanguageCodeHelper.TryResolve(code, out var newCode, _languageCodeResolverA);
            }
        }

        [Benchmark]
        public void B()
        {
            foreach (var code in _langaugeCodes)
            {
                LanguageCodeHelper.TryResolve(code, out var newCode, _languageCodeResolverB);
            }
        }

        [Benchmark]
        public void C()
        {
            foreach (var code in _langaugeCodes)
            {
                LanguageCodeHelper.TryResolve(code, out var newCode, _languageCodeResolverC);
            }
        }

        [Benchmark]
        public void D()
        {
            foreach (var code in _langaugeCodes)
            {
                LanguageCodeHelper.TryResolve(code, out var newCode, _languageCodeResolverD);
            }
        }

        [Benchmark]
        public void E()
        {
            foreach (var code in _langaugeCodes)
            {
                LanguageCodeHelper.TryResolve(code, out var newCode, _languageCodeResolverE);
            }
        }

        [Benchmark]
        public void F()
        {
            foreach (var code in _langaugeCodes)
            {
                LanguageCodeHelper.TryResolve(code, out var newCode, _languageCodeResolverF);
            }
        }

        [Benchmark]
        public void G()
        {
            foreach (var code in _langaugeCodes)
            {
                LanguageCodeHelper.TryResolve(code, out var newCode, _languageCodeResolverG);
            }
        }

        [Benchmark]
        public void H()
        {
            foreach (var code in _langaugeCodes)
            {
                LanguageCodeHelper.TryResolve(code, out var newCode, _languageCodeResolverH);
            }
        }
    }
}
