using AdminViewer.Models.Requests;

namespace AdminViewer.Models.Validators
{
    public class AddUserRequestValidatorTests
    {
        // Test AddUserRequestValidator
        [Fact]
        public void Validate_ShouldReturnValid_WhenUserIdAndNameAreProvided()
        {
            // Arrange
            var validator = new AddUserRequestValidator();
            var request = new AddUserRequest(123, "John Doe");

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.True(result.IsValid);
        }

        // Test AddUserRequestValidator with empty UserId
        [Fact]
        public void Validate_ShouldReturnInvalid_WhenUserIdIsEmpty()
        {
            // Arrange
            var validator = new AddUserRequestValidator();
            var request = new AddUserRequest(0, "John Doe");

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(AddUserRequest.UserId));
        }

        // Test AddUserRequestValidator with empty Name
        [Fact]
        public void Validate_ShouldReturnInvalid_WhenNameIsEmpty()
        {
            // Arrange
            var validator = new AddUserRequestValidator();
            var request = new AddUserRequest(123, string.Empty);

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(AddUserRequest.Name));
        }

        // Test AddUserRequestValidator with long Name
        [Fact]
        public void Validate_ShouldReturnInvalid_WhenNameExceedsMaxLength()
        {
            // Arrange
            var validator = new AddUserRequestValidator();
            var request = new AddUserRequest(123, new string('a', 201)); // 201 characters long

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(AddUserRequest.Name));
        }
    }
}
