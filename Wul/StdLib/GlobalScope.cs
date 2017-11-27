using System.Linq;
using System.Reflection;
using Wul.Interpreter;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    class Global
    {
        private static Scope _scope;

        public static Scope Scope => _scope ?? (_scope = new Scope(null));

        public static void RegisterDefaultFunctions()
        {
            //Types
            Scope["Bool"] = BoolType.Instance;
            Scope["Number"] = NumberType.Instance;
            Scope["List"] = ListType.Instance;
            Scope["Map"] = MapType.Instance;
            Scope["String"] = StringType.Instance;
            Scope["Function"] = FunctionType.Instance;
            Scope["SyntaxNode"] = SyntaxNodeType.Instance;
            Scope["Range"] = RangeType.Instance;

            var types = Assembly.GetAssembly(typeof(Global)).GetTypes();
            var fields = types.SelectMany(t => t.GetRuntimeFields());

            var namedFields = fields
                .Select(f => new { Field = f, Attributes = f.GetCustomAttributes<GlobalNameAttribute>()})
                .Where(f => f.Attributes.Any());

            foreach (var field in namedFields)
            {
                foreach (var globalname in field.Attributes)
                {
                    Scope[globalname.Name] = (IValue) field.Field.GetValue(null);
                }
            }
        }
    }
}
