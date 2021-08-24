namespace SourceCodeBuilding
{
    public enum SourceCodeBuilderStage
    {
        TopLevel,
        Namespace,
        Class,
        CodeBlock,
        ExpectingExpression
    }
}