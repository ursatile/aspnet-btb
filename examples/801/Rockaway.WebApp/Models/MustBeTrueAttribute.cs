using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Rockaway.WebApp.Models;

public class MustBeTrueAttribute : ValidationAttribute, IClientModelValidator {

	public override bool IsValid(object? value) => value is true;

	public void AddValidation(ClientModelValidationContext context) {
		var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
		context.Attributes.TryAdd("data-val", "true");
		context.Attributes.TryAdd("data-val-must-be-true", errorMessage);
	}
}