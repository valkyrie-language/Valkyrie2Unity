namespace Valkyrie2Unity.Parser
{
    /// <summary>
    /// 极简解析器
    /// </summary>
    public static class ValkyrieParser
    {

        public static ParseResult Parse(string source)
        {
            return new ParseResult { 
                Success = true, 
                ClassName = "MyFirstVkScript", 
                LogMessage = "Hello valkyrie!"
            };
        }
    }
}