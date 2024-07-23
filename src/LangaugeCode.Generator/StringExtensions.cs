namespace LangaugeCode.Generator
{
    public static class StringExtensions
    {
        public static string ToLiteral(this string valueTextForCompiler)
        {
            if (valueTextForCompiler is null)
            {
                return null;
            }

            return Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(valueTextForCompiler, false);
        }
    }
}
