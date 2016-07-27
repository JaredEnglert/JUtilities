using System.Collections.Generic;
using System.ComponentModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilitarian.FluentExcel.Attributes;

namespace Utilitarian.FluentExcel.Test.Unit.TestClasses
{
    [TestClass]
    public class WorkbookTests
    {
        private const string _sheetName = "Test Sheet Name";

        [TestMethod]
        public void ShouldTransform()
        {
            CreateTestCollection().ToExcelWorkBook(_sheetName).Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldTransformToMemoryStream()
        {
            CreateTestCollection().ToExcelWorkBook(_sheetName).ToMemoryStream().Should().NotBeNull();
        }

        private TestCollection<TestClass> CreateTestCollection(bool testCondition = true)
        {
            var testCollection =  new TestCollection<TestClass>
            {
                new TestClass
                {
                    ConditionalAverage = 1.23f,
                    ConditionalSum = 7.31f,
                    DoNotExport = "LMFAO",
                    NumberNoDecimal = 1234568790,
                    Money = 3.14f,
                    Normal = "Normal",
                    StringWithDescription = "Should have descriptive name"
                },
                new TestClass
                {
                    ConditionalAverage = 1.23f,
                    ConditionalSum = 7.31f,
                    DoNotExport = "LMFAO",
                    NumberNoDecimal = 1234568790,
                    Money = 3.14f,
                    Normal = "Normal",
                    StringWithDescription = "Should have descriptive name"
                }
            };

            testCollection.TestCondition = testCondition;

            return testCollection;
        }
        
        private class TestClass
        {
            public string Normal { get; set; }

            [DisplayName("String With Description")]
            public string StringWithDescription { get; set; }

            [ConditionalExport(typeof(TestCollectionExportCondition))]
            [Formula(FormulaType.Average)]
            public float ConditionalAverage { get; set; }

            [ConditionalExport(typeof(TestCollectionExportCondition))]
            [Formula(FormulaType.Sum)]
            public float ConditionalSum { get; set; }

            [FormatString(FileFormatters.NumberNoDecimal)]
            public int NumberNoDecimal { get; set; }

            [FormatString(FileFormatters.Currency)]
            public float Money { get; set; }

            [DoNotExport]
            public string DoNotExport { get; set; }
        }

        private class TestCollection<T> : List<T>
        {
            public bool TestCondition { get; set; }
        }

        private class TestCollectionExportCondition : IExportCondition
        {
            public bool IsTrue(object collection)
            {
                return ((TestCollection<TestClass>)collection).TestCondition;
            }
        }
    }
}
