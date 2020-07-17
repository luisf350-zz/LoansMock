using System;
using Loans.Domain.Applications;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Loans.Tests
{
    public class LoanApplicationProcessorShould
    {
        [Test]
        public void DeclineLowSalary()
        {
            // Arrange
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42,
                                                  product,
                                                  amount,
                                                  "Sarah",
                                                  25,
                                                  "133 Pluralsight Drive, Draper, Utah",
                                                  64_999);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();
            var mockCreditScorer = new Mock<ICreditScorer>();
            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            // Act
            sut.Process(application);

            // Assert
            Assert.That(application.GetIsAccepted(), Is.False);
        }

        [Test]
        public void Accept()
        {
            // Arrange
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42,
                                                  product,
                                                  amount,
                                                  "Sarah",
                                                  25,
                                                  "133 Pluralsight Drive, Draper, Utah",
                                                  65_000);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();
            mockIdentityVerifier
                .Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            var mockCreditScorer = new Mock<ICreditScorer>();
            mockCreditScorer
                .Setup(x => x.Score)
                .Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            // Act
            sut.Process(application);

            // Assert
            Assert.That(application.GetIsAccepted(), Is.True);
        }

        [Test]
        public void InitializeIdentityVerifier()
        {
            // Arrange
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42,
                                                  product,
                                                  amount,
                                                  "Sarah",
                                                  25,
                                                  "133 Pluralsight Drive, Draper, Utah",
                                                  65_000);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();
            mockIdentityVerifier
                .Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            var mockCreditScorer = new Mock<ICreditScorer>();
            mockCreditScorer
                .Setup(x => x.Score)
                .Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            // Act
            sut.Process(application);

            // Assert
            mockIdentityVerifier.Verify(x => x.Initialize());
        }

        [Test]
        public void CalculateScore()
        {
            // Arrange
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42,
                                                  product,
                                                  amount,
                                                  "Sarah",
                                                  25,
                                                  "133 Pluralsight Drive, Draper, Utah",
                                                  65_000);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();
            mockIdentityVerifier
                .Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            var mockCreditScorer = new Mock<ICreditScorer>();
            mockCreditScorer
                .Setup(x => x.Score)
                .Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            // Act
            sut.Process(application);

            // Assert
            mockCreditScorer.Verify(x => x.CalculateScore("Sarah", "133 Pluralsight Drive, Draper, Utah"));
        }

        [Test]
        public void NullReturnExample()
        {
            // Arrange
            var mock = new Mock<INullExample>();

            mock.Setup(x => x.SomeMethod()).Returns<string>(null);

            // Act
            string mockReturnValue = mock.Object.SomeMethod();

            // Assert
            Assert.IsNull(mockReturnValue);
        }

        [Test]
        public void AcceptUsingPartialMock()
        {
            // Arrange
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42,
                                                  product,
                                                  amount,
                                                  "Sarah",
                                                  25,
                                                  "133 Pluralsight Drive, Draper, Utah",
                                                  65_000);

            var mockIdentityVerifier = new Mock<IdentityVerifierServiceGateway>();
            mockIdentityVerifier
                .Protected()
                .Setup<bool>("CallService","Sarah", 25, "133 Pluralsight Drive, Draper, Utah")
                .Returns(true);

            var expectedTime = new DateTime(2000, 1, 1);
            mockIdentityVerifier
                .Protected()
                .Setup<DateTime>("GetCurrentTime")
                .Returns(expectedTime);

            var mockCreditScorer = new Mock<ICreditScorer>();
            mockCreditScorer
                .Setup(x => x.Score)
                .Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            // Act
            sut.Process(application);

            // Assert
            Assert.That(application.GetIsAccepted(), Is.True);
            Assert.AreEqual(mockIdentityVerifier.Object.LastCheckTime, expectedTime);
        } 
    }

    public interface INullExample
    {
        string SomeMethod();
    }
}
