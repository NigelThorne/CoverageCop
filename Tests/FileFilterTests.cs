using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;

namespace NCoverCop.Tests
{
    [TestFixture]
    public class FileFilterTests
    {
        [Test]
        public void Path_containing_no_files__RETURNS__Empty_results()
        {
            var values = new FileFilter(@"*.none").ToArray();
            Assert.That(values.Length == 0);
        }

        [Test]
        public void url__RETURNS__url_in_results()
        {
            var values = new FileFilter(@"http://test").ToArray();
            Assert.That(values.Length, Is.EqualTo(1));
            Assert.That(values, Contains(@"http://test"));
        }

        [Test]
        public void Given_a_fileName__RETURNS__Filename_as_the_only_result()
        {
            var values = new FileFilter(@"tests.cs").ToArray();
            Assert.That(values.Length, Is.EqualTo(1));
            Assert.That(values, Contains("tests.cs"));
        }

        [Test]
        public void Path_With_parentref__RETURNS__correct_results()
        {
            var values = new FileFilter(@"..\..\*.cs").ToArray();
            Assert.That(values, Contains(CurrentSourceFile()), 
                    $@"Should contains current source file: {CurrentSourceFile()}");
        }

        [Test]
        public void Wildcard__RETURNS__current_folder_results()
        {
            var values = Glob.GetMatches(@"*", 0).ToArray();
            var assemblyname = Assembly.GetAssembly(typeof(FileFilterTests)).CodeBase.Split('/').Last().Replace(".DLL", ".dll");

            Assert.That(values, Contains(assemblyname), "Current directory should contain current assembly: "+ assemblyname);
        }

        public string CurrentSourceFile([CallerFilePath] string file = "")
        {
            return file;
        }

        private static ContainsConstraint Contains(object @object)
        {
            return new ContainsConstraint(@object);
        }
    }

}
