using FluentValidation;
using KutokAccounting.Services.Vendors.DataTransferObjects;

namespace KutokAccounting.Services.Vendors.Validators;

public sealed class QueryParametersValidator : AbstractValidator<QueryParameters>
{
    public QueryParametersValidator()
    {
        RuleFor(p => p.Name).Length(1, 100);

        RuleFor(p => p.Page)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(p => p.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(10);
    }
}