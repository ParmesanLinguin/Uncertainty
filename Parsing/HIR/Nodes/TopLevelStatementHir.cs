using Uncertainty.Parsing.Types;

namespace Uncertainty.Parsing.HIR.Nodes;

record class ModuleHir : HirNode
{
    public FunctionDeclarationHir[] Functions { get; init; }

    public IModule Module { get; init; }

    public ModuleHir(FunctionDeclarationHir[] functions, IModule module)
    {
        Functions = functions;
        Module = module;
    }
}