using QoDL.DataAnnotations.Extensions.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace QoDL.DataAnnotations.Extensions.Tests
{
    public class ExpressionExtensionsTests
    {
#pragma warning disable S4144 // Methods should not have identical implementations
        [Theory]
        [ClassData(typeof(ExpressionPropertyTestData))]
        public void GetMemberName_WorksWithDifferentPropertyDataTypes<TProp>(Expression<Func<TestModel, TProp>> expression, string expectedName)
        {
            var exception = Record.Exception(() => Assert.Equal(expectedName, GetExpressionTargetName(expression)));
            Assert.Null(exception);
        }

        [Theory]
        [ClassData(typeof(ExpressionFieldTestData))]
        public void GetMemberName_WorksWithDifferentFieldDataTypes<TProp>(Expression<Func<TestModel, TProp>> expression, string expectedName)
        {
            var exception = Record.Exception(() => Assert.Equal(expectedName, GetExpressionTargetName(expression)));
            Assert.Null(exception);
        }

        [Theory]
        [ClassData(typeof(ExpressionUnboxedTestData))]
        public void GetMemberName_WorksWithDifferentUnboxedDataTypes(Expression<Func<TestModel, object>> expression, string expectedName)
        {
            var exception = Record.Exception(() => Assert.Equal(expectedName, GetExpressionTargetName(expression)));
            Assert.Null(exception);
        }

#pragma warning restore S4144 // Methods should not have identical implementations
        public class ExpressionUnboxedTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { (Expression<Func<TestModel, object>>)(x => x.StringProperty), nameof(TestModel.StringProperty) };
                yield return new object[] { (Expression<Func<TestModel, object>>)(x => x.IntProperty), nameof(TestModel.IntProperty) };
                yield return new object[] { (Expression<Func<TestModel, object>>)(x => x.StructProperty), nameof(TestModel.StructProperty) };
                yield return new object[] { (Expression<Func<TestModel, object>>)(x => x.ReferenceProperty), nameof(TestModel.ReferenceProperty) };
                yield return new object[] { (Expression<Func<TestModel, object>>)(x => x.StringField), nameof(TestModel.StringField) };
                yield return new object[] { (Expression<Func<TestModel, object>>)(x => x.IntField), nameof(TestModel.IntField) };
                yield return new object[] { (Expression<Func<TestModel, object>>)(x => x.StructField), nameof(TestModel.StructField) };
                yield return new object[] { (Expression<Func<TestModel, object>>)(x => x.ReferenceField), nameof(TestModel.ReferenceField) };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class ExpressionFieldTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { (Expression<Func<TestModel, string>>)(x => x.StringField), nameof(TestModel.StringField) };
                yield return new object[] { (Expression<Func<TestModel, int>>)(x => x.IntField), nameof(TestModel.IntField) };
                yield return new object[] { (Expression<Func<TestModel, DateTime>>)(x => x.StructField), nameof(TestModel.StructField) };
                yield return new object[] { (Expression<Func<TestModel, TestModel>>)(x => x.ReferenceField), nameof(TestModel.ReferenceField) };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class ExpressionPropertyTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { (Expression<Func<TestModel, string>>)(x => x.StringProperty), nameof(TestModel.StringProperty) };
                yield return new object[] { (Expression<Func<TestModel, int>>)(x => x.IntProperty), nameof(TestModel.IntProperty) };
                yield return new object[] { (Expression<Func<TestModel, DateTime>>)(x => x.StructProperty), nameof(TestModel.StructProperty) };
                yield return new object[] { (Expression<Func<TestModel, TestModel>>)(x => x.ReferenceProperty), nameof(TestModel.ReferenceProperty) };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private string GetExpressionTargetName<TModel, TProp>(Expression<Func<TModel, TProp>> expression)
            => expression.GetMemberName();

#pragma warning disable S3459
        public class TestModel
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
            public DateTime StructProperty { get; set; }
            public TestModel ReferenceProperty { get; set; }

            public string StringField;
            public int IntField;
            public DateTime StructField;
            public TestModel ReferenceField;
        }
#pragma warning restore S3459
    }
}
