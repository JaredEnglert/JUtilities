using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Utilitarian.Extensions.Test.Unit.TestClasses.ExceptionExtensions
{
    [TestClass]
    public class ToJsonTests
    {
        private const string TestExceptionMessage = "Test Exception Message";

        [TestMethod]
        public void ShouldSerialize()
        {
            new Exception(TestExceptionMessage).ToJson().Should().Contain(TestExceptionMessage);
        }

        [TestMethod]
        public void ShouldDeserialize()
        {
            JsonConvert.DeserializeObject<SerializableException>(new Exception(TestExceptionMessage).ToJson());
        }

        [TestMethod]
        public void ShouldSerializeMultipleLevels()
        {
            var level2 = new Exception(TestExceptionMessage + " - 2");
            var level1 = new Exception(TestExceptionMessage + " - 1", level2);
            var level0 = new Exception(TestExceptionMessage + " - 0", level1);

            level0.ToJson();
        }

        [TestMethod]
        public void ShouldBeCorrectMessage()
        {
            var serializableException = JsonConvert.DeserializeObject<SerializableException>(new Exception(TestExceptionMessage).ToJson());
            serializableException.Message.ShouldBeEquivalentTo(TestExceptionMessage);
        }

        [TestMethod]
        public void ShouldBeCorrectStackTrace()
        {
            var exception = new Exception(TestExceptionMessage);
            var stackTrace = exception.StackTrace;
            var serializableException = JsonConvert.DeserializeObject<SerializableException>(exception.ToJson());
            serializableException.StackTrace.ShouldBeEquivalentTo(stackTrace);
        }
    }
}
