using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uncertainty.Parsing.Types;
internal class ConcreteFunction : IFunction
{
    public IType ReturnType { get; init; }

    public IType[] Parameters { get; init; }

    public TypeParameter[] TypeParameters { get; init; }

    public string Name { get; init; }

    public IModule? ParentModule { get; init; }

    public ConcreteFunction(IType returnType, IType[] parameters, TypeParameter[] typeParameters, string name, IModule? parentModule)
    {
        ReturnType = returnType;
        Parameters = parameters;
        TypeParameters = typeParameters;
        Name = name;
        ParentModule = parentModule;
    }
}
