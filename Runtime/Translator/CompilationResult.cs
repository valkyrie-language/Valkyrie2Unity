namespace Valkyrie2Unity.Translator
{
    public record CompilationResult
    {
        public bool Success { get; set; }
        public string[] Errors { get; set; }
    }
}