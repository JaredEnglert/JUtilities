using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilitarian.FluentExcel.Attributes;

namespace Utilitarian.FluentExcel.Test.Unit.TestClasses
{
    [TestClass]
    public class WorkbookTests
    {
        private const string SheetName = "Test Sheet Name";

        [TestMethod]
        public void ShouldTransform()
        {
            CreateTestCollection().ToExcelWorkBook(SheetName).Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldTransformToMemoryStream()
        {
            CreateTestCollection().ToExcelWorkBook(SheetName).ToMemoryStream().Should().NotBeNull();
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
        
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
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

            [Format(ExcelFormatters.NumberNoDecimal)]
            public int NumberNoDecimal { get; set; }

            [Format(ExcelFormatters.Currency)]
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
