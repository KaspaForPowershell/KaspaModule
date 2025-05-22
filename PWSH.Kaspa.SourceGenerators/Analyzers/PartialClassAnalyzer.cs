namespace PWSH.Kaspa.SourceGenerators.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PartialClassAnalyzer 
    : 
    DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new
    (
#pragma warning disable RS2008 // Enable analyzer release tracking
        id: "GEN001",
#pragma warning restore RS2008 // Enable analyzer release tracking
        title: "Class must be partial",
        messageFormat: "The class '{0}' is marked with '{1}' but is not partial",
        category: "CodeGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        var semanticModel = context.SemanticModel;

        string? matchedAttributeName = null;

        // Check if the class has one of the target attributes and capture which one matched.
        var hasTargetAttribute = classDecl.AttributeLists
            .SelectMany(a => a.Attributes)
            .Any(attr =>
            {
                var symbol = semanticModel.GetSymbolInfo(attr).Symbol as IMethodSymbol;
                var attributeType = symbol?.ContainingType;

                if (attributeType == null) return false;

                var nameMatches = attributeType.Name switch
                {
                    var name when name == ResponseSchemaBoilerplateGenerator.ATTRIBUTE_NAME => true,
                    var name when name == RequestSchemaBoilerplateGenerator.ATTRIBUTE_NAME => true,
                    _ => false
                };

                var fullNameMatches = attributeType.ToDisplayString() switch
                {
                    var name when name == ResponseSchemaBoilerplateGenerator.ATTRIBUTE_NAME => true,
                    var name when name == RequestSchemaBoilerplateGenerator.ATTRIBUTE_NAME => true,
                    _ => false
                };

                if (nameMatches || fullNameMatches)
                {
                    matchedAttributeName = attributeType.ToDisplayString();
                    return true;
                }

                return false;
            });

        if (!hasTargetAttribute) return;

        // Check for partial modifier.
        var isPartial = classDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
        if (!isPartial)
        {
            var diagnostic = Diagnostic.Create
            (
                Rule,
                classDecl.Identifier.GetLocation(),
                classDecl.Identifier.Text,
                matchedAttributeName ?? "UnknownAttribute"
            );

            context.ReportDiagnostic(diagnostic);
        }
    }

}