using NUnit.Framework;
using BeeneticToolkit.Unity.Subjects;

namespace BeeneticToolkit.Unity.Tests {

    // The payoff test for Phase 3. Element's item fields are reached ONLY through the static API
    // (Count / FromKey / FromShortName), never by name — so under IL2CPP + aggressive managed
    // stripping, if the reflectively-discovered fields are removed, self-registration yields nothing
    // and these assertions fail. That failure IS the finding (fix: a link.xml preserve rule).
    //
    // Under Mono (editor PlayMode) there's no stripping, so this passes trivially; its real value is
    // when this assembly is built into and run from an IL2CPP player.
    public class Il2cppStrippingTests {

        [Test]
        public void AutoEnumItem_SelfRegisters_UnderManagedStripping() {
            Assert.AreEqual(3, Element.Count,
                "Element.Count should be 3; 0 means IL2CPP stripped the reflectively-discovered item fields");
            Assert.AreEqual("Hydrogen", Element.FromKey("h").Name);
            Assert.AreEqual("Iron", Element.FromShortName("Fe").Name);
        }
    }
}
