using BeeneticToolkit.Collections.Enums;

namespace BeeneticToolkit.Unity.Subjects {

    public enum ElementGroup { None = 0, Nonmetal, Metal }

    // IL2CPP-STRIPPING SUBJECT. A self-registering smart enum whose item fields are NEVER referenced
    // by name anywhere in the codebase — they exist only to be discovered reflectively by AutoEnumItem.
    // Under aggressive managed stripping, write-only static fields like these are prime removal
    // candidates; if IL2CPP strips them, self-registration finds nothing and Count == 0. Living in a
    // normal (non-test) runtime assembly means it is actually subject to stripping in a player build,
    // unlike a type defined inside a test assembly. This is the real-world AutoEnumItem risk.
    public sealed class Element : AutoEnumItem<Element, string, ElementGroup> {

        public static readonly Element Hydrogen = new Element("h",  "Hydrogen", "H",  ElementGroup.Nonmetal);
        public static readonly Element Helium   = new Element("he", "Helium",   "He", ElementGroup.Nonmetal);
        public static readonly Element Iron     = new Element("fe", "Iron",     "Fe", ElementGroup.Metal);

        private Element(string key, string name, string shortName, ElementGroup group)
            : base(key, name, shortName, group: group) { }
    }
}
