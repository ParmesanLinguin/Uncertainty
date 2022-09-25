namespace Uncertainty.Parsing.HIR.Nodes;

record class ProgramHir : HirNode
{
    public ModuleHir[] Modules { get; init; }

    public ProgramHir(ModuleHir[] modules)
    {
        Modules = modules;
    }
}
