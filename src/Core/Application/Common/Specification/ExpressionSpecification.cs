using System.Linq.Expressions;

namespace Cleanception.Application.Common.Specification;
public class ExpressionSpecification<T> : Specification<T>
{
    public ExpressionSpecification(Expression<Func<T, bool>> expression, Expression<Func<T, object?>>? orderExpression = null)
    {
        Query.Where(expression);
        if (orderExpression != null)
            Query.OrderByDescending(orderExpression);
    }
}

public class ExpressionSpecificationProjecting<T, TResult> : Specification<T, TResult>
{
    public ExpressionSpecificationProjecting(Expression<Func<T, bool>> expression, Expression<Func<T, object?>>? orderExpression = null)
    {
        Query.Where(expression);
        if (orderExpression != null)
            Query.OrderByDescending(orderExpression);
    }
}