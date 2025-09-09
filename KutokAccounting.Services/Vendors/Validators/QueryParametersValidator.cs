using FluentValidation;
using KutokAccounting.Services.Vendors.DataTransferObjects;

namespace KutokAccounting.Services.Vendors.Validators;

public sealed class QueryParametersValidator : AbstractValidator<QueryParameters>
{
    public QueryParametersValidator()
    {
        RuleFor(p => p.PageSize).LessThan(11);
    }
}