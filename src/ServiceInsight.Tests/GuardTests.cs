namespace Particular.ServiceInsight.Tests
{
    using System;
    using Desktop;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class GuardTests
    {
        GuardTest GuardTest;

        [SetUp]
        public void TestInitialize()
        {
            GuardTest = new GuardTest();
        }

        [Test]
        public void should_not_throw_when_values_are_inside_the_range()
        {
            GuardTest.AddRangeExclusive(15);
        }

        [Test]
        public void should_not_throw_when_values_equal_min_expected_values_inclusive()
        {
            GuardTest.AddRangeInclusive(10);
        }

        [Test]
        public void should_not_throw_when_values_equal_max_expected_values_inclusive()
        {
            GuardTest.AddRangeInclusive(20);
        }

        [Test]
        public void should_not_throw_when_values_equal_min_expected_values_exclusive()
        {
            GuardTest.AddRangeExclusive(11);
        }

        [Test]
        public void should_not_throw_when_values_equal_max_expected_values_exclusive()
        {
            GuardTest.AddRangeExclusive(19);
        }

        [Test]
        public void throws_when_inclusive_range_check_not_valid()
        {
            Should.Throw<Exception>(() => GuardTest.AddRangeExclusive(0));
        }

        [Test]
        public void throws_when_exclusive_range_check_not_valid()
        {
            Should.Throw<Exception>(() => GuardTest.AddRangeExclusive(20));
        }

        [Test]
        public void throws_when_value_equal_min_expected_values_exclusive()
        {
            Should.Throw<Exception>(() => GuardTest.AddRangeExclusive(10));
        }

        [Test]
        public void throws_when_value_equal_max_expected_values_exclusive()
        {
            Should.Throw<Exception>(() => GuardTest.AddRangeExclusive(20));
        }

        [Test]
        public void throws_when_value_equal_max_expected_values_inclusive()
        {
            Should.Throw<Exception>(() => GuardTest.AddRangeInclusive(21));
        }

        [Test]
        public void throws_when_value_equal_min_expected_values_inclusive()
        {
            Should.Throw<Exception>(() => GuardTest.AddRangeInclusive(9));
        }

        [Test]
        public void should_not_throw_exceptions_when_null_not_passed_in()
        {
            Should.NotThrow(() => GuardTest.Run("not null string"));
        }

        [Test]
        public void throws_when_variable_is_null_when_null_passed_in()
        {
            Should.Throw<ArgumentNullException>(() => GuardTest.Run(null));
        }

        [Test]
        public void throws_when_variable_is_empty_string_when_empty_string_passed_in()
        {
            Should.Throw<ArgumentException>(() => GuardTest.Run(string.Empty));
        }

        [Test]
        public void throws_invalid_operations_exception_when_checking_wrong_true_conditions()
        {
            Should.Throw<InvalidOperationException>(() => GuardTest.IsTrue(false));
        }

        [Test]
        public void should_not_throw_any_exceptions_when_checking_correct_true_conditions()
        {
            Should.NotThrow(() => GuardTest.IsTrue(true));
        }

        [Test]
        public void throws_invalid_operations_exception_when_checking_wrong_false_conditions()
        {
            Should.Throw<InvalidOperationException>(() => GuardTest.IsFalse(true));
        }

        [Test]
        public void should_not_throw_any_exception_when_checking_correct_false_condition_with_custom_exceptions()
        {
            Should.NotThrow(() => GuardTest.IsFalseWithException(false));
        }

        [Test]
        public void should_have_thrown_the_specified_exception_when_checking_wrong_false_condition_with_custom_exceptions()
        {
            var error = Should.Throw<Exception>(() => GuardTest.IsFalseWithException(true));
            error.Message.ShouldBe(GuardTest.ErrorMessage);
        }

        [Test]
        public void should_not_throw_any_exceptions_when_checking_correct_false_conditions()
        {
            Should.NotThrow(() => GuardTest.IsFalse(false));
        }
    }

    public class GuardTest
    {
        public const string ErrorMessage = "Something is wrong";

        public void Run(string testName)
        {
            Guard.NotNull(() => testName, testName);
            Guard.NotNullOrEmpty(() => testName, testName);
        }

        public void AddRangeExclusive(int x)
        {
            Guard.NotOutOfRangeExclusive(() => x, x, 10, 20);
        }

        public void AddRangeInclusive(int x)
        {
            Guard.NotOutOfRangeInclusive(() => x, x, 10, 20);
        }

        public void IsTrue(bool value)
        {
            Guard.True(value);
        }

        public void IsFalse(bool value)
        {
            Guard.False(value);
        }

        public void IsFalseWithException(bool value)
        {
            Guard.False(value, () => new Exception(ErrorMessage));
        }
    }

}