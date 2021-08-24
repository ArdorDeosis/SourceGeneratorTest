using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceCodeBuilding
{
    public class SourceCodeBuilder
    {
        private const string Indentation = "    ";
        
        private readonly Stack<SourceCodeBuilderStage> stageStack = new(new [] { SourceCodeBuilderStage.TopLevel });
        private readonly StringBuilder builder = new();
        
        private int currentIndentationLevel;

        public SourceCodeBuilderStage CurrentStage => stageStack.Peek();
        private string CurrentIndentation => new StringBuilder(Indentation.Length * currentIndentationLevel)
            .Insert(0, Indentation, currentIndentationLevel).ToString();

        public override string ToString()
        {
            if (CurrentStage != SourceCodeBuilderStage.TopLevel)
                throw new Exception("Code builder still has open stages");
            return builder.ToString();
        }

        public SourceCodeBuilder AddUsingDirective(string directive)
        {
            AssertStage(SourceCodeBuilderStage.TopLevel, SourceCodeBuilderStage.Namespace);
            builder.AppendLine($"{CurrentIndentation}using {directive};");
            return this;
        }

        public SourceCodeBuilder OpenNamespace(string name)
        {
            AssertStage(SourceCodeBuilderStage.TopLevel, SourceCodeBuilderStage.Namespace);
            builder.AppendLine($"{CurrentIndentation}namespace {name} {{");
            currentIndentationLevel++;
            stageStack.Push(SourceCodeBuilderStage.Namespace);
            return this;
        }

        public SourceCodeBuilder CloseNameSpace()
        {
            AssertStage(SourceCodeBuilderStage.Namespace);
            currentIndentationLevel--;
            builder.AppendLine($"{CurrentIndentation}}}");
            stageStack.Pop();
            return this;
        }

        public SourceCodeBuilder OpenClass(string name)
        {
            AssertStage(SourceCodeBuilderStage.TopLevel, SourceCodeBuilderStage.Namespace, SourceCodeBuilderStage.Class);
            builder.AppendLine($"{CurrentIndentation}class {name} {{");
            currentIndentationLevel++;
            stageStack.Push(SourceCodeBuilderStage.Class);
            return this;
        }

        public SourceCodeBuilder CloseClass()
        {
            AssertStage(SourceCodeBuilderStage.Class);
            currentIndentationLevel--;
            builder.AppendLine($"{CurrentIndentation}}}");
            stageStack.Pop();
            return this;
        }
        
        
        public SourceCodeBuilder OpenMethod(string name, string returnType)
        {
            AssertStage(SourceCodeBuilderStage.Class);
            builder.AppendLine($"{CurrentIndentation}{returnType} {name} () {{");
            currentIndentationLevel++;
            stageStack.Push(SourceCodeBuilderStage.CodeBlock);
            return this;
        }

        public SourceCodeBuilder CloseMethod()
        {
            AssertStage(SourceCodeBuilderStage.CodeBlock);
            currentIndentationLevel--;
            builder.AppendLine($"{CurrentIndentation}}}");
            stageStack.Pop();
            return this;
        }

        public SourceCodeBuilder AddExpressionBodiedMethod(string name, string returnType)
        {
            AssertStage(SourceCodeBuilderStage.Class);
            builder.Append($"{CurrentIndentation}{returnType} {name} () {{");
            currentIndentationLevel++;
            stageStack.Push(SourceCodeBuilderStage.CodeBlock);
            return this;
        }
        
        

        private void AssertStage(params SourceCodeBuilderStage[] validStages)
        {
            if (!validStages.Contains(CurrentStage))
                throw new Exception("Invalid Stage");
        }
    }
}