namespace Wul.Interpreter
{
    abstract class Table : IValue
    {
        public abstract IValue Get(IValue key);

        public abstract void Add(IValue key, IValue value);

        protected abstract void Remove(IValue key);

        public abstract void Assign(IValue key, IValue value);

        public abstract Number Count { get; }

        public string AsString()
        {
            return $"List[{Count.Value}]";
        }

        public abstract IValue this[IValue key] { get; set; }
    }
}