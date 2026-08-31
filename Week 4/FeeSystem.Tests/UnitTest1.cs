using NUnit.Framework;
using FeeSystem;
using System.Collections.Generic;

namespace FeeSystem.Tests
{
    [TestFixture]
    public class FeeCalculatorTests
    {
        private FeeCalculator _calc;

        [SetUp]
        public void Setup()
        {
            // Initialize a fresh calculator before each test
            _calc = new FeeCalculator();
        }

        // -----------------------------------------------------------------
        // Case 1: No payments → full fee outstanding
        // Design Idea: Happy path. Verifies basic calculation when no money is paid.
        // -----------------------------------------------------------------
        [Test]
        public void OutstandingBalance_NoPayments_ReturnsFullFee()
        {
            // Arrange
            var payments = new List<decimal>();
            
            // Act
            var result = _calc.OutstandingBalance(600m, payments);
            
            // Assert
            Assert.That(result, Is.EqualTo(600m));
        }

        // -----------------------------------------------------------------
        // Case 2: One partial payment (600 fee, 200 paid → 400)
        // Design Idea: Normal partition. Tests standard subtraction logic.
        // -----------------------------------------------------------------
        [Test]
        public void OutstandingBalance_OnePartialPayment_ReturnsCorrectBalance()
        {
            // Arrange
            var payments = new List<decimal> { 200m };
            
            // Act
            var result = _calc.OutstandingBalance(600m, payments);
            
            // Assert
            Assert.That(result, Is.EqualTo(400m));
        }

        // -----------------------------------------------------------------
        // Case 3: Several instalments (200 + 200 + 100 → 100)
        // Design Idea: Real client behaviour. Ensures the system sums multiple payments correctly.
        // -----------------------------------------------------------------
        [Test]
        public void OutstandingBalance_MultiplePayments_ReturnsCorrectBalance()
        {
            // Arrange
            var payments = new List<decimal> { 200m, 200m, 100m };
            
            // Act
            var result = _calc.OutstandingBalance(600m, payments);
            
            // Assert
            Assert.That(result, Is.EqualTo(100m));
        }

        // -----------------------------------------------------------------
        // Case 4: Fee fully paid → balance 0
        // Design Idea: Boundary value. Tests the exact point where debt becomes zero.
        // -----------------------------------------------------------------
        [Test]
        public void OutstandingBalance_FullyPaid_ReturnsZero()
        {
            // Arrange
            var payments = new List<decimal> { 600m };
            
            // Act
            var result = _calc.OutstandingBalance(600m, payments);
            
            // Assert
            Assert.That(result, Is.EqualTo(0m));
        }

        // -----------------------------------------------------------------
        // Case 5: Overpayment (600 fee, 700 paid → -100)
        // Design Idea: Boundary / unexpected input. Verifies system handles credit/negative balance.
        // -----------------------------------------------------------------
        [Test]
        public void OutstandingBalance_Overpayment_ReturnsNegativeBalance()
        {
            // Arrange
            var payments = new List<decimal> { 700m };
            
            // Act
            var result = _calc.OutstandingBalance(600m, payments);
            
            // Assert
            Assert.That(result, Is.EqualTo(-100m));
        }

        // -----------------------------------------------------------------
        // Case 6: Negative fee → throws ArgumentException
        // Design Idea: Error path. Ensures invalid input (negative fee) is rejected immediately.
        // -----------------------------------------------------------------
        [Test]
        public void OutstandingBalance_NegativeFee_ThrowsArgumentException()
        {
            // Arrange
            var payments = new List<decimal>();
            
            // Act & Assert
            // We expect the method to crash with an ArgumentException when fee is negative
            Assert.That(() => _calc.OutstandingBalance(-1m, payments), Throws.ArgumentException);
        }

        // -----------------------------------------------------------------
        // Case 7: Exactly half paid → cleared for exams is true
        // Design Idea: Boundary on business rule. Tests the specific threshold for exam clearance.
        // Note: Assumes a method IsClearedForExams exists. If not, adjust method name.
        // -----------------------------------------------------------------
        [Test]
        public void IsClearedForExams_ExactlyHalfPaid_ReturnsTrue()
        {
            // Arrange
            var payments = new List<decimal> { 300m }; // Half of 600
            
            // Act
            var result = _calc.IsClearedForExams(600m, payments);
            
            // Assert
            Assert.That(result, Is.True);
        }

        // -----------------------------------------------------------------
        // Case 8: One toea under half → cleared is false
        // Design Idea: Just-below boundary. Ensures strict enforcement of the 50% rule.
        // Note: 1 Toea = 0.01 Kina. Half of 600 is 300. One toea under is 299.99.
        // -----------------------------------------------------------------
        [Test]
        public void IsClearedForExams_JustUnderHalf_ReturnsFalse()
        {
            // Arrange
            var payments = new List<decimal> { 299.99m }; 
            
            // Act
            var result = _calc.IsClearedForExams(600m, payments);
            
            // Assert
            Assert.That(result, Is.False);
        }
    }
}   
