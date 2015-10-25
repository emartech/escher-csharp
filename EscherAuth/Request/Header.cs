namespace EscherAuth.Request
{
    public class Header
    {
        public string Name { get; }
        public string Value { get; }

        public Header(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var header = obj as Header;
            if (header == null)
            {
                return false;
            }

            return header.Name.Equals(Name) && header.Value.Equals(Value);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + Value.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return Name + ":" + Value;
        }
    }
}
